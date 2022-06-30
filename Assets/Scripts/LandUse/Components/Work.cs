using UnityEngine;

//calculate the yields for a plot--intended to allow for different types of work components later
public class Work : MonoBehaviour
{
    internal float Yield(Plot plot, Yields yields)
    {
        //return yields.Yield(plot) * ((plot.soil.depletion - 100f) * -0.01f);
        return yields.Yield(plot) * ((plot.soil.depletion - 1f) * -1f);
        //return yields.Yield(plot) * (1f - plot.soil.depletion);
    }
}
