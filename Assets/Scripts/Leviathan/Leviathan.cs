using System;
using UnityEngine;

//The settlement class:
//leviathan is a reference to Hobbes, representing the leviathan created by the covenant
//between settlement inhabitants and their leaders, who make all the decisions, with the original
//intention of being extendable to other heirarchies like corporations or religions
public class Leviathan : MonoBehaviour
{
    public Paradigm paradigm;

    static System.Random rnd = new System.Random();

    //global control reference
    public HumanSystemsManager manager;

    //component references
    public PopulationControl popCon;
    public Consumption consumption;
    public Production production;
    public ICONORHYTHM icono;
    public Renderer rend;

    //tracking stuff
    public bool abandoned = false;
    internal float rulesCost;

    // Start is called before the first frame update
    void Start()
    {
        //get component references
        popCon = GetComponent<PopulationControl>();
        consumption = GetComponent<Consumption>();
        production = GetComponent<Production>();
        icono = GetComponent<ICONORHYTHM>();
        rend = GetComponent<Renderer>();

        //add to global list of leviathans
        manager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<HumanSystemsManager>();
        manager.Addleviathan(this);

        //add self to paradigm leviathans list
        paradigm.numFollowers++;

        //setup
        Organizeleviathan();
    }

    //the basic leviathan loop, run through the Human system's manager's main loop
    public void Run(int numWeeks)
    {
        float weeksToMonths = numWeeks / 4f;

        //if this is a populated leviathan:
        if (!abandoned)
        {
            //Weekly production and consumption
            production.Run();
            consumption.Run();
            popCon.Run();

            //monthly
            if (weeksToMonths - Math.Truncate(weeksToMonths) == 0)
            {
                //is it time for a paradigm shift!?
                icono.Run();

                //use paradigm rules to organize this leviathan
                Organizeleviathan();

                //clean up monthly counts--->just moving the monthly totals array
                production.Monthly();
                consumption.Monthly();
            }
        }
        
        //this should be elsewhere
        if (!abandoned && rend.material.color != paradigm.colour)
        {
            rend.material.color = paradigm.colour;
        }
    }


    private void Organizeleviathan()
    {
        rulesCost = 0;

        //ORGANIZE THE leviathan ACCORDING TO PARADIGM RULES
        foreach (Rule r in paradigm.rules)
        {
            //check the rule's condition and if passes, implement reaction
            if (r.Check(this)) 
            { 
                rulesCost += r.Act(this);
            }
        }

        //basic costs (which leave less yield for people to actually eat)
        consumption.outsideChangeOUT += rulesCost * 3f;
        consumption.outsideChangeOUT += paradigm.influenceRadius * paradigm.influenceRadius;//the high cost of spreading ideas over area
        //consumption.outsideChangeOUT += paradigm.adoptThresh;//supression of foriegn ideas has a cost
    }

    //reset when everyone dies or leaves a leviathan
    public void Depopulation()
    {
        abandoned = true;
        rend.material.color = Color.red;
        consumption.totalSurplus = 0;
        consumption.monthlySurpluses.Fill(100);
        production.totalYield = 0;
        production.monthlyYields.Fill(100);
        //and let the manager know
        manager.Depopulation(this);
    }
}