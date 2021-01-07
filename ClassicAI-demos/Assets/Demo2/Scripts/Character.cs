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

    /// <summary>
    /// Updates the visual health bar
    /// </summary>
    public void UpdateHealth()
    {
        //healthBar.value = health;
        healthText.text = health.ToString();

        if(health >= (healthBar.maxValue * 0.5)) { // Color green for health above 50%
            healthColor.color = healthColors[0];
        }
        else if(health > (healthBar.maxValue * 0.15) && health < (healthBar.maxValue * 0.5)) { // Color orange for health between 15% and 50%
            healthColor.color = healthColors[1];
        }
        else { // Red color for health under 15%
            healthColor.color = healthColors[2];
        }
    }

    /// <summary>
    /// Selects the clicked character
    /// </summary>
    public void ClickOnCharacter()
    {
        // Selects the character only if it is alive
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

    /// <summary>
    /// Character receives damage function
    /// </summary>
    /// <param name="damage">Amount of damaged received</param>
    public void GetDamage(int damage)
    {
        switch(DefensiveState) {
            case CharacterStates.States.Normal: // Takes all teh damage
                health -= damage;
                simulatedHealth = health;
                break;

            case CharacterStates.States.Shielded: // Takes half of the damage
                health -= Mathf.RoundToInt(damage / 2);
                simulatedHealth = health;
                DefensiveState = CharacterStates.States.Normal;
                break;

            case CharacterStates.States.Guarded: // The guarded character receives no damage and the guardian takes 70% of the damage
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

            case CharacterStates.States.DeathExplosive: // If the damage kills it, damage its enemies with an explosion
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

        if(health < 0) {
            health = 0;
        }
        healthBar.value = health;
    }

    /// <summary>
    /// Simulates the damage for the simulation phase of the Monte Carlo Tree Search
    /// </summary>
    /// <param name="damage"></param>
    public void SimulatedGetDamage(int damage)
    {
        switch(SimulatedDefensiveState) {
            case CharacterStates.States.Normal: // Takes all the damage
                simulatedHealth -= damage;
                break;

            case CharacterStates.States.Shielded: // Takes half of the damage
                simulatedHealth -= Mathf.RoundToInt(damage / 2);
                SimulatedDefensiveState = CharacterStates.States.Normal;
                break;

            case CharacterStates.States.Guarded: // The guarded character receives no damage and the guardian takes 70% of the damage
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

            case CharacterStates.States.DeathExplosive: // If the damage kills it, damage its enemies with an explosion
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

    /// <summary>
    /// Character gets healed
    /// </summary>
    /// <param name="heal">Amount of health to heal</param>
    public void HealUp(int heal)
    {
        health += heal;
        if(health > healthBar.maxValue) { // The health cannot exceed the maximum value of the health bar
            health = (int)healthBar.maxValue;
        }
        healthBar.value = health;
    }

    /// <summary>
    /// Characters gets simulated healed
    /// </summary>
    /// <param name="heal">Amount of simulated health to heal</param>
    public void SimulatedHealUp(int heal)
    {
        simulatedHealth += heal;
        if(simulatedHealth > healthBar.maxValue) { // The simulated health cannot exceed the maximun value of the health bar
            simulatedHealth = (int)healthBar.maxValue;
        }
    }

    /// <summary>
    /// Returns the character pool
    /// </summary>
    public CharactersPool GetCharactersPool()
    {
        return charactersPool;
    }

    /// <summary>
    /// Returns the actual character health
    /// </summary>
    public int GetHealth()
    {
        return health;
    }

    /// <summary>
    /// Return the actual simulated health
    /// </summary>
    public int GetSimulatedHealth()
    {
        return simulatedHealth;
    }

    /// <summary>
    /// Resets the simulated health with the real health value
    /// </summary>
    public void ResetSimultedHealth()
    {
        simulatedHealth = health;
    }

    /// <summary>
    /// Revives a character with half of its health
    /// </summary>
    public void Revive()
    {
        health = Mathf.RoundToInt(healthBar.maxValue / 2);
        healthBar.value = health;
    }

    /// <summary>
    /// Simulates the revival of a character
    /// </summary>
    public void SimulatedRevive()
    {
        simulatedHealth = Mathf.RoundToInt(healthBar.maxValue / 2);
    }

    /// <summary>
    /// Return the maximun health of the character
    /// </summary>
    public int GetMaxHealth()
    {
        return maxHealth;
    }
}