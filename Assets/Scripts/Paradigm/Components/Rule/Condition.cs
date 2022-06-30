using UnityEngine;

//Abstract for rule conditions that are checked before that rule's react is implemented
public abstract class Condition : MonoBehaviour
{
    //return randomized rule--for paradigm mutation of new rules
    public abstract Condition Randomize(Paradigm paradigm);

    //for checks on rule implementation
    internal abstract bool Check(Leviathan leviathan);

}
