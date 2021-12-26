
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
      AssetDatabase.SaveAssets();
      EditorUtility.SetDirty(this);
      return state;
    }

    public T AddToPack<T>(bool defaultState = false) where T : State
    {
      return AddToPack<T>(null, false, 0, defaultState, null);
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


      // AddToPack<CrouchIdleState>("CrouchIdle");
      // AddToPack<CrouchWalkingState>("CrouchWalking");

      // AddToPack<DanglingState>("Dangling");

      // AddToPack<LedgeClimbingState>("LedgeClimbing");
      // LedgeGrabState ledgeGrabState = AddToPack<LedgeGrabState>("LedgeGrab");

      // AddToPack<SlidingState>("Sliding", true, 0.5f, false, fallingState);
      // AddToPack<PushingIdleState>("PushingIdle");
      // AddToPack<PushingState>("Pushing");
      // AddToPack<WallClingingState>("WallClinging");
      // AddToPack<WallJumpingState>("Jumping");
      // SwimmingState swimmingState = AddToPack<SwimmingState>("Swimming", true, 0, false, fallingState);

      // DamageState damageState = AddToPack<DamageState>("Damage", true, 0.5f, false);
      // damageState.StatesCanUnfreeze = new State[] { damageState };

      // SmashDownStartedState smashDownStartedState = AddToPack<SmashDownStartedState>("SmashDownStarted", true);
      // SmashDownCollidedState smashDownCollidedState = AddToPack<SmashDownCollidedState>("SmashDownCollided", true, 1, false, swimmingState, damageState);
      // SmashDownPerformedState smashDownPerformedState = AddToPack<SmashDownPerformedState>("SmashDownPerformed", true, 0, false, smashDownCollidedState, swimmingState, damageState);
      // smashDownStartedState.StatesCanUnfreeze = new State[] { smashDownPerformedState, damageState };


    }

    public void GenerateJumpState()
    {
      FallingState fallingState = AddToPack<FallingState>("Fall");
      // ledgeGrabState, primaryAttackStartedState, secondaryAttackStartedState, smashDownStartedState, ladderClimbingState
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
      // ledgeGrabState, primaryAttackStartedState, secondaryAttackStartedState, smashDownStartedState, ladderClimbingState
      JumpingFlipState jumpingFlipState = AddToPack<JumpingFlipState>("Jump Flip", true, 0, false, fallingState);
      AddToPack<LandedState>("Land", true, .25f, false, jumpingFlipState);
    }

    public void GenerateFallState()
    {
      FallingState fallingState = AddToPack<FallingState>("Fall");
    }

    public void GenerateRunState()
    {
      AddToPack<RunningState>("Running");
    }

    public void GeneratePrimaryAttackState()
    {
      PrimaryAttackStartedState primaryAttackStartedState = AddToPack<PrimaryAttackStartedState>();
      AddToPack<PrimaryAttackPerformedState>("PrimaryAttackPerformed");
      AddToPack<PrimaryAttackCanceledState>();

      if (HasState(typeof(JumpingState)))
      {
        AddToPack<JumpingState>("Jump", true, 0, false, primaryAttackStartedState);
      }
      if (HasState(typeof(JumpingFlipState)))
      {
        AddToPack<JumpingFlipState>("Jump Flip", true, 0, false, primaryAttackStartedState);
      }
    }

    public void GenerateSecondaryAttackState()
    {
      SecondaryAttackStartedState secondaryAttackStartedState = AddToPack<SecondaryAttackStartedState>();
      AddToPack<SecondaryAttackPerformedState>("SecondaryAttackPerformed");
      AddToPack<SecondaryAttackCanceledState>();

      if (HasState(typeof(JumpingState)))
      {
        AddToPack<JumpingState>("Jump", true, 0, false, secondaryAttackStartedState);
      }
      if (HasState(typeof(JumpingFlipState)))
      {
        AddToPack<JumpingFlipState>("Jump Flip", true, 0, false, secondaryAttackStartedState);
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
        AddToPack<JumpingState>("Jump", true, 0, false, dashingState);
      }
      if (HasState(typeof(JumpingFlipState)))
      {
        AddToPack<JumpingFlipState>("Jump Flip", true, 0, false, dashingState);
      }
    }

    public void GenerateLadderClimbingAbilityStates()
    {
      AddToPack<LadderIdleState>("Ladder Idle");
      LadderClimbingState ladderClimbingState = AddToPack<LadderClimbingState>("Ladder Climb");
      if (HasState(typeof(JumpingState)))
      {
        AddToPack<JumpingState>("Jump", true, 0, false, ladderClimbingState);
      }
      if (HasState(typeof(JumpingFlipState)))
      {
        AddToPack<JumpingFlipState>("Jump Flip", true, 0, false, ladderClimbingState);
      }
    }

    public void GenerateSlideAbilityStates()
    {
      FallingState fallingState = (FallingState)Find(typeof(FallingState));
      AddToPack<SlidingState>("Slide", true, 0.5f, false, fallingState);
    }
#endif
  }
}