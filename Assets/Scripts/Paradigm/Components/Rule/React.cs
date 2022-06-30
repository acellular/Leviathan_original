using UnityEngine;

//abstract react implemented by leviathans after the rule's condition check
public abstract class React : MonoBehaviour
{
    //return randomized version of react for paradigm mutation of new rules
    public abstract React Randomize(Paradigm paradigm);
    
    //implement the react in a leviathan if the rule's conditions are met
    internal abstract float Act(Leviathan leviathan);
}
