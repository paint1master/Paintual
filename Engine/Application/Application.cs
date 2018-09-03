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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public sealed class Application
    {
        public static readonly Application Instance = new Application();

        // these public static readonly have their values set once, since they are collections, 
        // collection content may, of course, vary
        public static readonly Engine.Preferences Prefs = new Engine.Preferences();
        public static readonly Engine.DefaultValues DefaultValues = new DefaultValues();
        public static readonly Engine.UISelectedValues UISelectedValues = new UISelectedValues();

        //private static Engine.Tools.Attributes.AttributeCollection m_attributeCollection = new Tools.Attributes.AttributeCollection();
        //public static Engine.ProgressInfo ProgressInformation;

        //private static bool uiCanReceiveRequests = false;

        // since its value may change during app life, it cannot be "public + readonly"
        private Engine.Utilities.Language.LanguageIdentifiers currentLanguage = Utilities.Language.LanguageIdentifiers.ENG;

        // TODO : use this to know value on Surface and laptop
        //MessageBox.Show(Engine.Calc.Matrix.NumericsVectorCount().ToString());

        private Application()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>see http://csharpindepth.com/Articles/General/Singleton.aspx </remarks>
        static Application() { }

        public Engine.Utilities.Language.LanguageIdentifiers CurrentLanguage
        {
            get { return currentLanguage; }
        }


        /*public static bool UICanReceiveRequests
        {
            get { return uiCanReceiveRequests; }
            set
            {
                uiCanReceiveRequests = value;

                if (value == true)
                {
                    // later, use code from https://stackoverflow.com/questions/10867574/c-sharp-how-to-save-a-function-call-for-in-memory-for-later-invoking
                    // to store method calls until UI is ready instead of manually calling each method here
                    //Engine.Application.Prefs.CompleteFirstApplicationPreferencesSetUp();
                }
            }
        }*/
    }

    public static class EngineCppLibrary
    {
        // see https://blogs.msdn.microsoft.com/jonathanswift/2006/10/03/dynamically-calling-an-unmanaged-dll-from-net-c/

        // the following three are required to connect to Dll
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);


        // to dynamically get the path to the installed DLL, should be in same directory as PaintualUI.exe
        private static string appcurrDirectory = Environment.CurrentDirectory;

        public static readonly string EngineCppDllPath = appcurrDirectory + @"\EngineCpp.dll";

        private static IntPtr s_pDll;

        // need to create as many pointers as there are functions to be called in Dll
        public static IntPtr Pointer_luminance;
        public static IntPtr Pointer_calculateRippleEffect;

        static EngineCppLibrary()
        {
            s_pDll = LoadLibrary(EngineCppDllPath);
            //oh dear, error handling here
            //if (s_pDll == IntPtr.Zero)

            // initialize all pointers here
            Pointer_luminance = GetProcAddress(s_pDll, "luminance");
            //oh dear, error handling here
            //if(pAddressOfFunctionToCall == IntPtr.Zero)

            Pointer_calculateRippleEffect = GetProcAddress(s_pDll, "calculateRippleEffect");
        }

        public static void FreeLibrary()
        {
            FreeLibrary(s_pDll);
        }


        // each call to a CPP dll to be declared as a delegate
        // because dll location depends on where the user has installed the application
        // DLLImport requires a constant value.
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Luminance(int n, int[] blue, int[] green, int[] red, double[] lum);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CalculateRippleEffect(int imageSize, double[] sum, double[] b1_1, double[] b1_2, double[] b1_3, double[] b1_4, double[] b2_1, double t_dampening);
    }
}
