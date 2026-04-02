using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class MineralZone : BaseController
{
    const float spacing = 1.1f;
    const int col = 8;
    const int row = 17;

    public Action<Mineral> OnMinedMineral;

    private List<Mineral> minerals = new();


    private void Start()
    {
        Init();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Managers.GameM.player.EquipPick(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Managers.GameM.player.EquipPick(false);
        }
    }

    public override bool Init()
    {
        if (!base.Init()) return false;
        float totalX = (col - 1) * spacing;
        float totalZ = (row - 1) * spacing;

        Vector3 origin = transform.position - new Vector3(totalX / 2f, 0, totalZ / 2f);

        for (int r = 0; r < row; r++)
        {
            for (int c = 0; c < col; c++)
            {
                Vector3 pos = origin + new Vector3(c * spacing, 0, r * spacing);
                Mineral mineral = Managers.ObjectM.SpawnMineral(pos);
                if (mineral == null) continue;
                mineral.zone = this;
                minerals.Add(mineral);
            }
        }

        return true;
    }

    public void ReSpawnMineral(Mineral _mineral, float _delay)
    {
        AsyncReSpawnMineral(_mineral, _delay).Forget();
    }

    async UniTaskVoid AsyncReSpawnMineral(Mineral _mineral, float _delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_delay));
        _mineral.gameObject.SetActive(true);
        _mineral.transform.DOScale(Vector3.one, 0.3f)
        .From(Vector3.zero)
        .SetEase(Ease.OutBack);
    }

}
