using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayToNode
{
    #region variables

    public Action Play { get; set; }
    public MonteCarloTreeNode Node { get; set; }

    #endregion variables

    public PlayToNode(Action play, MonteCarloTreeNode node)
    {
        Play = play;
        Node = node;
    }
}