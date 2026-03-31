using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class MachineController : BaseController
{

    float convertDelay = 1f;

    Stack<Transform> inputStack = new();
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

        return true;
    }


    public void AddMineral(Transform _mineral)
    {
        inputStack.Push(_mineral);

        if (!isRunning)
            AsyncConvert().Forget();
    }


    async UniTaskVoid AsyncConvert()
    {
        isRunning = true;
        while (inputStack.Count > 0)
        {
            Transform mineral = inputStack.Pop();
            Managers.ObjectM.DeSpawn<Transform>(mineral);
            await UniTask.Delay(TimeSpan.FromSeconds(convertDelay));


            handCuffZone.AddHandCuff();
        }

        isRunning = false;
    }
}
