﻿/**********************************************************

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
    public class WorkflowCollection
    {
        private Dictionary<int, Engine.Workflow> t_workflows;

        // if Workflow key is set to -1, then no Workflow is active
        private int t_activeWorkflowKey;

        internal WorkflowCollection()
        {
            t_workflows = new Dictionary<int, Engine.Workflow>();
        }

        public Engine.Workflow NewWorkflow()
        {
            int key = WorkflowCollection.CreateWorkflowKey();
            Engine.Workflow w = new Workflow(key);
            t_workflows.Add(key, w);

            t_activeWorkflowKey = key;
            RaiseWorkflowChanged();
            return w;
        }

        public Engine.Workflow NewWorkflow(string fileName)
        {
            int key = WorkflowCollection.CreateWorkflowKey();

            if (t_workflows.ContainsKey(key))
            {
                throw new ArgumentException(String.Format("A Workflow with the specified key ({0}) already exists in the Application.", key));
            }

            Engine.Workflow w = new Workflow(key, fileName);
            t_workflows.Add(key, w);

            t_activeWorkflowKey = key;
            RaiseWorkflowChanged();
            return w;
        }

        public Engine.Workflow NewWorkflow(Engine.Surface.Canvas c)
        {
            int key = WorkflowCollection.CreateWorkflowKey();

            if (t_workflows.ContainsKey(key))
            {
                throw new ArgumentException(String.Format("A Workflow with the specified key ({0}) already exists in the Application.", key));
            }

            Engine.Workflow w = new Workflow(key);
            t_workflows.Add(key, w);

            t_activeWorkflowKey = key;
            RaiseWorkflowChanged();
            return w;
        }

        public Engine.Workflow GetWorkflow(int key)
        {
            if (key == -1)
            {
                throw new ArgumentOutOfRangeException(String.Format("In {0}, the provided Workflow key {1} cannot be retrieved.", "WorkflowCollection.GetWorkflow", key));
            }

            Engine.Workflow w = t_workflows[key];

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
                throw new ArgumentOutOfRangeException(String.Format("In {0}, the provided Workflow key {1} cannot be set as active.", "WorkflowCollection.SetAsActiveWorkflow", key));
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

            Engine.Workflow w = t_workflows[key];
            Engine.Application.Viomes.EndViome(w.Viome.Key);

            w.RaiseClosing();

            t_workflows.Remove(key);

            w.Dispose();
        }

        public Engine.Workflow GetActiveWorkflow()
        {
            if (t_workflows.ContainsKey(t_activeWorkflowKey))
            {
                return t_workflows[t_activeWorkflowKey];
            }
            else
            {
                throw new ArgumentException(String.Format("The Workflow with the specified key ({0}) does not exist in the Application.", t_activeWorkflowKey));
            }
        }

        /// <summary>
        /// Removes Workflow activity focus (no Workflow is set as active).  
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
                WorkflowChanged(this, new WorkflowEventArgs(t_workflows[t_activeWorkflowKey]));
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
        public Engine.Workflow Workflow { get; set; }

        public WorkflowEventArgs(Engine.Workflow w)
        {
            Workflow = w;
        }
    }
}



