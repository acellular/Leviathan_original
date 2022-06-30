
//Basic react option that assigns a given land use to plots with the given soil type
public class ReactBasicLandUse : React
{
    public LandUse landUseAssign;
    public string soilToAssign;

    static System.Random rnd = new System.Random();

    //assigns a random land use to a random soil type for creating new random rules
    public override React Randomize(Paradigm paradigm)
    {
        ReactBasicLandUse react = Instantiate(this);
        react.soilToAssign = Soil.types[rnd.Next(0, 7)];
        react.landUseAssign = paradigm.possibleLandUses[rnd.Next(0, paradigm.possibleLandUses.Count)];
        return react;
    }

    //assign given land use to given soil type
    internal override float Act(Leviathan leviathan)
    {
        float cost = 0;
        if (leviathan.production.plots != null)
        {
            foreach (Plot p in leviathan.production.plots)
            {
                if (p.soil.type == soilToAssign) { p.landUse = landUseAssign; cost += 1; } //each plot adds to the cost
            }
            return cost;
        }
        else { return 0;}
    }
}