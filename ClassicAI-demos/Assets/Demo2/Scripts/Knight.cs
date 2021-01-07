using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Knight : MonoBehaviour
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
    [SerializeField] private GameObject hitParticles;

    [Header("Action 2")]
    [SerializeField] private string action2Name;

    [SerializeField] private string action2Description;
    [SerializeField] private GameObject action2;
    [SerializeField] private GameObject shieldParticles;

    [Header("Action 3")]
    [SerializeField] private string action3Name;

    [SerializeField] private string action3Description;
    [SerializeField] private GameObject action3;
    [SerializeField] private int action3Damage;
    [SerializeField] private GameObject sweepParticles;

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
            action1Button.onClick.AddListener(() => StartCoroutine(Action1(-1, false)));
            action2Button.onClick.RemoveAllListeners();
            action2Button.onClick.AddListener(() => Action2(false));
            action3Button.onClick.RemoveAllListeners();
            action3Button.onClick.AddListener(() => Action3(false));
        }
    }

    /// <summary>
    /// Executes the first knight action (Sword attack)
    /// </summary>
    /// <param name="index">Index of the enemy to attack</param>
    /// <param name="simulated">Is the action simulated or not</param>
    public IEnumerator Action1(int index, bool simulated)
    {
        character.EnemySelected = null;
        actionsMenu.SetActive(false);

        if(!character.isEnemy) {
            if(simulated) {
                character.EnemySelected = character.GetCharactersPool().enemies[index];
            }
            else {
                enemyToAttackText.SetActive(true);
                yield return new WaitUntil(() => character.EnemySelected != null); // Waits until the player selects an enemy to attack
                enemyToAttackText.SetActive(false);
            }
        }
        else {
            character.EnemySelected = character.GetCharactersPool().allies[index];
        }

        // If the character has the damage buffed, it deals more damage
        if(character.AttackingState == CharacterStates.States.DamageBuffed || character.SimulatedAttackingState == CharacterStates.States.DamageBuffed) {
            int damageExtra = Mathf.RoundToInt(action1Damage * 0.3f);
            if(simulated) {
                character.EnemySelected.GetComponent<Character>().SimulatedGetDamage(action1Damage + damageExtra);
                character.SimulatedAttackingState = CharacterStates.States.Normal;
            }
            else {
                character.EnemySelected.GetComponent<Character>().GetDamage(action1Damage + damageExtra);
                character.AttackingState = CharacterStates.States.Normal;
            }
        }
        else {
            if(simulated) {
                character.EnemySelected.GetComponent<Character>().SimulatedGetDamage(action1Damage);
            }
            else {
                character.EnemySelected.GetComponent<Character>().GetDamage(action1Damage);
            }
        }

        // If it is not simulated instantiates the particles and ends the turn
        if(!simulated) {
            Instantiate(hitParticles, character.EnemySelected.transform);
            EndTurn();
        }
    }

    /// <summary>
    /// Executes the second knight action (Shield itself)
    /// </summary>
    /// <param name="simulated">Is the action simulated or not</param>
    public void Action2(bool simulated)
    {
        actionsMenu.SetActive(false);

        // If it is not simulated instantiates the particles and ends the turn
        if(!simulated) {
            character.DefensiveState = CharacterStates.States.Shielded;
            Instantiate(shieldParticles, transform);
            EndTurn();
        }
        else {
            character.SimulatedDefensiveState = CharacterStates.States.Shielded;
        }
    }

    /// <summary>
    /// Executes the third knight action (Sword sweep)
    /// </summary>
    /// <param name="simulated">Is the action simulated or not</param>
    public void Action3(bool simulated)
    {
        actionsMenu.SetActive(false);

        if(!character.isEnemy) {
            // Deals damage to every enemy
            foreach(GameObject enemy in character.GetCharactersPool().enemies) {
                // If the character has the damage buffed, deals more damage
                if(character.AttackingState == CharacterStates.States.DamageBuffed || character.SimulatedAttackingState == CharacterStates.States.DamageBuffed) {
                    int damageExtra = Mathf.RoundToInt(action3Damage * 0.3f);
                    if(simulated) {
                        enemy.GetComponent<Character>().SimulatedGetDamage(action3Damage + damageExtra);
                        character.SimulatedAttackingState = CharacterStates.States.Normal;
                    }
                    else {
                        enemy.GetComponent<Character>().GetDamage(action3Damage + damageExtra);
                        character.AttackingState = CharacterStates.States.Normal;
                    }
                }
                else {
                    if(simulated) {
                        enemy.GetComponent<Character>().SimulatedGetDamage(action3Damage);
                    }
                    else {
                        enemy.GetComponent<Character>().GetDamage(action3Damage);
                    }
                }

                if(!simulated) {
                    Instantiate(sweepParticles, enemy.transform);
                }
            }
        }
        else {
            // Deals damage to every ally
            foreach(GameObject ally in character.GetCharactersPool().allies) {
                // If the character has the damage buffed, deals more damage
                if(character.AttackingState == CharacterStates.States.DamageBuffed || character.SimulatedAttackingState == CharacterStates.States.DamageBuffed) {
                    int damageExtra = Mathf.RoundToInt(action3Damage * 0.3f);
                    if(simulated) {
                        ally.GetComponent<Character>().SimulatedGetDamage(action3Damage + damageExtra);
                        character.SimulatedAttackingState = CharacterStates.States.Normal;
                    }
                    else {
                        ally.GetComponent<Character>().GetDamage(action3Damage + damageExtra);
                        character.AttackingState = CharacterStates.States.Normal;
                    }
                }
                else {
                    if(simulated) {
                        ally.GetComponent<Character>().SimulatedGetDamage(action3Damage);
                    }
                    else {
                        ally.GetComponent<Character>().GetDamage(action3Damage);
                    }
                }

                if(!simulated) {
                    Instantiate(sweepParticles, ally.transform);
                }
            }
        }

        // If it is not simulated end the turn
        if(!simulated) {
            EndTurn();
        }
    }

    /// <summary>
    /// Ends the turn
    /// </summary>
    private void EndTurn()
    {
        print("Knigh attacked");

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