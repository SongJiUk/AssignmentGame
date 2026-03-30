using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
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
