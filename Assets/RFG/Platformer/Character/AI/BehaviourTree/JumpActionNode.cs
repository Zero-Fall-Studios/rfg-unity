using UnityEngine;

namespace RFG
{
  using BehaviourTree;
  public class JumpActionNode : ActionNode
  {
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
      AIBrainBehaviour brain = context as AIBrainBehaviour;
      if (brain.Context.characterContext.settingsPack == null)
        return State.Failure;

      if (brain.Context.characterContext.controller.State.JustGotGrounded)
      {
        brain.Context.characterContext.controller.SetHorizontalForce(0);
        brain.Context.characterContext.character.MovementState.ChangeState(typeof(LandedState));
        return State.Success;
      }
      else if (brain.Context.characterContext.controller.State.IsGrounded)
      {
        JumpStart(brain.Context);
      }

      if (brain.Context.characterContext.character.IsInAirMovementState && brain.Context.characterContext.controller.Speed.y < 0)
      {
        brain.Context.characterContext.controller.State.IsFalling = true;
        brain.Context.characterContext.controller.State.IsJumping = false;
        brain.Context.characterContext.character.MovementState.ChangeState(typeof(FallingState));
      }

      return State.Running;
    }

    public void JumpStart(AIAjent ctx)
    {
      if (!CanJump(ctx))
      {
        return;
      }

      SettingsPack settings = ctx.characterContext.settingsPack;

      // Jump
      ctx.character.MovementState.ChangeState(typeof(JumpingState));
      ctx.controller.State.IsFalling = false;
      ctx.controller.State.IsJumping = true;
      ctx.controller.AddVerticalForce(Mathf.Sqrt(2f * settings.JumpHeight * Mathf.Abs(ctx.controller.Parameters.Gravity)));

      // // Move horizontally
      // float normalizedHorizontalSpeed = 0f;
      // if (ctx.controller.State.IsFacingRight)
      // {
      //   normalizedHorizontalSpeed = 1f;
      // }
      // else
      // {
      //   normalizedHorizontalSpeed = -1f;
      // }

      // float speed = settings.WalkingSpeed;
      // if (ctx.aggro != null && ctx.aggro.HasAggro)
      // {
      //   speed = settings.RunningSpeed;
      // }

      // float movementFactor = ctx.controller.Parameters.AirSpeedFactor;
      // float movementSpeed = normalizedHorizontalSpeed * speed * ctx.controller.Parameters.SpeedFactor;
      // float horizontalMovementForce = Mathf.Lerp(ctx.controller.Speed.x, movementSpeed, Time.deltaTime * movementFactor);

      // ctx.controller.SetHorizontalForce(horizontalMovementForce);
    }

    private bool CanJump(AIAjent ctx)
    {
      SettingsPack settings = ctx.characterContext.settingsPack;
      if (settings.Restrictions == JumpRestrictions.CanJumpAnywhere)
      {
        return true;
      }
      if (settings.Restrictions == JumpRestrictions.CanJumpOnGround && ctx.controller.State.IsGrounded)
      {
        return true;
      }
      return false;
    }

  }
}