using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SDI.Utils
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> Shuffle<TKey, TValue>(
           this Dictionary<TKey, TValue> source)
        {
            System.Random r = new System.Random();
            return source.OrderBy(x => r.Next())
               .ToDictionary(item => item.Key, item => item.Value);
        }
        private static System.Random rng = new System.Random();

        public static void Shuffle<T>(this IList<T> list)
        {
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
    }

}