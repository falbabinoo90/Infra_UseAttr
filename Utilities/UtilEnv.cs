using System.Diagnostics;


namespace Utilities
{
    public static class UtilEnv
    {

        public static bool IsBuiltInDebugMode
        {
            get
            {
#if DEBUG
                return true;
#else
				return false;
#endif
            }
        }

        public static bool IsDebuggerAttached
        {
            get
            {
                return Debugger.IsAttached;
            }
        }

        // Decorating this method with the DebuggerStepThrough attribute sets the break point on the call of this method.
        [DebuggerStepThrough]
        public static void BreakIfDebuggerAttached()
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }
    }
}
