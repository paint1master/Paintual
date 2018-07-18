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

namespace Engine.Xml
{
    public sealed class NodeNames
    {
        public const string document = "document";
        public const string evaluation = "evaluation";
        public const string cellularAutomataRuleNode = "cellularAutomataRuleNode";
        public const string id = "id";
        public const string rule = "rule";
        public const string propagate = "propagate";
        public const string direction = "direction";
        public const string numberOfRows = "numberOfRows";

        public const string comment = "#comment";

        public const string threshold = "threshold";
        public const string ifLessThanOrEqual = "ifLessThanOrEqual";
        public const string ifGreater = "ifGreater";

        public const string luminance = "luminance";

        public const string colorAction = "colorAction";
        public const string colorActionY = "colorActionY";
        public const string colorActionR = "colorActionR";
        public const string colorActionG = "colorActionG";
        public const string colorActionB = "colorActionB";
        public const string colorActionH = "colorActionH";
        public const string colorActionS = "colorActionS";
        public const string colorActionV = "colorActionV";

        public const string modifyCurrentPixel = "modifyCurrentPixel";

        //*************
        // node names for generic nodes
    }
}
