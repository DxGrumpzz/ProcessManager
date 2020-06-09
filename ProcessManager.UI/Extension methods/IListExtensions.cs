namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// A helper class that provides extensions for <see cref="IList{T}"/> class
    /// </summary>
    public static class IListExtensions
    {

        /// <summary>
        /// Swaps 2 elements
        /// </summary>
        /// <typeparam name="T"> The type of object inside the list </typeparam>
        /// <param name="list"> The list of elements to swap </param>
        /// <param name="item1"> The item that will switch place with <paramref name="item2"/> </param>
        /// <param name="item2"> The item that will switch place with <paramref name="item1"/> </param>
        /// <returns></returns>
        public static IList<T> Swap<T>(this IList<T> list, T item1, T item2)
        {
            // Find indices of the 2 items
            int item1Index = list.IndexOf(item1);
            int item2Index = list.IndexOf(item2);

            // Simple validation
            if ((item1Index == -1) || (item2Index == -1))
            {
                Debugger.Break();
                throw new Exception("Item not found");
            };

            // Using a sophisticated swap algorithm with a constant runtime of O(1), swap the 2 elements between in the list
            T temp = list[item1Index];

            list[item1Index] = item2;
            list[item2Index] = temp;

            return list;
        }

    };
};