
using UnityEngine;

namespace RFG
{
  using BehaviourTree;

  [AddComponentMenu("RFG/Platformer/Character/AI Behaviours/AI Brain")]
  public class AIBrainBehaviour : MonoBehaviour, INodeContext
  {
    [Tooltip("The speed / time it takes to rotate and make decisions")]
    public float RotateSpeed;

    [Tooltip("The stun cooldown time to start making decisions again")]
    public float StunCooldownTime = 5f;

    [Header("Weapons To Equip")]
    public WeaponItem PrimaryWeapon;
    public WeaponItem SecondaryWeapon;
    public bool HasAggro { get; set; }

    [HideInInspector]
    public AIAjent Context => _ctx;
    private Transform _transform;
    private Character _character;
    private CharacterController2D _controller;
    private Animator _animator;
    private Tween _movementPath;
    private EquipmentSet _equipmentSet;
    private Aggro _aggro;
    private AIAjent _ctx;

    private void Awake()
    {
      _transform = transform;
      _character = GetComponent<Character>();
      _controller = GetComponent<CharacterController2D>();
      _animator = GetComponent<Animator>();
      _movementPath = GetComponent<Tween>();
      _equipmentSet = GetComponent<EquipmentSet>();
      _aggro = GetComponent<Aggro>();

      // Equip Weapons
      if (PrimaryWeapon != null)
      {
        _equipmentSet.EquipPrimaryWeapon(PrimaryWeapon);
      }
      if (SecondaryWeapon != null)
      {
        _equipmentSet.EquipSecondaryWeapon(SecondaryWeapon);
      }
    }

    private void Start()
    {
      _ctx = new AIAjent();
      _ctx.transform = _transform;
      _ctx.character = _character;
      _ctx.characterContext = _character.Context;
      _ctx.controller = _controller;
      _ctx.aggro = _aggro;
      _ctx.animator = _animator;
      _ctx.movementPath = _movementPath;
      _ctx.equipmentSet = _equipmentSet;
      _ctx.aiState = this;
      _ctx.RotateSpeed = RotateSpeed;
      _ctx.RunningPower = _character.Context.settingsPack.RunningPower;
    }

    private void OnAggroChange(bool aggro)
    {
      HasAggro = aggro;
    }

    private void OnEnable()
    {
      if (_aggro != null)
      {
        _aggro.OnAggroChange += OnAggroChange;
      }
    }

    private void OnDisable()
    {
      if (_aggro != null)
      {
        _aggro.OnAggroChange -= OnAggroChange;
      }
    }
  }
}