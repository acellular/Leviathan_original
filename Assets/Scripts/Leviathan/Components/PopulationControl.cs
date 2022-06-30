using System;
using System.Collections.Generic;
using UnityEngine;

//Called monthly by leviathans. Handles immigration, emmigration, births and deaths in the settlement
public class PopulationControl : MonoBehaviour
{ 
    static System.Random rnd = new System.Random();

    //for randomizing inhabitants at start
    public Human humanPrefab;
    public int startingHumans;
    public List<Human> humans = new List<Human>();

    int numHumansOfAge = 0;//reset in the main popCon loop

    ICONORHYTHM icono;
    Leviathan leviathan;
    Consumption consumption;

    public int[] birthsLastNineMonths = new int[36];//36 because Run(), where this is used, is called weekly

    bool firstUpdate = true;

    public bool depopulated;

    private void Start()
    {
        birthsLastNineMonths.Fill(0);
        icono = GetComponent<ICONORHYTHM>();
        leviathan = GetComponent<Leviathan>();
        consumption = GetComponent<Consumption>();
    }

    private void Update()
    {
        if (firstUpdate)
        {
            if (startingHumans == 0)
            {
                Debug.Log("NO HUMANS AT START");
                leviathan.Depopulation();
            }
            else { GenerateHumans(startingHumans); }
            firstUpdate = false;
        }
    }

    public void Run()//called weekly!
    {
        //is there not enough food?
        if (consumption.weeklySurplus < 0)
        {
            //if there isn't enought food, is anyone going to leave?
            float baseEmProb = leviathan.paradigm.baseEmProb;
            if (rnd.NextDouble() < baseEmProb * ((100 - icono.comfort) / 2))
            {
                int numHumansEmmigrating = (int)Math.Round((100 - icono.comfort)
                    * (100 - icono.comfort) * humans.Count * (float)baseEmProb);
                List<Human> humansEmmigrating = new List<Human>();
                if (numHumansEmmigrating > 0)
                {
                    for (int i = 0; i < numHumansEmmigrating; i++)
                    {
                        int humansIndex = rnd.Next(0, humans.Count);
                        humansEmmigrating.Add(humans[humansIndex]);
                        humans.RemoveAt(humansIndex);
                    }
                    if (humans.Count == 0) { leviathan.Depopulation(); }
                    leviathan.manager.Emmigration(leviathan, humansEmmigrating);
                    icono.AdjustComfort(-(humans.Count / (humansEmmigrating.Count + 1)) * leviathan.paradigm.emmiCom);
                }
            }
        }

        //DEATHS
        List<Human> humansToDie = new List<Human>();
        foreach (Human h in humans)
        {
            if (h.ReadyToDie())
            {
                humansToDie.Add(h);
            }
        }
        foreach (Human h in humansToDie)
        {
            icono.AdjustComfort(leviathan.paradigm.deathCom / humans.Count);
            humans.Remove(h);
            Destroy(h.gameObject);
            if (humans.Count == 0) { leviathan.Depopulation(); }
        }

        //BIRTHS
        //can't keep having kids if already had them
        int lastNineMonthBirths = 0;
        for (int i = 0; i < birthsLastNineMonths.Length; i++)
        {
            lastNineMonthBirths += birthsLastNineMonths[i];
        }
        //then shift nine month's birth  array
        for (int i = 0; i < birthsLastNineMonths.Length - 1; i++)
        {
            birthsLastNineMonths[i] = birthsLastNineMonths[i + 1];
        }
        birthsLastNineMonths[birthsLastNineMonths.Length - 1] = 0;

        //find number of humans over 16
        numHumansOfAge = 0;
        foreach (Human h in humans)
        {
            if (h.age > 16 * 52) { numHumansOfAge++; }
        }
        numHumansOfAge = (int)Math.Round(numHumansOfAge * 0.5f);//only half can have kids

        //add up surpluses from past 9 months
        float nineMonthSurplus = 0;
        for (int i = consumption.monthlySurpluses.Length - 1; i > consumption.monthlySurpluses.Length - 10; i--)
        {
            nineMonthSurplus += consumption.monthlySurpluses[i];
        }
        if (nineMonthSurplus < 1) { nineMonthSurplus = 1; }

        //should there by ANY births?-->WANT IT TO EVOLVE FAST!?-->up this!
        if (numHumansOfAge > 1 && lastNineMonthBirths < numHumansOfAge && rnd.NextDouble() < ((icono.comfort * 0.01f) * leviathan.paradigm.birthRate) + leviathan.paradigm.birthRate)
        {
            int numBirths = (int)(numHumansOfAge * leviathan.paradigm.birthRate) + 1;
            if (numBirths < 0) { numBirths = 0; }

            //add to array of kids born in the last nine months
            birthsLastNineMonths[birthsLastNineMonths.Length - 1] = numBirths;

            //BIRTH THEM!
            for (int i = 0; i < numBirths; i++)
            {
                Human humanToAdd = Instantiate(humanPrefab);
                humanToAdd.age = 0;
                humans.Add(humanToAdd);
                icono.AdjustComfort(leviathan.paradigm.birthCom / humans.Count);//births increase comfort
            }
        }

        //increase ages
        foreach (Human h in humans)
        {
            h.age++;
        }
    }

    //called by leviathan during setup to populate with random humans
    internal void GenerateHumans(int numHumans)
    {        
        //otherwise make humans!
        for (int i = 0; i < numHumans; i++)
        {
            humans.Add(Instantiate(humanPrefab));
        }
    }

    //add humans to this leviathan LATER-->can do things like refuse to take them and send them out again
    internal void Immigration(List<Human> humansEmmigrating)
    {
        humans.AddRange(humansEmmigrating);
        if (leviathan.abandoned)
        {
            leviathan.abandoned = false;
        }
    }
}
