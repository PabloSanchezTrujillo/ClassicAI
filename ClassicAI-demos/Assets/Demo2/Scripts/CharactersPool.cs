using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharactersPool : MonoBehaviour
{
    #region variables

    public int Simulation { get; set; }
    public int Turn { get; set; }
    public GameObject[] allies;
    public GameObject[] enemies;

    [SerializeField] private Transform[] charactersPosition;
    [SerializeField] private GameObject difficultySelection;
    [SerializeField] private GameObject victorySign;
    [SerializeField] private GameObject defeatSign;
    [SerializeField] private GameObject selectionMenu;
    [SerializeField] private Button knightButton;
    [SerializeField] private Button healerButton;
    [SerializeField] private Button guardButton;
    [SerializeField] private Button necromancerButton;
    [SerializeField] private GameObject knight;
    [SerializeField] private GameObject healer;
    [SerializeField] private GameObject guard;
    [SerializeField] private GameObject necromancer;
    [SerializeField] private int timeBetweenEnemiesAttacks;

    private int selection;
    private int maxIterations;
    private Vector3 positionUI;
    private Quaternion rotationUI;

    #endregion variables

    private void Start()
    {
        Simulation = 1;
        Turn = 1;
        selection = 0;
        positionUI = new Vector3(0, 0, 0);
        rotationUI = Quaternion.Euler(0, 140, 0);
    }

    /// <summary>
    /// Resets the turn to 1 and enables the allies to attack again
    /// </summary>
    public void AlliesTurn()
    {
        //GeneralTurn++;
        Turn = 1;
        foreach(GameObject ally in allies) {
            ally.GetComponent<Character>().CanAttack = true;
        }
    }

    /// <summary>
    /// Enables the enmies to attack again. Runs the Monte Carlo Tree Search for each alive enemy
    /// </summary>
    /// <returns></returns>
    public IEnumerator EnemiesTurn()
    {
        //GeneralTurn++;
        int enemyPosition = 0;

        foreach(GameObject enemy in enemies) {
            enemy.GetComponent<Character>().CanAttack = true;
            State state = new State(Simulation, allies, enemies); // Creates the new state for the Monte Carlo tree

            if(enemy.GetComponent<Character>().GetHealth() > 0) {
                enemy.GetComponent<MonteCarloTreeSearch>().RunMonteCarloTreeSearch(state, maxIterations, enemyPosition); // Runs the MCTS
                Play bestPlay = enemy.GetComponent<MonteCarloTreeSearch>().BestPlay(state); // Selects the best play to execute

                yield return new WaitForSeconds(timeBetweenEnemiesAttacks); // Waits some seconds between enemies attacks

                EnemyAction(bestPlay);
            }

            enemyPosition++;
            ResetSimulatedHealth(); // Resets the simulated health for each new search in the Monte Carlo tree
        }
    }

    /// <summary>
    /// Executes the selected play for each enemy
    /// </summary>
    /// <param name="bestPlay">The selected play to execute</param>
    private void EnemyAction(Play bestPlay)
    {
        switch(bestPlay.Role) {
            case Roles.Role.Knight:
                if(bestPlay.Name == "Action1-0") { // Attacks the ally 0
                    StartCoroutine(bestPlay.GameObject.GetComponent<Knight>().Action1(0, false));
                }
                else if(bestPlay.Name == "Action1-1") { // Attacks the ally 1
                    StartCoroutine(bestPlay.GameObject.GetComponent<Knight>().Action1(1, false));
                }
                else if(bestPlay.Name == "Action2") { // Shields itself
                    bestPlay.GameObject.GetComponent<Knight>().Action2(false);
                }
                else if(bestPlay.Name == "Action3") { // Attacks both allies with reduced damage
                    bestPlay.GameObject.GetComponent<Knight>().Action3(false);
                }
                break;

            case Roles.Role.Healer:
                if(bestPlay.Name == "Action1-0") { // Heals the enemy 0
                    StartCoroutine(bestPlay.GameObject.GetComponent<Healer>().Action1(0, false));
                }
                else if(bestPlay.Name == "Action1-1") { // Heals the enemy 1
                    StartCoroutine(bestPlay.GameObject.GetComponent<Healer>().Action1(1, false));
                }
                else if(bestPlay.Name == "Action2-0") { // Attacks the ally 0
                    StartCoroutine(bestPlay.GameObject.GetComponent<Healer>().Action2(0, false));
                }
                else if(bestPlay.Name == "Action2-1") { // Attacks the ally 1
                    StartCoroutine(bestPlay.GameObject.GetComponent<Healer>().Action2(1, false));
                }
                else if(bestPlay.Name == "Action3-0") { // Buffs the enemy 0
                    StartCoroutine(bestPlay.GameObject.GetComponent<Healer>().Action3(0, false));
                }
                else if(bestPlay.Name == "Action3-1") { // Buffs the enemy 1
                    StartCoroutine(bestPlay.GameObject.GetComponent<Healer>().Action3(1, false));
                }
                break;

            case Roles.Role.Guard:
                if(bestPlay.Name == "Action1-0") { // Attacks the ally 0
                    StartCoroutine(bestPlay.GameObject.GetComponent<Guard>().Action1(0, false));
                }
                else if(bestPlay.Name == "Action1-1") { // Attacks the ally 1
                    StartCoroutine(bestPlay.GameObject.GetComponent<Guard>().Action1(1, false));
                }
                else if(bestPlay.Name == "Action2-0") { // Shields the enemy 0
                    StartCoroutine(bestPlay.GameObject.GetComponent<Guard>().Action2(0, false));
                }
                else if(bestPlay.Name == "Action2-1") { // Shields the enemy 1
                    StartCoroutine(bestPlay.GameObject.GetComponent<Guard>().Action2(1, false));
                }
                else if(bestPlay.Name == "Action3-0") { // Protects the enemy 0
                    StartCoroutine(bestPlay.GameObject.GetComponent<Guard>().Action3(0, false));
                }
                else if(bestPlay.Name == "Action3-1") { // Protects the enemy 1
                    StartCoroutine(bestPlay.GameObject.GetComponent<Guard>().Action3(1, false));
                }
                break;

            case Roles.Role.Necromancer:
                if(bestPlay.Name == "Action1") { // Attacks both allies
                    bestPlay.GameObject.GetComponent<Necromancer>().Action1(false);
                }
                else if(bestPlay.Name == "Action2") { // Revives the death enemy
                    bestPlay.GameObject.GetComponent<Necromancer>().Action2(false);
                }
                else if(bestPlay.Name == "Action3") { // Prepares the death explosion
                    bestPlay.GameObject.GetComponent<Necromancer>().Action3(false);
                }
                break;
        }
    }

    /// <summary>
    /// Resets allies and enemies simulated health
    /// </summary>
    private void ResetSimulatedHealth()
    {
        foreach(GameObject ally in allies) {
            ally.GetComponent<Character>().ResetSimultedHealth();
        }
        foreach(GameObject enemy in enemies) {
            enemy.GetComponent<Character>().ResetSimultedHealth();
        }
    }

    /// <summary>
    /// Knight character selection
    /// </summary>
    public void SelectKnight()
    {
        GameObject knightObject = Instantiate(knight, charactersPosition[selection].position, charactersPosition[selection].rotation);
        switch(selection) {
            case 0: // The knight is the ally 0
                allies[0] = knightObject;
                knightObject.GetComponent<MonteCarloTreeSearch>().enabled = false; // Monte Carlo Tree Search disabled
                break;

            case 1: // The knight is the ally 1
                allies[1] = knightObject;
                knightObject.GetComponent<MonteCarloTreeSearch>().enabled = false; // Monte Carlo Tree Search disabled
                break;

            case 2: // The knight is the enemy 0
                enemies[0] = knightObject;
                knightObject.GetComponent<Character>().isEnemy = true; // Monte Carlo Tree Search enabled
                knightObject.transform.GetChild(2).localRotation = rotationUI; // Health bar needs to be rotated
                break;

            case 3: // The knight is the enemy 1
                enemies[1] = knightObject;
                knightObject.GetComponent<Character>().isEnemy = true; // Monte Carlo Tree Search enabled
                knightObject.transform.GetChild(2).localRotation = rotationUI; // Health bar needs to be rotated
                break;
        }
        knightButton.interactable = false;
        selection++;

        // The player has chosen its two characters, it is the AI's turn
        if(selection == 2) {
            selectionMenu.SetActive(false);
            AIChoose();
        }
    }

    /// <summary>
    /// Healer character selection
    /// </summary>
    public void SelectHealer()
    {
        GameObject healerObject = Instantiate(healer, charactersPosition[selection].position, charactersPosition[selection].rotation);
        switch(selection) {
            case 0: // The healer is the ally 0
                allies[0] = healerObject;
                healerObject.GetComponent<MonteCarloTreeSearch>().enabled = false; // Monte Carlo Tree Seach disabled
                break;

            case 1: // The healer is the ally 1
                allies[1] = healerObject;
                healerObject.GetComponent<MonteCarloTreeSearch>().enabled = false; // Monte Carlo Tree Search disabled
                break;

            case 2: // The healer is the enemy 0
                enemies[0] = healerObject;
                healerObject.GetComponent<Character>().isEnemy = true; // Monte Carlo Tree Search enabled
                healerObject.transform.GetChild(2).localRotation = rotationUI; // Health bar needs to be rotated
                break;

            case 3: // The healer is the enemy 1
                enemies[1] = healerObject;
                healerObject.GetComponent<Character>().isEnemy = true; // Monte Carlo Tree Search enabled
                healerObject.transform.GetChild(2).localRotation = rotationUI; // Health bar needs to be rotated
                break;
        }
        healerButton.interactable = false;
        selection++;

        // The player has chosen its two characters, it is the AI's turn
        if(selection == 2) {
            selectionMenu.SetActive(false);
            AIChoose();
        }
    }

    /// <summary>
    /// Guard character selection
    /// </summary>
    public void SelectGuard()
    {
        GameObject guardObject = Instantiate(guard, charactersPosition[selection].position, charactersPosition[selection].rotation);
        switch(selection) {
            case 0: // The guard is the ally 0
                allies[0] = guardObject;
                guardObject.GetComponent<MonteCarloTreeSearch>().enabled = false; // Monte Carlo Tree Search disabled
                break;

            case 1: // The guard is the ally 1
                allies[1] = guardObject;
                guardObject.GetComponent<MonteCarloTreeSearch>().enabled = false; // Monte Carlo Tree Search disabled
                break;

            case 2: // The guard is the enemy 0
                enemies[0] = guardObject;
                guardObject.GetComponent<Character>().isEnemy = true; // Monte Carlo Tree Search enabled
                guardObject.transform.GetChild(2).localRotation = rotationUI; // Health bar needs to be rotated
                break;

            case 3: // The guard is the enemy 1
                enemies[1] = guardObject;
                guardObject.GetComponent<Character>().isEnemy = true; // Monte Carlo Tree Seach enabled
                guardObject.transform.GetChild(2).localRotation = rotationUI; // Health bar needs to be rotated
                break;
        }
        guardButton.interactable = false;
        selection++;

        // The player has chosen its two characters, it is the AI's turn
        if(selection == 2) {
            selectionMenu.SetActive(false);
            AIChoose();
        }
    }

    /// <summary>
    /// Necromancer character selection
    /// </summary>
    public void SelectNecromancer()
    {
        GameObject necromancerObject = Instantiate(necromancer, charactersPosition[selection].position, charactersPosition[selection].rotation);
        switch(selection) {
            case 0: // Necromancer is the ally 0
                allies[0] = necromancerObject;
                necromancerObject.GetComponent<MonteCarloTreeSearch>().enabled = false; // Monte Carlo Tree Search disabled
                break;

            case 1: // Necromancer is the ally 1
                allies[1] = necromancerObject;
                necromancerObject.GetComponent<MonteCarloTreeSearch>().enabled = false; // Monte Carlo Tree Search disabled
                break;

            case 2: // Necromancer is the enemy 0
                enemies[0] = necromancerObject;
                necromancerObject.GetComponent<Character>().isEnemy = true; // Monte Carlo Tree Search enabled
                necromancerObject.transform.GetChild(2).localRotation = rotationUI; // Health bar needs to be rotated
                break;

            case 3: // Necromancer is the enemy 1
                enemies[1] = necromancerObject;
                necromancerObject.GetComponent<Character>().isEnemy = true; // Monte Carlo Tree Search enabled
                necromancerObject.transform.GetChild(2).localRotation = rotationUI; // Health bar needs to be rotated
                break;
        }
        necromancerButton.interactable = false;
        selection++;

        // The player has chosen its two characters, it is the AI's turn
        if(selection == 2) {
            selectionMenu.SetActive(false);
            AIChoose();
        }
    }

    /// <summary>
    /// The AI chooses its characters
    /// </summary>
    private void AIChoose()
    {
        int previousRandom = -1;
        int random;

        for(int i = 0; i < 2; i++) {
            do {
                random = UnityEngine.Random.Range(0, 4);
            } while(random == previousRandom); // Cannot select the same character as the previous one

            switch(random) {
                case 0:
                    SelectKnight();
                    break;

                case 1:
                    SelectHealer();
                    break;

                case 2:
                    SelectGuard();
                    break;

                case 3:
                    SelectNecromancer();
                    break;
            }
            previousRandom = random;
        }
    }

    /// <summary>
    /// Sets the maximun iterations for the MCTS
    /// </summary>
    /// <param name="iterations">Number of maximum iterations</param>
    public void SetMaxIterations(int iterations)
    {
        maxIterations = iterations;
        difficultySelection.SetActive(false);
        selectionMenu.SetActive(true);
    }

    /// <summary>
    /// Passes to the next turn
    /// </summary>
    /// <param name="enemiesTurn">Marks if it is an enemy turn or not</param>
    public void PassTurn(bool enemiesTurn)
    {
        if(enemiesTurn) {
            int enemiesAlive = 0;

            // Counts the alive enemies
            foreach(GameObject enemy in enemies) {
                if(enemy.GetComponent<Character>().GetHealth() > 0) {
                    enemiesAlive++;
                }
            }

            Turn += (enemiesAlive == 1) ? 2 : 1;
        }
        else {
            int alliesAlive = 0;

            // Counts the alive allies
            foreach(GameObject ally in allies) {
                if(ally.GetComponent<Character>().GetHealth() > 0) {
                    alliesAlive++;
                }
            }

            Turn += (alliesAlive == 1) ? 2 : 1;
        }

        CheckEndGame();
    }

    /// <summary>
    /// Checks if the game has ended or not
    /// </summary>
    private void CheckEndGame()
    {
        int alliesAlive = 0;
        int enemiesAlive = 0;

        // Counts the alive allies
        foreach(GameObject ally in allies) {
            if(ally.GetComponent<Character>().GetHealth() > 0) {
                alliesAlive++;
            }
        }
        // Counts the alive enemies
        foreach(GameObject enemy in enemies) {
            if(enemy.GetComponent<Character>().GetHealth() > 0) {
                enemiesAlive++;
            }
        }

        // If there are no allies alive it is a defeat
        if(alliesAlive == 0)
            defeatSign.SetActive(true);
        // If there are no enemies alive it is a victory
        if(enemiesAlive == 0)
            victorySign.SetActive(true);
    }
}