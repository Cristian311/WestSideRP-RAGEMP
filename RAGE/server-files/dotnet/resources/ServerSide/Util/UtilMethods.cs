using System.Collections.Generic;

namespace ServerSide.Util
{
    internal class UtilMethods
    {
        public static bool TryGetElement<T>(List<T> list, int index, out T element)
        {
            if (index >= 0 && index < list.Count)
            {
                element = list[index];
                return true;
            }
            element = default;
            return false;
        }
    }
}
