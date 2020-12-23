using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CreditOne.P360FormSubmissionService.Extensions
{
    public static class FunctionExtensions
    {
        //
        // Measure performance of function that takes a string and returns a task
        // Returns result in milliseconds
        public static long Measure(this Func<string, Task> func, string arg)
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            func(arg);
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        // 
        // This is useful for functions that need retry functionality with no changes to function itself.
        // Ex: Given function bool foo(), if you want to execute until success or 3 times, you would use 
        //      foo.WithRetry(3);
        public static bool WithRetry(this Func<bool> func, int retries)
        {
            int counter = 0;
            bool success = false;

            while (counter++ < retries && !success)
            {
                success = func();
            }

            return success;
        }

        //
        // This converts a function that takes two parameters and returns a function that takes one argument
        // This is useful when a function expects a function that takes one argument, but needs to use two arguments
        public static Func<TParam1, TResult> Partial<TParam1, TParam2, TResult>(this Func<TParam1, TParam2, TResult> func, TParam2 parameter2)
        {
            return (p) => func(p, parameter2);
        }
    }
}
