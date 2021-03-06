﻿/**********************************************************

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

namespace Engine.Xml.Generic
{
    /// <summary>
    /// Reads an Xml node that contains a string as InnerText. If the node contains a child node, an error will occur
    /// </summary>
    public class XmlStringNode : XmlBaseNode
    {
        protected string t_innerText;

        public XmlStringNode(System.Xml.XmlNode node)
        {
            if (node.ChildNodes.Count > 0)
            {
                throw new ArgumentOutOfRangeException(String.Format("In Engine.Xml.XmlStringNode, an XmlStringNode cannot contain a child node."));
            }

            t_nodeName = node.Name;
            t_innerText = node.InnerText;
        }

        public string Value
        {
            get { return t_innerText; }
        }

    }
}
