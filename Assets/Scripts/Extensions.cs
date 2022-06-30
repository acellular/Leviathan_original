using System.Collections.Generic;

//helper stuff
static class Extensions
{
    private static System.Random rnd = new System.Random();

    public static void RndShuffle<T>(this IList<T> list)
    {
        for (int i = list.Count; i > 0; i--)
        {
            int nv = rnd.Next(0, i);
            T value = list[nv];
            list[nv] = list[i - 1];
            list[i - 1] = value;
        }
    }

    public static void RndShuffleAlt<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int nv = rnd.Next(0, list.Count);
            T value = list[nv];
            list[nv] = list[i];
            list[i] = value;
        }
    }

    public static void Fill<T>(this IList<T> ill, T value)
    {
        for (int i = 0; i < ill.Count; i++)
        {
            ill[i] = value;
        }
    }
}