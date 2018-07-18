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

namespace Engine.Utilities.Language
{
    public enum LanguageIdentifiers
    {
        ENG,
        FRE
    }
    public static class LanguageFormat
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">A string starting with the '#' sign followed by an index number. This index number is looked up
        /// in the translation dictionary, and the corresponding text element is returned according to the current language
        /// set for the application.</param>
        /// <param name="defaultValue">A default value to be displayed by the UI in case the index could not be found
        /// in the translation dictionary. Serves also as a visual token for developers to see what the index number refers to.</param>
        /// <returns></returns>
        public static string Format(string translationIndex, string defaultValue)
        {
            //LanguageFormat.Format("#101", "Seed");

            // TODO : complete dict code
            return defaultValue;
        }
    }
}
