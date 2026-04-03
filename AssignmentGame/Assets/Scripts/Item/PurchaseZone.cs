using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using System;

public class PurchaseZone : BaseController
{
    [SerializeField] PurchaseZoneData data;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] Image costFill;
    [SerializeField] GameObject[] targets;
    [SerializeField] PurchaseZone[] nextZone;

    int baseCost;
    int remainCost;
    public bool isReady = false;
    private CancellationTokenSource cts;
    bool isCompleted = false;
    private void Start()
    {
        Init();
    }
    public override bool Init()
    {
        if (!base.Init()) return false;
        baseCost = data.cost;
        remainCost = baseCost;
        if (costText != null) costText.text = remainCost.ToString();
        if (costFill != null) costFill.fillAmount = 0f;


        return true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isReady) return;
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        StartCheckZone(Managers.GameM.player);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
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

    async UniTaskVoid AsyncCheckZone(PlayerController _player, CancellationTokenSource _token)
    {
        try
        {
            while (true)
            {
                if (Managers.GameM.Money > 0)
                {
                    if (remainCost > 0)
                    {
                        remainCost -= 5;
                        Managers.GameM.Money -= 5;
                        Managers.UIM.MoneyTextCheck();
                        Transform money = _player.FollowStackSystem.RemoveMoney();
                        if (money == null) { remainCost += 5; break; }
                        money.DOJump(transform.position, 1f, 1, 0.1f)
                        .OnComplete(() =>
                        {
                            Managers.ObjectM.DeSpawn<Transform>(money);

                            float count = baseCost - remainCost;
                            if (costText != null)
                                DOTween.To(() => int.Parse(costText.text), x => costText.text = x.ToString(), remainCost, 0.1f);
                            if (costFill != null)
                                costFill.DOFillAmount(count / baseCost, 0.1f);

                            if (remainCost <= 0 && !isCompleted)
                            {
                                isCompleted = true;
                                Managers.GameM.Money += (-remainCost);
                                Managers.UIM.MoneyTextCheck();
                                OnPurchaseComplete();
                            }
                        });
                    }
                }


                await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: _token.Token);
            }
        }
        catch (OperationCanceledException) { }
    }


    void OnPurchaseComplete()
    {
        foreach (var zone in nextZone)
        {
            Vector3 originScale = zone.transform.localScale;
            zone.gameObject.SetActive(true);
            zone.isReady = false;

            DOTween.Sequence()
            .Append(zone.transform.DOScale(originScale * 1.5f, 0.4f).SetEase(Ease.OutBack))
            .Append(zone.transform.DOScale(originScale, 0.3f).SetEase(Ease.InBack))
            .OnComplete(() => zone.isReady = true);
        }

        if (data.type == PurchaseType.ActivateObject)
        {
            foreach (var target in targets)
            {
                target.SetActive(true);
            }

        }
        else if (data.type == PurchaseType.UnLockPlayer)
        {
            Managers.GameM.player.OnUnLock(data.unLockType);
        }
        else if (data.type == PurchaseType.SpawnNPC)
        {
            // 광부, 경찰 등 NPC 스폰
            // TODO: ObjectManager에 SpawnNPC() 구현 후 연결
            // for (int i = 0; i < data.npcSpawnCount; i++)
            //     Managers.ObjectM.SpawnNPC();
        }

        gameObject.SetActive(false);
    }
}
