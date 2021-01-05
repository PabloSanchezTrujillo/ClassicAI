using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    #region variables

    public GameObject EnemySelected { get; set; }
    public GameObject AllySelected { get; set; }
    public CharacterStates.States DefensiveState { get; set; }
    public CharacterStates.States SimulatedDefensiveState { get; set; }
    public CharacterStates.States AttackingState { get; set; }
    public CharacterStates.States SimulatedAttackingState { get; set; }
    public bool CanAttack { get; set; }
    public Roles.Role role;
    public bool isEnemy;

    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image healthColor;
    [SerializeField] private Text healthText;
    [SerializeField] private Color[] healthColors;

    private CharactersPool charactersPool;
    private int simulatedHealth;

    #endregion variables

    private void Awake()
    {
        charactersPool = FindObjectOfType<CharactersPool>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        healthBar.value = healthBar.maxValue = simulatedHealth = health;
        healthColor.color = healthColors[0];
        healthText.text = health.ToString();
        DefensiveState = CharacterStates.States.Normal;
        SimulatedDefensiveState = CharacterStates.States.Normal;
        AttackingState = CharacterStates.States.Normal;
        SimulatedAttackingState = CharacterStates.States.Normal;
        CanAttack = true;
    }

    public void UpdateHealth()
    {
        //healthBar.value = health;
        healthText.text = health.ToString();

        if(health >= (healthBar.maxValue * 0.5)) {
            healthColor.color = healthColors[0];
        }
        else if(health > (healthBar.maxValue * 0.15) && health < (healthBar.maxValue * 0.5)) {
            healthColor.color = healthColors[1];
        }
        else {
            healthColor.color = healthColors[2];
        }
    }

    public void ClickOnCharacter()
    {
        if(health <= 0)
            return;

        if(isEnemy) {
            foreach(GameObject ally in charactersPool.allies) {
                ally.GetComponent<Character>().EnemySelected = this.gameObject;
            }
        }
        else {
            foreach(GameObject ally in charactersPool.allies) {
                ally.GetComponent<Character>().AllySelected = this.gameObject;
            }
        }
    }

    public void GetDamage(int damage)
    {
        switch(DefensiveState) {
            case CharacterStates.States.Normal:
                health -= damage;
                simulatedHealth = health;
                break;

            case CharacterStates.States.Shielded:
                health -= Mathf.RoundToInt(damage / 2);
                simulatedHealth = health;
                DefensiveState = CharacterStates.States.Normal;
                break;

            case CharacterStates.States.Guarded:
                if(isEnemy) {
                    foreach(GameObject enemy in charactersPool.enemies) {
                        if(enemy != this.gameObject) {
                            int reducedDamage = Mathf.RoundToInt(damage * 0.7f);
                            enemy.GetComponent<Character>().GetDamage(reducedDamage);
                        }
                    }
                }
                else {
                    foreach(GameObject ally in charactersPool.allies) {
                        if(ally != this.gameObject) {
                            int reducedDamage = Mathf.RoundToInt(damage * 0.7f);
                            ally.GetComponent<Character>().GetDamage(reducedDamage);
                        }
                    }
                }
                DefensiveState = CharacterStates.States.Normal;
                break;

            case CharacterStates.States.DeathExplosive:
                health -= damage;
                simulatedHealth = health;
                if(health <= 0) {
                    if(GetComponent<Necromancer>() != null) {
                        GetComponent<Necromancer>().DeathExplosion(false);
                    }
                }
                DefensiveState = CharacterStates.States.Normal;
                break;
        }

        healthBar.value = health;
    }

    public void SimulatedGetDamage(int damage)
    {
        switch(SimulatedDefensiveState) {
            case CharacterStates.States.Normal:
                simulatedHealth -= damage;
                break;

            case CharacterStates.States.Shielded:
                simulatedHealth -= Mathf.RoundToInt(damage / 2);
                SimulatedDefensiveState = CharacterStates.States.Normal;
                break;

            case CharacterStates.States.Guarded:
                if(isEnemy) {
                    foreach(GameObject enemy in charactersPool.enemies) {
                        if(enemy != this.gameObject) {
                            int reducedDamage = Mathf.RoundToInt(damage * 0.7f);
                            enemy.GetComponent<Character>().SimulatedGetDamage(reducedDamage);
                        }
                    }
                }
                else {
                    foreach(GameObject ally in charactersPool.allies) {
                        if(ally != this.gameObject) {
                            int reducedDamage = Mathf.RoundToInt(damage * 0.7f);
                            ally.GetComponent<Character>().SimulatedGetDamage(reducedDamage);
                        }
                    }
                }
                SimulatedDefensiveState = CharacterStates.States.Normal;
                break;

            case CharacterStates.States.DeathExplosive:
                simulatedHealth -= damage;
                if(simulatedHealth <= 0) {
                    if(GetComponent<Necromancer>() != null) {
                        GetComponent<Necromancer>().DeathExplosion(true);
                    }
                }
                SimulatedDefensiveState = CharacterStates.States.Normal;
                break;
        }
    }

    public void HealUp(int heal)
    {
        health += heal;
        if(health > healthBar.maxValue) {
            health = (int)healthBar.maxValue;
        }
        healthBar.value = health;
    }

    public void SimulatedHealUp(int heal)
    {
        simulatedHealth += heal;
        if(simulatedHealth > healthBar.maxValue) {
            simulatedHealth = (int)healthBar.maxValue;
        }
    }

    public CharactersPool GetCharactersPool()
    {
        return charactersPool;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetSimulatedHealth()
    {
        return simulatedHealth;
    }

    public void ResetSimultedHealth()
    {
        simulatedHealth = health;
    }

    public void Revive()
    {
        health = Mathf.RoundToInt(healthBar.maxValue / 2);
        healthBar.value = health;
    }

    public void SimulatedRevive()
    {
        simulatedHealth = Mathf.RoundToInt(healthBar.maxValue / 2);
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}