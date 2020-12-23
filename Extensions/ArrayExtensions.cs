using System;

namespace CreditOne.P360FormSubmissionService.Extensions
{
    public static class ArrayExtensions
    {
        public static bool StartsWith<T>(this T[] array, T[] subarray) where T : IEquatable<T>
        {
            if (array == null)
                throw new ArgumentNullException($"parameter {nameof(array)} cannot be null when calling {nameof(StartsWith)}");
            if (subarray == null)
                throw new ArgumentNullException($"parameter {nameof(subarray)} cannot be null when calling {nameof(StartsWith)}");

            if (array.Length < subarray.Length)
                return false;

            for (int i = 0; i < subarray.Length; i++)
            {
                if (subarray[i].Equals(array[i]))
                    return false;
            }

            return true;
        }
    }
}
