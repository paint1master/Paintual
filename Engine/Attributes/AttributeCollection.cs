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

namespace Engine.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Also used by PaintualUI.Controls.PropertyPage.VisualPropertyPage</remarks>
    public class AttributeCollection
    {
        private Dictionary<string, IAttribute> m_attributes;

        public AttributeCollection() {
            this.m_attributes = new Dictionary<string, IAttribute>();
        }

        public void Add(string key, IAttribute a)
        {
            if (m_attributes.ContainsKey(key))
            {
                throw new ArgumentException(String.Format("An attribute with the specified key ({0}) already exists in the attributes collection.", key));
            }

            m_attributes.Add(key, a);
        }

        /// <summary>
        /// Returns an IAttribute that is linked in the collection to the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IAttribute Get(string key)
        {
            IAttribute a = m_attributes[key];

            if (a == null)
            {
                throw new ArgumentException(String.Format("The requested attribute with the specified key ({0}) does not exist in the attributes collection.", key));
            }

            return a;
        }

        public IAttribute Remove(string key) {
            if (m_attributes.ContainsKey(key))
            {
                IAttribute a = m_attributes[key];
                m_attributes.Remove(key);

                return a;
            }
            else {
                throw new ArgumentException(String.Format("The requested attribute with the specified key ({0}) does not exist in the attributes collection.", key));
            }
        }
    }
}
