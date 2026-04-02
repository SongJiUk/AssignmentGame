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
        DrillMining,
        SitMachine
    }

    public enum PrisonorState
    { 
        Waiting,
        WaitingInLine,
        WaitingHandCuff,
        WalkingRoom,
        WaitingRoom,
        InRoom
    }

    public enum WeaponState
    {
        Pick,
        Drill,
        SitMachine,
        MineWorker

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
        Drill,
        MiningMachine,
    }
}
