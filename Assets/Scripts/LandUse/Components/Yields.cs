using UnityEngine;

//A list of yields and depletion rates
//given by a particular land use on different soil types.
//Use to create prefabs in editor for different landuses
public class Yields : MonoBehaviour
{
    public float yieldBare = 1;
    public float yieldClay = 1;
    public float yieldSand = 1;
    public float yieldSilt = 1;
    public float yieldPeat = 1;
    public float yieldChalk = 1;
    public float yieldLoam = 1;

    public float depletionBare = .01f;
    public float depletionClay = .01f;
    public float depletionSand = .01f;
    public float depletionSilt = .01f;
    public float depletionPeat = .01f;
    public float depletionChalk = .01f;
    public float depletionLoam = .01f;

    //global control reference
    HumanSystemsManager manager;

    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<HumanSystemsManager>();
    }

    //returns the yield by soil type of the given plot
    internal float Yield(Plot plot)
    {
        string t = plot.soil.type;
        float y = 0;
        
        if (t == "bare" ) { y = yieldBare; }
        else if (t == "clay") { y = yieldBare; }
        else if (t == "sand") { y = yieldSand; }
        else if (t == "silt") { y = yieldSilt; }
        else if (t == "peat") { y = yieldPeat; }
        else if (t == "chalk") { y = yieldChalk; }
        else if (t == "loam") { y = yieldLoam; }
        
        return y * manager.yieldMultiplier;
    }

    internal float Depletion(string t)//should be a seperate object?
    {

        float d = 0;

        if (t == "bare") { d = depletionBare; }
        else if (t == "clay") { d = depletionBare; }
        else if (t == "sand") { d = depletionSand; }
        else if (t == "silt") { d = depletionSilt; }
        else if (t == "peat") { d = depletionPeat; }
        else if (t == "chalk") { d = depletionChalk; }
        else if (t == "loam") { d = depletionLoam; }

        return d;
    }
}

