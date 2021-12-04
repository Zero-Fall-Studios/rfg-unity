using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFG
{
  [CustomEditor(typeof(AnimationClips))]
  public class AnimationClipsEditor : Editor
  {
    private VisualElement rootElement;
    private Editor editor;

    public void OnEnable()
    {
      rootElement = new VisualElement();

      var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/RFG/Animation/Editor/AnimationClipsEditor/AnimationClipsEditor.uss");
      rootElement.styleSheets.Add(styleSheet);
    }

    public override VisualElement CreateInspectorGUI()
    {
      rootElement.Clear();

      UnityEngine.Object.DestroyImmediate(editor);
      editor = Editor.CreateEditor(this);
      IMGUIContainer container = new IMGUIContainer(() =>
      {
        if (target)
        {
          OnInspectorGUI();
        }
      });
      rootElement.Add(container);

      var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/RFG/Animation/Editor/AnimationClipsEditor/AnimationClipsEditor.uxml");
      visualTree.CloneTree(rootElement);

      VisualElement mainContainer = rootElement.Q<VisualElement>("container");

      TextField animatorControllerPath = new TextField()
      {
        label = "Animator Controller Path"
      };

      mainContainer.Add(animatorControllerPath);

      Button generateClipsButton = new Button()
      {
        text = "Generate Clips"
      };
      generateClipsButton.clicked += () =>
      {
        Create(animatorControllerPath.value);
      };
      mainContainer.Add(generateClipsButton);

      return rootElement;
    }

    private void Create(string animatorControllerPath)
    {
      AnimationClips animationClips = (AnimationClips)target;

      if (animationClips.clips.Count == 0)
      {
        LogExt.Warn<AnimationClipsEditor>("This animation clips contains no clips");
        return;
      }

      Texture2D texture = Selection.activeObject as Texture2D;

      string path = AssetDatabase.GetAssetPath(texture);
      string animationsPath = animatorControllerPath.RemoveLast("/");

      UnityEditor.Animations.AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(animatorControllerPath);
      if (animatorController == null)
      {
        LogExt.Warn<AnimationClipsEditor>("Animation Controller not found");
        return;
      }

      Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();

      float frameRate = 60f;
      float timeStep = (frameRate / 60f) / 10f;

      foreach (AnimationClipItem animationItem in animationClips.clips)
      {
        bool newClip = false;
        AnimationClip animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{animationsPath}/{animationItem.name}.anim");

        if (animClip == null)
        {
          animClip = new AnimationClip();
          animClip.name = animationItem.name;
          newClip = true;
        }

        animClip.frameRate = frameRate;

        if (!newClip)
        {
          animClip.ClearCurves();
        }

        List<ObjectReferenceKeyframe> spriteKeyFrames = new List<ObjectReferenceKeyframe>();
        float time = 0;
        for (int i = animationItem.framesStart; i <= animationItem.framesEnd; i++)
        {
          ObjectReferenceKeyframe spriteKeyFrame = new ObjectReferenceKeyframe();
          spriteKeyFrame.time = time;
          spriteKeyFrame.value = sprites[i];
          time += timeStep;
          spriteKeyFrames.Add(spriteKeyFrame);
        }

        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";
        AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, spriteKeyFrames.ToArray());

        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(animClip);
        settings.loopTime = animationItem.loop;
        AnimationUtility.SetAnimationClipSettings(animClip, settings);

        if (!string.IsNullOrEmpty(animationItem.animationEventFunction))
        {
          AnimationEvent evt = new AnimationEvent();
          evt.time = animationItem.animationEventTime;
          evt.functionName = animationItem.animationEventFunction;
          AnimationUtility.SetAnimationEvents(animClip, new AnimationEvent[] { evt });
        }

        if (newClip)
        {
          AssetDatabase.CreateAsset(animClip, $"{animationsPath}/{animClip.name}.anim");
          animatorController.AddMotion(animClip);
        }
      }

      EditorUtility.SetDirty(animatorController);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
    }

  }
}