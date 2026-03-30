using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager 
{
    public PlayerController player;

    public void Init()
    {
        player = Managers.ObjectM.SpawnPlayer(Vector3.zero);

        Joystick joystick = GameObject.Find("Joystick").GetComponent<Joystick>();
        player.SetJoyStick(joystick);
    }
}
