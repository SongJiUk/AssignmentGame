using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[CreateAssetMenu(fileName = "PurchaseZoneData", menuName = "Data/PurchaseZoneData")]
public class PurchaseZoneData : ScriptableObject
{
    public int cost;
    public Define.PurchaseType type;
    public Define.UnLockType unLockType;
    public Define.NPCType npcType;
    public int npcSpawnCount;
}
