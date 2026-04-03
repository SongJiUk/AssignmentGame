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
    [SerializeField] WeaponData currentWeaponData;
    public WeaponData CurrentWeaponData => currentWeaponData;
    GameObject currentWeapon;
    Joystick joyStick;
    Animator anim;
    FollowStackSystem followStackSystem;
    public FollowStackSystem FollowStackSystem { get { return followStackSystem; } }
    const float moveSpeed = 3f;
    const float moveToRot = 5f;
    Define.State currentState = State.Idle;
    Mineral mineral;
    bool isBoardingMachine = false;
    public override bool Init()
    {
        if (!base.Init()) return false;
        if (anim == null) anim = gameObject.GetComponent<Animator>();
        if (followStackSystem == null) followStackSystem = gameObject.GetComponent<FollowStackSystem>();
        currentWeapon = pick;

        EquipPick(false);
        drill.SetActive(false);
        miningMachine.SetActive(false);
        return true;
    }

    public void EquipPick(bool _isEquip)
    {
        currentWeapon.SetActive(_isEquip);
    }
    public void OnChangeHoldHandCuff(bool _isHold)
    {
        anim.SetBool("IsHoldHandCuff", _isHold);
    }

    public void SetJoyStick(Joystick _joystick)
    {
        joyStick = _joystick;
    }


    public void OnMineralEnter(Mineral _mineral)
    {
        mineral = _mineral;
        switch (currentWeaponData.weaponState)
        {
            case Define.WeaponState.Pick:
                bool IsMining = anim.GetBool("IsMining");
                if (IsMining) return;

                anim.SetBool("IsMining", true);
                break;

            case Define.WeaponState.Drill:

                _mineral.Mining();
                _mineral.zone.ReSpawnMineral(_mineral, _mineral.reSpawnTime);
                _mineral.gameObject.SetActive(false);
                break;

            case Define.WeaponState.SitMachine:


                _mineral.Mining();
                _mineral.zone.ReSpawnMineral(_mineral, _mineral.reSpawnTime);
                _mineral.gameObject.SetActive(false);
                break;

        }


    }

    public void OnEnterMineralZone()
    {
        switch (currentWeaponData.weaponState)
        {
            case Define.WeaponState.Pick:
                EquipPick(true);
                break;

            case Define.WeaponState.Drill:
                EquipPick(true);
                anim.SetBool("IsDrilling", true);
                break;
            case Define.WeaponState.SitMachine:
                EquipPick(true);
                isBoardingMachine = true;
                anim.SetState(State.SitMachine);
                break;

        }
    }

    public void OnExitMineralZone()
    {
        switch (currentWeaponData.weaponState)
        {
            case Define.WeaponState.Pick:
                EquipPick(false);
                break;

            case Define.WeaponState.Drill:
                EquipPick(false);
                anim.SetBool("IsDrilling", false);
                break;

            case Define.WeaponState.SitMachine:
                EquipPick(false);
                isBoardingMachine = false;
                break;
        }
    }

    public void OnMineralExit(Mineral _mineral)
    {
        if (mineral != _mineral) return;

        switch (currentWeaponData.weaponState)
        {
            case Define.WeaponState.Pick:
                anim.SetBool("IsMining", false);
                break;

            case Define.WeaponState.Drill:
                break;

            case Define.WeaponState.SitMachine:
                break;

        }
    }

    public void OnMiningAnimEnd()
    {
        mineral.Mining();
        mineral.zone.ReSpawnMineral(mineral, mineral.reSpawnTime);
        mineral.gameObject.SetActive(false);
    }

    public void OnUnLock(Define.UnLockType _type)
    {
        switch (_type)
        {
            case Define.UnLockType.Drill:
                currentWeaponData = currentWeaponData.nextWeapon;
                currentWeapon = drill;
                //currentWeapon.gameObject.SetActive(true);
                break;

            case Define.UnLockType.MiningMachine:
                currentWeaponData = currentWeaponData.nextWeapon;
                currentWeapon = miningMachine;
                //currentWeapon.gameObject.SetActive(true);
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
                if (!isBoardingMachine)
                {
                    anim.SetState(Define.State.Idle);
                    currentState = State.Idle;
                }

            }

            return;
        }

        State nextState = isBoardingMachine ? State.SitMachine : State.Run;

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
