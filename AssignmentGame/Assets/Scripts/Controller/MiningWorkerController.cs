using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class MiningWorkerController : NPCController
{
    Define.MiningWorkerState workerState;
    MineralZone mineralzone;
    MachineZone machineZone;
    Mineral mineral;

    const float MiningInterval = 0.5f;

    public override bool Init()
    {
        if (!base.Init()) return false;

        return true;
    }

    public void SetInfo(MineralZone _zone, MachineZone _machineZone)
    {
        mineralzone = _zone;
        machineZone = _machineZone;
        ChangeState(Define.MiningWorkerState.MovingToMineral);
    }
    public void ChangeState(Define.MiningWorkerState _workerState)
    {
        workerState = _workerState;
        switch (workerState)
        {
            case Define.MiningWorkerState.MovingToMineral:
                // TOOD : 미네랄로 이동 
                AsyncMoveToNearMineral().Forget();
                break;

            case Define.MiningWorkerState.Mining:
                //TODO : 미네랄 공격
                StartMining().Forget();
                break;
        }
    }


    async UniTaskVoid AsyncMoveToNearMineral()
    {
        while (true)
        {
            mineral = mineralzone.GetNearestMineral(transform.position);
            if (mineral != null) break;
            await UniTask.Delay(500);
        }

        mineral.SetTargeted(true);
        await AsyncMoveToPosition(mineral.transform.position);

        if(!mineral.gameObject.activeSelf)
        {
            mineral.SetTargeted(false);
            mineral = null;
            AsyncMoveToNearMineral().Forget();
            return;
        }

        ChangeState(Define.MiningWorkerState.Mining);
    }

    async UniTaskVoid StartMining()
    {
        mineral.machineZone = machineZone;
        while (mineral != null && mineral.gameObject.activeSelf)
        {
            anim.SetState(Define.State.Mining);

            await UniTask.Delay((int)(MiningInterval * 1000));
        }
        mineral = null;
        ChangeState(Define.MiningWorkerState.MovingToMineral);
    }

    public void OnMiningAnimEnd()
    {
        if (mineral == null || !mineral.gameObject.activeSelf) return;

        mineral.NPCMining();
    }
}
