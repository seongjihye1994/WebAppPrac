using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppPrac.Logs
{
    public class AopController
    {
        public string ControllerName { get; set; }
        public string ControllerMethod { get; set; }
        public string ActionArguments { get; set; }
    }
}
