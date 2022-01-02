
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using MyBox;
#endif

namespace RFG
{
  [CreateAssetMenu(fileName = "New State Pack", menuName = "RFG/State/State Pack")]
  public class StatePack : ScriptableObject
  {
    public List<State> States;

    public State DefaultState;

    public void Add(State state)
    {
      if (States == null)
      {
        States = new List<State>();
      }
      States.Add(state);
    }

    public void Remove(State state)
    {
      States.Remove(state);
    }

    public State Find(State state)
    {
      return States.Find(s => s == state);
    }

    public bool HasState(State state)
    {
      return Find(state) != null;
    }

    public State Find(Type type)
    {
      return States.Find(state => state.GetType().Equals(type));
    }

    public bool HasState(Type type)
    {
      return Find(type) != null;
    }

#if UNITY_EDITOR
    public T AddToPack<T>(string enterClip = null, bool freezeState = false, float waitToUnfreezeTime = 0f, bool defaultState = false, params State[] statesCanUnFreeze) where T : State
    {
      T state = null;
      if (States != null && States.Count > 0)
      {
        state = States.Find(s => s.GetType().Equals(typeof(T))) as T;
      }

      if (state == null)
      {
        state = ScriptableObject.CreateInstance<T>();
        Add(state);
        AssetDatabase.AddObjectToAsset(state, this);
      }

      state.name = state.GetType().ToString().Last();

      if (!string.IsNullOrEmpty(enterClip))
      {
        state.EnterClip = enterClip;
      }

      if (freezeState)
      {
        state.FreezeState = freezeState;
      }

      if (waitToUnfreezeTime > 0)
      {
        state.WaitToUnfreezeTime = waitToUnfreezeTime;
      }

      if (statesCanUnFreeze != null && statesCanUnFreeze.Length > 0)
      {
        if (state.StatesCanUnfreeze == null || state.StatesCanUnfreeze.Length == 0)
        {
          state.StatesCanUnfreeze = statesCanUnFreeze;
        }
        else
        {
          state.StatesCanUnfreeze = state.StatesCanUnfreeze.Concat(statesCanUnFreeze).Distinct().ToArray();
        }
      }

      if (defaultState)
      {
        DefaultState = state;
      }
      RemoveNullStates();
      AssetDatabase.SaveAssets();
      EditorUtility.SetDirty(this);
      return state;
    }

    public T AddToPack<T>(bool defaultState = false) where T : State
    {
      return AddToPack<T>(null, false, 0, defaultState, null);
    }

    public void AddStatesCanUnfreeze<T>(params State[] statesCanUnFreeze) where T : State
    {
      T state = null;
      if (States != null && States.Count > 0)
      {
        state = States.Find(s => s.GetType().Equals(typeof(T))) as T;
      }
      if (state == null)
      {
        return;
      }
      if (statesCanUnFreeze != null && statesCanUnFreeze.Length > 0)
      {
        if (state.StatesCanUnfreeze == null || state.StatesCanUnfreeze.Length == 0)
        {
          state.StatesCanUnfreeze = statesCanUnFreeze;
        }
        else
        {
          state.StatesCanUnfreeze = state.StatesCanUnfreeze.Concat(statesCanUnFreeze).Distinct().ToArray();
        }
      }
      RemoveNullStates();
    }

    [ButtonMethod]
    public void RemoveNullStates()
    {
      States = States.Where(s => s != null).ToList();
      foreach (State state in States)
      {
        if (state.StatesCanUnfreeze != null)
        {
          state.StatesCanUnfreeze = state.StatesCanUnfreeze.Where(s => s != null).ToArray();
        }
      }
      EditorUtility.SetDirty(this);
    }

    public void GenerateCharacterStates()
    {
      AddToPack<SpawnState>(true);
      AddToPack<AliveState>();
      AddToPack<DeadState>();
      AddToPack<DeathState>("Death", true, 1f);
    }

    public void GenerateMovementStates()
    {
      AddToPack<IdleState>("Idle", false, 0, true);
      AddToPack<WalkingState>("Walk");
      AddToPack<WalkingUpSlopeState>("Walk Up Slope");
      AddToPack<WalkingDownSlopeState>("Walk");
      AddToPack<RunningState>("Run");
      AddToPack<RunningUpSlopeState>("Run Up Slope");
      AddToPack<RunningDownSlopeState>("Run");
    }

    public void GenerateJumpState()
    {
      FallingState fallingState = AddToPack<FallingState>("Fall");
      JumpingState jumpingState = AddToPack<JumpingState>("Jump", true, 0, false, fallingState);
      AddToPack<LandedState>("Land", true, .25f, false, jumpingState);
    }

    public void GenerateDoubleJumpState()
    {
      DoubleJumpState doubleJumpState = AddToPack<DoubleJumpState>("Jump", true, 0, false);
      AddToPack<LandedState>("Land", true, .25f, false, doubleJumpState);
    }

    public void GenerateJumpFlipState()
    {
      FallingState fallingState = AddToPack<FallingState>("Fall");
      JumpingFlipState jumpingFlipState = AddToPack<JumpingFlipState>("Jump Flip", true, 0, false, fallingState);
      AddToPack<LandedState>("Land", true, .25f, false, jumpingFlipState);
    }

    public void GenerateFallState()
    {
      AddToPack<FallingState>("Fall");
    }

    public void GeneratePrimaryAttackState()
    {
      PrimaryAttackStartedState primaryAttackStartedState = AddToPack<PrimaryAttackStartedState>();
      AddToPack<PrimaryAttackPerformedState>("Primary Attack Performed");
      AddToPack<PrimaryAttackCanceledState>();

      if (HasState(typeof(JumpingState)))
      {
        AddStatesCanUnfreeze<JumpingState>(primaryAttackStartedState);
      }
      if (HasState(typeof(JumpingFlipState)))
      {
        AddStatesCanUnfreeze<JumpingFlipState>(primaryAttackStartedState);
      }
    }

