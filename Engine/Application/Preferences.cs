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
using System.Xml;

namespace Engine
{
    public delegate void ApplicationPreferencesEventHandler(ApplicationPreferencesEventArgs e);

    public enum ApplicationPreferencesRequest
    {
        LibraryPath
    }

    public class Preferences
    {
        private string appDataPath;
        //private string t;
        private string appPrefFileName = @"\applicationPreferences.xml";

        private Engine.Xml.DocumentNode xdoc;

        private string brushes;
        private string m_effects;
        private string materials;
        private string images;
        private string contentLookUpFolder;
        private string statisticsCollectorFolder;


        public Preferences()
        {
            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Belisssle\PaintualUI\PaintTools";

            xdoc = null;

            // the new installer makes sure applicationPreferences.xml exists in app Data folder

            if (!Engine.Utilities.SFO.FileExists(appDataPath + appPrefFileName))
            {
                xdoc = new Engine.Xml.DocumentNode();
                CreateApplicationPreferencesFile(appDataPath + appPrefFileName);
            }
            else
            {
                xdoc = new Engine.Xml.DocumentNode(appDataPath + appPrefFileName);
                Parse();
            }
            // for the rest of the first time initialization code (when LibraryPath is not yet set)
            // we need to get a call from Engine.Application when UI is ready.
            // see Engine.Application.UICanReceiveRequests
        }

        /// <summary>
        /// Parses the xml document and fills the current object's properties
        /// </summary>
        private void Parse()
        {
            Brushes = xdoc.XmlDocument.SelectSingleNode("document/brushes").InnerText;
            Materials = xdoc.XmlDocument.SelectSingleNode("document/materials").InnerText; ;
            Images = xdoc.XmlDocument.SelectSingleNode("document/images").InnerText; ;
            contentLookUpFolder = xdoc.XmlDocument.SelectSingleNode("document/contentLookUpFolder").InnerText;
            statisticsCollectorFolder = xdoc.XmlDocument.SelectSingleNode("document/statisticsCollectorFolder").InnerText;
        }

        private void CreateApplicationPreferencesFile(string fileName)
        {
            string fileContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n"
                               + "<document>\r\n"
                               + "  <brushes>\\Brushes\\</brushes>\r\n"
                               + "  <effects>\\Effects\\</effects>\r\n"
                               + "  <images>\\Images\\</images>\r\n"
                               + "  <materials>\\Materials\\</materials>\r\n"
                               + "  <contentLookUpFolder></contentLookUpFolder>\r\n"
                               + "  <statisticsCollectorFolder></statisticsCollectorFolder>\r\n"
                               + "</document>\r\n";
            xdoc.XmlDocument.LoadXml(fileContent);

            // do not use a defautl library folder value because when app runs the first time,
            // the user must provide a valid folder for it.
            Brushes = "\\Brushes\\";
            Effects = "\\Effects\\";
            Materials = "\\Materials\\";
            Images = "\\Images\\";
            ContentLookUpFolder = "";
            StatisticsCollectorFolder = "";

            Save(fileName);
        }

        //******************
        public void Save(string fileName)
        {
            xdoc.XmlDocument.SelectSingleNode("document/brushes").InnerText = Brushes;
            xdoc.XmlDocument.SelectSingleNode("document/effects").InnerText = Effects;
            xdoc.XmlDocument.SelectSingleNode("document/materials").InnerText = Materials;
            xdoc.XmlDocument.SelectSingleNode("document/images").InnerText = Images;
            xdoc.XmlDocument.SelectSingleNode("document/contentLookUpFolder").InnerText = ContentLookUpFolder;
            xdoc.XmlDocument.SelectSingleNode("document/statisticsCollectorFolder").InnerText = statisticsCollectorFolder;

            CheckDirectories();

            xdoc.FileName = fileName;
            xdoc.Save();
        }

        private void CheckDirectories()
        {
            // do not check for ContentLookup folder, since it is user defined
            Engine.Utilities.SFO.DirectoryExists(appDataPath + Brushes);
            Engine.Utilities.SFO.DirectoryExists(appDataPath + Materials);
            Engine.Utilities.SFO.DirectoryExists(appDataPath + Effects);
            Engine.Utilities.SFO.DirectoryExists(appDataPath + Images);

        }

        public string ApplicationDataPath
        {
            get { return this.appDataPath; }
        }

        public string Brushes
        {
            get { return brushes; }
            set { brushes = value; }
        }

        public string ContentLookUpFolder
        {
            get { return this.contentLookUpFolder; }
            set { this.contentLookUpFolder = value; }
        }

        public string Effects
        {
            get { return this.m_effects; }
            set { this.m_effects = value; }
        }

        public string Materials
        {
            get { return materials; }
            set { materials = value; }
        }

        public string Images
        {
            get { return images; }
            set { images = value; }
        }

        public string StatisticsCollectorFolder
        {
            get { return statisticsCollectorFolder; }
            set { statisticsCollectorFolder = value; }
        }

        public event ApplicationPreferencesEventHandler ApplicationPreferencesValueRequested;

        public void RaiseApplicationPreferencesValueRequested(ApplicationPreferencesRequest value)
        {
            if (ApplicationPreferencesValueRequested != null)
                ApplicationPreferencesValueRequested(new ApplicationPreferencesEventArgs(value));
        }
    }

    public class ApplicationPreferencesEventArgs
    {

        private ApplicationPreferencesRequest appPrefRequest;

        public ApplicationPreferencesEventArgs(ApplicationPreferencesRequest value)
        {
            this.appPrefRequest = value;
        }
    }
}
