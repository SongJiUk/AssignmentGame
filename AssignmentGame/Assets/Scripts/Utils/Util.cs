using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util 
{
   

    public static void SetState(this Animator _animator, Define.PlayerState _state)
    {
        _animator.SetInteger("PlayerState", (int)_state);
    }

}
