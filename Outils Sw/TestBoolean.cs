using System;

namespace Outils
{
    public static class TestBoolean
    {
        public static Boolean HasFlag(this int val, int test)
        {
            return (val & test) > 0;
        }

        public static int ToInt(this Boolean val)
        {
            return Convert.ToInt32(val);
        }

        public static Boolean ToBoolean(this int val)
        {
            if (val == 0)
                return false;

            return true;
        }
    }
}
