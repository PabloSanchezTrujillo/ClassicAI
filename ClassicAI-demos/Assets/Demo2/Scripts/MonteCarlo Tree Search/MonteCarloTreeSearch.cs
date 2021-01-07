using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;

public class MonteCarloTreeSearch : MonoBehaviour
{
    #region variables

    [SerializeField] private float UCB1ExploreParam;

    private Character character;
    private Knight knight;
    private Healer healer;
    private Guard guard;
    private Necromancer necromancer;
    private Dictionary<string, MonteCarloTreeNode> treeNodes;
    private GameObject[] allCharacters;
    private int enemyPosition;
    private int characterIterator;

    private enum Winner
    {
        None,
        Allies,
        Enemies
    }

    #endregion variables

    private void Awake()
    {
        treeNodes = new Dictionary<string, MonteCarloTreeNode>();
        allCharacters = new GameObject[4];
        character = GetComponent<Character>();
    }

    /// <summary>
    /// Stores the correct component based on the different roles
    /// </summary>
    private void Start()
    {
        allCharacters[0] = character.GetCharactersPool().enemies[0];
        allCharacters[1] = character.GetCharactersPool().enemies[1];
        allCharacters[2] = character.GetCharactersPool().allies[0];
        allCharacters[3] = character.GetCharactersPool().allies[1];

        // For each different character stores the correct role-component
        foreach(GameObject character in allCharacters) {
            switch(character.GetComponent<Character>().role) {
                case Roles.Role.Knight:
                    knight = character.GetComponent<Knight>();
                    break;

                case Roles.Role.Healer:
                    healer = character.GetComponent<Healer>();
                    break;

                case Roles.Role.Guard:
                    guard = character.GetComponent<Guard>();
                    break;

                case Roles.Role.Necromancer:
                    necromancer = character.GetComponent<Necromancer>();
                    break;
            }
        }
    }

    /// <summary>
    /// Creates a new node in the Monte Carlo Tree
    /// </summary>
    /// <param name="state">The game state stored in the node</param>
    private void CreateNode(State state)
    {
        if(!treeNodes.ContainsKey(state.GetHash())) { // Checks first if it is a new game state
            List<Play> unexpandedPlays = LegalPlays(state);
            MonteCarloTreeNode newNode = new MonteCarloTreeNode(null, null, state, unexpandedPlays);
            treeNodes.Add(state.GetHash(), newNode);
        }
    }

    /// <summary>
    /// Runs the Monte Carlo Tree search for an enemy
    /// </summary>
    /// <param name="state">The actual game state</param>
    /// <param name="maxIterations">Maximum number of simulated iterations</param>
    /// <param name="enemyPosition">Enemy position in the array (0 or 1)</param>
    public void RunMonteCarloTreeSearch(State state, int maxIterations, int enemyPosition)
    {
        int iterations = 0;
        this.enemyPosition = enemyPosition;
        characterIterator = enemyPosition;
        CreateNode(state);
        print(character.role + " [Initial State]: " + state.AlliesHealth + " - " + state.EnemiesHealth);

        // Monte Carlo Tree search algorithm
        while(iterations < maxIterations) {
            character.GetCharactersPool().Simulation++;
            MonteCarloTreeNode selectedNode = Select(state);
            Winner winner = DecideWinner(state);

            if(!selectedNode.IsLeaf() && winner == Winner.None) { // Simulates the game until it finds a winner
                selectedNode = Expand(selectedNode);
                winner = Simulate(selectedNode);
            }
            Backpropagate(selectedNode, winner);

            iterations++;
        }
    }

