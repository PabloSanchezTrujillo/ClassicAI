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