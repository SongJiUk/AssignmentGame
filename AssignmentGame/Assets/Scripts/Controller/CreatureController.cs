using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    bool isInit = false;
    public virtual bool Init()
    {
        if (isInit) return false;
        isInit = true;

        return true;

    }

}
