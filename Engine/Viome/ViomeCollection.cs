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
        private Dictionary<int, Engine.Viome> t_viomes;

        // if VIOME key is set to -1, then no Viome is active
        private int t_activeViomeKey;

        internal ViomeCollection()
        {
            t_viomes = new Dictionary<int, Viome>();
        }

        internal Viome NewViome(Engine.Workflow w)
        {
            int key = ViomeCollection.CreateViomeKey();

            if (t_viomes.ContainsKey(key))
            {
                throw new ArgumentException(String.Format("A Viome with the specified key ({0}) already exists in the Application.", key));
            }

            Engine.Viome v = new Viome(w, key);
            t_viomes.Add(key, v);

            t_activeViomeKey = key;
            v.ChangeImage(w.Canvas, true);
            RaiseViomeChanged();
            return v;
        }

        public Viome GetViome(int key)
        {
            if (key == -1)
            {
                throw new ArgumentOutOfRangeException(String.Format("In {0}, the provided Viome key {1} cannot be retrieved.", "ViomeCollection.GetViome", key));
            }

            Viome w = t_viomes[key];

            if (w == null)
            {
                throw new ArgumentException(String.Format("The Viome with the specified key ({0}) does not exist in the Application.", key));
            }

            return w;
        }

        public void SetAsActiveViome(int key)
        {
            if (key == -1)
            {
                throw new ArgumentOutOfRangeException(String.Format("In {0}, the provided Viome key {1} cannot be set as active.", "ViomeCollection.SetAsActiveViome", key));
            }

            if (t_viomes.ContainsKey(key))
            {
                if (key != t_activeViomeKey)
                {
                    t_activeViomeKey = key;
                    RaiseViomeChanged();
                }
            }
            else
            {
                throw new ArgumentException(String.Format("The Viome with the specified key ({0}) does not exist in the Application.", key));
            }
        }

        public void EndViome(int key)
        {
            if (key == -1)
            {
                return;
            }

            Engine.Viome v = t_viomes[key];

            t_viomes.Remove(key);

            v.Dispose();
        }

        public Viome GetActiveViome()
        {
            if (t_viomes.ContainsKey(t_activeViomeKey))
            {
                return t_viomes[t_activeViomeKey];
            }
            else
            {
                throw new ArgumentException(String.Format("The Viome with the specified key ({0}) does not exist in the Application.", t_activeViomeKey));
            }            
        }

        /// <summary>
        /// Removes Viome activity focus (no Viome is set as active).  
        /// </summary>
        public void DeactivateViome()
        {
            t_activeViomeKey = -1;
            RaiseViomeChanged();
        }

        #region Events

        public delegate void ViomeChangedEventHandler(object sender, ViomeEventArgs e);
        public event ViomeChangedEventHandler ViomeChanged;

        private void RaiseViomeChanged()
        {
            if (ViomeChanged != null)
            {
                ViomeChanged(this, new ViomeEventArgs( t_viomes[t_activeViomeKey]));
            }
        }

        #endregion

        #region Static
        private static int s_key = 0;
        public static int CreateViomeKey()
        {
            s_key++;

            return s_key;
        }
        #endregion
    }

    public class ViomeEventArgs : EventArgs
    {
        public Viome Viome { get; set; }

        public ViomeEventArgs(Viome v)
        {
            Viome = v;
        }
    }
}


