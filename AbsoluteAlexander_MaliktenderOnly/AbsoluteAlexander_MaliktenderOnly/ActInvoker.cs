using System;

using Advanced_Combat_Tracker;

namespace AbsoluteAlexander_MaliktenderOnly
{
    /// <summary>
    /// ActInvoker
    /// </summary>
    public static class ActInvoker
    {
        /// <summary>
        /// ACTメインフォームでInvokeする
        /// </summary>
        /// <param name="action">実行するアクション</param>
        public static void Invoke(Action action)
        {
            if (ActGlobals.oFormActMain != null &&
                ActGlobals.oFormActMain.IsHandleCreated &&
                !ActGlobals.oFormActMain.IsDisposed &&
                ActGlobals.oFormActMain.InvokeRequired)
            {
                ActGlobals.oFormActMain.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}