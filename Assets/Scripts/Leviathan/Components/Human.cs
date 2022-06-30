using UnityEngine;

//The people who live and work in settlements
public class Human : MonoBehaviour
{
    public float age = 2600;//in weeks
    public int foodDeficit = 0;//starvation level
    static System.Random rnd = new System.Random();

    void Awake()
    {
        //generate random stats for humans created at simulation start
        age = (int)(age * rnd.NextDouble());
        name = "Human-" + rnd.Next(0, 1000000);
    }

    //returns how much this human eats, and if not enough adds to human's food deficit
    internal float Eat(float surplusAvailable)
    {
        //kids eat less than adults
        float foodNeeded;
        if (age / 52 > 20) { foodNeeded = 12; }//if over 20 years
        else { foodNeeded = 2 + (age / 104); }//if under 20 years

        //if not enough food add to this human's food deficit and return 
        if (surplusAvailable < foodNeeded)
        {
            foodDeficit += (int)foodNeeded - (int)surplusAvailable;
            return foodNeeded;
        }
        
        //otherwise eat all that want, reduce food deficit and return all that eaten
        foodDeficit -= (int)(foodNeeded / 2);
        if (foodDeficit < 0) { foodDeficit = 0; }
        return foodNeeded;
    }

    //according to age and whether they're getting enough food, is this human ready to die?
    internal bool ReadyToDie()
    {
        float deathProb = 0.0002f;
        //if (rnd.NextDouble() < (((foodDeficit / 15f) + (age / 520)) * deathProb) + deathProb)
        //if (rnd.NextDouble() < (((foodDeficit / 12f) + (age / 520f)) * deathProb) + deathProb)//TEST0.18-E.1
        if (rnd.NextDouble() < (((foodDeficit * 0.0833f) + (age * 0.00192f)) * deathProb) + deathProb)
        {
            return true;
        }
        return false;
    }

}
