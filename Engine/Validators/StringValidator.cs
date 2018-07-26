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

using System.Text.RegularExpressions;

namespace Engine.Validators
{
    public class StringValidator : Validators.Validator
    {
        protected bool cannotBeEmpty;

        public StringValidator()
        {

        }

        public StringValidator(string regEx)
        {
            pattern = regEx;
        }

        public override bool Validate()
        {
            validated = null;

            if (CannotBeEmpty && string.IsNullOrEmpty(inputValue))
            { 
                errorMessage = "This field cannot be empty. Enter a value.";
                return false;
            }


            if (string.IsNullOrEmpty(pattern))
            {
                // no regexp to validate against, therefore anything is potentially valid
                validated = inputValue;
                return true;
            }
            else
            {
                Regex r = new Regex(pattern);
                Match match = r.Match(inputValue);

                if (match.Success)
                {
                    validated = inputValue;
                    return true;
                }
                else
                {
                    errorMessage = "The field value is not in the right format";
                    return false;
                }
            }
        }

        public bool CannotBeEmpty
        {
            get { return cannotBeEmpty; }
            set { cannotBeEmpty = value; }
        }

        public string Validated
        {
            get { return (string)validated; }
        }
    }
}
