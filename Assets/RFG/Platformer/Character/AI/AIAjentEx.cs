using UnityEngine;

namespace RFG
{
  public static class AIAjentEx
  {
    public static bool JustRotated(this AIAjent ctx)
    {
      if (ctx.JustRotated)
      {
        if (Time.time - ctx.LastTimeRotated < ctx.RotateSpeed)
        {
          ctx.controller.SetHorizontalForce(0);
          ctx.controller.SetVerticalForce(0);
          return true;
        }
      }
      return false;
    }

    public static bool RotateTowards(this AIAjent ctx)
    {
      if (ctx.aggro != null && ctx.aggro.target2 != null)
      {
        bool didRotate = ctx.controller.RotateTowards(ctx.aggro.target2);
        if (didRotate && ctx.RotateSpeed > 0)
        {
          ctx.JustRotated = true;
          ctx.LastTimeRotated = Time.time;
          ctx.controller.SetHorizontalForce(0);
          ctx.controller.SetVerticalForce(0);
          return true;
        }
      }
      return false;
    }

    public static bool TouchingWalls(this AIAjent ctx)
    {
      if ((ctx.controller.State.IsWalking || ctx.controller.State.IsRunning))
      {
        if ((ctx.controller.State.IsCollidingLeft) || (ctx.controller.State.IsCollidingRight))
        {
          ctx.controller.State.IsWalking = false;
          ctx.controller.State.IsRunning = false;
          return true;
        }
      }
      return false;
    }

    public static void FlipOnCollision(this AIAjent ctx)
    {
      if (ctx.controller.State.IsCollidingLeft || ctx.controller.State.IsCollidingRight)
      {
        ctx.controller.Flip();
        ctx.JustRotated = true;
        ctx.LastTimeRotated = Time.time;
      }
    }

    public static void FlipOnLevelBoundsCollision(this AIAjent ctx)
    {
      if (ctx.controller.State.TouchingLevelBounds)
      {
        ctx.controller.Flip();
        ctx.JustRotated = true;
        ctx.LastTimeRotated = Time.time;
      }
    }

    public static bool IsDangling(this AIAjent ctx)
    {
      if (ctx.characterContext.settingsPack == null || !ctx.characterContext.settingsPack.CanDangle)
        return false;

      SettingsPack _settings = ctx.characterContext.settingsPack;

      Vector3 raycastOrigin = Vector3.zero;
      if (ctx.controller.State.IsFacingRight)
      {
        raycastOrigin = ctx.transform.position + _settings.DanglingRaycastOrigin.x * Vector3.right + _settings.DanglingRaycastOrigin.y * ctx.transform.up;
      }
      else
      {
        raycastOrigin = ctx.transform.position - _settings.DanglingRaycastOrigin.x * Vector3.right + _settings.DanglingRaycastOrigin.y * ctx.transform.up;
      }

      RaycastHit2D hit = RFG.Physics2D.RayCast(raycastOrigin, -ctx.transform.up, _settings.DanglingRaycastLength, ctx.controller.PlatformMask | ctx.controller.OneWayPlatformMask | ctx.controller.OneWayMovingPlatformMask, Color.gray, true);

      if (!hit)
      {
        return true;
      }
      return false;
    }

    public static void FlipOnDangle(this AIAjent ctx)
    {
      if (ctx.characterContext.settingsPack == null || !ctx.characterContext.settingsPack.CanDangle)
        return;

      if (ctx.IsDangling())
      {
        ctx.controller.Flip();
        ctx.JustRotated = true;
        ctx.LastTimeRotated = Time.time;
      }
    }

    public static bool PauseOnDangle(this AIAjent ctx)
    {
      if (ctx.characterContext.settingsPack == null || !ctx.characterContext.settingsPack.CanDangle)
        return false;

      if (ctx.IsDangling())
      {
        ctx.MoveHorizontally(0);
        return true;
      }
      return false;
    }

