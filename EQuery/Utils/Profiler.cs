using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EQuery.Utils
{
    public class Profiler : IDisposable
    {
        private static readonly Profiler _instance = new Profiler();

        private Profiler()
        {
        }

        private bool _isActivated = false;
        private bool _isStarted = false;
        private readonly Dictionary<string, Watch> _watches = new Dictionary<string, Watch>();

        public static Profiler Begin()
        {
            _instance._isActivated = true;
            _instance._isStarted = true;
            return _instance;
        }

        public static void End()
        {
            _instance._isStarted = false;
        }

        internal static IDisposable Watch(string name)
        {
            if (!_instance._isStarted)
                return null;
            
            Watch result;
            if (!_instance._watches.TryGetValue(name, out result))
            {
                result = _instance._watches[name] = new Watch(name);
            }

            result.Begin();

            return result;
        }

        internal static void EndWatch(string name)
        {
            if (!_instance._isStarted)
                return;

            Watch result;
            if (_instance._watches.TryGetValue(name, out result))
            {
                result.End();
            }
        }

        public static string Statistics()
        {
            if (!_instance._isActivated)
            {                
                return "The Profiler is not activated.";
            }

            var sbuf = new StringBuilder();
            double total = 0.0;
            foreach (var watch in _instance._watches)
            {
                var ms = watch.Value.Elapsed.TotalMilliseconds;
                total += ms;
            }

            foreach (var watch in _instance._watches)
            {
                var ms = watch.Value.Elapsed.TotalMilliseconds;
                sbuf.AppendLine(watch.Key + ": " + ms + " (" + (ms / total).ToString("P02") + ")");                
            }

            sbuf.AppendLine("Total: " + total);
            return sbuf.ToString();
        }

        public void Reset()
        {
            _instance._isActivated = false;
            _instance._isStarted = false;
            _watches.Clear();
        }

        #region IDisposable Members

        public void Dispose()
        {
            _isStarted = false;
        }

        #endregion
    }
}
