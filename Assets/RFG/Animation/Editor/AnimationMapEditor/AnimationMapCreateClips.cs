using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;

namespace RFG
{
  public class AnimationMapCreateClips
  {
    public static void Create(AnimationMap animationMap, string animatorControllerPath)
    {
      if (animationMap.animations.Count == 0)
      {
        LogExt.Warn<AnimationMapSpriteSlicer>("This animation map contains no animations");
        return;
      }

      Texture2D texture = Selection.activeObject as Texture2D;

      string path = AssetDatabase.GetAssetPath(texture);
      string animationsPath = $"{path.RemoveLast("/")}/../Animations";


      UnityEditor.Animations.AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(animatorControllerPath);

      if (animatorController == null)
      {
        LogExt.Warn<AnimationMapSpriteSlicer>("Animation Controller not found");
        return;
      }

      Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();

      int spriteIndex = 0;
      float frameRate = 25f;
      float timeStep = (frameRate / 60f) / 10f;

      foreach (AnimationItem animationItem in animationMap.animations)
      {
        AnimationClip animClip = new AnimationClip();
        animClip.name = animationItem.name;
        animClip.frameRate = frameRate;

        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";

        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[animationItem.frames];
        float time = 0;
        for (int i = 0; i < (animationItem.frames); i++)
        {
          Debug.Log($"{animClip.name} time: {time}");
          spriteKeyFrames[i] = new ObjectReferenceKeyframe();
          spriteKeyFrames[i].time = time;
          spriteKeyFrames[i].value = sprites[spriteIndex++];
          time += timeStep;
        }
        AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, spriteKeyFrames);
        AssetDatabase.CreateAsset(animClip, $"{animationsPath}/{animClip.name}.anim");

        animatorController.AddMotion(animClip);
      }

      EditorUtility.SetDirty(animatorController);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
    }
  }
}