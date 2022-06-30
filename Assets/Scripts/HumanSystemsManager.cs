using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Text;

//Control system and contains what is essentially the model's main loop.
//Runs through each leviathan (settlement) and tracks both settlements and
//paradigms for output (by pressing space in game, or at specified week)
//NOTE ON TIME: Months in this model have exactly four weeks, which are the lowest unit of time
public class HumanSystemsManager : MonoBehaviour
{
    //multipler of yields created by plots--to change yields across the system
    public float yieldMultiplier = 12;

    System.Random rnd = new System.Random();

    //leviathan lists
    List<Leviathan> leviathans = new List<Leviathan>();
    List<Leviathan> depopulatedleviathans = new List<Leviathan>();
    
    List<Paradigm> paradigms = new List<Paradigm>();

    //for FBM calulation
    int climateOffsetX = 0;
    int climateOffsetY = 0;

    public int numWeeks = 1;
    public bool FBMYieldMultiplier = false;
    //0.22 more setup etc
    public float FBMstability = .6f;
    float baseYM = 9;
    float baseFBMyield = 1;
    float FMBmultiplier = 1;
    public int endTestAndOutputAtMonth = 0;//if set to 0, test continues indefinitely

    //TRACKING!
    StringBuilder popLine = new StringBuilder();
    StringBuilder rulesBirthRate = new StringBuilder();
    StringBuilder mutationRates = new StringBuilder();
    StringBuilder sensitivity = new StringBuilder();
    StringBuilder comfortAndExpects = new StringBuilder();
    int numMutations = 0;
    int numAdoptions = 0;
    StringBuilder mutationsAdoptions = new StringBuilder();
    StringBuilder currentParas = new StringBuilder();
    StringBuilder yieldMultiplierTrack = new StringBuilder();
    StringBuilder individualLevTracker = new StringBuilder();


    // Start is called before the first frame update
    void Start()
    {
        //random seeds of the simple climate FBM
        climateOffsetX = rnd.Next(1, 100000);
        climateOffsetY = rnd.Next(1, 100000);

        //NEW from v0.22
        baseYM = yieldMultiplier;
        baseFBMyield = baseYM * FBMstability;
        FMBmultiplier = (baseYM - baseFBMyield) * 2;
    }

    //simple climate change using FBM and the yield multipler
    private void ChangeYieldMultiplier()
    {
        if (FBMYieldMultiplier)
        {
            //set up FBM variables-->TODO---SHOULD BE DONE ELSE WHERE INSTEAD OF AGAIN EVERY TIME...
            float scale = 8000;//the number of weeks the cycle repeats sorta
            float lacunarity = 3f;
            float H = .7f;
            float frequency = 3;
            float amplitude = .5f;
            int octaves = 10;

            // calculate sample indices based on the coordinates, the scale and the offset
            float sampleX = (numWeeks + climateOffsetX) / scale;
            float sampleY = (climateOffsetY) / scale;

            //create gain from the Hurst exponent-->lower the hurst the more volatile it becomes,
            //e.g. when H = 1, G = .5 and the FBM looks like mountains
            //when H = 1/2, G = .7 and the FBM looks like the stock market
            float gain = (float)Math.Pow(2, -H);

            yieldMultiplier = baseFBMyield + (FBM.Noise(sampleX, sampleY, lacunarity, gain, frequency, amplitude, octaves, true) * FMBmultiplier);
        }
    }

