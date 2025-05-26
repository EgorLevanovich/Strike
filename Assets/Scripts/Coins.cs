using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coins : MonoBehaviour
{
    public int price = 50;
    public GameObject _itemPrefab;
    public Button _buttonPrefab;
    public Text _preseText;

    private void Start()
    {
        _preseText.text = price.ToString();
        _buttonPrefab.onClick.AddListener(TryBuyItem);
    }

    private void TryBuyItem()
    {
        if(Money.Instance.Coins >= price)
        {
            Money.Instance.AddCoins(-price);
            GiveItem();
        }
    }

    private void GiveItem()
    {
        if(_itemPrefab != null)
        {
            Instantiate(_itemPrefab, transform.position, Quaternion.identity);
        }
        Debug.Log("Item is added:" + _itemPrefab.name);
    }
}
