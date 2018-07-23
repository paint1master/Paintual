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

namespace Engine.Validators
{
    public class ValueListValidator : Validator, IValidated
    {
        private Engine.Attributes.ValueList t_list;

        public ValueListValidator(Engine.Attributes.ValueList list)
        {
            t_list = list;
        }

        public override bool Validate()
        {
            if (t_list.Dictionary.ContainsKey(this.inputValue))
            {
                validated = t_list.Dictionary[this.inputValue];

                return true;
            }

            errorMessage = Engine.Utilities.Language.LanguageFormat.Format("ErrorMessages", "1", "Invalid value");

            return false;
        }

        public int Validated
        {
            get { return (int)validated; }
        }

        object IValidated.Validated => validated;
    }
}
