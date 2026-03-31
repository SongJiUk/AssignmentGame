using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class MachineZone : BaseController
{
    private CancellationTokenSource cts;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        StartCheck(player);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

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

    async UniTaskVoid AsyncCheck(PlayerController _player, CancellationTokenSource _token)
    {
        try
        {
            while (_player.FollowStackSystem.MineralCount > 0)
            {
                Transform mineral = _player.FollowStackSystem.RemoveMineral();
                if (mineral != null)
                {

                }


                await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: _token.Token);
            }
        }
        catch (OperationCanceledException)
        {

        }

    }

}
