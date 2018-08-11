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

namespace PaintualUI.Code
{
    /// <summary>
    /// Provides a centralized means to access various instances used all around Paintual UI
    /// </summary>
    public sealed class Application
    {
        public static readonly Application Instance  = new Application();

        // MainWindow is responsible for setting values
        internal PaintualUI.Code.VisualPropertyPageManager VisualPropertyPageManager { get; set; }
        internal PaintualUI.Code.ActiveContentHelper ActiveContentHelper { get; set; }

        private Application()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>see http://csharpindepth.com/Articles/General/Singleton.aspx </remarks>
        static Application() { }


    }
}
