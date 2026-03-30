using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : BaseController
{
    public override bool Init()
    {
        if (!base.Init()) return false;

        return true;
    }
}
