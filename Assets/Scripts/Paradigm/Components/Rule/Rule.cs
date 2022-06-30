using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//a paradigm rule used by leviathan to organize itself, made up of conditions and reacts (actions)
public class Rule : MonoBehaviour
{
    public List<Condition> conditions = new List<Condition>();//CHANGE THIS TO COMPONENT STYLE???
    public List<React> reacts = new List<React>();

    //for preset rules
    void Awake()
    {
        //conditions = GetComponents<Condition>().ToList();
        //reacts = GetComponents<React>().ToList();
    }

    //Check each condition in this rule
    //should change this to components to be consistent in editor?
    internal bool Check(Leviathan leviathan)
    {
        foreach (Condition c in conditions)
        {
            if (!c.Check(leviathan)) { return false; }
        }
        return true;
    }

    //do the actions specified by this rule
    //should change this to components to be consistent in editor?
    //AND RETURN THE COST OF THIS ACTION

    internal float Act(Leviathan leviathan)
    {
        float cost = 0;
        foreach (React a in reacts)
        {
            cost += a.Act(leviathan);
        }
        return cost;
    }

    //should change this to adding components to be consistent in editor?
    internal void AddConditionsAndActions(List<Condition> c, List<React> a)
    {
        foreach (Condition con in c) { conditions.Add(con); }
        foreach (React rea in a) { reacts.Add(rea); }
    }
}
