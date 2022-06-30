
//Basic condition checking for a soil type on any plot in a settlement.
//Since plot soil types are currently assigned randomly, most settlements have
//plots with most soil types, meaning this does not yet do much.
//Sould be more usefull if combined with a climate simulation that results
//in settlements with different dominant soil types.
public class ConditionRandomSoilCheck : Condition
{
    public string soilCheck;

    static System.Random rnd = new System.Random();

    //for mutating new rules, returns a randomized version of this condition
    public override Condition Randomize(Paradigm paradigm)
    {
        ConditionRandomSoilCheck newC = Instantiate(this);
        newC.soilCheck = Soil.types[rnd.Next(0, 6)];
        return newC;
    }

    //for implementation when the rule is followed by leviathans
    //do any plots have the soil type
    internal override bool Check(Leviathan leviathan)
    {
        foreach (Plot p in leviathan.production.plots)
        {
            if (p.soil.type == soilCheck) { return true; }
        }
        return false;
    }
}