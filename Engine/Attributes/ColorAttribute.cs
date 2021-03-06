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

namespace Engine.Attributes
{
    public class ColorAttribute : IAttribute
    {
        private Engine.Color.Cell t_color;

        public ColorAttribute() {

        }

        public ColorAttribute(Engine.Color.Cell c) {
            t_color = c;
        }

        public Engine.Color.Cell Value {
            get {
                return t_color;
            }
            set {
                t_color = value;
            }
        }

        void IAttribute.SetValue(object val)
        {
            if (val.GetType() == typeof(Engine.Color.Cell))
            {
                t_color = (Engine.Color.Cell)val;
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        object IAttribute.Value
        {
            get
            {
                return t_color;
            }
        }

        string IAttribute.Type
        {
            get
            {
                return typeof(Engine.Color.Cell).FullName;
            }
        }

        protected IAttribute Property(Properties p)
        {
            throw new ArgumentOutOfRangeException(String.Format("The property \"{0}\" does not exist in \"{1}\".", p.ToString(), ((IAttribute)(this)).Type));
        }

        IAttribute IAttribute.Property(Properties p)
        {
            return Property(p);
        }
    }
}
