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
        characterIterator = 0;
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

    public void RunMonteCarloTreeSearch(State state, int maxIterations)
    {
        int iterations = 0;
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

        print("STOP");
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
        State actualState = node.State;
        Winner winner = DecideWinner(actualState);

        while(winner == Winner.None) {
            List<Play> legalPlays = LegalPlays(actualState);
            int randomIndex = UnityEngine.Random.Range(0, legalPlays.Count);
            Play randomPlay = legalPlays[randomIndex];
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

    // TODO: Revisar las LegalPlays para tener en cuenta el state
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
                legalPlaysList.Add(new Play(() => StartCoroutine(knight.SimulatedAction1()), simCharacter, simCharacterRole, "Action1", charactersPool.Simulation));
                legalPlaysList.Add(new Play(() => knight.SimulatedAction2(), simCharacter, simCharacterRole, "Action2", charactersPool.Simulation));
                legalPlaysList.Add(new Play(() => knight.SimulatedAction3(), simCharacter, simCharacterRole, "Action3", charactersPool.Simulation));
                break;

            case Roles.Role.Healer:
                if(enemies[0].GetSimulatedHealth() > 0 && enemies[1].GetSimulatedHealth() > 0) { // Both enemies are alive
                    legalPlaysList.Add(new Play(() => StartCoroutine(healer.SimulatedAction1()), simCharacter, simCharacterRole, "Action1", charactersPool.Simulation));
                    legalPlaysList.Add(new Play(() => StartCoroutine(healer.SimulatedAction3()), simCharacter, simCharacterRole, "Action3", charactersPool.Simulation));
                }
                legalPlaysList.Add(new Play(() => StartCoroutine(healer.SimulatedAction2()), simCharacter, simCharacterRole, "Action2", charactersPool.Simulation));
                break;

            case Roles.Role.Guard:
                if(enemies[0].GetSimulatedHealth() > 0 && enemies[1].GetSimulatedHealth() > 0) { // Both enemies are alive
                    legalPlaysList.Add(new Play(() => StartCoroutine(guard.SimulatedAction2()), simCharacter, simCharacterRole, "Action2", charactersPool.Simulation));
                    legalPlaysList.Add(new Play(() => StartCoroutine(guard.SimulatedAction3()), simCharacter, simCharacterRole, "Action3", charactersPool.Simulation));
                }
                legalPlaysList.Add(new Play(() => StartCoroutine(guard.SimulatedAction1()), simCharacter, simCharacterRole, "Action1", charactersPool.Simulation));
                break;

            case Roles.Role.Necromancer:
                if(enemies[0].GetSimulatedHealth() <= 0 || enemies[1].GetSimulatedHealth() <= 0) { // One enemy is dead
                    legalPlaysList.Add(new Play(() => necromancer.SimulatedAction2(), simCharacter, simCharacterRole, "Action2", charactersPool.Simulation));
                }
                if(character.GetSimulatedHealth() <= 30) { // Can be killed in that turn
                    legalPlaysList.Add(new Play(() => necromancer.SimulatedAction3(), simCharacter, simCharacterRole, "Action3", charactersPool.Turn));
                }
                legalPlaysList.Add(new Play(() => necromancer.SimulatedAction1(), simCharacter, simCharacterRole, "Action1", charactersPool.Turn));
                break;
        }

        return legalPlaysList;
    }

    private State NextState(State actualState, Play play)
    {
        // TODO: Simulate the next state
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