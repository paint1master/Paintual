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
using System.IO;

namespace Engine.Utilities
{
    public static class SFO
    {

        public static bool FileExists(string fileName)
        {
            FileInfo aFile = new FileInfo(fileName);
            return aFile.Exists;
        }


        public static bool DirectoryExists(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);

            if (di.Exists)
            {
                return true;
            }

            di.Create();

            return true;
        }


        public static string FileOpenReadClose(string fileName)
        {
            string newString = "";

            if (FileExists(fileName))
            {
                StreamReader sr = new StreamReader(fileName, System.Text.Encoding.UTF7);

                newString = sr.ReadToEnd();

                sr.Close();
            }
            //aFile object will be destroyed when this method completes
            return newString;
        }
    }
}
