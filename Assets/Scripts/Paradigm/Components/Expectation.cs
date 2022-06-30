using UnityEngine;

//Used by leviathans to check results against promises
//and to assess the appeal of a counter paradigm.
//Currently just compares flat rate expectations to surplus,
//without adjusting for the number of humans and plots,
//essentially becoming the extremely simplistic story:
//"If you adopt this paradigm--these rules--you will make x in profit."
//But because leviathans then feedback information (word of mouth) about how they're
//doing, expectations still tend to self-adjust to a reasonable average
public class Expectation : MonoBehaviour
{
    public float expectedResult = 50;
    Paradigm paradigm;

    private void Start()
    {
        paradigm = GetComponent<Paradigm>();
    }

    internal float Expectations(Leviathan leviathan)
    {
        return expectedResult;
    }

    //"WORD OF MOUTH" update from leviathans following this paradigm
    internal void Adjust(Leviathan leviathan)
    {
        expectedResult += (((leviathan.icono.comfort - 50) * 0.001f)
        + ((leviathan.consumption.monthlySurpluses[leviathan.consumption.monthlySurpluses.Length - 1] - expectedResult) * 0.002f)) / paradigm.numFollowers;
        if (expectedResult < 1) { expectedResult = 1f; }
    }

    //adjust the expectations of new paradigms
    internal void New()
    {
        //Randomize the initial confidence creators have in their newly created paradigm.
        //Think the many inventors and researches who's expectations for their innovations were
        //completely out of line with what they actually created
        //expectedResult *= (float)(Paradigm.rnd.NextDouble() *.5f) + .75f;//does some interesting things especially when skewed lower that I didn't have time to get into
    }
}
