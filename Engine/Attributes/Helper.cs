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
    public static class Helper
    {

        public static Engine.Surface.Canvas GetDefaultImage(Engine.Attributes.IAttribute attr) {
            Engine.Surface.Canvas img;
            if (attr.GetType() == typeof(Engine.Attributes.ImageAttribute)) {
                img = ((Engine.Attributes.ImageAttribute)attr).Value;
            }
            else if (attr.GetType() ==  typeof(Engine.Attributes.MultiImageAttribute)) {
                img = (Engine.Surface.Canvas)attr.Property(Properties.DefaultImage);

                //img = ((Engine.Attributes.MultiImageAttribute)attr).DefaultImage;
            }
            else {
                throw new ArgumentException(String.Format("Unsupported type format {0} in Engine.Tools.Attributes.Helper", attr.GetType().ToString()));
            }

            return img;
        }
    }
}
