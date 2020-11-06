using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonteCarloTreeSearch : MonoBehaviour
{
    #region variables

    private Character character;
    private Knight knight;
    private Healer healer;
    private Guard guard;
    private Necromancer necromancer;

    #endregion variables

    private void Awake()
    {
        character = GetComponent<Character>();
        switch(character.role) {
            case Roles.Role.Knight:
                knight = GetComponent<Knight>();
                break;

            case Roles.Role.Healer:
                healer = GetComponent<Healer>();
                break;

            case Roles.Role.Guard:
                guard = GetComponent<Guard>();
                break;

            case Roles.Role.Necromancer:
                necromancer = GetComponent<Necromancer>();
                break;
        }
    }

    public void EnemyChoose()
    {
        switch(character.role) {
            case Roles.Role.Knight:
                knight.Action3();
                break;

            case Roles.Role.Healer:
                StartCoroutine(healer.Action2());
                break;

            case Roles.Role.Guard:
                StartCoroutine(guard.Action1());
                break;

            case Roles.Role.Necromancer:
                necromancer.Action1();
                break;
        }
    }
}