    public void GenerateSecondaryAttackState()
    {
      SecondaryAttackStartedState secondaryAttackStartedState = AddToPack<SecondaryAttackStartedState>();
      AddToPack<SecondaryAttackPerformedState>("Secondary Attack Performed");
      AddToPack<SecondaryAttackCanceledState>();

      if (HasState(typeof(JumpingState)))
      {
        AddStatesCanUnfreeze<JumpingState>(secondaryAttackStartedState);
      }
      if (HasState(typeof(JumpingFlipState)))
      {
        AddStatesCanUnfreeze<JumpingFlipState>(secondaryAttackStartedState);
      }
    }

    public void GenerateAttackAbilityStates()
    {
      GeneratePrimaryAttackState();
      GenerateSecondaryAttackState();
    }

    public void GenerateDashAbilityStates()
    {
      DashingState dashingState = AddToPack<DashingState>("Dash", true);
      if (HasState(typeof(JumpingState)))
      {
        AddStatesCanUnfreeze<JumpingState>(dashingState);
      }
      if (HasState(typeof(JumpingFlipState)))
      {
        AddStatesCanUnfreeze<JumpingFlipState>(dashingState);
      }
    }

    public void GenerateLadderClimbingAbilityStates()
    {
      AddToPack<LadderIdleState>("Ladder Idle");
      LadderClimbingState ladderClimbingState = AddToPack<LadderClimbingState>("Ladder Climb");
      if (HasState(typeof(JumpingState)))
      {
        AddStatesCanUnfreeze<JumpingState>(ladderClimbingState);
      }
      if (HasState(typeof(JumpingFlipState)))
      {
        AddStatesCanUnfreeze<JumpingFlipState>(ladderClimbingState);
      }
    }

    public void GenerateSlideAbilityStates()
    {
      FallingState fallingState = (FallingState)Find(typeof(FallingState));
      AddToPack<SlidingState>("Slide", true, 0.5f, false, fallingState);
    }

    public void GeneratePushAbilityStates()
    {
      AddToPack<PushingIdleState>("Push Idle");
      AddToPack<PushingState>("Push");
    }

    public void GenerateDamageState()
    {
      DamageState damageState = AddToPack<DamageState>("Damage", true, 0.5f, false);
      damageState.StatesCanUnfreeze = new State[] { damageState };

      if (HasState(typeof(SmashDownStartedState)))
      {
        AddStatesCanUnfreeze<SmashDownStartedState>(damageState);
      }
      if (HasState(typeof(SmashDownCollidedState)))
      {
        AddStatesCanUnfreeze<SmashDownCollidedState>(damageState);
      }
      if (HasState(typeof(SmashDownPerformedState)))
      {
        AddStatesCanUnfreeze<SmashDownPerformedState>(damageState);
      }
    }

    public void GenerateSmashDownAbilityStates()
    {
      SwimmingState swimmingState = (SwimmingState)Find(typeof(SwimmingState));
      DamageState damageState = (DamageState)Find(typeof(DamageState));
      SmashDownStartedState smashDownStartedState = AddToPack<SmashDownStartedState>("Smash Down Started", true);
      SmashDownCollidedState smashDownCollidedState = AddToPack<SmashDownCollidedState>("Smash Down Collided", true, 1, false, swimmingState, damageState);
      SmashDownPerformedState smashDownPerformedState = AddToPack<SmashDownPerformedState>("Smash Down Performed", true, 0, false, smashDownCollidedState, swimmingState, damageState);
      smashDownStartedState.StatesCanUnfreeze = new State[] { smashDownPerformedState, damageState };

      if (HasState(typeof(JumpingState)))
      {
        AddStatesCanUnfreeze<JumpingState>(smashDownStartedState);
      }
      if (HasState(typeof(JumpingFlipState)))
      {
        AddStatesCanUnfreeze<JumpingFlipState>(smashDownStartedState);
      }
    }

    public void GenerateSwimAbilityStates()
    {
      FallingState fallingState = (FallingState)Find(typeof(FallingState));
      SwimmingState swimmingState = AddToPack<SwimmingState>("Swim", true, 0, false, fallingState);

      if (HasState(typeof(SmashDownPerformedState)))
      {
        AddStatesCanUnfreeze<SmashDownPerformedState>(swimmingState);
      }
      if (HasState(typeof(SmashDownCollidedState)))
      {
        AddStatesCanUnfreeze<SmashDownCollidedState>(swimmingState);
      }
    }

    public void GenerateWallClingingAbilityStates()
    {
      AddToPack<WallClingingState>("Wall Cling");
    }

    public void GenerateWallJumpAbilityStates()
    {
      AddToPack<WallJumpingState>("Wall Jump");
    }

    public void GenerateCrouchStates()
    {
      AddToPack<CrouchIdleState>("Crouch Idle");
      AddToPack<CrouchWalkingState>("Crouch Walk");
    }

    public void GenerateDanglingBehaviourStates()
    {
      AddToPack<DanglingState>("Dangling");
    }

    public void GenerateLedgeGrabAbilityStates()
    {
      AddToPack<LedgeClimbingState>("Ledge Climb");
      LedgeGrabState ledgeGrabState = AddToPack<LedgeGrabState>("Ledge Grab");

      if (HasState(typeof(JumpingState)))
      {
        AddStatesCanUnfreeze<JumpingState>(ledgeGrabState);
      }
      if (HasState(typeof(JumpingFlipState)))
      {
        AddStatesCanUnfreeze<JumpingFlipState>(ledgeGrabState);
      }
    }
#endif
  }
}