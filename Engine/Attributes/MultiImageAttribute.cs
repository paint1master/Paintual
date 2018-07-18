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
    public class MultiImageAttribute : IAttribute
    {
        private List<Engine.Surface.Canvas> t_imageList;

        private Engine.Attributes.StringAttribute t_listName;
        private Engine.Attributes.ImageAttribute t_defaultImage;

        public MultiImageAttribute() { }

        public MultiImageAttribute(List<Engine.Surface.Canvas> imageList) {
            t_imageList = imageList;

            t_defaultImage = new Engine.Attributes.ImageAttribute(t_imageList[0]);
        }

        public List<Engine.Surface.Canvas> Value {
            get
            {
                return t_imageList;
            }
        }

        public string Name {
            get { return t_listName.Value; }
            set {
                if (t_listName == null) {
                    t_listName = new StringAttribute(value);
                    return;
                }

                t_listName.Value = value; }
        }

        public Engine.Surface.Canvas DefaultImage
        {
            get { return t_defaultImage.Value; }
            set
            {
                // TODO : find a way to be sure that the default image is already part of the ListImage
                t_defaultImage = new Engine.Attributes.ImageAttribute(value);
            }
        }

        object IAttribute.Value
        {
            get
            {
                return t_imageList;
            }
        }

        void IAttribute.SetValue(object val)
        {
            if (val.GetType() == typeof(List<Engine.Surface.Canvas>))
            {
                t_imageList = (List<Engine.Surface.Canvas>)val;
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
                return typeof(List<Engine.Surface.Canvas>).FullName;
            }
        }

        protected IAttribute Property(Properties p)
        {
            if (p == Properties.Name)
            {
                return t_listName;
            }

            if (p == Properties.DefaultImage)
            {
                return t_defaultImage;
            }
            throw new ArgumentOutOfRangeException(String.Format("The property \"{0}\" does not exist in \"{1}\".", p.ToString(), ((IAttribute)(this)).Type));
        }

        IAttribute IAttribute.Property(Properties p)
        {
            return Property(p);
        }
    }
}
