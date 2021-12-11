
using System;
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
      State foundState = States.Find(s => s == state);
      if (foundState == null)
      {
        LogExt.Warn<StatePack>($"State {foundState.ToString()} not found in state pack");
      }
      return state;
    }

    public bool HasState(State state)
    {
      return Find(state) != null;
    }

    public State Find(Type type)
    {
      State state = States.Find(state => state.GetType().Equals(type));
      if (state == null)
      {
        LogExt.Warn<StatePack>($"State {type.ToString()} not found in state pack");
      }
      return state;
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
        state.StatesCanUnfreeze = statesCanUnFreeze;
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
      IdleState idleState = AddToPack<IdleState>("Idle", false, 0, true);
      AddToPack<WalkingState>("Walking");
      // AddToPack<RunningState>("Running");

      // FallingState fallingState = AddToPack<FallingState>("Falling");

      // AddToPack<CrouchIdleState>("CrouchIdle");
      // AddToPack<CrouchWalkingState>("CrouchWalking");

      // AddToPack<DanglingState>("Dangling");
      // DashingState dashingState = AddToPack<DashingState>("Dashing", true);
      // AddToPack<LadderIdleState>("LadderIdle");
      // LadderClimbingState ladderClimbingState = AddToPack<LadderClimbingState>("LadderClimbing");
      // AddToPack<LedgeClimbingState>("LedgeClimbing");
      // LedgeGrabState ledgeGrabState = AddToPack<LedgeGrabState>("LedgeGrab");

      // AddToPack<WalkingUpSlopeState>("WalkingUpSlope");
      // AddToPack<RunningUpSlopeState>("RunningUpSlope");
      // AddToPack<WalkingDownSlopeState>("Walking");
      // AddToPack<RunningDownSlopeState>("Running");
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

      // JumpingState jumpingState = AddToPack<JumpingState>("Jumping", true, 0, false, ledgeGrabState, primaryAttackStartedState, secondaryAttackStartedState, fallingState, smashDownStartedState, dashingState, ladderClimbingState);
      // JumpingFlipState jumpingFlipState = AddToPack<JumpingFlipState>("JumpingFlip", true, 0, false, ledgeGrabState, primaryAttackStartedState, secondaryAttackStartedState, fallingState, smashDownStartedState, dashingState, ladderClimbingState);
      // DoubleJumpState doubleJumpState = AddToPack<DoubleJumpState>("Jumping", true, 0, false);

      // AddToPack<LandedState>("Landed", true, .25f, false, jumpingState, jumpingFlipState, doubleJumpState);
    }

    public void GenerateAttackAbilityStates()
    {
      PrimaryAttackStartedState primaryAttackStartedState = AddToPack<PrimaryAttackStartedState>();
      AddToPack<PrimaryAttackPerformedState>("PrimaryAttackPerformed");
      AddToPack<PrimaryAttackCanceledState>();
      SecondaryAttackStartedState secondaryAttackStartedState = AddToPack<SecondaryAttackStartedState>();
      AddToPack<SecondaryAttackPerformedState>("SecondaryAttackPerformed");
      AddToPack<SecondaryAttackCanceledState>();
    }
#endif
  }
}