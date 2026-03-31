using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum PlayerState
    {
        Idle,
        Run,
        HoldHandCuff,
        Mining,
        DrillMining,
        SitMachine
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

}
