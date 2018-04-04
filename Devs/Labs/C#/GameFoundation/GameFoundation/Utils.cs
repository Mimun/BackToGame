using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive.Linq;

namespace GameFoundation
{
    public static class Utils
    {
        public static IDisposable setTimeout(Action action, double timeSpan)
        {
            var result = Observable.Timer(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(timeSpan)).Subscribe(_=>action?.Invoke());
            return result;
        }
        public static IDisposable setInterval(Action action, int timeSpan) {
            return null;
        }
    }
}
