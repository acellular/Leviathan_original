//STOCHASTIC ICONORHYTHM with fully random mutation--i.e. people are creative whether uncomfortable or not
//but adoption is still thresholded by the usual stochastic calculation on comfort
public class ICONORHYTHMStochRndMutation: ICONORHYTHM
{
    //CORE OF THE MODEL: In response to the continuous change of the flux:
    //change nothing, become complacent, mitigate, copy another paradigm (mimesis) or mutate
    public override void Response()
    {
        float discomfort = (100 - comfort);
        float threshProb = (100 - leviathan.paradigm.threshold) * leviathan.paradigm.baseMimesis;
        bool counterPicked = false;
        bool newPara = false;

        //tracking
        isParaNewAdopt = 0;

        //spread word of mouth about how this leviathan feels about the paradigm it follows
        leviathan.paradigm.UpdateExpectations(leviathan);

        //complacency--inhabitants work less
        if (discomfort < leviathan.paradigm.threshold / 3)
        {
            production.AdjustWorkRate(-.002f);
        }
        //if uncomfortable, look for new paradigm to adopt via MIMESIS
        else if (rnd.NextDouble() < discomfort * discomfort * discomfort * threshProb)
        {
            foreach (Paradigm ct in counterParadigms)
            {
                //what are the expected returns offered by other known paradigms?
                //Are they a certain amount better than what the current
                //paradigm offer's? (relative to the leviathan's current comfort)
                if (ct.Compare(leviathan) > leviathan.paradigm.threshold * leviathan.paradigm.adoptThresh / discomfort)
                {
                    //add existing paradigm to old paradigms
                    AddToOldParadigms(leviathan.paradigm);

                    //and adopt the first good counter as new paradigm
                    leviathan.paradigm = ct;
                    counterPicked = true;
                    AdjustComfort(10);//then add to comfortableness-->cause at least feel good about changing something...

                    //tracking
                    isParaNewAdopt = 1;

                    newPara = true;
                    leviathan.paradigm.totalAdoptions++;
                    break;
                }
            }
        }

        //at a much lower rate than looking for other paradigms to copy, MUTATE a new paradigm
        //IN THIS VERSION MUTATION IS FULLY RANDOM--i.e. people are creative whether uncomfortable or not
        //but that does not mean that other settlements will actually adopt these mutations
        if (!counterPicked && rnd.NextDouble() < 125000 * (threshProb * leviathan.paradigm.mutationRate))
        {
            //add current para to old paradigms list
            AddToOldParadigms(leviathan.paradigm);

            //MUTATE!!! (via current paradigm's rules on mutation)
            Paradigm p = leviathan.paradigm.Mutate();
            leviathan.paradigm = p;
            AdjustComfort(20);//then add to comfortableness-->cause at least feel good about changing something...
            newPara = true;
        }

        //whether adopting or mutating a new paradigm, also MITIGATE if uncomfortable by increasing work rate
        if (discomfort > leviathan.paradigm.threshold)
        {
            GetComponent<Production>().AdjustWorkRate(.01f);
        }

        //word spreads of the current paradigm to neighbours
        if (!newPara)
        {
            UpdateNeighbourCounters();
            counterParadigms.Clear();//no memory of paradigms never adopted
        }
        else
        {
            counterParadigms.Clear();
        }
    }
}
