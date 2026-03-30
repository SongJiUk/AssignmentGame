using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{

    public PlayerController player { get; private set; }


    public PlayerController SpawnPlayer(Vector3 _pos)
    {
        GameObject go = Managers.ResourceM.Instantiate("Player", _pooling: false);
        if (go == null) return null;

        player = go.GetOrAddComponent<PlayerController>();
        player.Init();
        Managers.GameM.player = player;
        player.transform.position = _pos;
        return player;
    }
}