    /// <summary>
    /// Selection phase: selects the next child node to explore
    /// </summary>
    /// <param name="state">Simulated state of the game/param>
    private MonteCarloTreeNode Select(State state)
    {
        print("SELECT");
        treeNodes.TryGetValue(state.GetHash(), out MonteCarloTreeNode selectedNode); // Selects the next node based on the actual game state

        // Explores the node until all its children are fully expanded and only if the selected node is not a leaf node
        while(selectedNode.IsFullyExpanded() && !selectedNode.IsLeaf()) {
            List<Play> allPlays = selectedNode.AllPlays();
            Play bestPlay = null;
            float bestUCB1 = Mathf.NegativeInfinity;

            // Selects the best next play based on the UCB1 parameter
            foreach(Play play in allPlays) {
                float childUCB1 = selectedNode.ChildNode(play).GetUCB1(UCB1ExploreParam);

                if(childUCB1 > bestUCB1) {
                    bestPlay = play;
                    bestUCB1 = childUCB1;
                }
            }

            selectedNode = selectedNode.ChildNode(bestPlay);
        }

        return selectedNode;
    }

    /// <summary>
    /// Expand phase: expands successively the legal plays for the selected node and its children
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private MonteCarloTreeNode Expand(MonteCarloTreeNode node)
    {
        print("EXPAND");
        List<Play> unexpandedPlays = node.UnexpandedPlays();
        int randomIndex = UnityEngine.Random.Range(0, unexpandedPlays.Count);
        Play selectedPlay = unexpandedPlays[randomIndex]; // Selects one play randomly to advance to the next node
        State childState = NextState(selectedPlay);
        List<Play> childLegalPlays = LegalPlays(childState); // All possible legal plays to get to the next child
        MonteCarloTreeNode childNode = node.Expand(selectedPlay, childState, childLegalPlays);

        treeNodes.Add(childState.GetHash(), childNode);

        return childNode;
    }

    /// <summary>
    /// Simulation phase: simulates the action to advance to the next possible state
    /// </summary>
    /// <param name="node">Node to simulate</param>
    private Winner Simulate(MonteCarloTreeNode node)
    {
        print("SIMULATE");
        // The character iterator iterates between the four different characters to simulate each one of them
        characterIterator = enemyPosition;
        State actualState = node.State;
        Winner winner = DecideWinner(actualState);

        while(winner == Winner.None) {
            List<Play> legalPlays = LegalPlays(actualState);
            int randomIndex = UnityEngine.Random.Range(0, legalPlays.Count);
            Play randomPlay = legalPlays[randomIndex]; // Selects one random play from the possible different legal plays
            actualState = NextState(randomPlay); // Simulates the next state
            winner = DecideWinner(actualState);

            characterIterator++; // Iterates to the next character
            if(characterIterator >= 4) {
                characterIterator = 0;
            }
        }

        return winner;
    }

    /// <summary>
    /// Backpropagation phase: backpropagates to the Monte Carlo Tree the results obtained from the simulation
    /// </summary>
    /// <param name="node">The node that is being explored</param>
    /// <param name="winner">The winner of the simulation (Allies/Enemies)</param>
    private void Backpropagate(MonteCarloTreeNode node, Winner winner)
    {
        print("BACKPROPAGATE");
        while(node != null) {
            node.NumberOfPlays++;
            if(winner == Winner.Allies) { // Flip because each node statistics are used for its parent node choice, not its own.
                node.NumberOfWins++; // Wins for the enemies
            }

            node = node.Parent;
        }
    }

    /// <summary>
    /// Selects the best play from all the possible plays that can be performed from a defined state
    /// </summary>
    /// <param name="state">The game state to select the best play from</param>
    /// <returns></returns>
    public Play BestPlay(State state)
    {
        treeNodes.TryGetValue(state.GetHash(), out MonteCarloTreeNode node); // Gets the node related to the game state
        CreateNode(state);

        // This action can only be performed if the node is fully expanded with all its children
        if(!node.IsFullyExpanded()) {
            throw new Exception("Not enough information!");
        }

        List<Play> allPlays = node.AllPlays();
        Play bestPlay = null;
        int maxPlays = -1000000000;

        foreach(Play play in allPlays) {
            MonteCarloTreeNode childNode = node.ChildNode(play);

            // Selects the play that has been performed the most, that will be the best play
            if(childNode.NumberOfPlays > maxPlays) {
                bestPlay = play;
                maxPlays = childNode.NumberOfPlays;
            }
        }

        return bestPlay;
    }

