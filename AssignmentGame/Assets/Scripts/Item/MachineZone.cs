using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class MachineZone : BaseController
{
    private CancellationTokenSource cts;
    Stack<Transform> mineralStack = new();

    float baseY = 0.1f;
    float spacingZ = 05f;
    float spacingY = 0.2f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        StartCheck(Managers.GameM.player);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        StopCheck();
    }

    public void StartCheck(PlayerController _player)
    {
        cts = new CancellationTokenSource();
        AsyncCheck(_player, cts).Forget();
    }

    public void StopCheck()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

    public void AddMineral(Transform _mineral)
    {
        int number = mineralStack.Count;
        int row = number / 2;
        int col = number % 2;

        Vector3 centerPos = transform.position + new Vector3(0, baseY, 0);

        _mineral.position = new Vector3(
        centerPos.x,
        centerPos.y + spacingY * row,
        centerPos.z + spacingZ * (col == 0 ? -0.2f : 0.2f));

        mineralStack.Push(_mineral);

    }
    async UniTaskVoid AsyncCheck(PlayerController _player, CancellationTokenSource _token)
    {
        try
        {
            while (_player.FollowStackSystem.MineralCount > 0)
            {
                Transform mineral = _player.FollowStackSystem.RemoveMineral();
                if (mineral != null)
                {
                    AddMineral(mineral);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: _token.Token);
            }
        }
        catch (OperationCanceledException)
        {

        }

    }

}
