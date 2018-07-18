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
    public class ImageAttribute : IAttribute
    {
        private Engine.Surface.Canvas t_image;
        private Engine.Attributes.StringAttribute t_name;

        public ImageAttribute() { }

        public ImageAttribute(Engine.Surface.Canvas img)
        {
            t_image = img;
        }

        public string Name {
            get { return t_name.Value; }
            set {
                Engine.Attributes.StringAttribute str = new StringAttribute(value);
                t_name = str;
            }
        }

        public Engine.Surface.Canvas Value
        {
            get
            {
                return t_image;
            }
        }

        object IAttribute.Value
        {
            get
            {
                return null;
            }
        }

        void IAttribute.SetValue(object val)
        {
            if (val.GetType() == typeof(Engine.Surface.Canvas))
            {
                t_image = (Engine.Surface.Canvas)val;
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        string IAttribute.Type
        {
            get
            {
                return typeof(Engine.Surface.Canvas).FullName;
            }
        }

        protected IAttribute Property(Properties p)
        {
            if (p == Properties.Name) {
                return t_name;
            }

            throw new ArgumentOutOfRangeException(String.Format("The attribute \"{0}\" does not support properties.", ((IAttribute)(this)).Type));
        }

        IAttribute IAttribute.Property(Engine.Attributes.Properties p)
        {
            return Property(p);
        }
    }
}