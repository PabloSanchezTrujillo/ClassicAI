using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    #region variables

    public GameObject[] Allies { get; set; }
    public GameObject[] Enemies { get; set; }
    public int AlliesHealth { get; set; }
    public int EnemiesHealth { get; set; }

    private int simulation;

    #endregion variables

    public State(int simulation, GameObject[] allies, GameObject[] enemies)
    {
        this.simulation = simulation;
        Allies = allies;
        Enemies = enemies;
        AlliesHealth = 0;
        EnemiesHealth = 0;

        foreach(GameObject ally in Allies) {
            AlliesHealth += ally.GetComponent<Character>().GetSimulatedHealth();
        }
        foreach(GameObject enemy in Enemies) {
            EnemiesHealth += enemy.GetComponent<Character>().GetSimulatedHealth();
        }
    }

    public void UpdateState(int alliesHealth, int enemiesHealth)
    {
        AlliesHealth = alliesHealth;
        EnemiesHealth = enemiesHealth;
    }

    public string GetHash()
    {
        CharactersPool charactersPool = Allies[0].GetComponent<Character>().GetCharactersPool();
        return "(" + simulation + ") " +
            Allies[0].name + "+" + Allies[1].name + "=" + AlliesHealth.ToString() +
            " - " + Enemies[0].name + "+" + Enemies[1].name + "=" + EnemiesHealth.ToString();
    }
}