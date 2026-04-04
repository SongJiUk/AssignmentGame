using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class FollowStackSystem : MonoBehaviour
{
    const int MaxHandCuff = 30;
    float lastMaxTiem = -999f;
    [SerializeField] Transform ItemRowPos_1;
    [SerializeField] Transform ItemRowPos_2;
    [SerializeField] Transform HandCuffPos;
    [SerializeField] Transform MaxTextPos;

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

    public bool IsHandCuffFull => handCuffCount >= MaxHandCuff;
    public void AddMineral()
    {
        if (Managers.GameM.player.CurrentWeaponData.maxMineralCount <= mineralStack.Count)
        {
            Managers.UIM.TryShowMax(MaxTextPos.position, ref lastMaxTiem);
            return;
        }

        if (moneyStack.Count > 0)
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

            if (mineralStack.Count == 0 && moneyStack.Count > 0)
            {
                int i = 0;
                foreach (var money in moneyStack)
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
        Managers.GameM.Money += 5;
        Managers.UIM.MoneyTextCheck();
        int number = moneyStack.Count;

        if (mineralStack.Count > 0) _money.SetParent(ItemRowPos_2);
        else _money.SetParent(ItemRowPos_1);

        _money.localPosition = Vector3.up * moneySpacing * number;
        _money.localRotation = Quaternion.identity;
        moneyStack.Push(_money);

        if (!hasReceivedFirstMoney)
        {
            hasReceivedFirstMoney = true;
            Managers.GameM.cam.PlayCutScene(2f, 0, new GameObject[] { _zone.DrillZone.gameObject }).Forget();
        }
    }

    public Transform RemoveMoney()
    {
        if (moneyStack.Count > 0)
        {
            Transform money = moneyStack.Pop();
            money.SetParent(null);
            money.localRotation = Quaternion.identity;

            return money;
        }
        return null;
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
