using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;

    public void MoneyTextCheck()
    {
        moneyText.text = Managers.GameM.Money.ToString();
    }
}
