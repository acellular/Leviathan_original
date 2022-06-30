using UnityEngine;
using System.Linq;

//mutate a new paradigm out of this one, by adding or removing rules (but no trait change)
public class MutationBasic : Mutation
{
    static System.Random rnd = new System.Random();
    public int maxRules = 50;//was 15 forever up to TEST J

    internal override Paradigm Run()
    {
        Debug.Log("MUTATING!");
        Paradigm paradigm = GetComponent<Paradigm>();
        Paradigm paraNew = Instantiate(paradigm);

        //SO, for rnd delete a rule (if any to delete)
        if (paraNew.rules.Any() && rnd.Next(0, 2) == 1)
        {
            paraNew.rules.RemoveAt(rnd.Next(0, paradigm.rules.Count));
        }

        //AND, rnd add a new random rule (prefered over deletion in this version--making iconoclasms less common)
        if (rnd.Next(0, 3) > 0)
        {
            if (paraNew.rules.Count < maxRules) { paraNew.NewRandomRule(); }
        }
        //then update the new expectations to be a bit higher
        paraNew.GetComponent<Expectation>().New();

        //give it a new name
        paraNew.name = "ParadigmID-" + rnd.Next(0, 10000);

        //tracking
        paraNew.numFollowers = 1;
        paraNew.maxFollowers = 1;

        return paraNew;
    }

}
