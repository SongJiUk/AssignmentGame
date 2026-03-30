using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerController : CreatureController
{
    Joystick joyStick;
    float moveSpeed = 3f;
    public override bool Init()
    {
        if (!base.Init()) return false;

        return true;
    }

    public void OnInArea()
    {

    }

    public void SetJoyStick(Joystick _joystick)
    {
        joyStick = _joystick;
    }

    private void Update()
    {
        if (joyStick == null) return;

        float x = joyStick.Horizontal;
        float y = joyStick.Vertical;

        if (Mathf.Abs(x) < 0.1f && Mathf.Abs(y) < 0.1f) return;

        Vector3 dir = new Vector3(x, 0, y).normalized;
        transform.position += dir * Time.deltaTime* moveSpeed;
    }
}
