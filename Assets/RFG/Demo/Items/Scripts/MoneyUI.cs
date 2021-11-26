using UnityEngine;
using TMPro;

namespace RFG
{
  public class MoneyUI : MonoBehaviour
  {
    [field: SerializeField] public Inventory Inventory { get; set; }
    [field: SerializeField] private TMP_Text Amount { get; set; }

    private void Start()
    {
      Amount.SetText(Inventory.Money.ToString());
    }

    private void OnEnable()
    {
      Inventory.OnUpdateMoney += OnUpdateMoney;
    }

    private void OnDisable()
    {
      Inventory.OnUpdateMoney += OnUpdateMoney;
    }

    private void OnUpdateMoney(int amount)
    {
      Amount.SetText(amount.ToString());
    }
  }
}