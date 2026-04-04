using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;

public class HandCuffZone : BaseController
{
    [SerializeField] Image zoneImage;
    [SerializeField] GameObject maxText;


    const float baseY = 0.04f;
    const float spacingY = 0.08f;
    const int MaxCount = 50;
    Stack<Transform> handCuffStack = new();
    public int HandCuffCount => handCuffStack.Count;
    public bool IsFull => handCuffStack.Count >= MaxCount;

    CancellationTokenSource playercts;
    CancellationTokenSource jailercts;
    JailerController jailer;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playercts?.Cancel();
            playercts?.Dispose();
            playercts = new CancellationTokenSource();
            zoneImage.color = Color.green;
            AsyncCheck(Managers.GameM.player, playercts).Forget();

        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Jailer"))
        {
            if (jailer == null) jailer = other.GetComponent<JailerController>();
            jailercts?.Cancel();
            jailercts?.Dispose();
            jailercts = new CancellationTokenSource();
            zoneImage.color = Color.green;
            AsyncCheck(jailer, jailercts).Forget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playercts?.Cancel();
            playercts?.Dispose();
            playercts = null;
            zoneImage.color = Color.white;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Jailer"))
        {
            jailercts?.Cancel();
            jailercts?.Dispose();
            jailercts = null;
            zoneImage.color = Color.white;
        }
    }


    

    async UniTaskVoid AsyncCheck(IHandCuffReceiver _receiver, CancellationTokenSource _token)
    {
        try
        {
            while (true)
            {
                if (handCuffStack.Count > 0)
                {
                    Transform handCuff = handCuffStack.Pop();
                    maxText.SetActive(false);
                    if (handCuff == null) continue;
                    
                    handCuff.DOJump(_receiver.HandCuffPosition, jumpPower: 1f, numJumps: 1, duration: 0.1f)
                        .OnComplete(() =>
                        {
                            _receiver.AddHandCuff(handCuff);
                        });

                }

                await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: _token.Token);
            }
        }
        catch (OperationCanceledException)
        {

        }
    }

    public void AddHandCuff()
    {
        if (IsFull) return;

        int number = handCuffStack.Count;
        Vector3 pos = transform.position + Vector3.up * (baseY + spacingY * number);
        var handcuff = Managers.ObjectM.SpawnHandCuff(pos);
        handCuffStack.Push(handcuff);

        if (IsFull) maxText.SetActive(IsFull);
    }

}
