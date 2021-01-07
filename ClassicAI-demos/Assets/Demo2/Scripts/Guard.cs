using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guard : MonoBehaviour
{
    #region variables

    [SerializeField] private GameObject actionsMenu;
    [SerializeField] private GameObject enemyToAttackText;
    [SerializeField] private GameObject allyToHelpText;

    [Header("Action 1")]
    [SerializeField] private string action1Name;

    [SerializeField] private string action1Description;
    [SerializeField] private GameObject action1;
    [SerializeField] private int damage;
    [SerializeField] private GameObject damageParticles;

    [Header("Action 2")]
    [SerializeField] private string action2Name;

    [SerializeField] private string action2Description;
    [SerializeField] private GameObject action2;
    [SerializeField] private GameObject shieldParticles;

    [Header("Action 3")]
    [SerializeField] private string action3Name;

    [SerializeField] private string action3Description;
    [SerializeField] private GameObject action3;
    [SerializeField] private GameObject guardedParticles;

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
            action2Button.onClick.AddListener(() => StartCoroutine(Action2(-1, false)));
            action3Button.onClick.RemoveAllListeners();
            action3Button.onClick.AddListener(() => StartCoroutine(Action3(-1, false)));
        }
    }

    /// <summary>
    /// Executes the first guard action (Attack an enemy)
    /// </summary>
    /// <param name="index">Index of the enemy attacked</param>
    /// <param name="simulated">Is simulated or not</param>
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
                yield return new WaitUntil(() => character.EnemySelected != null); // Waits until the player selects an enemy
                enemyToAttackText.SetActive(false);
            }
        }
        else {
            character.EnemySelected = character.GetCharactersPool().allies[index];
        }

        // If the character has the damage buffed deals more damage
        if(character.AttackingState == CharacterStates.States.DamageBuffed || character.SimulatedAttackingState == CharacterStates.States.DamageBuffed) {
            int damageExtra = Mathf.RoundToInt(damage * 0.3f);
            if(simulated) {
                character.EnemySelected.GetComponent<Character>().SimulatedGetDamage(damage + damageExtra);
                character.SimulatedAttackingState = CharacterStates.States.Normal;
            }
            else {
                character.EnemySelected.GetComponent<Character>().GetDamage(damage + damageExtra);
                character.AttackingState = CharacterStates.States.Normal;
            }
        }
        else {
            if(simulated) {
                character.EnemySelected.GetComponent<Character>().SimulatedGetDamage(damage);
            }
            else {
                character.EnemySelected.GetComponent<Character>().GetDamage(damage);
            }
        }

        // If it is not simulated, instantiates the particles and ends the turn
        if(!simulated) {
            Instantiate(damageParticles, character.EnemySelected.transform);
            EndTurn();
        }
    }

    /// <summary>
    /// Executes the second guard action (Guard)
    /// </summary>
    /// <param name="index">Index of the ally to guard</param>
    /// <param name="simulated">Is simulated or not</param>
    /// <returns></returns>
    public IEnumerator Action2(int index, bool simulated)
    {
        character.AllySelected = null;
        actionsMenu.SetActive(false);

        if(!character.isEnemy) {
            if(simulated) {
                character.AllySelected = character.GetCharactersPool().allies[index];
                character.AllySelected.GetComponent<Character>().SimulatedDefensiveState = CharacterStates.States.Shielded;
            }
            else {
                allyToHelpText.SetActive(true);
                character.CanAttack = false;
                yield return new WaitUntil(() => character.AllySelected != null); // Waits until the player selects an ally
                allyToHelpText.SetActive(false);
                character.AllySelected.GetComponent<Character>().DefensiveState = CharacterStates.States.Shielded;
            }
        }
        else {
            character.AllySelected = character.GetCharactersPool().enemies[index];
            character.AllySelected.GetComponent<Character>().DefensiveState = CharacterStates.States.Shielded;
        }

        // If it is not simulated, instantiates the particles and ends the turn
        if(!simulated) {
            Instantiate(shieldParticles, character.AllySelected.transform);
            EndTurn();
        }
    }

    /// <summary>
    /// Executes the third guard action (Bodyguard)
    /// </summary>
    /// <param name="index">Index of the character to protect</param>
    /// <param name="simulated">Is simulated or not</param>
    /// <returns></returns>
    public IEnumerator Action3(int index, bool simulated)
    {
        character.AllySelected = null;
        actionsMenu.SetActive(false);

        if(!character.isEnemy) {
            if(simulated) {
                character.AllySelected = character.GetCharactersPool().allies[index];
                character.AllySelected.GetComponent<Character>().SimulatedDefensiveState = CharacterStates.States.Guarded;
            }
            else {
                allyToHelpText.SetActive(true);
                character.CanAttack = false;
                yield return new WaitUntil(() => character.AllySelected != null); // Waits until the player selects an ally
                allyToHelpText.SetActive(false);
                character.AllySelected.GetComponent<Character>().DefensiveState = CharacterStates.States.Guarded;
            }
        }
        else {
            character.AllySelected = character.GetCharactersPool().enemies[index];
            character.AllySelected.GetComponent<Character>().DefensiveState = CharacterStates.States.Guarded;
        }

        // If it is not simulated, instantiates the particles and ends the turn
        if(!simulated) {
            Instantiate(guardedParticles, character.AllySelected.transform);
            EndTurn();
        }
    }

    /// <summary>
    /// Ends the turn
    /// </summary>
    private void EndTurn()
    {
        print("Guard attacked");

        CharactersPool charactersPool = character.GetCharactersPool();
        character.CanAttack = false; // Disabled the character to attack
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