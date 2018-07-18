/**********************************************************

MIT License

Copyright (c) 2018 Michel Belisle

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

**********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class ViomeCollection
    {
        private Dictionary<int, Engine.Viome> t_workflows;

        // if workflow key is set to -1, then no workflow is active
        private int t_activeWorkflowKey;

        internal ViomeCollection()
        {
            t_workflows = new Dictionary<int, Viome>();
        }

        public Viome NewWorkflow(Engine.Surface.Canvas c)
        {
            int key = ViomeCollection.CreateWorkflowKey();

            if (t_workflows.ContainsKey(key))
            {
                throw new ArgumentException(String.Format("A Viome with the specified key ({0}) already exists in the Application.", key));
            }

            Engine.Viome w = new Viome(key);
            t_workflows.Add(key, w);

            t_activeWorkflowKey = key;
            w.SetImage(c);
            RaiseWorkflowChanged();
            return w;
        }

        public Viome GetWorkflow(int key)
        {
            if (key == -1)
            {
                throw new ArgumentOutOfRangeException(String.Format("In {0}, the provided Viome key {1} cannot be retrieved.", "ViomeCollection.GetWorkflow", key));
            }

            Viome w = t_workflows[key];

            if (w == null)
            {
                throw new ArgumentException(String.Format("The Viome with the specified key ({0}) does not exist in the Application.", key));
            }

            return w;
        }

        public void SetAsActiveWorkflow(int key)
        {
            if (key == -1)
            {
                throw new ArgumentOutOfRangeException(String.Format("In {0}, the provided workflow key {1} cannot be set as active.", "WorkflowCollection.SetAsActiveWorkflow", key));
            }

            if (t_workflows.ContainsKey(key))
            {
                if (key != t_activeWorkflowKey)
                {
                    t_activeWorkflowKey = key;
                    RaiseWorkflowChanged();
                }
            }
            else
            {
                throw new ArgumentException(String.Format("The Workflow with the specified key ({0}) does not exist in the Application.", key));
            }
        }

        public void EndWorkflow(int key)
        {
            if (key == -1)
            {
                return;
            }

            Engine.Viome w = t_workflows[key];

            t_workflows.Remove(key);

            w.Dispose();


        }

        public Viome GetActiveWorkflow()
        {
            if (t_workflows.ContainsKey(t_activeWorkflowKey))
            {
                return t_workflows[t_activeWorkflowKey];
            }
            else
            {
                throw new ArgumentException(String.Format("The Viome with the specified key ({0}) does not exist in the Application.", t_activeWorkflowKey));
            }            
        }

        /// <summary>
        /// Removes workflow activity focus (no workflow is set to active).  
        /// </summary>
        public void DeactivateWorkflow()
        {
            t_activeWorkflowKey = -1;
            RaiseWorkflowChanged();
        }

        #region Events

        public delegate void WorkflowChangedEventHandler(object sender, WorkflowEventArgs e);
        public event WorkflowChangedEventHandler WorkflowChanged;

        private void RaiseWorkflowChanged()
        {
            if (WorkflowChanged != null)
            {
                WorkflowChanged(this, new WorkflowEventArgs( t_workflows[t_activeWorkflowKey]));
            }
        }

        #endregion

        #region Static
        private static int s_key = 0;
        public static int CreateWorkflowKey()
        {
            s_key++;

            return s_key;
        }
        #endregion
    }

    public class WorkflowEventArgs : EventArgs
    {
        public Viome Viome { get; set; }

        public WorkflowEventArgs(Viome w)
        {
            Viome = w;
        }
    }
}


