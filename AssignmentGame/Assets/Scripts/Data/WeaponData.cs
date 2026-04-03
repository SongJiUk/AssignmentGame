using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/WeaponData")]
public class WeaponData : ScriptableObject
{
    public Define.WeaponState weaponState;
    public int maxMineralCount;
    public WeaponData nextWeapon;

}
