using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PurchaseZoneData", menuName = "Data/RoomData")]
public class RoomData : ScriptableObject
{
    public int maxCount;
    public RoomData nextLevel;
}
