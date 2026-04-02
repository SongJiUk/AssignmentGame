using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonRoom : BaseController
{
    [SerializeField] PurchaseZone upgradeZone;
    int maxCount = 20;
    int currentCount = 0;

    public bool HasSpace => currentCount < maxCount;
    
    public void Enter()
    {
        currentCount++;
    }
}
