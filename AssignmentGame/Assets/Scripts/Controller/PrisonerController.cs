using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class PrisonerController : NPCController
{
    [SerializeField] GameObject obj;
    [SerializeField] GameObject HandCuff;
    [SerializeField] Material[] materials;
    [SerializeField] Material prisonerMaterial;


    [SerializeField] SkinnedMeshRenderer render;
    [SerializeField] TextMeshProUGUI handCuffText;
    [SerializeField] Image handCuffFill;
    [SerializeField] GameObject handCuffUI;
    [SerializeField] GameObject NoCellUI;


    public bool IsArrived { get; private set; }
    [SerializeField] public bool IsWaitingHandCuff = false;
    Rigidbody rigid;
    public bool NeedMoreHandCuff => currentHandCuffs < needHandCuffs;

    Define.PrisonerState prisonorState = Define.PrisonerState.Waiting;
    Define.PrisonerState arriveState;
    
    Vector3 targetPos;
    
    PrisonerZone zone;
    [SerializeField] int needHandCuffs;
    [SerializeField] int currentHandCuffs;

    List<Transform> moneyList = new();

    public override bool Init()
    {
        if (!base.Init()) return false;

        if (rigid == null) rigid = GetComponent<Rigidbody>();
        return true;
    }
    public void SetInfo(Vector3 _pos, Define.PrisonerState _arriveState, PrisonerZone _zone)
    {
        rigid.isKinematic = true;
        HandCuff.SetActive(false);
        Material material = materials[Random.Range(0, materials.Length)];
        render.material = material;
        zone = _zone;
        targetPos = _pos;
        arriveState = _arriveState;
        needHandCuffs = Random.Range(1, 6);
        currentHandCuffs = 0;
        IsWaitingHandCuff = false;
        ChangeState(Define.PrisonerState.Waiting);
    }


    public void ChangeState(Define.PrisonerState _prisonorState)
    {
        prisonorState = _prisonorState;
        switch (prisonorState)
        {
            case Define.PrisonerState.Waiting:

                IsWaitingHandCuff = false;
                anim.SetState(Define.State.Idle);
                handCuffUI.SetActive(false);
                NoCellUI.SetActive(false);
                MoveAndArrive().Forget();
                break;
            case Define.PrisonerState.WaitingInLine:
                IsWaitingHandCuff = false;
                break;

            case Define.PrisonerState.WaitingHandCuff:
                IsWaitingHandCuff = true;
                handCuffUI.SetActive(true);
                handCuffText.text = $"{needHandCuffs}";
                handCuffFill.fillAmount = 0;
                break;

            case Define.PrisonerState.WalkingRoom:
                IsWaitingHandCuff = false;
                render.material = prisonerMaterial;
                handCuffUI.SetActive(false);
                AsyncWalkToRoom().Forget();
                break;

            case Define.PrisonerState.WaitingRoom:
                //TODO ; 대기하고있다가, RoomUpgrade되면 차레차례 들어가야됌 영상에없는데 시간남으면 해보기
                if(!zone.PrisonRoom.HasWaitingPrisoner)
                {
                    NoCellUI.SetActive(true);
                    zone.PrisonRoom.SetWaitingPrisoner(true);
                }
                //TODO : 업그레이드 되면 초기화 및 false로 변경
                break;

            case Define.PrisonerState.InRoom:
                rigid.isKinematic = false;
                zone.PrisonRoom.Enter();
                transform.LookAt(zone.RoomWayPoint[0]);
                break;
        }
    }


    async UniTaskVoid AsyncWalkToRoom()
    {
        await AsyncMoveToPosition(zone.RoomWayPoint[0].position);

        if (zone.PrisonRoom.HasSpace)
        {
            zone.PrisonRoom.Reserve();
            transform.LookAt(zone.RoomWayPoint[1]);
            await AsyncMoveToPosition(zone.RoomWayPoint[1].position);
            ChangeState(Define.PrisonerState.InRoom);
        }
        else
        {
            transform.LookAt(zone.RoomWayPoint[1]);
            rigid.isKinematic = false;
            ChangeState(Define.PrisonerState.WaitingRoom);
        }
    }
    public void MoveToWaiting(Vector3 _pos, Define.PrisonerState _arriveState)
    {
        arriveState = _arriveState;
        targetPos = _pos;
        ChangeState(Define.PrisonerState.Waiting);
    }
    async UniTaskVoid MoveAndArrive()
    {
        await AsyncMoveToPosition(targetPos);
        ChangeState(arriveState);
    }

    protected override async UniTask AsyncMoveToPosition(Vector3 _targetPos)
    {
        IsArrived = false;
        await base.AsyncMoveToPosition(_targetPos);
        IsArrived = true;
    }

    public void ReceiveHandCuff()
    {
        currentHandCuffs++; 
        handCuffText.text = $"{needHandCuffs - currentHandCuffs}";
        handCuffFill.fillAmount = (float)currentHandCuffs/ needHandCuffs;
        if (currentHandCuffs >= needHandCuffs)
        {
            CreateMoney();
            anim.SetBool("IsHandCuff", true);
            HandCuff.SetActive(true);
            zone.OnPrisonerLeft();
            ChangeState(Define.PrisonerState.WalkingRoom);
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

    private void Update()
    {
        if(NoCellUI.activeSelf)
            NoCellUI.transform.LookAt(Camera.main.transform);
    }
}
