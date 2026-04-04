using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;

public class PrisonerZone : BaseController
{
    [SerializeField] Transform handCuffPos;
    [SerializeField] Transform[] prisonerUntilHandCuffPos;
    [SerializeField] Transform[] roomWayPoint;
    PrisonRoom prisonRoom;
    MoneyZone moneyZone;

    [SerializeField] Image zoneImage;

    public PrisonRoom PrisonRoom => prisonRoom;
    public MoneyZone MoneyZone => moneyZone;
    public Transform[] RoomWayPoint => roomWayPoint;

    Stack<Transform> handCuffStack = new();
    public int HandCuffCount => handCuffStack.Count;
    Queue<PrisonerController> prisonerQueue = new();


    const float spacingY = 0.08f;
    bool isGiving = false;
    CancellationTokenSource playercts;
    CancellationTokenSource jailercts;
    CancellationTokenSource deskcts;
    int workerCount = 0;
    JailerController jailer;
    private void Start()
    {
        Init();
    }
    public override bool Init()
    {
        if (!base.Init()) return false;
        if (prisonRoom == null) prisonRoom = GetComponentInChildren<PrisonRoom>();
        if (moneyZone == null) moneyZone = GetComponentInChildren<MoneyZone>();
        AsyncPrisonerInit().Forget();

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            workerCount++;
            playercts?.Cancel();
            playercts?.Dispose();
            zoneImage.color = Color.green;

            playercts = new CancellationTokenSource();
            AsyncCheck(Managers.GameM.player, playercts).Forget();
            TryGiveHandCuffFromDesk();
        }

        if(other.gameObject.layer == LayerMask.NameToLayer("Jailer"))
        {
            if (jailer == null) jailer = other.GetComponent<JailerController>();

            workerCount++;
            jailercts?.Cancel();
            jailercts?.Dispose();
            zoneImage.color = Color.green;
            jailercts = new CancellationTokenSource();
            AsyncCheck(jailer, jailercts).Forget();
            TryGiveHandCuffFromDesk();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            zoneImage.color = Color.white;
            workerCount--;
            playercts?.Cancel();
            playercts?.Dispose();
            playercts = null;
        }


        if (other.gameObject.layer == LayerMask.NameToLayer("Jailer"))
        {
            zoneImage.color = Color.white;
            workerCount--;
            jailercts?.Cancel();
            jailercts?.Dispose();
            jailercts = null;
        }
    }
    #region HandCuff����
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

    #region Prisoner����
    async UniTaskVoid AsyncPrisonerInit()
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

        Define.PrisonerState arriveState = _index == prisonerUntilHandCuffPos.Length - 1
            ? Define.PrisonerState.WaitingHandCuff
            : Define.PrisonerState.WaitingInLine;

        prisoner.Init();
        prisoner.SetInfo(prisonerUntilHandCuffPos[_index].position, arriveState, this);
        prisoner.transform.LookAt(prisonerUntilHandCuffPos[2].position);
        prisonerQueue.Enqueue(prisoner);
        return prisoner;
    }


    public void OnPrisonerLeft()
    {
        if (prisonerQueue.Count > 0) prisonerQueue.Dequeue();

        var prisoners = new List<PrisonerController>(prisonerQueue);
        int i = prisonerUntilHandCuffPos.Length - 1;
        foreach (var prisoner in prisoners)
        {

            Define.PrisonerState arriveState = i == prisonerUntilHandCuffPos.Length - 1
                ? Define.PrisonerState.WaitingHandCuff
                : Define.PrisonerState.WaitingInLine;

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
    

    async UniTaskVoid AsyncCheck(IHandCuffGiver _giver, CancellationTokenSource _token)
    {
        try
        {
            while (true)
            {
                PrisonerController prisoner = prisonerQueue.Count > 0 ? prisonerQueue.Peek() : null;

                if (_giver.HandCuffCount> 0)
                {
                    if (prisoner != null && prisoner.IsWaitingHandCuff && prisoner.NeedMoreHandCuff)
                    {
                        Transform handcuff = _giver.RemoveHandCuff();
                        if (handcuff != null)
                        {
                            SendHandCuffToPrisoner(handcuff, prisoner);
                        }

                    }
                    else
                    {
                        Transform handCuff = _giver.RemoveHandCuff();
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

        deskcts?.Cancel();
        deskcts?.Dispose();
        deskcts = new CancellationTokenSource();
        AsyncGiveFromDesk(deskcts).Forget();

    }

    async UniTaskVoid AsyncGiveFromDesk(CancellationTokenSource _token)
    {
        try
        {
            isGiving = true;
            PrisonerController prisoner = prisonerQueue.Peek();

            while (handCuffStack.Count > 0 && prisoner.IsWaitingHandCuff && prisoner.NeedMoreHandCuff)
            {
                Transform handCuff = handCuffStack.Pop();
                prisoner.ReceiveHandCuff();
                handCuff.DOJump(prisoner.transform.position, 1f, 1, 0.1f)
                    .OnComplete(() =>
                    {
                        Managers.ObjectM.DeSpawn<Transform>(handCuff);
                    });

                await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: _token.Token);
            }

            isGiving = false;
        }
        catch (OperationCanceledException) { isGiving = false; }



    }

    public void OnPrisonerArrviedToDesk(PrisonerController _prisoner)
    {
        if (prisonerQueue.Peek() == _prisoner)
        {
            _prisoner.ChangeState(Define.PrisonerState.WaitingHandCuff);
        }
    }
}