    /// <summary>
    /// Selects the possible legal plays from a state
    /// </summary>
    /// <param name="state">The state to get the legal plays from</param>
    private List<Play> LegalPlays(State state)
    {
        List<Play> legalPlaysList = new List<Play>();
        CharactersPool charactersPool = character.GetCharactersPool();

        GameObject simCharacter = allCharacters[characterIterator];
        Roles.Role simCharacterRole = simCharacter.GetComponent<Character>().role;

        // Each different role has different legal plays
        switch(simCharacterRole) {
            case Roles.Role.Knight:
                legalPlaysList = KnightLegalPlays(simCharacter, simCharacterRole, charactersPool);
                break;

            case Roles.Role.Healer:
                legalPlaysList = HealerLegalPlays(simCharacter, simCharacterRole, charactersPool);
                break;

            case Roles.Role.Guard:
                legalPlaysList = GuardLegalPlays(simCharacter, simCharacterRole, charactersPool);
                break;

            case Roles.Role.Necromancer:
                legalPlaysList = NecromancerLegalPlays(simCharacter, simCharacterRole, charactersPool);
                break;
        }

        return legalPlaysList;
    }

    /// <summary>
    /// Legal plays for the knight
    /// </summary>
    /// <param name="simCharacter">The character to get the legal plays from</param>
    /// <param name="simCharacterRole">The role of the character</param>
    /// <param name="charactersPool">Character pool for all the character</param>
    private List<Play> KnightLegalPlays(GameObject simCharacter, Roles.Role simCharacterRole, CharactersPool charactersPool)
    {
        List<Play> knightLegalPlays = new List<Play>();

        // Characters driven by the AI
        Character[] enemies =
        {
            charactersPool.enemies[0].GetComponent<Character>(),
            charactersPool.enemies[1].GetComponent<Character>()
        };
        // Characters driven by the player
        Character[] allies =
        {
            charactersPool.allies[0].GetComponent<Character>(),
            charactersPool.allies[1].GetComponent<Character>()
        };

        // Legal plays constraints
        if(allies[0].GetSimulatedHealth() > 0) { // Ally 0 is alive
            knightLegalPlays.Add(new Play(() => knight.Action1(0, true), simCharacter, simCharacterRole, "Action1-0", charactersPool.Simulation));
        }
        if(allies[1].GetSimulatedHealth() > 0) { // Ally 1 is alive
            knightLegalPlays.Add(new Play(() => knight.Action1(1, true), simCharacter, simCharacterRole, "Action1-1", charactersPool.Simulation));
        }
        if(allies[0].GetSimulatedHealth() > 0 && allies[1].GetSimulatedHealth() > 0) { // Both allies are alive
            knightLegalPlays.Add(new Play(() => knight.Action3(true), simCharacter, simCharacterRole, "Action3", charactersPool.Simulation));
        }
        knightLegalPlays.Add(new Play(() => knight.Action2(true), simCharacter, simCharacterRole, "Action2", charactersPool.Simulation));

        return knightLegalPlays;
    }

