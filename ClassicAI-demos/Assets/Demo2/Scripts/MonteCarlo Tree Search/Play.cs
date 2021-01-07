using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play
{
    #region variables

    public Action Action { get; set; }
    public GameObject GameObject { get; set; }
    public string Name { get; set; }
    public Roles.Role Role { get; set; }

    private readonly int turn;

    #endregion variables

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="action">Character action of the play</param>
    /// <param name="gameObject">Character that executes the play</param>
    /// <param name="role">The role of the character</param>
    /// <param name="name">The name of the play</param>
    /// <param name="turn">The turn when it was performed this play</param>
    public Play(Action action, GameObject gameObject, Roles.Role role, string name, int turn)
    {
        Action = action;
        GameObject = gameObject;
        Name = name;
        Role = role;
        this.turn = turn;
    }

    public string GetHash()
    {
        return Role.ToString() + "-" + Action.Method.Name + "-" + turn.ToString();
    }
}