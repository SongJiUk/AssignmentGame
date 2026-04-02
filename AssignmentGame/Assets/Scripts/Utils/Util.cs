using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util 
{
   

    public static void SetState(this Animator _animator, Define.State _state)
    {
        _animator.SetInteger("State", (int)_state);
    }

}
