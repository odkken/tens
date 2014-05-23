using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public static class Util
    {
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
    }
}
