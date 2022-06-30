
//Set the birth rate for the leviathan
//Because this is a react instead of a paradigm trait,
//birth rates are sticky, not changing with a new paradigm unless
//that new paradigm also has a rule with this react
public class ReactSetBirthRate : React
{
    public float birthRate;

    static System.Random rnd = new System.Random();

    //return react with randomized birth rate
    public override React Randomize(Paradigm paradigm) 
    {
        ReactSetBirthRate react = Instantiate(this);
        react.birthRate = (float)((rnd.NextDouble() * .3) + .1f);
        return react;
    }

    //assign given land use to given soil type
    internal override float Act(Leviathan leviathan)
    {
        //leviathan.popCon.baseBirthProb = birthRate; SUPERCEDED BY PARADIGM TRAIT
        return 1;
    }
}
