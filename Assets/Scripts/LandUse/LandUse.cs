using UnityEngine;

//Base class for how land plots should be used
//made up of yield and work components
public class LandUse : MonoBehaviour
{
    internal Work work;
    internal Yields yields;

    void Start()
    {
        work = GetComponent<Work>();
        yields = GetComponent<Yields>();
    }

    internal float Yield(Plot plot)
    {
        return work.Yield(plot, yields);
    }

    //how much does this land use deplete the soil (negative depletion regenerates the soil)
    internal void Deplete(Plot plot)
    {
        plot.soil.depletion += yields.Depletion(plot.soil.type);
        if (plot.soil.depletion < 0) { plot.soil.depletion = 0; }
        else if (plot.soil.depletion > 1) { plot.soil.depletion = 1; }
        //plot.soil.depletion = .5f;//TEST
    }
}