    public static void MoveHorizontally(this AIAjent ctx, float speed)
    {
      if (speed == 0)
      {
        ctx.controller.SetHorizontalForce(speed);
        ctx.character.MovementState.ChangeState(typeof(IdleState));
        return;
      }

      float _normalizedHorizontalSpeed = 0f;

      if (ctx.controller.State.IsFacingRight)
      {
        _normalizedHorizontalSpeed = 1f;
      }
      else
      {
        _normalizedHorizontalSpeed = -1f;
      }

      float movementFactor = ctx.controller.Parameters.GroundSpeedFactor;
      float movementSpeed = _normalizedHorizontalSpeed * speed * ctx.controller.Parameters.SpeedFactor;
      float horizontalMovementForce = Mathf.Lerp(ctx.controller.Speed.x, movementSpeed, Time.deltaTime * movementFactor);

      ctx.controller.SetHorizontalForce(horizontalMovementForce);
    }

    public static void MoveVertically(this AIAjent ctx, float speed)
    {
      if (ctx.characterContext.settingsPack.CanFollowVertically)
      {
        float normalizedVerticalSpeed = 0f;
        if (ctx.aggro.target2.transform.position.y > ctx.transform.position.y)
        {
          normalizedVerticalSpeed = 1f;
        }
        else
        {
          normalizedVerticalSpeed = -1f;
        }
        float airMovementFactor = ctx.controller.Parameters.AirSpeedFactor;
        float airMovementSpeed = normalizedVerticalSpeed * speed * ctx.controller.Parameters.SpeedFactor;
        float verticalMovementForce = Mathf.Lerp(ctx.controller.Speed.y, airMovementSpeed, Time.deltaTime * airMovementFactor);
        ctx.controller.SetVerticalForce(verticalMovementForce);
      }
    }

    public static void Attack(this AIAjent ctx)
    {
      if (ctx.equipmentSet == null)
      {
        return;
      }
      int decisionIndex = UnityEngine.Random.Range(0, 100);
      if (decisionIndex < 900)
      {
        ctx.equipmentSet.PrimaryWeapon?.Perform();
      }
      else
      {
        ctx.equipmentSet.SecondaryWeapon?.Perform();
      }
    }

    public static void PrimaryAttack(this AIAjent ctx)
    {
      ctx.character.MovementState.ChangeState(typeof(PrimaryAttackStartedState));
    }

    public static void SecondaryAttack(this AIAjent ctx)
    {
      ctx.character.MovementState.ChangeState(typeof(SecondaryAttackStartedState));
    }

    public static float WalkOrRun(this AIAjent ctx)
    {
      if (ctx.characterContext.settingsPack == null)
        return 0;

      bool useRunning = !ctx.RunningCooldown;

      SettingsPack _settings = ctx.characterContext.settingsPack;

      float speed = useRunning ? _settings.RunningSpeed : _settings.WalkingSpeed;

      if (useRunning)
      {
        ctx.RunningPower -= _settings.PowerGainPerFrame;
        if (ctx.RunningPower <= 0)
        {
          ctx.RunningCooldown = true;
          speed = _settings.WalkingSpeed;
          ctx.LastTimeRunningCooldown = Time.time;
          ctx.controller.State.IsRunning = false;
          ctx.controller.State.IsWalking = true;
          ctx.character.MovementState.ChangeState(typeof(WalkingState));
        }
        else
        {
          ctx.character.MovementState.ChangeState(typeof(RunningState));
        }
      }
      else
      {
        ctx.character.MovementState.ChangeState(typeof(WalkingState));
        if (Time.time - ctx.LastTimeRunningCooldown > _settings.CooldownTimer)
        {
          ctx.RunningPower += _settings.PowerGainPerFrame;
          if (ctx.RunningPower >= _settings.RunningPower)
          {
            ctx.RunningPower = _settings.RunningPower;
            ctx.RunningCooldown = false;
          }
        }
      }
      return speed;
    }

  }
}