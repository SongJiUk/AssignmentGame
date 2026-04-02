using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;

public class HandCuffZone : BaseController
{
    const float baseY = 0.04f;
    const float spacingY = 0.08f;
    Stack<Transform> handCuffStack = new();

    private CancellationTokenSource cts;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = new CancellationTokenSource();
            StartCheck(Managers.GameM.player);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }
    }


    public void StartCheck(PlayerController _player)
    {
        AsyncCheck(_player,cts).Forget();
    }

    async UniTaskVoid AsyncCheck(PlayerController _player ,CancellationTokenSource _token)
    {
        try
        {
            while(true)
            {
                if(handCuffStack.Count > 0)
                {
                    Transform handCuff = handCuffStack.Pop();
                    if (handCuff == null) continue;
                    Vector3 targetPos = _player.FollowStackSystem.HandCuffPosition;

                    handCuff.DOJump(targetPos, jumpPower: 1f, numJumps: 1, duration: 0.1f)
                        .OnComplete(() =>
                        {
                            _player.FollowStackSystem.AddHandCuff(handCuff);
                        });

                }

                await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: _token.Token);
            }
        }
        catch(OperationCanceledException)
        {

        }
    }

    public void AddHandCuff()
    {
        int number = handCuffStack.Count;
        Vector3 pos = transform.position + Vector3.up * (baseY + spacingY * number);
        var handcuff = Managers.ObjectM.SpawnHandCuff(pos);
        handCuffStack.Push(handcuff);
    }

}
