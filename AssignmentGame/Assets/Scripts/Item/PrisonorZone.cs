using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;

//2가지 관리함, Prisonor, HandCuff
public class PrisonerZone : BaseController
{
    [SerializeField] Transform handCuffPos;
    [SerializeField] Transform[] prisonerUntilHandCuffPos;
    [SerializeField] Transform[] roomWayPoint;
    [SerializeField] PrisonRoom prisonRoom;
    [SerializeField] MoneyZone moneyZone;

    public PrisonRoom PrisonRoom => prisonRoom;
    public MoneyZone MoneyZone => moneyZone;
    public Transform[] RoomWayPoint => roomWayPoint;

    Stack<Transform> handCuffStack = new();
    Queue<PrisonerController> prisonerQueue = new();


    const float spacingY = 0.08f;
    bool isGiving = false;
    CancellationTokenSource cts;
    int workerCount = 0;
    private void Start()
    {
        Init();
    }
    public override bool Init()
    {
        if (!base.Init()) return false;
        if (prisonRoom == null) prisonRoom = GetComponentInChildren<PrisonRoom>();
        if (moneyZone == null) moneyZone = GetComponentInChildren<MoneyZone>();
        AsyncPrisonorInit().Forget();

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            workerCount++;
            cts?.Cancel();
            cts?.Dispose();

            cts = new CancellationTokenSource();
            StartCheck(Managers.GameM.player);
            TryGiveHandCuffFromDesk();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            workerCount--;
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }
    }
    #region HandCuff관련
    void AddToDesk(Transform _handCuff)
    {
        int number = handCuffStack.Count;
        Vector3 targetPos = handCuffPos.position + (Vector3.up * spacingY * number);
        Vector3 originScale = _handCuff.localScale;

        handCuffStack.Push(_handCuff);
        _handCuff.DOJump(targetPos, jumpPower: 2f, numJumps: 1, duration: 0.1f)
            .OnComplete(() =>
            {
                _handCuff.DOScale(originScale, 0.3f)
                .From(originScale / 2)
                .SetEase(Ease.OutBack);
            });
    }

    public Transform RemoveHandCuff()
    {
        if (handCuffStack.Count == 0) return null;
        return handCuffStack.Pop();
    }
    #endregion

    #region Prisonor관련
    async UniTaskVoid AsyncPrisonorInit()
    {
        for (int i = prisonerUntilHandCuffPos.Length - 1; i >= 0; i--)
        {
            PrisonerController prisoner = SpawnPrisoner(i);

            await UniTask.WaitUntil(() => prisoner.IsArrived);
        }
    }

    public PrisonerController SpawnPrisoner(int _index)
    {
        PrisonerController prisoner = Managers.ObjectM.SpawnPrisoner(prisonerUntilHandCuffPos[0].position);

        Define.PrisonorState arriveState = _index == prisonerUntilHandCuffPos.Length - 1
            ? Define.PrisonorState.WaitingHandCuff
            : Define.PrisonorState.WaitingInLine;


        prisoner.SetInfo(prisonerUntilHandCuffPos[_index].position, arriveState, this);
        prisoner.transform.LookAt(prisonerUntilHandCuffPos[2].position);
        prisonerQueue.Enqueue(prisoner);
        return prisoner;
    }


    public void OnPrisonorLeft()
    {
        prisonerQueue.Dequeue();

        var prisoners = new List<PrisonerController>(prisonerQueue);
        int i = prisonerUntilHandCuffPos.Length - 1;
        foreach (var prisoner in prisoners)
        {

            Define.PrisonorState arriveState = i == prisonerUntilHandCuffPos.Length - 1
                ? Define.PrisonorState.WaitingHandCuff
                : Define.PrisonorState.WaitingInLine;

            prisoner.MoveToWaiting(prisonerUntilHandCuffPos[i].position, arriveState);
            i--;
        }

        AsyncOnPrisonerLeft().Forget();
    }

    async UniTaskVoid AsyncOnPrisonerLeft()
    {

        var prisoners = new List<PrisonerController>(prisonerQueue);

        foreach (var prisoner in prisoners)
            await UniTask.WaitUntil(() => prisoner.IsArrived);


        SpawnPrisoner(0);
    }


    #endregion
    public void StartCheck(PlayerController _player)
    {
        AsyncCheck(_player, cts).Forget();
    }

    async UniTaskVoid AsyncCheck(PlayerController _player, CancellationTokenSource _token)
    {
        try
        {
            while(true)
            {
                PrisonerController prisoner = prisonerQueue.Count > 0 ? prisonerQueue.Peek() : null;
                if(_player.FollowStackSystem.handCuffCount > 0)
                {
                    if (prisoner != null && prisoner.IsWaitingHandCuff && prisoner.NeedMoreHandCuff)
                    {
                        Transform handcuff = _player.FollowStackSystem.RemoveHandCuff();
                        if (handcuff != null)
                        {
                            SendHandCuffToPrisoner(handcuff, prisoner);
                        }

                    }
                    else
                    {
                        Transform handCuff = _player.FollowStackSystem.RemoveHandCuff();
                        if (handCuff != null)
                        {
                            AddToDesk(handCuff);
                        }
                    }
                }
                else
                {
                    TryGiveHandCuffFromDesk();
                }
                
                await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: _token.Token);
            }
            
        }
        catch (OperationCanceledException) { }
    }

   
    public void SendHandCuffToPrisoner(Transform _handCuff, PrisonerController _prisoner)
    {
        _prisoner.ReceiveHandCuff();
        _handCuff.DOJump(_prisoner.transform.position, 1f, 1, 0.1f)
            .OnComplete(() =>
            {
                
                Managers.ObjectM.DeSpawn<Transform>(_handCuff);
            });
    }
  
  

    public void TryGiveHandCuffFromDesk()
    {
        if (workerCount <= 0) return;
        if (prisonerQueue.Count == 0) return;
        if (!prisonerQueue.Peek().IsWaitingHandCuff) return;
        if (handCuffStack.Count == 0) return;
        if (isGiving) return;

        AsyncGiveFromDesk().Forget();

    }

    async UniTaskVoid AsyncGiveFromDesk()
    {
        isGiving = true;
        PrisonerController prisoner = prisonerQueue.Peek();

        while(handCuffStack.Count > 0 && prisoner.IsWaitingHandCuff && prisoner.NeedMoreHandCuff)
        {
            Transform handCuff = handCuffStack.Pop();
            prisoner.ReceiveHandCuff();
            handCuff.DOJump(prisoner.transform.position, 1f, 1, 0.1f)
                .OnComplete(() =>
                {
                    Managers.ObjectM.DeSpawn<Transform>(handCuff);
                });

            await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
        }

        isGiving = false;
    }

    public void OnPrisonerArrviedToDesk(PrisonerController _prisoner)
    {
        if(prisonerQueue.Peek() == _prisoner)
        {
            _prisoner.ChangeState(Define.PrisonorState.WaitingHandCuff);
        }
    }
}