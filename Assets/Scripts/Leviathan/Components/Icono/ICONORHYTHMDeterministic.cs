//DETERMINISTIC ICONORHYTHM
public class ICONORHYTHMDeterministic : ICONORHYTHM
{
    //CORE OF THE MODEL: In response to the continuous change of the flux:
    //change nothing, become complacent, mitigate, copy another paradigm (mimesis) or mutate
    public override void Response()
    {
        float discomfort = (100 - comfort);
        bool newPara = false;

        //tracking
        isParaNewAdopt = 0;

        leviathan.paradigm.UpdateExpectations(leviathan);

        int mutateThresh = leviathan.paradigm.threshold + 30;
        if (mutateThresh > 99) { mutateThresh = 99; }

        //complacency--people work less when comfortable
        if (discomfort < leviathan.paradigm.threshold * .33f)
        {
            production.AdjustWorkRate(-.002f);
        }

        //mimesis search
        else if (discomfort > leviathan.paradigm.threshold && discomfort < mutateThresh)
        {
            foreach (Paradigm ct in counterParadigms)
            {
                if (ct.Compare(leviathan) > leviathan.paradigm.threshold * leviathan.paradigm.adoptThresh / (float)discomfort)
                {
                    AddToOldParadigms(leviathan.paradigm);

                    //and adopt the first good counter as new paradigm
                    leviathan.paradigm = ct;
                    AdjustComfort(10);

                    //for tracking
                    isParaNewAdopt = 1;
                    newPara = true;
                    leviathan.paradigm.totalAdoptions++;
                    break;
                }
            }
        }
        else if (discomfort > mutateThresh) //MUTATE
        {
            //add current para to old paradigms list
            AddToOldParadigms(leviathan.paradigm);

            //MUTATE (via current paradigm's rules on mutation)
            Paradigm p = leviathan.paradigm.Mutate();
            leviathan.paradigm = p;
            AdjustComfort(20);
            newPara = true;
        }


        if (discomfort > leviathan.paradigm.threshold) //MITIGATE!
        {
            GetComponent<Production>().AdjustWorkRate(.01f);
        }

        //spread knowledge of paradigm
        if (!newPara)
        {
            UpdateNeighbourCounters();
            counterParadigms.Clear();
        }
        else
        {
            counterParadigms.Clear();
        }
    }
}
