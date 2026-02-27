using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessM.Core.Services
{
    public static class AffinityHelper
    {
        public static long BuildMask(IEnumerable<bool> cores)
        {
            if (cores == null)
                throw new ArgumentNullException(nameof(cores));

            long mask = 0;
            int index = 0;

            foreach (var enabled in cores)
            {
                if (enabled)
                    mask |= (1L << index);

                index++;
            }

            return mask;
        }

        public static List<int> GetEnabledCores(long mask, int totalCores)
        {
            var result = new List<int>();

            for (int i = 0; i < totalCores; i++)
            {
                if ((mask & (1L << i)) != 0)
                    result.Add(i);
            }

            return result;
        }

        public static string MaskToString(long mask, int totalCores)
        {
            var cores = GetEnabledCores(mask, totalCores);

            if (cores.Count == 0)
                return "-";

            if (cores.Count == totalCores)
                return "All";

            return string.Join(", ", cores);
        }

        public static bool IsCoreEnabled(IntPtr mask, int index)
        {
            long m = mask.ToInt64();
            return (m & (1L << index)) != 0;
        }
    }
}