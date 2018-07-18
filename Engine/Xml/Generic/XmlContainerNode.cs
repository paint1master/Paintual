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

namespace Engine.Xml.Generic
{
    /// <summary>
    /// Represents an XML node that contains one or more nodes. Usually XML files in Paintual do have two types of nodes :
    /// nodes that contain values (InnerText) and node that contains nodes.
    /// </summary>
    public class XmlContainerNode
    {
        protected List<XmlNode> t_childrenNodes;

        public XmlContainerNode(System.Xml.XmlNode node)
        {
            t_childrenNodes = new List<XmlNode>();

            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                // TODO replace System.Xml.XmlNode with proper instances of Engine.Xml.Generic
                t_childrenNodes.Add(node.ChildNodes[i]);
            }
        }

        public int Count
        {
            get { return t_childrenNodes.Count; }
        }

        public List<XmlNode> Children
        {
            get { return t_childrenNodes; }
        }
    }
}
