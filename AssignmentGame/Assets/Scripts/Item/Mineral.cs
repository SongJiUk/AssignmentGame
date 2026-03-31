using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mineral : BaseController
{
    public float reSpawnTime = 5f;
    public int hp = 3;
    public MineralZone zone;

    void OnEnable()
    {
        hp = 3;
    }
    public void Mining()
    {
        Managers.GameM.player.FollowStackSystem.AddMineral();
        Managers.GameM.player.OnMineralExit(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        Managers.GameM.player.OnMineralEnter(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        Managers.GameM.player.OnMineralExit(this);
    }
}
