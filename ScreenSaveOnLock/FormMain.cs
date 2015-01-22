using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ScreenSaveOnLock {
    public partial class FormMain : Form {
        public const int KEEPALIVE_TIMEOUT = 50000;
        public const int SCREENSAVER_TIMEOUT = 50000;

        public uint idleTime = 0;
        public StateTracker screenSaverActive = new StateTracker(false);
        public StateTracker logoffState = new StateTracker(false);
        public StateTracker sessionLockState = new StateTracker(false);

        public FormMain() {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e) {
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
        }

        void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e) {
            string logMsg;
            switch(e.Reason) {
                case SessionSwitchReason.ConsoleConnect:
                    logMsg = "ConsoleConnect";
                    break;
                case SessionSwitchReason.ConsoleDisconnect:
                    logMsg = "ConsoleDisconnect";
                    break;
                case SessionSwitchReason.RemoteConnect:
                    logMsg = "RemoteConnect";
                    break;
                case SessionSwitchReason.RemoteDisconnect:
                    logMsg = "RemoteDisconnect";
                    break;
                case SessionSwitchReason.SessionLock:
                    sessionLockState.flag = true;
                    logMsg = "SessionLock";
                    break;
                case SessionSwitchReason.SessionUnlock:
                    sessionLockState.flag = false;
                    logMsg = "SessionUnlock";
                    break;
                case SessionSwitchReason.SessionLogoff:
                    logoffState.flag = true;
                    logMsg = "SessionLogoff";
                    break;
                case SessionSwitchReason.SessionLogon:
                    logoffState.flag = false;
                    logMsg = "SessionLogon";
                    break;
                case SessionSwitchReason.SessionRemoteControl:
                    logMsg = "SessionRemoteControl";
                    break;
                default:
                    logMsg = String.Format("Unknown: {0}", e);
                    break;
            }
            log(logMsg);
        }

        private void log(String message) {
            Debug.WriteLine("{0:MM/dd/yyyy HH:mm:ss.fff}: {1}", DateTime.Now, message);
        }

        private void timer1_Tick(object sender, EventArgs e) {
            bool isRunning = Pinvoke.GetScreenSaverRunning();
            if (screenSaverActive.flag != isRunning) {
                screenSaverActive.flag = isRunning;
                log(String.Format("Screensaver state change: {0}", screenSaverActive.flag ? "started" : "stopped"));
            }

            uint lastInput = Pinvoke.GetLastInputTime();
            if (lastInput < idleTime) {
                log(String.Format("Idle time reset: was {0}ms, is {1}ms", idleTime, lastInput));
            }
            idleTime = lastInput;

            if (!logoffState.flag) {
                // Reset user idle timer to block screensaver by tapping shift periodically
                if (!screenSaverActive.flag && idleTime > KEEPALIVE_TIMEOUT) {
                    SendKeys.Send("+");
                    log("De-idle");
                }

                /* If screensaver has been inactive and workstation has been 
                 * locked for SCREENSAVER_TIMEOUT ms then start screensaver.
                 * Cannot depend on user idle timer due to keepalive above.
                 */
                if (!screenSaverActive.flag && sessionLockState.flag
                && screenSaverActive.when > SCREENSAVER_TIMEOUT 
                && sessionLockState.when > SCREENSAVER_TIMEOUT) {
                    Pinvoke.StartScreenSaver();
                    //LockWorkStationSafe();
                    log("Starting screensaver");
                }
            }
        }

        private void toolStripMenuItemAbout_Click(object sender, EventArgs e) {

        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e) {
            notifyIcon1.Visible = false;
        }


    }
}
