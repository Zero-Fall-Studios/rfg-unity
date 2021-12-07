using UnityEngine.InputSystem;

namespace RFG
{
  public class StateCharacterContext : StateAnimatorContext
  {
    public Character character;
    public CharacterController2D controller;
    public PlayerInput playerInput;

    // Packs
    public SettingsPack settingsPack
    {
      get
      {
        if (CurrentSettingsPack == null)
        {
          ResetSettingsPack();
        }
        return CurrentSettingsPack;
      }
    }

    public SettingsPack CurrentSettingsPack = null;
    public SettingsPack DefaultSettingsPack = null;

    // Behaviours
    public HealthBehaviour healthBehaviour;

    public StateCharacterContext()
    {
      CurrentSettingsPack = null;
      DefaultSettingsPack = null;
    }

    public void ResetSettingsPack()
    {
      CurrentSettingsPack = DefaultSettingsPack;
    }

    public void OverrideSettingsPack(SettingsPack settings)
    {
      CurrentSettingsPack = settings;
    }
  }
}
