using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;

public class MoneyZone : BaseController
{
    [SerializeField] PurchaseZone drillzone;

    public PurchaseZone DirllZone => drillzone;
    Stack<Transform> moneyStack = new();
    float baseY = 0.14f;
    float spacingY = 0.07f;
    float spacingX = 0.4f;
    float spacingZ = 0.2f;

    const int cols = 2;
    const int rows = 3;

    private CancellationTokenSource cts;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        StartCheckZone(Managers.GameM.player);
    }

    private void OnTriggerExit(Collider other)
    {
        StopCheck();
    }

    public void StartCheckZone(PlayerController _player)
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = new CancellationTokenSource();
        AsyncZoneCheck(_player, cts).Forget();
    }
    public void StopCheck()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

    async UniTaskVoid AsyncZoneCheck(PlayerController _player, CancellationTokenSource _token)
    {
        try
        {
            while (true)
            {
                if (moneyStack.Count > 0)
                {
                    Transform money = moneyStack.Pop();
                    if (money == null) continue;
                    money.DOJump(Managers.GameM.player.transform.position, jumpPower: 1f, numJumps: 1, duration: 0.1f)
                        .OnComplete(() =>
                        {
                            _player.FollowStackSystem.AddMoney(money, this);
                        });
                }

                await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken : _token.Token);
            }
        }
        catch (OperationCanceledException) { }
    }

    public void AddMoney(Transform _money)
    {
        int index = moneyStack.Count;
        float centerX = (cols - 1) / 2f * spacingX;
        float centerZ = (rows - 1) / 2f * spacingZ;

        int layer = index / 6;
        int col = (index % 6) % 2;
        int row = (index % 6) / 2;

        Vector3 pos = transform.position
            + Vector3.up * (layer * spacingY + baseY)
            + Vector3.right * (col * spacingX- centerX)
            + Vector3.forward * (row * spacingZ - centerZ);

        _money.position = pos;
        moneyStack.Push(_money);
    }


}