    /// <summary>
    /// Legal plays for the healer
    /// </summary>
    /// <param name="simCharacter">The character to get the legal plays from</param>
    /// <param name="simCharacterRole">The role of the character</param>
    /// <param name="charactersPool">Character pool for all the character</param>
    private List<Play> HealerLegalPlays(GameObject simCharacter, Roles.Role simCharacterRole, CharactersPool charactersPool)
    {
        List<Play> healerLegalPlays = new List<Play>();

        // Characters driven by the AI
        Character[] enemies =
        {
            charactersPool.enemies[0].GetComponent<Character>(),
            charactersPool.enemies[1].GetComponent<Character>()
        };
        // Characters driven by the player
        Character[] allies =
        {
            charactersPool.allies[0].GetComponent<Character>(),
            charactersPool.allies[1].GetComponent<Character>()
        };

        // Legal plays constraints
        if(enemies[0].GetSimulatedHealth() > 0 && enemies[0].GetSimulatedHealth() < enemies[0].GetMaxHealth()) { // Enemy 0 is alive but not full health
            healerLegalPlays.Add(new Play(() => healer.Action1(0, true), simCharacter, simCharacterRole, "Action1-0", charactersPool.Simulation));
        }
        if(enemies[1].GetSimulatedHealth() > 0 && enemies[1].GetSimulatedHealth() < enemies[1].GetMaxHealth()) { // Enemy 1 is alive but not full health
            healerLegalPlays.Add(new Play(() => healer.Action1(1, true), simCharacter, simCharacterRole, "Action1-1", charactersPool.Simulation));
        }
        if(allies[0].GetSimulatedHealth() > 0) { // Ally 0 is alive
            healerLegalPlays.Add(new Play(() => healer.Action2(0, true), simCharacter, simCharacterRole, "Action2-0", charactersPool.Simulation));
        }
        if(allies[1].GetSimulatedHealth() > 0) { // Ally 1 is alive
            healerLegalPlays.Add(new Play(() => healer.Action2(1, true), simCharacter, simCharacterRole, "Action2-1", charactersPool.Simulation));
        }
        if(enemies[0].GetSimulatedHealth() > 0) { // Enemy 0 is alive
            healerLegalPlays.Add(new Play(() => healer.Action3(0, true), simCharacter, simCharacterRole, "Action3-0", charactersPool.Simulation));
        }
        if(enemies[1].GetSimulatedHealth() > 0) { // Enemy 1 is alive
            healerLegalPlays.Add(new Play(() => healer.Action3(1, true), simCharacter, simCharacterRole, "Action3-1", charactersPool.Simulation));
        }

        return healerLegalPlays;
    }

    /// <summary>
    /// Legal plays for the guard
    /// </summary>
    /// <param name="simCharacter">The character to get the legal plays from</param>
    /// <param name="simCharacterRole">The role of the character</param>
    /// <param name="charactersPool">Character pool for all the character</param>
    private List<Play> GuardLegalPlays(GameObject simCharacter, Roles.Role simCharacterRole, CharactersPool charactersPool)
    {
        List<Play> guardLegalPlays = new List<Play>();
        bool enemyNeedsHelp = false;

        // Characters driven by the AI
        Character[] enemies =
        {
            charactersPool.enemies[0].GetComponent<Character>(),
            charactersPool.enemies[1].GetComponent<Character>()
        };
        // Characters driven by the player
        Character[] allies =
        {
            charactersPool.allies[0].GetComponent<Character>(),
            charactersPool.allies[1].GetComponent<Character>()
        };

        // Legal plays constraints
        if(enemies[0].GetSimulatedHealth() > 0
            && enemies[0].GetSimulatedHealth() <= enemies[0].GetMaxHealth() / 2) { // Enemy 0 has half or less of its health and is alive
            guardLegalPlays.Add(new Play(() => guard.Action2(0, true), simCharacter, simCharacterRole, "Action2-0", charactersPool.Simulation));
            enemyNeedsHelp = true;
        }
        if(enemies[1].GetSimulatedHealth() > 0
            && enemies[1].GetSimulatedHealth() <= enemies[1].GetMaxHealth() / 2) { // Enemy 1 has half or less of its health and is alive
            guardLegalPlays.Add(new Play(() => guard.Action2(1, true), simCharacter, simCharacterRole, "Action2-1", charactersPool.Simulation));
            enemyNeedsHelp = true;
        }
        if(allies[0].GetSimulatedHealth() > 0
            && !enemyNeedsHelp) { // Ally 0 is alive and no enemy needs help
            guardLegalPlays.Add(new Play(() => guard.Action1(0, true), simCharacter, simCharacterRole, "Action1-0", charactersPool.Simulation));
        }
        if(allies[1].GetSimulatedHealth() > 0
            && !enemyNeedsHelp) { // Ally 1 is alive and no enemy needs help
            guardLegalPlays.Add(new Play(() => guard.Action1(1, true), simCharacter, simCharacterRole, "Action1-1", charactersPool.Simulation));
        }
        if(character.GetSimulatedHealth() > enemies[0].GetSimulatedHealth()
            && enemies[0].GetSimulatedHealth() > 0) { // The guard has more health than the enemy (0) protected (guard is enemy 1)
            guardLegalPlays.Add(new Play(() => guard.Action3(0, true), simCharacter, simCharacterRole, "Action3-0", charactersPool.Simulation));
        }
        if(character.GetSimulatedHealth() > enemies[1].GetSimulatedHealth()
            && enemies[1].GetSimulatedHealth() > 0) { // The guard has more health than the enemy (1) protected (guard is enemy 0)
            guardLegalPlays.Add(new Play(() => guard.Action3(1, true), simCharacter, simCharacterRole, "Action3-1", charactersPool.Simulation));
        }

        return guardLegalPlays;
    }

