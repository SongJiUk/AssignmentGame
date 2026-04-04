using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class NPCController : BaseController
{
    protected float speed = 2f;
    protected Animator anim;

    public override bool Init()
    {
        if (!base.Init()) return false;
        if (anim == null) anim = GetComponent<Animator>();

        return true;
    }

   

    protected virtual async UniTask AsyncMoveToPosition(Vector3 _targetPos)
    {
        transform.LookAt(_targetPos);
        while ((transform.position - _targetPos).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, Time.deltaTime * speed);
            anim.SetState(Define.State.Run);
            await UniTask.Yield();
        }
        anim.SetState(Define.State.Idle);
    }
}
