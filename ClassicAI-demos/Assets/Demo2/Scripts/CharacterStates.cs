using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStates : MonoBehaviour
{
    public enum States
    {
        Normal,
        Shielded,
        DamageBuffed,
        Guarded,
        DeathExplosive
    }
}