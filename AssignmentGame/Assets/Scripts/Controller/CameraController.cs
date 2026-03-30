using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : BaseController
{
    PlayerController player;
    Vector3 Offset = new Vector3(6.5f, 7f, -10f);
    float smoothSpeed = 3f;
    private void Start()
    {
        Init();
    }

    public override bool Init()
    {
        if (!base.Init()) return false;
        player = Managers.GameM.player;
        return true;
    }


    public void SetTarget()
    {

    }

    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 target = player.transform.position + Offset;
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * smoothSpeed);
    }
}
