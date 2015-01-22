using System;

namespace ScreenSaveOnLock {
    public class StateTracker {
        private bool _flag = false;
        public bool flag {
            set {
                _tickCount = (uint)Environment.TickCount;
                _flag = value;
            }
            get {
                return _flag;
            }
        }

        private uint _tickCount = (uint)Environment.TickCount;
        public uint tickCount {
            get { return _tickCount; }
        }

        public uint when {
            get {
                return (uint)Environment.TickCount - _tickCount;
            }
        }

        public StateTracker() {
        }

        public StateTracker(bool value) {
            _flag = value;
        }
    }
}
