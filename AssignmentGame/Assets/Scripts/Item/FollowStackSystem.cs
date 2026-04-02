using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowStackSystem : MonoBehaviour
{
   
    [SerializeField] Transform ItemRowPos_1;
    [SerializeField] Transform ItemRowPos_2;
    [SerializeField] Transform HandCuffPos;

    const float mineralSpacing = 0.2f;
    const float handCuffSpacing = 0.08f;
    const float moneySpacing = 0.07f;

    bool wasHolding = false;
    bool hasReceivedFirstMoney = false;
    public Vector3 HandCuffPosition => HandCuffPos.transform.position;

    Stack<Transform> mineralStack = new();
    Stack<Transform> moneyStack = new();
    Stack<Transform> handCuffStack = new();

    public int MineralCount => mineralStack.Count;
    public int MoneyCount => moneyStack.Count;
    public int handCuffCount => handCuffStack.Count;

    public void AddMineral()
    {
        if(moneyStack.Count > 0)
        {
            int i = 0;
            foreach (var money in moneyStack)
            {
                money.SetParent(ItemRowPos_2);
                money.localPosition = Vector3.up * moneySpacing * i;
                money.localRotation = Quaternion.identity;
                i++;
            }
        }

        int number = mineralStack.Count;
        var item = Managers.ObjectM.SpawnBackMineral(Vector3.zero);
        item.SetParent(ItemRowPos_1);
        item.localPosition = Vector3.up * mineralSpacing * number;
        item.localRotation = Quaternion.identity;
        mineralStack.Push(item);

    }

    public Transform RemoveMineral()
    {
        if (mineralStack.Count > 0)
        {
            Transform mineral = mineralStack.Pop();
            mineral.SetParent(null);
            mineral.localRotation = Quaternion.identity;

            if(mineralStack.Count == 0 && moneyStack.Count > 0)
            {
                int i = 0;
                foreach(var money in moneyStack)
                {
                    money.SetParent(ItemRowPos_1);
                    money.localPosition = Vector3.up * moneySpacing * i;
                    money.localRotation = Quaternion.identity;
                    i++;
                }
            }

            return mineral;
        }
        return null;
    }

    public void AddMoney(Transform _money, MoneyZone _zone)
    {
        int number = moneyStack.Count;
        
        if(mineralStack.Count > 0) _money.SetParent(ItemRowPos_2);
        else _money.SetParent(ItemRowPos_1);

        _money.localPosition = Vector3.up * moneySpacing * number;
        _money.localRotation = Quaternion.identity;
        moneyStack.Push(_money);

        if(!hasReceivedFirstMoney)
        { 
            hasReceivedFirstMoney = true;
            Managers.GameM.cam.PlayCutScene(_zone.DirllZone, 2f).Forget();
        }
    }

    public void RemoveMoney()
    {

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
        if (handCuffStack.Count > 0)
        {
            Transform handCuff = handCuffStack.Pop();
            handCuff.SetParent(null);
            handCuff.localRotation = Quaternion.identity;
            return handCuff;
        }
        else return null;
    }

    void LateUpdate()
    {

        bool isHolding = handCuffStack.Count > 0;
        if (isHolding != wasHolding)
        {
            Managers.GameM.player.OnChangeHoldHandCuff(isHolding);
            wasHolding = isHolding;
        }
     
    }
}
