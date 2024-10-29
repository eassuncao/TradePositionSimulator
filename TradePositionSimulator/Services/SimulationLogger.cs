using System;
using System.Collections.Generic;

namespace TradePositionSimulator.Services
{
    public class SimulationLogger
    {
        private List<string> _logs = new();

        public void Log(string message)
        {
            _logs.Add(message);
        }

        public string GetLogs()
        {
            return string.Join("\n", _logs);
        }

        public void Clear()
        {
            _logs.Clear();
        }
    }
}