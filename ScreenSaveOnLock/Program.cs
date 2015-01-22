using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace ScreenSaveOnLock {
    static class Program {
        private static FormMain formMain;
        private static String assemblyGuid;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            IEnumerable<System.Runtime.InteropServices.GuidAttribute> attrs = Assembly.GetExecutingAssembly().GetCustomAttributes(false).OfType<System.Runtime.InteropServices.GuidAttribute>();
            Debug.Assert(attrs.Any(),"Assembly information must include GUID.");
            assemblyGuid = attrs.First().Value;

            bool onlyInstance = false;
            Mutex mutex = new Mutex(true, @"Global\" + assemblyGuid, out onlyInstance);
            if (!onlyInstance) {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            formMain = new FormMain();
            Application.Run(formMain);

            GC.KeepAlive(mutex);
        }
    }
}
