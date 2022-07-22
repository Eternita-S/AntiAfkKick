using Dalamud.Game;
using Dalamud.Logging;
using Dalamud.Plugin;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiAfkKick {
  class TickScheduler : IDisposable {
    long executeAt;
    Action function;
    Framework framework;

    public TickScheduler(Action function, Framework framework, long delayMS = 0) {
      this.executeAt = Environment.TickCount64 + delayMS;
      this.function = function;
      this.framework = framework;
      framework.Update += new Framework.OnUpdateDelegate(this.Execute);
    }

    public void Dispose() {
      this.framework.Update -= new Framework.OnUpdateDelegate(this.Execute);
    }

    void Execute(object _) {
      if (Environment.TickCount64 < executeAt)
        return;
      try {
        function();
      } catch (Exception e) {
        PluginLog.Error(e.Message + "\n" + e.StackTrace, Array.Empty<object>());
      }
      this.Dispose();
    }
  }
}
