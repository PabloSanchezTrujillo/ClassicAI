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
    private Dictionary<int, MonteCarloTreeNode> treeNodes;

    private enum Winner
    {
        None,
        Allies,
        Enemies
    }

    #endregion variables

    private void Awake()
    {
        treeNodes = new Dictionary<int, MonteCarloTreeNode>();

        character = GetComponent<Character>();
        switch(character.role) {
            case Roles.Role.Knight:
                knight = GetComponent<Knight>();
                break;

            case Roles.Role.Healer:
                healer = GetComponent<Healer>();
                break;

            case Roles.Role.Guard:
                guard = GetComponent<Guard>();
                break;

            case Roles.Role.Necromancer:
                necromancer = GetComponent<Necromancer>();
                break;
        }
    }

    private void CreateNode(State state)
    {
        if(!treeNodes.ContainsKey(state.GetHashCode())) {
            List<Action> unexpandedPlays = LegalPlays(state);
            MonteCarloTreeNode newNode = new MonteCarloTreeNode(null, null, state, unexpandedPlays);
            treeNodes.Add(state.GetHashCode(), newNode);
        }
    }

    public void RunMonteCarloTreeSearch(State state, int maxIterations)
    {
        int iterations = 0;
        CreateNode(state);

        while(iterations < maxIterations) {
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
        MonteCarloTreeNode selectedNode;
        treeNodes.TryGetValue(state.GetHashCode(), out selectedNode);

        while(selectedNode.IsFullyExpanded() && !selectedNode.IsLeaf()) {
            List<Action> allPlays = selectedNode.AllPlays(character.role, knight, healer, guard, necromancer);
            Action bestPlay = null;
            float bestUCB1 = -1000000000;

            foreach(Action play in allPlays) {
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
        List<Action> unexpandedPlays = node.UnexpandedPlays();
        int randomIndex = UnityEngine.Random.Range(0, unexpandedPlays.Count);
        Action selectedPlay = unexpandedPlays[randomIndex];
        State childState = NextState(node.State, selectedPlay);
        List<Action> childLegalPlays = LegalPlays(childState);
        MonteCarloTreeNode childNode = node.Expand(selectedPlay, childState, childLegalPlays);

        treeNodes.Add(childState.GetHashCode(), childNode);

        return childNode;
    }

    private Winner Simulate(MonteCarloTreeNode node)
    {
        State actualState = node.State;
        Winner winner = DecideWinner(actualState);

        while(winner == Winner.None) {
            List<Action> legalPlays = LegalPlays(actualState);
            int randomIndex = UnityEngine.Random.Range(0, legalPlays.Count);
            Action randomPlay = legalPlays[randomIndex];
            actualState = NextState(actualState, randomPlay);
            winner = DecideWinner(actualState);
        }

        return winner;
    }

    private void Backpropagate(MonteCarloTreeNode node, Winner winner)
    {
        while(node != null) {
            node.NumberOfPlays++;
            if(winner == Winner.Allies) { // Flip because each node’s statistics are used for its parent node’s choice, not its own.
                node.NumberOfWins++;
            }

            node = node.Parent;
        }
    }

    private Action BestPlay(State state)
    {
        MonteCarloTreeNode node;
        treeNodes.TryGetValue(state.GetHashCode(), out node);
        CreateNode(state);

        if(!node.IsFullyExpanded()) {
            throw new Exception("Not enough information!");
        }

        List<Action> allPlays = node.AllPlays(character.role, knight, healer, guard, necromancer);
        Action bestPlay = null;
        int maxPlays = -1000000000;

        foreach(Action play in allPlays) {
            MonteCarloTreeNode childNode = node.ChildNode(play);

            if(childNode.NumberOfPlays > maxPlays) {
                bestPlay = play;
                maxPlays = childNode.NumberOfPlays;
            }
        }

        return bestPlay;
    }

    // TODO: Revisar las LegalPlays para tener en cuenta el state
    private List<Action> LegalPlays(State state)
    {
        List<Action> legalPlaysList = new List<Action>();
        Character[] enemies =
        {
            character.GetCharactersPool().enemies[0].GetComponent<Character>(),
            character.GetCharactersPool().enemies[1].GetComponent<Character>()
        };

        switch(character.role) {
            case Roles.Role.Knight:
                legalPlaysList.Add(() => StartCoroutine(knight.Action1()));
                legalPlaysList.Add(() => knight.Action2());
                legalPlaysList.Add(() => knight.Action3());
                break;

            case Roles.Role.Healer:
                if(enemies[0].GetHealth() > 0 && enemies[1].GetHealth() > 0) { // Both enemies are alive
                    legalPlaysList.Add(() => StartCoroutine(healer.Action1()));
                    legalPlaysList.Add(() => StartCoroutine(healer.Action3()));
                }
                legalPlaysList.Add(() => StartCoroutine(healer.Action2()));
                break;

            case Roles.Role.Guard:
                if(enemies[0].GetHealth() > 0 && enemies[1].GetHealth() > 0) { // Both enemies are alive
                    legalPlaysList.Add(() => StartCoroutine(guard.Action2()));
                    legalPlaysList.Add(() => StartCoroutine(guard.Action3()));
                }
                legalPlaysList.Add(() => StartCoroutine(guard.Action1()));
                break;

            case Roles.Role.Necromancer:
                if(enemies[0].GetHealth() <= 0 || enemies[1].GetHealth() <= 0) { // One enemy is dead
                    legalPlaysList.Add(() => necromancer.Action2());
                }
                if(character.GetHealth() <= 30) { // Can be killed in that turn
                    legalPlaysList.Add(() => necromancer.Action3());
                }
                legalPlaysList.Add(() => necromancer.Action1());
                break;
        }

        return legalPlaysList;
    }

    private State NextState(State actualState, Action play)
    {
        int alliesHealth = 0;
        int enemiesHealth = 0;

        play(); // Runs the play method
        foreach(GameObject ally in character.GetCharactersPool().allies) {
            alliesHealth += ally.GetComponent<Character>().GetHealth();
        }
        foreach(GameObject enemy in character.GetCharactersPool().enemies) {
            enemiesHealth += enemy.GetComponent<Character>().GetHealth();
        }

        return new State(alliesHealth, enemiesHealth);
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