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
    public interface IAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <remarks>Within the class implementing Property(int p), there is a method that has the signature Property(enumTypeSpecificToClass p)
        /// for this to be used easily, we not need to know the int value, just the enum type like in :
        ///   ColorAttribute c = new ColorAttribute();
        ///   c.Property((int)Properties.Tint);
        ///   
        /// Not particularly nice, but works</remarks>
        /// <returns></returns>
        IAttribute Property(Engine.Attributes.Properties p);
        object Value { get; }
        string Type { get; }

        void SetValue(object val);
    }

    public enum Properties
    {
        NoPropertySupported = 0,
        Name = 1,
        Color = 2,
        Byte = 3,
        DefaultImage = 4
    }
}
