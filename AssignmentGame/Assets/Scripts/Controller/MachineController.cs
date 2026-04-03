using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class MachineController : BaseController
{

    float convertDelay = 0.5f;

    private bool isRunning = false;
    HandCuffZone handCuffZone;
    MachineZone machineZone;
    void Start()
    {
        Init();
    }


    public override bool Init()
    {
        if (!base.Init()) return false;
        if (handCuffZone == null) handCuffZone = gameObject.GetComponentInChildren<HandCuffZone>();
        if (machineZone == null) machineZone = gameObject.GetComponentInChildren<MachineZone>();
        machineZone.OnMineralArrive += AddMineral;

        return true;
    }


    public void AddMineral(Transform _mineral)
    {
        if (!isRunning)
            AsyncConvert().Forget();
    }


    async UniTaskVoid AsyncConvert()
    {
        isRunning = true;

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

        while (machineZone.MineralCount > 0)
        {
            if (handCuffZone.IsFull)
            {
                await UniTask.WaitUntil(() => !handCuffZone.IsFull);
            }

            Transform mineral = machineZone.RemoveMineral();
            await UniTask.Delay(TimeSpan.FromSeconds(convertDelay));
            Managers.ObjectM.DeSpawn<Transform>(mineral);
            handCuffZone.AddHandCuff();
        }

        isRunning = false;
    }


    void OnDestroy()
    {
        machineZone.OnMineralArrive -= AddMineral;
    }

}
