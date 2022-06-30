using UnityEngine;
using System.Linq;

//Mutate a new paradigm out of this one, changing traits as well as adding or removing rules
public class MutationAdvanced : MutationBasic
{
    static System.Random rnd = new System.Random();

    internal override Paradigm Run()
    {
        Debug.Log("MUTATING!");
        Paradigm paradigm = GetComponent<Paradigm>();
        Paradigm paraNew = Instantiate(paradigm);//start mutating a new paradigm out of a copy of the old one

        //SO, rnd delete a rule (if any to delete)
        if (paraNew.rules.Any() && rnd.Next(0, 2) == 1)
        {
            paraNew.rules.RemoveAt(rnd.Next(0, paradigm.rules.Count));
        }

        //AND, rnd add a new random rule (prefered over deletion in this version--making iconoclasms less common)
        if (rnd.Next(0, 3) > 0)
        {
            if (paraNew.rules.Count < maxRules) { paraNew.NewRandomRule(); }
        }
        //then update the new expectations (change in expectations class)
        paraNew.GetComponent<Expectation>().New();

        //change base threshold trait
        paraNew.threshold = (int)BoundedChange(paraNew.threshold, rnd.Next(-5, 6), 50, 85);
        //change influence radius
        paraNew.influenceRadius = (int)BoundedChange(paraNew.influenceRadius, rnd.Next(-1, 2), 0, 20);
        //change sensitivity
        paraNew.sensitivity = BoundedChange(paraNew.sensitivity, ((float)rnd.NextDouble() - .5f) * .02f, 0.01f, .5f);
        //change deathCOm
        paraNew.deathCom = (int)BoundedChange(paraNew.deathCom, rnd.Next(-3, 4), -50, -1);
        //change birthCom
        paraNew.birthCom = (int)BoundedChange(paraNew.birthCom, rnd.Next(-3, 4), 1, 50);
        //change emmiCom
        paraNew.emmiCom = BoundedChange(paraNew.emmiCom, ((float)rnd.NextDouble() - .5f) * 0.01f, .00001f, .5f);
        //change adoption threshold
        paraNew.adoptThresh = BoundedChange(paraNew.adoptThresh, (float)rnd.NextDouble() - .5f, 1, 20);
        //change mutation rate
        paraNew.mutationRate = BoundedChange(paraNew.mutationRate, ((float)rnd.NextDouble() - .5f) * 0.001f, .00001f, .005f);

        //NEW
        //change emmigration rates
        paraNew.baseEmProb = BoundedChange(paraNew.baseEmProb, ((float)rnd.NextDouble() - .5f) * .0001f, .000001f, .0001f);
        //change storage desired
        paraNew.storageWant = BoundedChange(paraNew.storageWant, rnd.Next(-50, 50), 100, 100000);
        //change birth rate
        paraNew.birthRate = BoundedChange(paraNew.birthRate, ((float)rnd.NextDouble() - .5f) * 0.001f, 0.001f, 1);

        //give the para a new name
        paraNew.name = "ParadigmID-" + rnd.Next(0, 10000);

        //tracking
        paraNew.numFollowers = 1;
        paraNew.maxFollowers = 1;

        return paraNew;
    }

    //helper
    float BoundedChange(float toChange, float change, float min, float max)
    {
        float newValue = toChange + change;
        if (newValue < min) { newValue = min; }
        else if (newValue > max) { newValue = max; }
        return newValue;
    }
}
