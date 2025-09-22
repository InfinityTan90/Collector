using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection.Global_var
{
    class CollectorPageInstance
    {
        public static Collection.Pages.Collector Instance;

        public static bool IsPlayerWorking
        {
            set
            {
                Instance.IsPlayerWorking = value;
            }
        }
    }
}
