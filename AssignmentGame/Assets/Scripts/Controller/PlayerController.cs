using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using static Define;
using static Util;

public class PlayerController : CreatureController
{

    [SerializeField] GameObject pick;
    [SerializeField] GameObject drill;
    [SerializeField] GameObject miningMachine;
    Joystick joyStick;
    Animator anim;
    FollowStackSystem followStackSystem;
    public FollowStackSystem FollowStackSystem { get { return followStackSystem; } }
    const float moveSpeed = 3f;
    const float moveToRot = 5f;
    Define.State currentState = State.Idle;
    int handCuffCount;
    Mineral mineral;
    public override bool Init()
    {
        if (!base.Init()) return false;
        if (anim == null) anim = gameObject.GetComponent<Animator>();
        if (followStackSystem == null) followStackSystem = gameObject.GetComponent<FollowStackSystem>();
        EquipPick(false);
        drill.SetActive(false);
        miningMachine.SetActive(false);
        return true;
    }

    public void EquipPick(bool _isEquip)
    {
        pick.SetActive(_isEquip);
    }
    public void OnChangeHoldHandCuff(bool _isHold)
    {
        anim.SetBool("IsHoldHandCuff", _isHold);
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

    public void OnUnLock(Define.UnLockType _type)
    {
        switch(_type)
        {
            case Define.UnLockType.Drill:
                //드릴 활성화 + 애니메이션 전환
                break;

            case Define.UnLockType.MiningMachine:
                //기계 활성화 + 애니메이션 전환
                break;

        }
    }

    private void Update()
    {
        if (joyStick == null) return;

        float x = joyStick.Horizontal;
        float y = joyStick.Vertical;

        if (Mathf.Abs(x) < 0.1f && Mathf.Abs(y) < 0.1f)
        {
            if (currentState != State.Idle)
            {
                anim.SetState(Define.State.Idle);
                currentState = State.Idle;
            }

            return;
        }

        State nextState = State.Run;

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
