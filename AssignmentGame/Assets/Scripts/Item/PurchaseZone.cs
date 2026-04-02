using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchaseZone : BaseController
{
    [SerializeField] PurchaseZoneData data;
    [SerializeField] Image BGImage;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] Slider costSlider;
    [SerializeField] GameObject[] targets;
    [SerializeField] PurchaseZone[] nextZone;

    int remainCost;

    private void Start()
    {
        Init();
    }
    public override bool Init()
    {
        if (!base.Init()) return false;
        remainCost = data.cost;


        return true;
    }
}
