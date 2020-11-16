using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayToNode
{
    #region variables

    public Play Play { get; set; }
    public MonteCarloTreeNode Node { get; set; }

    #endregion variables

    public PlayToNode(Play play, MonteCarloTreeNode node)
    {
        Play = play;
        Node = node;
    }
}