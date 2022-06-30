using System;

//basic react option that assigns a given land use to plots with the given soil type
public class ReactLandUseRotation : React
{
    public LandUse landUseYear1;
    public LandUse landUseYear2;
    public string soilToAssign;

    static System.Random rnd = new System.Random();

    //assigns a random land use for the two years of crop rotation
    public override React Randomize(Paradigm paradigm) 
    {
        ReactLandUseRotation react = Instantiate(this);
        react.soilToAssign = Soil.types[rnd.Next(0, 7)];
        react.landUseYear1 = paradigm.possibleLandUses[rnd.Next(0, paradigm.possibleLandUses.Count)];
        react.landUseYear2 = paradigm.possibleLandUses[rnd.Next(0, paradigm.possibleLandUses.Count)];
        return react;
    }

    //Assign given land use to given soil type according to the year of crop rotation,
    //messy method that should eventually move
    internal override float Act(Leviathan leviathan)
    {
        float cost = 0;
        if (leviathan.production.plots != null)
        {
            foreach (Plot p in leviathan.production.plots)
            {
                if (p.soil.type == soilToAssign)
                {
                    float weeksTo2Years = leviathan.manager.numWeeks / 104f;
                    if (p.yearOffSet == 0 || p.yearOffSet == 2)
                    {
                        if (weeksTo2Years - Math.Truncate(weeksTo2Years) < .5)//if first year of crop rotation
                        {
                            p.landUse = landUseYear1;
                        }
                        else
                        {
                            p.landUse = landUseYear2;
                        }
                    }
                    if (p.yearOffSet == 1 || p.yearOffSet == 3)
                    {
                        if (weeksTo2Years - Math.Truncate(weeksTo2Years) < .5)//if second year of crop rotation
                        {
                            p.landUse = landUseYear2;
                        }
                        else
                        {
                            p.landUse = landUseYear1;
                        }
                    }
                    cost += 1.5f;//previously 1
                }
            }
            return cost;
        }
        else { return 0; }
    }
}
