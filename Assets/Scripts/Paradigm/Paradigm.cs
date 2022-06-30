using System.Collections.Generic;
using UnityEngine;

//The basic understanding of the world followed by settlements.
//Contains both the rules that settlements live by
//and how to mutate new paradigms with new rules out of the current paradigm
public class Paradigm : MonoBehaviour
{
    public int threshold;//defines how adverse followers of this paradigm are to change
    public float sensitivity = .2f;//defines how sensitive followers are to expectations not being met
    public int influenceRadius = 5;

    //TRAITS
    public int deathCom = -10;//base change to comfort on death of inhabitant
    public int birthCom = 4;//base change to comfort on birth of inhabitant
    public float emmiCom = 0.05f;//used in calculation of comfort on emmigration of inhabitants
    public float adoptThresh = 2.5f;
    public float mutationRate = 0.002f;
    public float baseMimesis = .0000001f;//base probability for mimesis and mutation calculation
    //NEW
    public float baseEmProb = .00008f;
    public float storageWant = 2000;
    public float birthRate = .4f;

    //what this paradigm says to do
    public List<Rule> rules;
    public Rule rulePrefab;
    public List<Condition> possibleConditionTypes = new List<Condition>();
    public List<React> possibleReactTypes = new List<React>();//essentially the technology available to people, e.g. crop rotations
    public List<LandUse> possibleLandUses = new List<LandUse>();

    //for tracking follower leviathans
    public int numFollowers = 0;
    public int maxFollowers = 0;
    public int totalAdoptions = 0;
    public int monthsActive = 0;

    //components
    public Expectation expectations;
    public MutationBasic mutation;

    //colour!
    public Color colour;

    //global control reference
    HumanSystemsManager manager;

    internal static System.Random rnd = new System.Random();

    private void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<HumanSystemsManager>();
        manager.AddParadigm(this);
    }

    private void Start()
    {
        expectations = GetComponent<Expectation>();
        mutation = GetComponent<MutationBasic>();

        //give this para a random color
        colour = Random.ColorHSV();

        //it now exists
        monthsActive = 1;

        //if created, must be by a leviathan that will be following it
        totalAdoptions = 1;
    }

    private void Update()
    {
        //tracking number of adherents
        if (maxFollowers < numFollowers) { maxFollowers = numFollowers; }
    }
 
    //compares expectations of this paradigm for leviathans considering
    //adopting it, relative to the leviathan the paradigm already follows
    internal float Compare (Leviathan leviathan)
    {
        return expectations.Expectations(leviathan) / leviathan.paradigm.Expectations(leviathan);
    }

    //used to compare to actual results or to other paradigms
    internal float Expectations(Leviathan leviathan)
    {
        return expectations.Expectations(leviathan);
    }

    //modify this paradigm by removing and adding rules
    internal Paradigm Mutate()
    {
        return mutation.Run();
    }

    //adjust expectations-->accessed by all leviathans using this paradigm creating "word of mouth"
    internal void UpdateExpectations(Leviathan leviathan)
    {
        expectations.Adjust(leviathan);
    }

    //used by mutation to add new random rules when creating a new paradigm from this one
    internal void NewRandomRule()
    {
        //currently just adding one random condition and react--the conditions don't affect much right now
        //could be expanded later to have rules with multiple conditions and reacts
        Rule newRule = Instantiate(rulePrefab);
        newRule.conditions.Add(possibleConditionTypes[rnd.Next(0, possibleConditionTypes.Count)].Randomize(this));
        newRule.reacts.Add(possibleReactTypes[rnd.Next(0, possibleReactTypes.Count)].Randomize(this));
        rules.Add(newRule);
    }

}
