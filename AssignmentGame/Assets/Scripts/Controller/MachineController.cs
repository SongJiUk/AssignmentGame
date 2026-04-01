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

        await UniTask.WaitUntil(() => machineZone.InFlightCount == 0);

        while (machineZone.MineralCount > 0)
        {
            Transform mineral = machineZone.RemoveMineral();
            await UniTask.Delay(TimeSpan.FromSeconds(convertDelay));
            Managers.ObjectM.DeSpawn<Transform>(mineral);

            handCuffZone.AddHandCuff();
        }

        isRunning = false;
    }


    //재정렬코드(시작할때 첫번째게 빠져보이면 위가 비어보임)
    void OnDestroy()
    {
        machineZone.OnMineralArrive -= AddMineral;
    }

}
