using System.Collections.Generic;
using UnityEngine;


//The basic rhythm of complacency, mitigation, mimesis and mutation
//faced by all decision making agents as they hurtle through time
//and specifically by leviathans (settlements) in this model
public abstract class ICONORHYTHM : MonoBehaviour
{
    //how comfortable is the leviathan with how things are going
    public float comfort = 60;

    public Paradigm[] oldParadigms = new Paradigm[10];//was used in testing to stop leviathans adopting already tried paradigms, now simply to view in editor
    public List<Paradigm> counterParadigms = new List<Paradigm>();

    //references to other components
    internal PopulationControl popCon;
    internal Leviathan leviathan;
    internal Consumption consumption;
    internal Production production;

    internal static System.Random rnd = new System.Random();

    //tracking
    internal int isParaNewAdopt;


    //CORE OF THE MODEL: In response to the continuous change of the flux:
    //change nothing, become complacent, mitigate, copy another paradigm (mimesis) or mutate
    public abstract void Response();

    public virtual void Start()
    {
        //get component references
        popCon = GetComponent<PopulationControl>();
        leviathan = GetComponent<Leviathan>();
        consumption = GetComponent<Consumption>();
        production = GetComponent<Production>();
    }

    //spread word of this paradigm to neighbouring leviathans using Unity's collider for proximity
    public virtual void UpdateNeighbourCounters()
    {
        Collider[] colliders = Physics.OverlapSphere(leviathan.transform.position, leviathan.paradigm.influenceRadius);
        //print(colliders.Length);

        foreach (Collider c in colliders)
        {
            Leviathan colliderleviathan = c.GetComponent<Leviathan>();
            if (!colliderleviathan.abandoned && colliderleviathan.paradigm != leviathan.paradigm)
            {
                colliderleviathan.icono.counterParadigms.Add(leviathan.paradigm);
            }
        }
    }

    internal void Run()
    {
        //update comfort
        UpdateComfort();

        //then using comfort, continue with status quo, mitigate, copy or mutate?
        Response();
    }

    public void UpdateComfort()
    {
        //are returns increasing
        float gettingBetter = (consumption.monthlySurpluses[consumption.monthlySurpluses.Length - 1]
            - consumption.monthlySurpluses[consumption.monthlySurpluses.Length - 2]) / leviathan.production.plots.Count;

        //word-of-mouth expectations, becoming a sort of promise
        float expectedReturns = leviathan.paradigm.Expectations(leviathan);

        //real monthly returns
        float monthlyReturns = consumption.monthlySurpluses[consumption.monthlySurpluses.Length - 1];

        //how do expectations and reality compare?
        float expectsVsReal = ((monthlyReturns + expectedReturns) / (expectedReturns + .1f)) - 2
            + (consumption.totalSurplus / (leviathan.paradigm.storageWant * popCon.humans.Count + .1f));

        //add thus current catharsis
        float howAreThingsGoing = (expectsVsReal + gettingBetter) * leviathan.paradigm.sensitivity;

        AdjustComfort(howAreThingsGoing);
    }

    public void AddToOldParadigms(Paradigm paradigm)
    {
        for (int i = 0; i < oldParadigms.Length - 1; i++)
        {
            oldParadigms[i] = oldParadigms[i + 1];
        }
        oldParadigms[oldParadigms.Length - 1] = paradigm;
    }

    //keep comfort 0 - 100, should probably be a property if I was doing things properly
    public void AdjustComfort(float adjust)
    {
        comfort += adjust;
        if (comfort < 0) { comfort = 0; }
        else if (comfort > 100) { comfort = 100; }
    }



}
