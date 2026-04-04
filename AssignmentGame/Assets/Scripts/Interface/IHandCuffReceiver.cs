using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHandCuffReceiver
{
    void AddHandCuff(Transform _handcuff);
    Vector3 HandCuffPosition { get; }
    bool IsFullStack { get; }
}
