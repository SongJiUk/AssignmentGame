using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowStackSystem : MonoBehaviour
{
    const float spacing = 0.2f;
    [SerializeField] Transform ItemRowPos_1;
    [SerializeField] Transform ItemRowPos_2;

    Stack<Transform> mineralStack = new();
    Stack<Transform> moneyStack = new();

    public int MineralCount => mineralStack.Count;
    public int MoneyCount => moneyStack.Count;

    public void AddMineral()
    {
        int number = mineralStack.Count;
        Vector3 pos = ItemRowPos_1.position + (Vector3.up * spacing * number);
        var item = Managers.ObjectM.SpawnBackMineral(pos);
        mineralStack.Push(item);
    }

    public Transform RemoveMineral()
    {
        if (mineralStack.Count > 0)
        {
            return mineralStack.Pop();
        }
        else return null;
    }

    public void AddMoney()
    {

    }

    public void RemoveMoney()
    {

    }

    void LateUpdate()
    {
        if (mineralStack.Count > 0)
        {
            int i = 0;
            foreach (var mineral in mineralStack)
            {

                Vector3 pos = ItemRowPos_1.position + (Vector3.up * spacing * i);
                mineral.transform.position = pos;
                i++;
            }
        }

        if (moneyStack.Count > 0)
        {

        }
    }
}
