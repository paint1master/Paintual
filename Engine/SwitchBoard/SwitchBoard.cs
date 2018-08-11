using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.SwitchBoard
{
    internal class SwitchBoard
    {

        public SwitchBoard()
        {
            /* the idea here is to create a event and event handler management system
             * that would eventually be used like
             * SwitchBoard sb = new SwitchBoard()
             * 
             * // register instances
             * sb.Add(viome)
             * sb.Add(PaintualCanvas control) <- may be a problem because controls are not in Engine dll
             * 
             * // then register events handling
             * sb.Viome.To.PaintualCanvas.When.UpdateImageRequest
             * 
             * // unregistering could be like
             * sb.Stop.Viome.To.PaintualCanvas.When.UpdateImageRequest
             * */
        }
    }
}
