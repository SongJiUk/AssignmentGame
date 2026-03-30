using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerController : CreatureController
{

    public override bool Init()
    {
        if (!base.Init()) return false;

        return true;
    }

    public void OnInArea()
    {

    }
}
