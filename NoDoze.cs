using System;
using System.Runtime.InteropServices;

namespace NoDozer
{
    /// <summary>
    /// Provides methods to prevent the system and display from entering sleep mode.
    /// </summary>
    /// <remarks>Uses native Windows API calls to manage system sleep behavior. Intended for scenarios where
    /// continuous system activity is required.</remarks>
    public static class NoDoze
    {
        [DllImport("kernel32.dll")]
        private static extern uint SetThreadExecutionState(uint esFlags);

        private const uint ES_CONTINUOUS = 0x80000000;
        private const uint ES_SYSTEM_REQUIRED = 0x00000001;
        private const uint ES_DISPLAY_REQUIRED = 0x00000002;

        private static uint _previousState;

        /// <summary>
        /// Prevents the system from sleeping. Returns an IDisposable that calls AllowSleep on dispose.
        /// </summary>
        /// <param name="preventDisplaySleep">Also prevent the display from sleeping. Defaults to false.</param>
        public static IDisposable PreventSleep(bool preventDisplaySleep = false)
        {
            uint flags = ES_CONTINUOUS | ES_SYSTEM_REQUIRED;
            if (preventDisplaySleep)
                flags |= ES_DISPLAY_REQUIRED;

            _previousState = SetThreadExecutionState(flags);
            if (_previousState == 0)
                throw new InvalidOperationException("SetThreadExecutionState failed to prevent sleep.");

            return new SleepGuard();
        }

        /// <summary>
        /// Restores the previous thread execution state, allowing the system to sleep normally.
        /// </summary>
        public static void AllowSleep()
        {
            if (SetThreadExecutionState(_previousState | ES_CONTINUOUS) == 0)
                throw new InvalidOperationException("SetThreadExecutionState failed to restore sleep state.");
        }

        private sealed class SleepGuard : IDisposable
        {
            public void Dispose() => AllowSleep();
        }
    }
}
