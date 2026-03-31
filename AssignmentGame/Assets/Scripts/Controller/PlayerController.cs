using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using static Define;
using static Util;

public class PlayerController : CreatureController
{


    Joystick joyStick;
    Animator anim;
    FollowStackSystem followStackSystem;
    public FollowStackSystem FollowStackSystem { get { return followStackSystem; } }
    const float moveSpeed = 3f;
    const float moveToRot = 5f;
    Define.PlayerState currentState = PlayerState.Idle;
    int handCuffCount;
    Mineral mineral;
    public override bool Init()
    {
        if (!base.Init()) return false;
        if (anim == null) anim = gameObject.GetComponent<Animator>();
        if (followStackSystem == null) followStackSystem = gameObject.GetComponent<FollowStackSystem>();

        return true;
    }

    public void OnInArea()
    {

    }

    public void SetJoyStick(Joystick _joystick)
    {
        joyStick = _joystick;
    }


    public void OnMineralEnter(Mineral _mineral)
    {
        bool IsMining = anim.GetBool("IsMining");
        mineral = _mineral;

        if (IsMining) return;


        anim.SetBool("IsMining", true);
    }

    public void OnMineralExit(Mineral _mineral)
    {
        if (mineral != _mineral) return;
        anim.SetBool("IsMining", false);
    }

    public void OnMiningAnimEnd()
    {
        mineral.Mining();
        mineral.zone.ReSpawnMineral(mineral, mineral.reSpawnTime);
        mineral.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (joyStick == null) return;

        float x = joyStick.Horizontal;
        float y = joyStick.Vertical;

        if (Mathf.Abs(x) < 0.1f && Mathf.Abs(y) < 0.1f)
        {
            if (currentState != PlayerState.Idle)
            {
                anim.SetState(Define.PlayerState.Idle);
                currentState = PlayerState.Idle;
            }

            return;
        }

        PlayerState nextState = handCuffCount > 0 ? PlayerState.HoldHandCuff : PlayerState.Run;

        if (currentState != nextState)
        {
            currentState = nextState;
            anim.SetState(currentState);
        }


        Vector3 dir = new Vector3(x, 0, y).normalized;
        transform.position += dir * Time.deltaTime * moveSpeed;
        var rot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * moveToRot);

    }
}
