using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utilities.Iterativ
{
    public class Skipper
    {
        private int t_skip;
        private int t_count;

        public Skipper(int skipAmount)
        {
            t_skip = skipAmount;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true when skipping, false when resetting</returns>
        public bool Skip()
        {
            t_count++;

            if (t_count < t_skip)
            {
                //System.Diagnostics.Debug.WriteLine(String.Format("skip false {0}", t_count));
                return false;
            }

            else
            {
                t_count = 0;
                //System.Diagnostics.Debug.WriteLine(String.Format("skip true {0}", t_count));
                return true;
            }
        }
    }

    public class Repeater
    {

        public Repeater()
        {

        }
    }
}
