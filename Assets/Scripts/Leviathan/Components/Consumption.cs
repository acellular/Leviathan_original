using UnityEngine;

//settlements using up their yields
public class Consumption : MonoBehaviour
{
    public float weeklySurplus = 1;
    public float[] monthlySurpluses = new float[120];//goes negative, representing the food people couldn't get to eat, used for comfort calculation
    public float totalSurplus = 0;//never falls below 0, essentially represents stored food for lean times
    public Leviathan leviathan;

    //to let other classes use yields then added to the monthly calculation
    public float outsideChangeOUT = 0;

    private void Awake()
    {
        monthlySurpluses.Fill(weeklySurplus*4);//avoiding null errors..
    }

    private void Start()
    {
        leviathan = GetComponent<Leviathan>();    
    }
   
    //Called weekly by leviathan.Organizeleviathan()
    internal void Run()
    {
        //add in outside change--happening first means there might not be enough left to eat
        weeklySurplus = GetComponent<Production>().weeklyYield - outsideChangeOUT;
        totalSurplus += weeklySurplus;

        //inhabitants eat
        foreach (Human h in leviathan.popCon.humans)
        {
            //if there is any food left, eat it
            //if there isn't anything to eat, add to inhabitant's food deficit,
            //used to calculate probability of death
            float eat = h.Eat(totalSurplus);
            weeklySurplus -= eat;
            totalSurplus -= eat;
        }

        //updates and resets
        if (totalSurplus < 0) { totalSurplus = 0; }
        monthlySurpluses[monthlySurpluses.Length - 1] += weeklySurplus;
        outsideChangeOUT = 0;
    }


    //monthly surplus array cleanup
    internal void Monthly()
    {
        for (int i = 0; i < monthlySurpluses.Length - 1; i++)
        {
            monthlySurpluses[i] = monthlySurpluses[i + 1];
        }
        monthlySurpluses[monthlySurpluses.Length - 1] = 0;
    }
}
