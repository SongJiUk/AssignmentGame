using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class PrisonerController : BaseController
{
    // 0.81(��Ȳ�� -> �˼��� �̰ɷ� ����)
    [SerializeField] GameObject obj;
    [SerializeField] GameObject HandCuff;

    public bool IsArrived { get; private set; }
    [SerializeField] public bool IsWaitingHandCuff = false;
    Rigidbody rigid;
    public bool NeedMoreHandCuff => currentHandCuffs < needHandCuffs;

    Define.PrisonorState prisonorState = Define.PrisonorState.Waiting;
    Define.PrisonorState arriveState;
    float speed = 2f;
    Vector3 targetPos;
    Animator anim;
    PrisonerZone zone;
    [SerializeField] int needHandCuffs;
    [SerializeField] int currentHandCuffs;

    List<Transform> moneyList = new();

    //TODO : ���� ���� ���� isKinematic ���ְ�, ���� �����ϸ� ���ֱ�
    public void SetInfo(Vector3 _pos, Define.PrisonorState _arriveState, PrisonerZone _zone)
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (rigid == null) rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = true;
        HandCuff.SetActive(false);

        zone = _zone;
        targetPos = _pos;
        arriveState = _arriveState;
        needHandCuffs = Random.Range(1, 6);
        currentHandCuffs = 0;
        IsWaitingHandCuff = false;
        ChangeState(Define.PrisonorState.Waiting);
    }


    public void ChangeState(Define.PrisonorState _prisonorState)
    {
        prisonorState = _prisonorState;
        switch (prisonorState)
        {
            case Define.PrisonorState.Waiting:

                IsWaitingHandCuff = false;
                anim.SetState(Define.State.Idle);
                MoveAndArrive().Forget();
                break;
            case Define.PrisonorState.WaitingInLine:
                IsWaitingHandCuff = false;
                break;

            case Define.PrisonorState.WaitingHandCuff:
                IsWaitingHandCuff = true;
                break;

            case Define.PrisonorState.WalkingRoom:
                IsWaitingHandCuff = false;
                AsyncWalkToRoom().Forget();
                break;

            case Define.PrisonorState.WaitingRoom:
                //TODO : �� �ø��� �Ѿ��?
                break;

            case Define.PrisonorState.InRoom:
                rigid.isKinematic = false;
                transform.LookAt(zone.RoomWayPoint[0]);
                break;

        }
    }


    async UniTaskVoid AsyncWalkToRoom()
    {
        await AsyncMoveToPosition(zone.RoomWayPoint[0].position);

        if (zone.PrisonRoom.HasSpace)
        {
            zone.PrisonRoom.Enter();
            transform.LookAt(zone.RoomWayPoint[1]);
            await AsyncMoveToPosition(zone.RoomWayPoint[1].position);
            ChangeState(Define.PrisonorState.InRoom);
        }
        else
        {
            transform.LookAt(zone.RoomWayPoint[1]);
            rigid.isKinematic = false;
            ChangeState(Define.PrisonorState.WaitingRoom);
        }
    }
    public void MoveToWaiting(Vector3 _pos, Define.PrisonorState _arriveState)
    {
        arriveState = _arriveState;
        targetPos = _pos;
        ChangeState(Define.PrisonorState.Waiting);
    }
    async UniTaskVoid MoveAndArrive()
    {
        await AsyncMoveToPosition(targetPos);
        ChangeState(arriveState);
    }

    async UniTask AsyncMoveToPosition(Vector3 _targetPos)
    {
        IsArrived = false;
        while ((transform.position - _targetPos).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, Time.deltaTime * speed);
            anim.SetState(Define.State.Run);
            await UniTask.Yield();
        }
        IsArrived = true;
        anim.SetState(Define.State.Idle);
    }

    public void ReceiveHandCuff()
    {
        currentHandCuffs++;
        if (currentHandCuffs >= needHandCuffs)
        {
            CreateMoney();
            anim.SetBool("IsHandCuff", true);
            HandCuff.SetActive(true);
            zone.OnPrisonerLeft();
            ChangeState(Define.PrisonorState.WalkingRoom);
        }
    }

    public void CreateMoney()
    {
        for (int i = 0; i < needHandCuffs; i++)
        {
            Transform money = Managers.ObjectM.SpawnMoney(transform.position);
            moneyList.Add(money);
        }

        foreach (var money in moneyList)
        {
            money.DOJump(zone.MoneyZone.transform.position, 1f, 1, 0.3f)
                .OnComplete(() =>
                {
                    zone.MoneyZone.AddMoney(money);
                });
        }

        moneyList.Clear();
    }


}
