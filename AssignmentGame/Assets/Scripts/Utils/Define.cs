using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum State
    {
        Idle,
        Run,
        Mining,
        SitMachine
    }

    public enum PrisonerState
    {
        Waiting,
        WaitingInLine,
        WaitingHandCuff,
        WalkingRoom,
        WaitingRoom,
        InRoom
    }

    public enum MiningWorkerState
    {
        MovingToMineral,
        Mining,
    }

    public enum JailerState
    {
        Walk
    }


    public enum WeaponState
    {
        Pick,
        Drill,
        SitMachine,

    }

    public enum ItemType
    {
        Money,
        Mineral,
        Handcuffs
    }

    public enum ItemState
    {
        OnPlayer,
        OnMachine,
        OnPrisoner,
        OnGround,
    }

    public enum UnLockType
    {
        None,
        Drill,
        MiningMachine,
    }
    public enum PurchaseType
    {
        ActivateObject,
        UnLockPlayer,
        SpawnNPC,
        UpgradeRoom
    }
    public enum NPCType
    {
        None,
        MiningWorker,
        Jailer
    }

    public enum TutorialStep
    {
        GoToMineralZone,
        MineMineral,
        GoToMachineZone,
        GetHandCuff,
        GoToPrisonerZone,
        GetMoney,
        Complete
        
    }
}
