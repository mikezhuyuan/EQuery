using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EQuery.Utils
{
    class Watch : IDisposable
    {
        public string Name { get; private set; }
        public Watch (string name)
        {
            Name = name;
        }

        private Stopwatch _watch = new Stopwatch();
        public IDisposable Begin()
        {
            _watch.Start();
            return this;
        }

        public void End()
        {
            _watch.Stop();
        }

        public TimeSpan Elapsed
        {
            get { return _watch.Elapsed; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            End();
        }

        #endregion
    }
}
