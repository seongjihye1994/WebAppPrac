using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppPrac.Logs
{
    public class LogDBTarget
    {
        public string LoggerName { get; set; }
        public string Condition { get; set; }
        private NameCondition ConditionValue { get; set; }

        public string FilePath { get; set; }

        public string CommandText { get; set; }

        public List<string> Parameters { get; set; }

        public bool IsMatched(string loggerName)
        {
            if (ConditionValue == NameCondition.NotClassfied)
            {
                var condition = NameCondition.Equals;
                if (!string.IsNullOrEmpty(Condition))
                    Enum.TryParse(Condition, true, out condition);

                ConditionValue = condition;
            }

            switch (ConditionValue)
            {
                case NameCondition.Equals:
                    if (loggerName == LoggerName)
                        return true;
                    break;
                case NameCondition.StartsWith:
                    if (loggerName.StartsWith(LoggerName))
                        return true;
                    break;
                case NameCondition.EndsWith:
                    if (loggerName.EndsWith(LoggerName))
                        return true;
                    break;
                case NameCondition.Contains:
                    if (loggerName.Contains(LoggerName))
                        return true;
                    break;
            }

            return false;
        }
    }

}