using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Policy;
using UnityEditor.PackageManager;
using UnityEngine;

public class MonteCarloTreeNode
{
    #region variables

    public int NumberOfPlays { get; set; }
    public int NumberOfWins { get; set; }
    public State State { get; set; }
    public MonteCarloTreeNode Parent { get; set; }

    private Action play;
    private Dictionary<int, PlayToNode> children;
    private int unexpandedChildren;

    #endregion variables

    public MonteCarloTreeNode(MonteCarloTreeNode parent, Action play, State state, List<Action> unexpandedPlays)
    {
        Parent = parent;
        this.play = play;
        State = state;
        children = new Dictionary<int, PlayToNode>();
        NumberOfPlays = 0;
        NumberOfWins = 0;
        unexpandedChildren = unexpandedPlays.Count;
        foreach(Action unexpandedPlay in unexpandedPlays) {
            children.Add(unexpandedPlay.GetHashCode(), new PlayToNode(unexpandedPlay, null));
        }
    }

    public List<Action> UnexpandedPlays()
    {
        List<Action> unexpandedPlays = new List<Action>();

        foreach(PlayToNode child in children.Values) {
            if(child.Node == null) {
                unexpandedPlays.Add(child.Play);
            }
        }

        return unexpandedPlays;
    }

    public MonteCarloTreeNode ChildNode(Action play)
    {
        PlayToNode child;
        children.TryGetValue(play.GetHashCode(), out child);

        if(child == null) {
            throw new Exception("No such play!");
        }
        else if(child.Node == null) {
            throw new Exception("Child is not expanded!");
        }

        return child.Node;
    }

    public float GetUCB1(float explorationParam)
    {
        float param1 = NumberOfWins / NumberOfPlays;
        double param2 = explorationParam * Math.Sqrt(Math.Log(Parent.NumberOfPlays) / NumberOfPlays);
        float UCB1 = param1 + (float)param2;

        return UCB1;
    }

    public MonteCarloTreeNode Expand(Action play, State childState, List<Action> unexpandedPlays)
    {
        if(!children.ContainsKey(play.GetHashCode())) {
            throw new Exception("No such play!");
        }

        MonteCarloTreeNode childNode = new MonteCarloTreeNode(this, play, childState, unexpandedPlays);
        children.Add(play.GetHashCode(), new PlayToNode(play, childNode));

        return childNode;
    }

    public Dictionary<int, PlayToNode> GetChildren()
    {
        return children;
    }

    public bool IsFullyExpanded()
    {
        foreach(PlayToNode child in children.Values) {
            if(child.Node == null) {
                return false;
            }
        }

        return true;
    }

    public bool IsLeaf()
    {
        return (children.Count == 0) ? true : false;
    }

    public List<Action> AllPlays(Roles.Role role, Knight knight, Healer healer, Guard guard, Necromancer necromancer)
    {
        List<Action> allPlays = new List<Action>();

        switch(role) {
            case Roles.Role.Knight:
                allPlays.Add(() => knight.FireAction1());
                allPlays.Add(() => knight.FireAction2());
                allPlays.Add(() => knight.FireAction3());
                break;

            case Roles.Role.Healer:
                allPlays.Add(() => healer.FireAction1());
                allPlays.Add(() => healer.FireAction2());
                allPlays.Add(() => healer.FireAction3());
                break;

            case Roles.Role.Guard:
                allPlays.Add(() => guard.FireAction1());
                allPlays.Add(() => guard.FireAction2());
                allPlays.Add(() => guard.FireAction3());
                break;

            case Roles.Role.Necromancer:
                allPlays.Add(() => necromancer.FireAction1());
                allPlays.Add(() => necromancer.FireAction2());
                allPlays.Add(() => necromancer.FireAction3());
                break;
        }

        return allPlays;
    }
}