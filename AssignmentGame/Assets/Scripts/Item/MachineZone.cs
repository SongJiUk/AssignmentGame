using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using DG.Tweening;

public class MachineZone : BaseController
{
    const int MaxCount = 50;
    [SerializeField] Image zoneImage;
    [SerializeField] GameObject maxText;

    public event Action<Transform> OnMineralArrive;
    Stack<Transform> convertStack = new();
    public int MineralCount => convertStack.Count;
    public bool IsFull => convertStack.Count >= MaxCount;

    private CancellationTokenSource cts;
    public int reserveSlotNum = 0;

    float baseY = 0.1f;
    float spacingZ = 0.5f;
    float spacingY = 0.2f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        zoneImage.color = Color.green;
        StartCheckZone(Managers.GameM.player);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        zoneImage.color = Color.white;
        StopCheck();
    }

    public void StartCheckZone(PlayerController _player)
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = new CancellationTokenSource();
        AsyncCheckZone(_player, cts).Forget();
    }

    public void StopCheck()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

    public void AddMineral(Transform _mineral)
    {
        int number = reserveSlotNum++;
        int row = number / 2;
        int col = number % 2;

        Vector3 centerPos = transform.position + new Vector3(0, baseY, 0);

        Vector3 targetPos = new Vector3(
        centerPos.x,
        centerPos.y + spacingY * row,
        centerPos.z + spacingZ * (col == 0 ? -0.2f : 0.2f));


        convertStack.Push(_mineral);
        OnMineralArrive?.Invoke(_mineral);

        Vector3 originScale = _mineral.localScale;
        _mineral.DOJump(targetPos, jumpPower: 2f, numJumps: 1, duration: 0.1f)
        .OnComplete(() =>
        {
            _mineral.DOScale(originScale, 0.2f)
            .From(Vector3.zero)
            .SetEase(Ease.OutBack);
        });
    }

    public Transform RemoveMineral()
    {
        if (convertStack.Count == 0) return null;
        reserveSlotNum--;
        return convertStack.Pop();
    }
    async UniTaskVoid AsyncCheckZone(PlayerController _player, CancellationTokenSource _token)
    {
        try
        {
            while (true)
            {
                if (IsFull)
                {
                    maxText.SetActive(true);
                    await UniTask.WaitUntil(() => !IsFull, cancellationToken: _token.Token);
                    maxText.SetActive(false);
                    continue;
                }

                if (_player.FollowStackSystem.MineralCount > 0)
                {
                    Transform mineral = _player.FollowStackSystem.RemoveMineral();
                    if (mineral != null)
                    {
                        AddMineral(mineral);
                    }
                }




                await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: _token.Token);
            }
        }
        catch (OperationCanceledException)
        {

        }

    }

}