    /// <summary>
    /// Legal plays for the necromancer
    /// </summary>
    /// <param name="simCharacter">The character to get the legal plays from</param>
    /// <param name="simCharacterRole">The role of the character</param>
    /// <param name="charactersPool">Character pool for all the character</param>
    private List<Play> NecromancerLegalPlays(GameObject simCharacter, Roles.Role simCharacterRole, CharactersPool charactersPool)
    {
        List<Play> necromancerLegalPlays = new List<Play>();

        // Characters driven by the AI
        Character[] enemies =
        {
            charactersPool.enemies[0].GetComponent<Character>(),
            charactersPool.enemies[1].GetComponent<Character>()
        };
        // Characters driven by the player
        Character[] allies =
        {
            charactersPool.allies[0].GetComponent<Character>(),
            charactersPool.allies[1].GetComponent<Character>()
        };

        // Legal plays constraints
        if(enemies[0].GetSimulatedHealth() <= 0 && character.GetSimulatedHealth() > 0) { // Enemy 0 is dead (Necromancer is enemy 1)
            necromancerLegalPlays.Add(new Play(() => necromancer.Action2(true), simCharacter, simCharacterRole, "Action2", charactersPool.Simulation));
        }
        if(enemies[1].GetSimulatedHealth() <= 0 && character.GetSimulatedHealth() > 0) { // Enemy 1 is dead (Necromancer is enemy 0)
            necromancerLegalPlays.Add(new Play(() => necromancer.Action2(true), simCharacter, simCharacterRole, "Action2", charactersPool.Simulation));
        }
        if(character.GetSimulatedHealth() <= 30) { // Could be killed in that turn
            necromancerLegalPlays.Add(new Play(() => necromancer.Action3(true), simCharacter, simCharacterRole, "Action3", charactersPool.Turn));
        }
        necromancerLegalPlays.Add(new Play(() => necromancer.Action1(true), simCharacter, simCharacterRole, "Action1", charactersPool.Turn));

        return necromancerLegalPlays;
    }

    /// <summary>
    /// Executes the play action to get to the next state
    /// </summary>
    /// <param name="play"></param>
    /// <returns></returns>
    private State NextState(Play play)
    {
        play.Action(); // Runs the play method
        CharactersPool charactersPool = character.GetCharactersPool();

        return new State(charactersPool.Simulation, charactersPool.allies, charactersPool.enemies);
    }

    /// <summary>
    /// Decides the winner based on the health
    /// </summary>
    /// <param name="state">Game state where you need to decide the winner</param>
    /// <returns></returns>
    private Winner DecideWinner(State state)
    {
        if(state.EnemiesHealth <= 0) { // Enemies have no health left. Allies win
            return Winner.Allies;
        }
        else if(state.AlliesHealth <= 0) { // Allies have no health left. Enemies win
            return Winner.Enemies;
        }
        else { // No one has won yet
            return Winner.None;
        }
    }
}