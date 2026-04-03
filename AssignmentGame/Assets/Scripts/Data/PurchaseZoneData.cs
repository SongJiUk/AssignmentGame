using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PurchaseType
{
    ActivateObject, // 오브젝트 활성화 (감옥 업그레이드 등)
    UnLockPlayer,   // 플레이어 무기 변경 (곡괭이→드릴, 드릴→기계)
    SpawnNPC,       // NPC 스폰 (광부, 경찰)
}

[CreateAssetMenu(fileName = "PurchaseZoneData", menuName = "Data/PurchaseZoneData")]
public class PurchaseZoneData : ScriptableObject
{
    public int cost;
    public PurchaseType type;
    public Define.UnLockType unLockType; // UnLockPlayer일 때만 사용
    public int npcSpawnCount;            // SpawnNPC일 때만 사용
}
