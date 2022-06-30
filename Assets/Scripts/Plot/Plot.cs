using UnityEngine;

//holds a soil type, and returns yield according to landuse assigned to it
public class Plot : MonoBehaviour
{
    public LandUse landUse;

    static System.Random rnd = new System.Random();

    public Soil soil;
    public int yearOffSet;

    private void Awake()
    {
        //somewhat messy way of making sure not all plots of the
        //same soil type are have the same crop type for each year of a crop rotation
        //later: land use rotations should be figured at the leviathan level
        yearOffSet = rnd.Next(0, 4);
    }

    void Start()
    {
        soil = GetComponent<Soil>();
    }

    public float Yield()
    {
        //use the land according to assigned LandUse and then deplete the soil
        float y = landUse.Yield(this);
        landUse.Deplete(this);
        return y;
    }
}
