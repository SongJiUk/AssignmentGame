using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHandCuffGiver
{
    Transform RemoveHandCuff();
    int HandCuffCount { get; }
}
