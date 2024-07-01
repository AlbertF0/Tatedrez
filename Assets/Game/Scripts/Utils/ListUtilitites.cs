using System;
using System.Collections.Generic;

public static class ListUtilities
{
    public static void ShuffleList<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;

        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}