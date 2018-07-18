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
    public sealed class Application
    {
        public static readonly Application Instance = new Application();

        // these public static readonly have their values set once, since they are collections, 
        // collection content may, of course, vary
        public static readonly Engine.ViomeCollection Viomes = new Engine.ViomeCollection();
        public static readonly Engine.Preferences Prefs = new Engine.Preferences();
        public static readonly Engine.DefaultValues DefaultValues = new DefaultValues();
        public static readonly Engine.UISelectedValues UISelectedValues = new UISelectedValues();

        //private static Engine.Tools.Attributes.AttributeCollection m_attributeCollection = new Tools.Attributes.AttributeCollection();
        //public static Engine.Debug.DebugInfoCollector ApplicationDebugInfoCollector;
        //public static Engine.ProgressInfo ProgressInformation;

        //private static bool uiCanReceiveRequests = false;
        //private static Engine.Reactual.Analyze.Stats.StatisticsCollector statsCollector;
        //private static Dictionary<string, Engine.Data.PropertyBagItem> s_propertyBag;

        // since its value may change during app life, it cannot be "public + readonly"
        private Engine.Utilities.Language.LanguageIdentifiers currentLanguage = Utilities.Language.LanguageIdentifiers.ENG;

        private Application()
        {
            //Engine.Application.ApplicationDebugInfoCollector = new Engine.Debug.DebugInfoCollector();
            //Engine.Application.ApplicationDebugInfoCollector.Debugging = true;

            //ApplicationDebugInfoCollector.Add("application initializing");

            //ApplicationDebugInfoCollector.Add("Application preferences initialized.");

            //ProgressInformation = new ProgressInfo();
            //statsCollector = new Reactual.Analyze.Stats.StatisticsCollector();
            //statsCollector.Collecting = true;

            //s_propertyBag = new Dictionary<string, Data.PropertyBagItem>();
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
}
