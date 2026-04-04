using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class JailerController : NPCController, IHandCuffReceiver, IHandCuffGiver
{
    const int HandCuffLine = 10;
    [SerializeField] Transform HandCuffPos;
    const float handCuffSpacing = 0.08f;

    Define.JailerState jailerState;
    PrisonerZone prisonerZone;
    HandCuffZone handCuffZone;
    Stack<Transform> handCuffStack = new();


    bool isFilling = false;

    public Vector3 HandCuffPosition => HandCuffPos.position;
    public bool IsFullStack => handCuffStack.Count >= HandCuffLine;
    public int HandCuffCount => handCuffStack.Count;

    public override bool Init()
    {
        if (!base.Init()) return false;

        return true;
    }
    public void SetInfo(PrisonerZone _prisonerZone, HandCuffZone _handCuffZone)
    {
        prisonerZone = _prisonerZone;
        handCuffZone = _handCuffZone;
        handCuffStack = new Stack<Transform>();
        ChangeState(Define.JailerState.Walk);
    }

    public void ChangeState(Define.JailerState _jailerState)
    {
        jailerState = _jailerState;
        switch (jailerState)
        {
            case Define.JailerState.Walk:
                AsyncWalkJailer().Forget();
                break;
        }
    }

    async UniTaskVoid AsyncWalkJailer()
    {
       
        while (true)
        {
            if(handCuffStack.Count == 0)
            {
                await AsyncMoveToPosition(handCuffZone.transform.position);
                await UniTask.WaitUntil(() => handCuffStack.Count >= HandCuffLine);
            }

            await AsyncMoveToPosition(prisonerZone.transform.position);

            await UniTask.WaitUntil(() => handCuffStack.Count == 0);
            await UniTask.WaitUntil(() => prisonerZone.HandCuffCount == 0);
        }
    }


    public void AddHandCuff(Transform _handCuff)
    {
        int number = handCuffStack.Count;
        _handCuff.SetParent(HandCuffPos);
        _handCuff.localPosition = Vector3.up * handCuffSpacing * number;
        _handCuff.localRotation = Quaternion.identity;
        handCuffStack.Push(_handCuff);
    }

    public Transform RemoveHandCuff()
    {
        if(handCuffStack.Count > 0)
        {
            Transform handCuff = handCuffStack.Pop();
            handCuff.SetParent(null);
            handCuff.localRotation = Quaternion.identity;
            return handCuff;
        }
        else return null;
    }
}
