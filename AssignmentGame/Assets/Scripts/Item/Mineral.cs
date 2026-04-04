using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Mineral : BaseController
{
    public float reSpawnTime = 5f;
    public int hp = 3;
    public MineralZone zone;
    public MachineZone machineZone;

    public bool IsTargeted { get; private set; }
    public void SetTargeted(bool _value) => IsTargeted = _value;


    void OnEnable()
    {
        hp = 2;
        IsTargeted = false;
        

    }
    public void Mining()
    {
        Managers.GameM.player.FollowStackSystem.AddMineral();
        Managers.GameM.player.OnMineralExit(this);
    }

    public void NPCMining()
    {
        hp--;
        if(hp <= 0)
        {
            IsTargeted = false;
            if (!machineZone.IsFull)
            {
                Transform mineral = Managers.ObjectM.SpawnBackMineral(transform.position);
                mineral.DOJump(machineZone.transform.position, 1f, 1, 0.1f)
                    .OnComplete(() =>
                    {
                        machineZone.AddMineral(mineral);
                    });
            }
            zone.ReSpawnMineral(this, reSpawnTime);
            gameObject.SetActive(false);
        }
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
