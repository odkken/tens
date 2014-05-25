using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Common
{
    public static class Util
    {
        private static System.Random rand;

        public static void Shuffle<T>(this IList<T> list)
        {
            var rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T Pop<T>(this IList<T> theList)
        {
            var local = theList[theList.Count - 1];
            theList.RemoveAt(theList.Count - 1);
            return local;
        }

        public static void Push<T>(this IList<T> theList, T item)
        {
            theList.Add(item);
        }

        public static void Swap<T>(this IList<T> theList, int indexA, int indexB)
        {
            var tmp = theList[indexA];
            theList[indexA] = theList[indexB];
            theList[indexB] = tmp;
        }

        public static float NextGaussian(float stdDev, float mean = 0)
        {
            if(rand == null)
                rand = new System.Random(); //reuse this if you are generating many
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            return (float)randNormal;
        }

    }
}
