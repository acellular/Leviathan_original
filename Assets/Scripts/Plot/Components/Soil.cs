using UnityEngine;

//The basic type of land, use through plots combined with landuse to calculate yields through work.
//Contains both the types of possible soils (which should be later moved to the editor)
//and also the depletion of the soil in the specific plots soil components are added to.
public class Soil : MonoBehaviour
{
    public string type;

    //for random stuff in conditions and reactions
    public static string[] types =
        { "bare", "clay", "sand", "silt", "peat", "chalk", "loam" };

    static System.Random rnd = new System.Random();
    public float depletion = 0; //(float)rnd.NextDouble()*.5f;//starting depletion

    void Awake()
    {
        //give this soil a random type if none already given
        if (type == null || type == "")
        {
            type = types[rnd.Next(0, 7)];
        }
    }
}
