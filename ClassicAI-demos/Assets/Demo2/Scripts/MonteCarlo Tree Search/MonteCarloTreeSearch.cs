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

    private void Start()
    {
        allCharacters[0] = character.GetCharactersPool().enemies[0];
        allCharacters[1] = character.GetCharactersPool().enemies[1];
        allCharacters[2] = character.GetCharactersPool().allies[0];
        allCharacters[3] = character.GetCharactersPool().allies[1];

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

    private void CreateNode(State state)
    {
        if(!treeNodes.ContainsKey(state.GetHash())) {
            List<Play> unexpandedPlays = LegalPlays(state);
            MonteCarloTreeNode newNode = new MonteCarloTreeNode(null, null, state, unexpandedPlays);
            treeNodes.Add(state.GetHash(), newNode);
        }
    }

    public void RunMonteCarloTreeSearch(State state, int maxIterations, int enemyPosition)
    {
        int iterations = 0;
        this.enemyPosition = enemyPosition;
        characterIterator = enemyPosition;
        CreateNode(state);
        print(character.role + " [Initial State]: " + state.AlliesHealth + " - " + state.EnemiesHealth);

        while(iterations < maxIterations) {
            character.GetCharactersPool().Simulation++;
            MonteCarloTreeNode selectedNode = Select(state);
            Winner winner = DecideWinner(state);

            if(!selectedNode.IsLeaf() && winner == Winner.None) {
                selectedNode = Expand(selectedNode);
                winner = Simulate(selectedNode);
            }
            Backpropagate(selectedNode, winner);

            iterations++;
        }
    }

    private MonteCarloTreeNode Select(State state)
    {
        print("SELECT");
        MonteCarloTreeNode selectedNode;
        treeNodes.TryGetValue(state.GetHash(), out selectedNode);

        while(selectedNode.IsFullyExpanded() && !selectedNode.IsLeaf()) {
            List<Play> allPlays = selectedNode.AllPlays(character.role, character.GetCharactersPool().Simulation, knight, healer, guard, necromancer);
            Play bestPlay = null;
            float bestUCB1 = -1000000000;

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

    private MonteCarloTreeNode Expand(MonteCarloTreeNode node)
    {
        print("EXPAND");
        List<Play> unexpandedPlays = node.UnexpandedPlays();
        int randomIndex = UnityEngine.Random.Range(0, unexpandedPlays.Count);
        Play selectedPlay = unexpandedPlays[randomIndex];
        State childState = NextState(node.State, selectedPlay);
        List<Play> childLegalPlays = LegalPlays(childState);
        MonteCarloTreeNode childNode = node.Expand(selectedPlay, childState, childLegalPlays);

        treeNodes.Add(childState.GetHash(), childNode);

        return childNode;
    }

    private Winner Simulate(MonteCarloTreeNode node)
    {
        print("SIMULATE");
        characterIterator = enemyPosition;
        State actualState = node.State;
        Winner winner = DecideWinner(actualState);

        while(winner == Winner.None) {
            List<Play> legalPlays = LegalPlays(actualState);
            int randomIndex = UnityEngine.Random.Range(0, legalPlays.Count);
            Play randomPlay = null;
            if(randomIndex < legalPlays.Count) {
                randomPlay = legalPlays[randomIndex];
            }
            else {
                print("RandomIndex out of range: " + randomIndex);
            }
            actualState = NextState(actualState, randomPlay);
            winner = DecideWinner(actualState);

            characterIterator++;
            if(characterIterator >= 4) {
                characterIterator = 0;
            }
        }

        return winner;
    }

    private void Backpropagate(MonteCarloTreeNode node, Winner winner)
    {
        print("BACKPROPAGATE");
        while(node != null) {
            node.NumberOfPlays++;
            if(winner == Winner.Allies) { // Flip because each node’s statistics are used for its parent node’s choice, not its own.
                node.NumberOfWins++;
            }

            node = node.Parent;
        }
    }

    public Play BestPlay(State state)
    {
        MonteCarloTreeNode node;
        treeNodes.TryGetValue(state.GetHash(), out node);
        CreateNode(state);

        if(!node.IsFullyExpanded()) {
            throw new Exception("Not enough information!");
        }

        List<Play> allPlays = node.AllPlays(character.role, character.GetCharactersPool().Simulation, knight, healer, guard, necromancer);
        Play bestPlay = null;
        int maxPlays = -1000000000;

        foreach(Play play in allPlays) {
            MonteCarloTreeNode childNode = node.ChildNode(play);

            if(childNode.NumberOfPlays > maxPlays) {
                bestPlay = play;
                maxPlays = childNode.NumberOfPlays;
            }
        }

        return bestPlay;
    }

    private List<Play> LegalPlays(State state)
    {
        List<Play> legalPlaysList = new List<Play>();
        CharactersPool charactersPool = character.GetCharactersPool();
        Character[] enemies =
        {
            charactersPool.enemies[0].GetComponent<Character>(),
            charactersPool.enemies[1].GetComponent<Character>()
        };

        GameObject simCharacter = allCharacters[characterIterator];
        Roles.Role simCharacterRole = simCharacter.GetComponent<Character>().role;

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
        if(enemies[1].GetSimulatedHealth() > 0 && enemies[1].GetSimulatedHealth() < enemies[1].GetMaxHealth()) { // Enemy 1 is alive
            healerLegalPlays.Add(new Play(() => healer.Action1(1, true), simCharacter, simCharacterRole, "Action1-1", charactersPool.Simulation));
        }
        if(allies[0].GetSimulatedHealth() > 0) { // Ally 0 is allive
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

    private List<Play> GuardLegalPlays(GameObject simCharacter, Roles.Role simCharacterRole, CharactersPool charactersPool)
    {
        List<Play> guardLegalPlays = new List<Play>();
        bool allyNeedsHelp = false;

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
            && enemies[0].GetSimulatedHealth() <= enemies[0].GetMaxHealth() / 2) { // Enemy 0 has half of its health and is alive
            guardLegalPlays.Add(new Play(() => guard.Action2(0, true), simCharacter, simCharacterRole, "Action2-0", charactersPool.Simulation));
            allyNeedsHelp = true;
        }
        if(enemies[1].GetSimulatedHealth() > 0
            && enemies[1].GetSimulatedHealth() <= enemies[1].GetMaxHealth() / 2) { // Enemy 1 has half of its health and is alive
            guardLegalPlays.Add(new Play(() => guard.Action2(1, true), simCharacter, simCharacterRole, "Action2-1", charactersPool.Simulation));
            allyNeedsHelp = true;
        }
        if(allies[0].GetSimulatedHealth() > 0
            && !allyNeedsHelp) { // Ally 0 is alive and the enemies have more than half of their health
            guardLegalPlays.Add(new Play(() => guard.Action1(0, true), simCharacter, simCharacterRole, "Action1-0", charactersPool.Simulation));
        }
        if(allies[1].GetSimulatedHealth() > 0
            && !allyNeedsHelp) { // Ally 1 is alive and the enemies have more than half of their health
            guardLegalPlays.Add(new Play(() => guard.Action1(1, true), simCharacter, simCharacterRole, "Action1-1", charactersPool.Simulation));
        }
        if(character.GetSimulatedHealth() > enemies[0].GetSimulatedHealth()
            && enemies[0].GetSimulatedHealth() > 0) { // The guard has more health than the enemy protected (guard is enemy 1)
            guardLegalPlays.Add(new Play(() => guard.Action3(0, true), simCharacter, simCharacterRole, "Action3-0", charactersPool.Simulation));
        }
        if(character.GetSimulatedHealth() > enemies[1].GetSimulatedHealth()
            && enemies[1].GetSimulatedHealth() > 0) { // The guard has more health than the enemy protected (guard is enemy 0)
            guardLegalPlays.Add(new Play(() => guard.Action3(1, true), simCharacter, simCharacterRole, "Action3-1", charactersPool.Simulation));
        }

        return guardLegalPlays;
    }

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
        if(character.GetSimulatedHealth() <= 30) { // Can be killed in that turn
            necromancerLegalPlays.Add(new Play(() => necromancer.Action3(true), simCharacter, simCharacterRole, "Action3", charactersPool.Turn));
        }
        necromancerLegalPlays.Add(new Play(() => necromancer.Action1(true), simCharacter, simCharacterRole, "Action1", charactersPool.Turn));

        return necromancerLegalPlays;
    }

    private State NextState(State actualState, Play play)
    {
        play.Action(); // Runs the play method
        CharactersPool charactersPool = character.GetCharactersPool();

        return new State(charactersPool.Simulation, charactersPool.allies, charactersPool.enemies);
    }

    private Winner DecideWinner(State state)
    {
        if(state.EnemiesHealth <= 0) {
            return Winner.Allies;
        }
        else if(state.AlliesHealth <= 0) {
            return Winner.Enemies;
        }
        else {
            return Winner.None;
        }
    }
}