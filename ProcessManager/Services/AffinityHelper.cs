using System;

namespace ProcessManager.Services
{
    public static class AffinityHelper
    {
        public static bool IsCoreEnabled(IntPtr mask, int coreIndex)
        {
            long value = mask.ToInt64();
            return (value & (1L << coreIndex)) != 0;
        }

        public static IntPtr BuildMask(bool[] cores)
        {
            long mask = 0;

            for (int i = 0; i < cores.Length; i++)
                if (cores[i]) mask |= (1L << i);

            return new IntPtr(mask);
        }
    }
}