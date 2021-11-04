using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  using SceneGraph;

  public enum CharacterType { Player, AI }
  [AddComponentMenu("RFG/Platformer/Character/Character")]
  public class Character : MonoBehaviour, IPooledObject
  {
    [Header("Type")]
    public CharacterType CharacterType = CharacterType.Player;

    [Header("Location")]
    public Transform SpawnAt;

    [Header("Settings")]
    public InputPack InputPack;
    public SettingsPack SettingsPack;

    [Header("Character State")]
    public RFG.StateMachine CharacterState;

    [Header("Movement State")]
    public RFG.StateMachine MovementState;

    [field: SerializeField] public bool IsReady { get; set; } = false;

    [HideInInspector]
    public StateCharacterContext Context => _characterContext;
    public CharacterController2D Controller => _controller;
    private StateCharacterContext _characterContext = new StateCharacterContext();
    private CharacterController2D _controller;
    private List<Component> _abilities;
    private List<SceneDoor> _sceneDoors;

    private void Awake()
    {
      InitContext();
      InitAbilities();
      IsReady = true;
    }

    private void InitContext()
    {
      _characterContext = new StateCharacterContext();
      _controller = GetComponent<CharacterController2D>();
      _characterContext.transform = transform;
      _characterContext.animator = GetComponent<Animator>();
      _characterContext.character = this;
      _characterContext.controller = _controller;
      _characterContext.inputPack = InputPack;
      _characterContext.DefaultSettingsPack = SettingsPack;
      _characterContext.healthBehaviour = GetComponent<HealthBehaviour>();

      // Bind the character context to the state context
      CharacterState.Init();
      CharacterState.Bind(_characterContext);

      MovementState.Init();
      MovementState.Bind(_characterContext);
    }

    private void InitAbilities()
    {
      Component[] abilities = GetComponents(typeof(IAbility)) as Component[];
      if (abilities.Length > 0)
      {
        _abilities = new List<Component>(abilities);
      }
    }

    private void Update()
    {
      CharacterState.Update();
      MovementState.Update();
    }

    public void SetMovementStatePack(RFG.StatePack statePack)
    {
      MovementState.SetStatePack(statePack);
    }

    public void RestoreDefaultMovementStatePack()
    {
      MovementState.RestoreDefaultStatePack();
    }

    public void OverrideSettingsPack(SettingsPack settings)
    {
      _characterContext.OverrideSettingsPack(settings);
    }

    public void ResetSettingsPack()
    {
      _characterContext.ResetSettingsPack();
    }

    public void OnObjectSpawn(params object[] objects)
    {
      CharacterState.ResetToDefaultState();
      MovementState.ResetToDefaultState();
    }

    public void SetSpawnPosition()
    {
      // Default to the postion they are currently at
      SpawnAt = transform;

      if (CharacterType == CharacterType.Player)
      {
        // If the character is a player and there is a current checkpoint
        if (Checkpoint.currentCheckpoint != null)
        {
          SpawnAt.position = Checkpoint.currentCheckpoint.transform.position;
        }
        else
        {
          // Else if there is no current checkpoint check to see if the came from a scene door
          // If they have spawn at that scene door
          SceneDoor toSceneDoor = SceneGraphManager.Instance.FindToSceneDoor();
          if (toSceneDoor != null)
          {
            SpawnAt.position = toSceneDoor.transform.position + toSceneDoor.spawnOffset;
          }
        }
      }
    }

    public void Kill()
    {
      StartCoroutine(KillCo());
    }

    public IEnumerator KillCo()
    {
      yield return new WaitForSeconds(0.1f);
      CharacterState.ChangeState(typeof(DeathState));
    }

    public void Respawn()
    {
      StartCoroutine(RespawnCo());
    }

    public IEnumerator RespawnCo()
    {
      yield return new WaitForSecondsRealtime(1f);
      CharacterState.ChangeState(typeof(SpawnState));
    }

    public void EnableAllAbilities()
    {
      if (_abilities != null)
      {
        foreach (Behaviour ability in _abilities)
        {
          ability.enabled = true;
        }
      }
    }

    public void DisableAllAbilities()
    {
      if (_abilities != null)
      {
        foreach (Behaviour ability in _abilities)
        {
          ability.enabled = false;
        }
      }
    }

    private void OnEnable()
    {
      if (_characterContext.inputPack != null)
      {
        if (_characterContext.inputPack.Movement != null)
        {
          _characterContext.inputPack.Movement.action.Enable();
        }
      }
    }

    private void OnDisable()
    {
      if (_characterContext.inputPack != null)
      {
        if (_characterContext.inputPack.Movement != null)
        {
          _characterContext.inputPack.Movement.action.Disable();
        }
      }
    }

  }
}