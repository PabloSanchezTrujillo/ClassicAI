﻿using System;
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

    private Play play;
    private Dictionary<string, PlayToNode> children;
    private int unexpandedChildren;

    #endregion variables

    /// <summary>
    /// Monte Carlo Tree node constructor
    /// </summary>
    /// <param name="parent">Parent node</param>
    /// <param name="play">Node's play</param>
    /// <param name="state">Actual state of the game</param>
    /// <param name="unexpandedPlays">List of possibles unexpanded plays</param>
    public MonteCarloTreeNode(MonteCarloTreeNode parent, Play play, State state, List<Play> unexpandedPlays)
    {
        Parent = parent;
        this.play = play;
        State = state;
        children = new Dictionary<string, PlayToNode>();
        NumberOfPlays = 0;
        NumberOfWins = 0;
        unexpandedChildren = unexpandedPlays.Count;
        foreach(Play unexpandedPlay in unexpandedPlays) {
            children.Add(unexpandedPlay.GetHash(), new PlayToNode(unexpandedPlay, null));
        }
    }

    /// <summary>
    /// List of the children plays not expanded yet
    /// </summary>
    public List<Play> UnexpandedPlays()
    {
        List<Play> unexpandedPlays = new List<Play>();

        foreach(PlayToNode child in children.Values) {
            if(child.Node == null) {
                unexpandedPlays.Add(child.Play);
            }
        }

        return unexpandedPlays;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="play"></param>
    /// <returns></returns>
    public MonteCarloTreeNode ChildNode(Play play)
    {
        PlayToNode child;
        children.TryGetValue(play.GetHash(), out child);

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

    public MonteCarloTreeNode Expand(Play play, State childState, List<Play> unexpandedPlays)
    {
        if(!children.ContainsKey(play.GetHash())) {
            throw new Exception("No such play!");
        }

        MonteCarloTreeNode childNode = new MonteCarloTreeNode(this, play, childState, unexpandedPlays);
        children[play.GetHash()] = new PlayToNode(play, childNode);
        //children.Add(play.GetHash(), new PlayToNode(play, childNode));

        return childNode;
    }

    public Dictionary<string, PlayToNode> GetChildren()
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

    public List<Play> AllPlays(Roles.Role role, int turn, Knight knight, Healer healer, Guard guard, Necromancer necromancer)
    {
        List<Play> allPlays = new List<Play>();

        foreach(PlayToNode child in children.Values) {
            allPlays.Add(child.Play);
        }

        return allPlays;
    }
}