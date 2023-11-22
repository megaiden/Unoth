using System.Collections.Generic;

namespace Assets.Scripts.Utils
{
    public static class ExtensionClass
    {
        // Extension method to append the element
        public static T[] Appends<T>(this T[] array, T item)
        {
            List<T> list = new List<T>(array);
            list.Add(item);

            return list.ToArray();
        }
    }
}