    //Main loop through leviathans while also tracking
    void Update()
    {
        //simple climate change if selected
        if (FBMYieldMultiplier) { ChangeYieldMultiplier(); }
        
        //tracking
        int totalPop = 0;
        float totalYield = 0;
        float totalCosts = 0;

        //main leviathans loop!
        foreach (Leviathan l in leviathans)
        {
            l.Run(numWeeks);

            //tracking
            if (!l.abandoned)
            {
                totalPop += l.popCon.humans.Count;
                totalYield += l.production.weeklyYield;
                numAdoptions += l.icono.isParaNewAdopt;
                totalCosts += l.rulesCost;


            }
        }
        //add to tracker
        popLine.Append(numWeeks);
        popLine.Append(',');
        popLine.Append(totalPop);
        popLine.Append(',');
        popLine.Append(totalYield);
        popLine.Append(',');
        popLine.Append(totalCosts);
        popLine.Append('\n');

        

        //monthly only
        float weeksToMonths = numWeeks / 4f;
        if (weeksToMonths - Math.Truncate(weeksToMonths) == 0)
        {
            /*//CREATES BIG FILES
            //individual leviathan tracking
            individualLevTracker.Append(numWeeks);
            for (int i = 0; i < 20; i++)
            {
                individualLevTracker.Append(',');
                individualLevTracker.Append(leviathans[i].production.monthlyYields[leviathans[i].production.monthlyYields.Length-2]);
                individualLevTracker.Append(',');
                individualLevTracker.Append(leviathans[i].popCon.humans.Count);
                individualLevTracker.Append(',');
                individualLevTracker.Append(leviathans[i].icono.comfort);
            }
            individualLevTracker.Append('\n');
            */

            //keep track of mutations and adoptions
            mutationsAdoptions.Append(weeksToMonths);
            mutationsAdoptions.Append(',');
            mutationsAdoptions.Append(numMutations);
            mutationsAdoptions.Append(',');
            mutationsAdoptions.Append(numAdoptions);
            mutationsAdoptions.Append(',');
            mutationsAdoptions.Append(numAdoptions + numMutations);
            mutationsAdoptions.Append('\n');
            //reset       
            numMutations = 0;
            numAdoptions = 0;

            //monthly tracking
            foreach (Paradigm p in paradigms)
            {
                p.numFollowers = 0; //reset number of adherent leviathans of each paradigm
            }

            //more tracking, gets a bit messy
            int numActivelevs = 0;
            int numRulesAdded = 0;
            float birthRatesAdded = 0;
            float adoptThreshAdded = 0;
            float mutDivAdded = 0;
            int threshAdded = 0;
            float birthComAdded = 0;
            float deathComAdded = 0;
            float emmiComAdded = 0;
            float sensiAdded = 0;
            int influenceAdded = 0;
            float comfortAdded = 0;
            float expectsAdded = 0;
            float emProbAdded = 0;
            float storageAdded = 0;
            float workRateAdded = 0;

            foreach (Leviathan l in leviathans)
            {
                if (!l.abandoned) 
                { 
                    l.paradigm.numFollowers++;
                    numRulesAdded += l.paradigm.rules.Count;
                    birthRatesAdded += l.paradigm.birthRate;
                    adoptThreshAdded += l.paradigm.adoptThresh;
                    mutDivAdded += l.paradigm.mutationRate;
                    threshAdded += l.paradigm.threshold;
                    birthComAdded += l.paradigm.birthCom;
                    deathComAdded += l.paradigm.deathCom;
                    emmiComAdded += l.paradigm.emmiCom;
                    sensiAdded += l.paradigm.sensitivity;
                    influenceAdded += l.paradigm.influenceRadius;
                    comfortAdded += l.icono.comfort;
                    expectsAdded += l.paradigm.expectations.expectedResult;
                    emProbAdded += l.paradigm.baseEmProb;
                    storageAdded += l.paradigm.storageWant;
                    workRateAdded += l.production.workRate;
                    numActivelevs++;
                }
            }

            //keep track average # of rules and average birth rate
            rulesBirthRate.Append(weeksToMonths);
            rulesBirthRate.Append(',');
            rulesBirthRate.Append(numRulesAdded / (float)numActivelevs);
            rulesBirthRate.Append(',');
            rulesBirthRate.Append(birthRatesAdded / (float)numActivelevs);
            rulesBirthRate.Append('\n');

            //keep track average adoptionThresh, mutationRate, threshold
            mutationRates.Append(weeksToMonths);
            mutationRates.Append(',');
            mutationRates.Append(adoptThreshAdded / (float)numActivelevs);
            mutationRates.Append(',');
            mutationRates.Append(mutDivAdded / (float)numActivelevs);
            mutationRates.Append(',');
            mutationRates.Append(threshAdded / (float)numActivelevs);
            mutationRates.Append('\n');

            //keep track average birthCom, deathCom, emmiCom, sensitivity, influence
            sensitivity.Append(weeksToMonths);
            sensitivity.Append(',');
            sensitivity.Append(birthComAdded / (float)numActivelevs);
            sensitivity.Append(',');
            sensitivity.Append(deathComAdded / (float)numActivelevs);
            sensitivity.Append(',');
            sensitivity.Append(emmiComAdded / (float)numActivelevs);
            sensitivity.Append(',');
            sensitivity.Append(sensiAdded / (float)numActivelevs);
            sensitivity.Append(',');
            sensitivity.Append(influenceAdded / (float)numActivelevs);
            sensitivity.Append('\n');

            //keep track average # comfort and expectedResults AND NEW--emPROB AND STORAGEWANT
            comfortAndExpects.Append(weeksToMonths);
            comfortAndExpects.Append(',');
            comfortAndExpects.Append(comfortAdded / (float)numActivelevs);
            comfortAndExpects.Append(',');
            comfortAndExpects.Append(expectsAdded / (float)numActivelevs);
            comfortAndExpects.Append(',');

            //keep track average # comfort and expectedResults
            comfortAndExpects.Append(emProbAdded / (float)numActivelevs);
            comfortAndExpects.Append(',');
            comfortAndExpects.Append(storageAdded / (float)numActivelevs);
            comfortAndExpects.Append(',');
            comfortAndExpects.Append(workRateAdded / (float)numActivelevs);
            comfortAndExpects.Append('\n');


            //make paras keep track of max unique leviathan followers
            int mostAdherents = 0;
            int numberActive = 0;
            foreach (Paradigm p in paradigms)
            {
                if (p.maxFollowers < p.numFollowers) { p.maxFollowers = p.numFollowers; };

                //and keep track of largest paradigm and total number of active paradigms each week and tiem this para is active
                if (p.numFollowers > 0) { numberActive++; p.monthsActive++; }
                if (p.numFollowers > mostAdherents) { mostAdherents = p.numFollowers; }
            }

            //keep track of largest paradigm and total number of active paradigms each week
            currentParas.Append(weeksToMonths);
            currentParas.Append(',');
            currentParas.Append(mostAdherents);
            currentParas.Append(',');
            currentParas.Append(numberActive);
            currentParas.Append('\n');

            //keep track of yieldMultiplier (to show FBM change if used)
            yieldMultiplierTrack.Append(numWeeks);
            yieldMultiplierTrack.Append(',');
            yieldMultiplierTrack.Append(yieldMultiplier);
            yieldMultiplierTrack.Append('\n');
        }

        //Output tracking to file if space pressed.
        //Probably better ways to do this.
        //Python scripts used for plotting and analysis because better libraries
        if (Input.GetKeyDown("space") || endTestAndOutputAtMonth * 4  == numWeeks)
        {
            using (StreamWriter sw = new StreamWriter("Paradigms_Tracking.csv"))
            {
                sw.WriteLine("Paradigm,Max Adherents,Total Adoptions,Months Active");
                for (int i = 0; i < paradigms.Count;  i++ )
                {
                    if (i > 0)//skip starting paradigm so it doesn't skew the stats
                    {
                        sw.WriteLine(paradigms[i].name + ',' + paradigms[i].maxFollowers + ',' 
                            + paradigms[i].totalAdoptions + ',' + paradigms[i].monthsActive);
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter("Population_Tracking.csv"))
            {
                sw.WriteLine("Week,Total Pop,Total Yield");
                sw.WriteLine(popLine);
            }

            using (StreamWriter sw = new StreamWriter("Change_Tracking.csv"))
            {
                sw.WriteLine("Month,Mutations,Adoptions,Total Paradigm Change");
                sw.WriteLine(mutationsAdoptions);
            }

            using (StreamWriter sw = new StreamWriter("Paradigm_Sizes.csv"))
            {
                sw.WriteLine("Month,Largest Paradigm,Number of Active Paradigms,");
                sw.WriteLine(currentParas);
            }

            using (StreamWriter sw = new StreamWriter("Yield_Multiplier.csv"))
            {
                sw.WriteLine("Week,Yield Multiplier");
                sw.WriteLine(yieldMultiplierTrack);
            }

            using (StreamWriter sw = new StreamWriter("Rates_Tracking.csv"))
            {
                sw.WriteLine("Month,Average # of Rules,Average Birth Rate");
                sw.WriteLine(rulesBirthRate);
            }

            using (StreamWriter sw = new StreamWriter("MutationRates_Tracking.csv"))
            {
                sw.WriteLine("Month,AvAdoptionThresh,AvMutationDivider,AvThreshold");
                sw.WriteLine(mutationRates);
            }

            using (StreamWriter sw = new StreamWriter("Sensitivity_Tracking.csv"))
            {
                sw.WriteLine("Month,AvBirthCom,AvDeathCom,AvEmmiCom,AvSensitivity,AvInfluenceRadius");
                sw.WriteLine(sensitivity);
            }

            using (StreamWriter sw = new StreamWriter("ComfortExpects_Tracking.csv"))
            {
                sw.WriteLine("Month,AvComfortableness,AvExpectedResults");
                sw.WriteLine(comfortAndExpects);
            }
            /*
            using (StreamWriter sw = new StreamWriter("IndividualLev_Tracking.csv"))
            {
                sw.WriteLine("Month,Yields,Population,Comfort, AND SO ON");
                sw.WriteLine(individualLevTracker);
            }
            */

            print("OUTPUT SUCCESSFULL");

            //end test if set length
            if (endTestAndOutputAtMonth != 0 && endTestAndOutputAtMonth * 4 == numWeeks) 
            {
                print("TEST ENDED");
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            }
        }

        //the novelty of time
        numWeeks++;
    }

    //add paradigm to manager list
    internal void AddParadigm(Paradigm paradigm)
    {
        paradigms.Add(paradigm);
        numMutations++;
    }

    internal void Addleviathan(Leviathan leviathan)
    {
       leviathans.Add(leviathan);
    }


    //migration system
    /*
    public void Emmigration(Leviathan leviathan, List<Human> humansEmmigrating)
    {
        Collider[] colliders = Physics.OverlapSphere(leviathan.transform.position, leviathan.paradigm.influenceRadius * 4);
        float whereTo = (float)rnd.NextDouble();
        if (whereTo < 0.5f && depopulatedleviathans.Any())//find a nearby depopulated place, otherwise return home
        {
            bool foundDepop = false;
            colliders.RndShuffleAlt();//otherwise migration tends to the direction that Physics.OverlapSphere seems to list first
            foreach (Collider c in colliders)
            {
                Leviathan colliderleviathan = c.GetComponent<Leviathan>();
                if (colliderleviathan.abandoned)
                {
                    //reset leviathan according to what people believed in leviathan they just left
                    colliderleviathan.paradigm = leviathan.paradigm;
                    colliderleviathan.popCon.Immigration(humansEmmigrating);
                    colliderleviathan.icono.comfort = 45;//I mean, this should probably be based on something a bit more complex?
                    //colliderleviathan.popCon.baseBirthProb = leviathan.popCon.baseBirthProb;
                    colliderleviathan.production.workRate = leviathan.production.workRate;
                    depopulatedleviathans.Remove(colliderleviathan);
                    foundDepop = true;
                    break;
                }
            }
            if (!foundDepop)
            {
                if (whereTo < .2) { leviathan.popCon.Immigration(humansEmmigrating); }
                else { colliders[rnd.Next(0, colliders.Length)].GetComponent<Leviathan>().popCon.Immigration(humansEmmigrating); }
            }
        }
        else if (whereTo < 0.95f)//go to a random nearby leviathan
        {
            Leviathan levToRepopulate = colliders[rnd.Next(0, colliders.Length)].GetComponent<Leviathan>();
            if (levToRepopulate.abandoned == true)
            {
                //reset leviathan according to what people believed in leviathan they just left
                levToRepopulate.paradigm = leviathan.paradigm;
                //levToRepopulate.popCon.baseBirthProb = leviathan.popCon.baseBirthProb;
                levToRepopulate.icono.comfort = 45;
                levToRepopulate.production.workRate = leviathan.production.workRate;
                depopulatedleviathans.Remove(levToRepopulate);
            }
            levToRepopulate.popCon.Immigration(humansEmmigrating);
        }
        else //some migrants may travel to any random leviathan, not just nearby leviathans, still favouring abandoned leviathans
        {
            if (depopulatedleviathans.Any())
            {
                //reset leviathan according to what people believed in leviathan they just left
                Leviathan levToRepopulate = depopulatedleviathans[rnd.Next(0, depopulatedleviathans.Count)];
                levToRepopulate.popCon.Immigration(humansEmmigrating);
                levToRepopulate.paradigm = leviathan.paradigm;
                levToRepopulate.GetComponent<ICONORHYTHM>().comfort = 45;
                //levToRepopulate.popCon.baseBirthProb = leviathan.popCon.baseBirthProb;
                levToRepopulate.production.workRate = leviathan.production.workRate;
                depopulatedleviathans.Remove(levToRepopulate);
            }
            else
            {
                leviathans[rnd.Next(0, leviathans.Count)].popCon.Immigration(humansEmmigrating);
            }
        }
    }*/

    //SIMPLE MIGRATION
    ///*
    public void Emmigration(Leviathan leviathan, List<Human> humansEmmigrating)
    {
        Collider[] colliders = Physics.OverlapSphere(leviathan.transform.position, leviathan.paradigm.influenceRadius * 4);
        float whereTo = (float)rnd.NextDouble();

        bool foundDepop = false;
        colliders.RndShuffleAlt();//otherwise migration tends to the direction that Physics.OverlapSphere seems to list first
        foreach (Collider c in colliders)
        {
            Leviathan colliderleviathan = c.GetComponent<Leviathan>();
            if (colliderleviathan.abandoned)
            {
                //reset leviathan according to what people believed in leviathan they just left
                colliderleviathan.paradigm = leviathan.paradigm;
                colliderleviathan.popCon.Immigration(humansEmmigrating);
                colliderleviathan.icono.comfort = 45;//I mean, this should probably be based on something a bit more complex?
                colliderleviathan.production.workRate = leviathan.production.workRate;
                depopulatedleviathans.Remove(colliderleviathan);
                foundDepop = true;
                break;
            }
        }
        if (!foundDepop)
        {
            if (whereTo < .2) { leviathan.popCon.Immigration(humansEmmigrating); }
            else { colliders[rnd.Next(0, colliders.Length)].GetComponent<Leviathan>().popCon.Immigration(humansEmmigrating); }
        }

    }
    //*/

    public void Depopulation(Leviathan leviathan)
    {
        depopulatedleviathans.Add(leviathan);
    }
}
