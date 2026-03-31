using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mineral : BaseController
{
    public float reSpawnTime = 5f;
    public int hp = 3;


    void OnEnable()
    {
        hp = 3;
    }
    public void Mining()
    {
        //TODO : 플레이어 등 뒤로 특정 오브젝트를 풀링해서 플레이어의 등 뒤에 스택으로 쌓아줌
        Managers.GameM.player.FollowStackSystem.AddMineral();

    }
}
