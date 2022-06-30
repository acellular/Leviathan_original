using System.Collections.Generic;
using UnityEngine;

//called monthly by leviathans to track yields produced and any other inbound yields
public class Production : MonoBehaviour
{
    public float weeklyYield = 1;
    public float[] monthlyYields = new float[120];
    public float totalYield = 0;
    public float workRate = .7f;//modifies base human potential
    public float humanPotential = 0;

    static System.Random rnd = new System.Random();

    public Plot plotPrefab;
    public int numPlots = 10;//for randomized setup
    public List<Plot> plots = new List<Plot>();

    Leviathan leviathan;
    
    internal float outsideChangeIN = 0;

    private void Awake()
    {
        monthlyYields.Fill(100);//not just zero because don't want to be dividing by expectations
    }

    private void Start()
    {
        leviathan = GetComponent<Leviathan>();

        //randomize plot soil types and starting land uses
        for (int i = 0; i < numPlots; i++)
        {
            Plot p = Instantiate<Plot>(plotPrefab);
            p.landUse = leviathan.paradigm.possibleLandUses[rnd.Next(0, leviathan.paradigm.possibleLandUses.Count)];
            plots.Add(p);
        }
    }

    //main production loop
    internal void Run()
    {
        //reset
        weeklyYield = 0;
        humanPotential = 0;

        //what is the work potential of current inhabitants?
        foreach (Human h in leviathan.popCon.humans)
        {
            //if (h.age < 16 * 52) { humanPotential += (h.age / 52f) / 16f; }
            //else if (h.age > 35 * 52) { humanPotential += 35f / (h.age / 52f); }

            if (h.age < 832) { humanPotential += h.age * 0.0012f; }//for better performace
            else if (h.age > 1820) { humanPotential += 35f / (h.age * 0.019f); }
            else { humanPotential++; }
        }
        //multiplied by workRate
        humanPotential *= workRate;

        //"fast sigmoid"-->can't just pile many workers on the same plot and expect linear increases
        float potentialPart = humanPotential / (float)plots.Count * 2;
        float adjustedPotentialPerPlot = potentialPart / (1 + potentialPart);

        //then work the land and add to yields
        //implied in this method is communal/central organization within settlements
        foreach (Plot plot in plots)
        {
            float y = plot.Yield() * adjustedPotentialPerPlot;
            if (y < 0) { y = 0; }
            weeklyYield += y;
        }

        //add any gifts and clear them
        weeklyYield += outsideChangeIN;
        outsideChangeIN = 0;

        //update totals
        monthlyYields[monthlyYields.Length - 1] += weeklyYield;
        totalYield += weeklyYield;
    }

    //keeping track of monthly yields
    internal void Monthly()
    {
        for (int i = 0; i < monthlyYields.Length - 1; i++)
        {
            monthlyYields[i] = monthlyYields[i + 1];
        }
        monthlyYields[monthlyYields.Length - 1] = 0;
    }

    //would probably be a property if I were a proper programmer
    internal void AdjustWorkRate(float workRateAdjust)
    {
        workRate += workRateAdjust;
        if (workRate > 1) { workRate = 1; }
        else if (workRate < 0) { workRate = 0; }
    }
}
