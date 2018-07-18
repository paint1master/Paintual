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

namespace Engine.Xml
{
    public class DocumentNode : BaseNode
    {
        private System.Xml.XmlDocument t_xmlDoc;
        private string t_fileName;

        public DocumentNode()
        {
            t_nodeName = Engine.Xml.NodeNames.document;
            t_xmlDoc = new XmlDocument();
        }

        public DocumentNode(string fileName) : this()
        {
            if (String.IsNullOrEmpty(fileName))
            {
                throw new Exception("In Engine.Xml.DocumentNode, fileName cannot be null or empty.");
            }

            string tempStr = Engine.Utilities.SFO.FileOpenReadClose(fileName);

            t_fileName = fileName;
            t_xmlDoc.LoadXml(tempStr);
        }

        public void Save()
        {
            t_xmlDoc.Save(t_fileName);
        }

        /// <summary>
        /// Returns the full name including path and extension of the Xml file loaded by this instance.
        /// </summary>
        public string FileName
        {
            get { return t_fileName; }
            set { t_fileName = value; }
        }

        public System.Xml.XmlDocument XmlDocument
        {
            get { return t_xmlDoc; }
        }
    }
}
