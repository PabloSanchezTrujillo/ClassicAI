using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    #region variables

    public int AlliesHealth { get; set; }
    public int EnemiesHealth { get; set; }

    #endregion variables

    public State(int alliesHealth, int enemiesHealth)
    {
        AlliesHealth = alliesHealth;
        EnemiesHealth = enemiesHealth;
    }

    public void UpdateState(int alliesHealth, int enemiesHealth)
    {
        AlliesHealth = alliesHealth;
        EnemiesHealth = enemiesHealth;
    }
}