
//Condition always returning true, meaning the react always acts
public class ConditionEmpty : Condition
{
    static System.Random rnd = new System.Random();

    //for mutating new rules, returns a random version of this condition
    public override Condition Randomize(Paradigm paradigm)
    {
        Condition newC = Instantiate(this);
        return newC;
    }

    //for implementation when the rule is followed by leviathans
    internal override bool Check(Leviathan leviathan)
    {
        return true;
    }
}