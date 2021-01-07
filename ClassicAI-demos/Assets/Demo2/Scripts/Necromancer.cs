using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Necromancer : MonoBehaviour
{
    #region variables

    [SerializeField] private GameObject actionsMenu;
    [SerializeField] private GameObject enemyToAttackText;
    [SerializeField] private GameObject allyToHelpText;

    [Header("Action 1")]
    [SerializeField] private string action1Name;

    [SerializeField] private string action1Description;
    [SerializeField] private GameObject action1;
    [SerializeField] private int action1Damage;
    [SerializeField] private GameObject magicalDamage;

    [Header("Action 2")]
    [SerializeField] private string action2Name;

    [SerializeField] private string action2Description;
    [SerializeField] private GameObject action2;
    [SerializeField] private GameObject reviveParticles;

    [Header("Action 3")]
    [SerializeField] private string action3Name;

    [SerializeField] private string action3Description;
    [SerializeField] private GameObject action3;
    [SerializeField] private int action3Damage;
    [SerializeField] private GameObject deathParticles;

    private Character character;
    private Text action1TextName;
    private Text action1TextDescription;
    private Button action1Button;
    private Text action2TextName;
    private Text action2TextDescription;
    private Button action2Button;
    private Text action3TextName;
    private Text action3TextDescription;
    private Button action3Button;

    #endregion variables

    /// <summary>
    /// Gets all the components related to the actions menu UI
    /// </summary>
    private void Awake()
    {
        character = GetComponent<Character>();

        FindObjects();
        action1TextName = action1.transform.GetChild(0).GetComponent<Text>();
        action1TextDescription = action1.transform.GetChild(1).GetComponent<Text>();
        action1Button = action1.GetComponent<Button>();
        action2TextName = action2.transform.GetChild(0).GetComponent<Text>();
        action2TextDescription = action2.transform.GetChild(1).GetComponent<Text>();
        action2Button = action2.GetComponent<Button>();
        action3TextName = action3.transform.GetChild(0).GetComponent<Text>();
        action3TextDescription = action3.transform.GetChild(1).GetComponent<Text>();
        action3Button = action3.GetComponent<Button>();
    }

    /// <summary>
    /// Search for the different actions UI objects
    /// </summary>
    private void FindObjects()
    {
        GameObject actionsUI = GameObject.FindGameObjectWithTag("ActionsMenu");
        actionsMenu = actionsUI.transform.GetChild(0).gameObject;
        enemyToAttackText = actionsUI.transform.GetChild(1).gameObject;
        allyToHelpText = actionsUI.transform.GetChild(2).gameObject;
        action1 = actionsMenu.transform.GetChild(0).gameObject;
        action2 = actionsMenu.transform.GetChild(1).gameObject;
        action3 = actionsMenu.transform.GetChild(2).gameObject;
    }

    /// <summary>
    /// Selects the character when clicked, sets the actions menu texts and button listeners
    /// </summary>
    public void SelectCharacter()
    {
        // Selects the character only if it is not an enemy, it is the player's turn, the character can attack and is alive
        if(!character.isEnemy && character.GetCharactersPool().Turn < 3 && character.CanAttack && character.GetHealth() > 0) {
            // Sets the action menu texts
            actionsMenu.SetActive(true);
            action1TextName.text = action1Name;
            action1TextDescription.text = action1Description;
            action2TextName.text = action2Name;
            action2TextDescription.text = action2Description;
            action3TextName.text = action3Name;
            action3TextDescription.text = action3Description;

            // Sets the action buttons listeners
            action1Button.onClick.RemoveAllListeners();
            action1Button.onClick.AddListener(() => Action1(false));
            action2Button.onClick.RemoveAllListeners();
            action2Button.onClick.AddListener(() => Action2(false));
            action3Button.onClick.RemoveAllListeners();
            action3Button.onClick.AddListener(() => Action3(false));
        }
    }

    /// <summary>
    /// Executes the first necromancer action (attacks both enemies)
    /// </summary>
    /// <param name="simulated">Is simulated or not</param>
    public void Action1(bool simulated)
    {
        actionsMenu.SetActive(false);

        if(!character.isEnemy) {
            foreach(GameObject enemy in character.GetCharactersPool().enemies) {
                // If the attack is buffed, deals more damage
                if(character.AttackingState == CharacterStates.States.DamageBuffed || character.SimulatedAttackingState == CharacterStates.States.DamageBuffed) {
                    int damageExtra = Mathf.RoundToInt(action1Damage * 0.3f);
                    if(simulated) {
                        enemy.GetComponent<Character>().SimulatedGetDamage(action1Damage + damageExtra);
                        character.SimulatedAttackingState = CharacterStates.States.Normal;
                    }
                    else {
                        enemy.GetComponent<Character>().GetDamage(action1Damage + damageExtra);
                        character.AttackingState = CharacterStates.States.Normal;
                    }
                }
                else {
                    if(simulated) {
                        enemy.GetComponent<Character>().SimulatedGetDamage(action1Damage);
                    }
                    else {
                        enemy.GetComponent<Character>().GetDamage(action1Damage);
                    }
                }

                // If it is not simulated instantiates particles in both enemies
                if(!simulated) {
                    Instantiate(magicalDamage, enemy.transform);
                }
            }
        }
        else {
            foreach(GameObject ally in character.GetCharactersPool().allies) {
                if(character.AttackingState == CharacterStates.States.DamageBuffed || character.SimulatedAttackingState == CharacterStates.States.DamageBuffed) {
                    int damageExtra = Mathf.RoundToInt(action1Damage * 0.3f);
                    if(simulated) {
                        ally.GetComponent<Character>().SimulatedGetDamage(action1Damage + damageExtra);
                        character.SimulatedAttackingState = CharacterStates.States.Normal;
                    }
                    else {
                        ally.GetComponent<Character>().GetDamage(action1Damage + damageExtra);
                        character.AttackingState = CharacterStates.States.Normal;
                    }
                }
                else {
                    if(simulated) {
                        ally.GetComponent<Character>().SimulatedGetDamage(action1Damage);
                    }
                    else {
                        ally.GetComponent<Character>().GetDamage(action1Damage);
                    }
                }

                // If it is not simulated instantiated particles in both allies
                if(!simulated) {
                    Instantiate(magicalDamage, ally.transform);
                }
            }
        }

        // If it is not simulated ends turn
        if(!simulated) {
            EndTurn();
        }
    }

    /// <summary>
    /// Executes the second necromancer action (Revive)
    /// </summary>
    /// <param name="simulated"></param>
    public void Action2(bool simulated)
    {
        actionsMenu.SetActive(false);

        if(!character.isEnemy) {
            // Iterates through all the allies to check wich one is dead and revive it
            foreach(GameObject ally in character.GetCharactersPool().allies) {
                if(simulated) {
                    if(ally.GetComponent<Character>().GetSimulatedHealth() <= 0) {
                        ally.GetComponent<Character>().SimulatedRevive();
                    }
                }
                else {
                    if(ally.GetComponent<Character>().GetHealth() <= 0) {
                        ally.GetComponent<Character>().Revive();
                        Instantiate(reviveParticles, ally.transform);
                    }
                }
            }
        }
        else {
            // Iterates through all the enemies to check wich one is dead and revive it
            foreach(GameObject enemy in character.GetCharactersPool().enemies) {
                if(simulated) {
                    if(enemy.GetComponent<Character>().GetSimulatedHealth() <= 0) {
                        enemy.GetComponent<Character>().SimulatedRevive();
                    }
                }
                else {
                    if(enemy.GetComponent<Character>().GetHealth() <= 0) {
                        enemy.GetComponent<Character>().Revive();
                        Instantiate(reviveParticles, enemy.transform);
                    }
                }
            }
        }

        // If it is not simulated ends the turn
        if(!simulated) {
            EndTurn();
        }
    }

    /// <summary>
    /// Executes the third necromancer action (Sets the death explosion)
    /// </summary>
    /// <param name="simulated"></param>
    public void Action3(bool simulated)
    {
        actionsMenu.SetActive(false);

        if(!simulated) {
            character.DefensiveState = CharacterStates.States.DeathExplosive;
            // If it is not simulated end the turn
            EndTurn();
        }
        else {
            character.DefensiveState = CharacterStates.States.DeathExplosive;
        }
    }

    /// <summary>
    /// Triggers the death explosion if gets killed in that turn
    /// </summary>
    /// <param name="simulated"></param>
    public void DeathExplosion(bool simulated)
    {
        if(!character.isEnemy) {
            // Attacks both enemies
            foreach(GameObject enemy in character.GetCharactersPool().enemies) {
                if(simulated) {
                    enemy.GetComponent<Character>().SimulatedGetDamage(action3Damage);
                }
                else {
                    enemy.GetComponent<Character>().GetDamage(action3Damage);
                    Instantiate(deathParticles, enemy.transform);
                }
            }
        }
        else {
            // Attacks both allies
            foreach(GameObject ally in character.GetCharactersPool().allies) {
                if(simulated) {
                    ally.GetComponent<Character>().SimulatedGetDamage(action3Damage);
                }
                else {
                    ally.GetComponent<Character>().GetDamage(action3Damage);
                    Instantiate(deathParticles, ally.transform);
                }
            }
        }
    }

    /// <summary>
    /// Ends the turn
    /// </summary>
    private void EndTurn()
    {
        print("Necromancer attacked");

        CharactersPool charactersPool = character.GetCharactersPool();
        character.CanAttack = false; // Disables the character to attack
        //charactersPool.Simulation++;
        charactersPool.PassTurn(character.isEnemy);

        if(charactersPool.Turn == 3) { // Passes the turn to the enemies
            print("Enemies turn");
            StartCoroutine(charactersPool.EnemiesTurn());
        }
        else if(charactersPool.Turn == 5) { // Passes the turn to the allies
            print("Allies turn");
            charactersPool.AlliesTurn();
        }
    }
}