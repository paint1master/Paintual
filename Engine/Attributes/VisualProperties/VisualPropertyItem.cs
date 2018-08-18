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

namespace Engine.Effects
{
    public class VisualPropertyItem
    {
        private string actualPropertyName;

        // properties nullable if possible because control building decision is based on whether or not some or all
        // of those properties have been set
        private string displayName = "";

        private int? rangeMinimumValue;
        private int? rangeMaximumValue;
        private Engine.Attributes.Meta.DisplayControlTypes displayControlType = Engine.Attributes.Meta.DisplayControlTypes.None;
        private Engine.PropertyDataTypes dataType = Engine.PropertyDataTypes.Undefined;
        private Type typeDeclaration;
        private Engine.Attributes.Meta.ValidatorTypes validatorType = Attributes.Meta.ValidatorTypes.Undefined;
        private Engine.Attributes.ValueList valueList;
        private object defaultValue;

        // represents the index position of the corresponding control in the FlowPanel of the VisualPropertyPage
        private int index;

        private string regularExpression;

        public VisualPropertyItem()
        {

        }

        public string ActualPropertyName
        {
            get { return actualPropertyName; }
            set { actualPropertyName = value; }
        }

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        public int? RangeMinimumValue
        {
            get { return rangeMinimumValue; }
            set { rangeMinimumValue = value; }
        }

        public int? RangeMaximumValue
        {
            get { return rangeMaximumValue; }
            set { rangeMaximumValue = value; }
        }

        public Engine.Attributes.Meta.DisplayControlTypes DisplayControlType
        {
            get { return displayControlType; }
            set { displayControlType = value; }
        }

        public Engine.PropertyDataTypes DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        public Type TypeDeclaration
        {
            get { return typeDeclaration; }
            set { typeDeclaration = value; }
        }

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        public Engine.Attributes.Meta.ValidatorTypes ValidatorType
        {
            get { return validatorType; }
            set { validatorType = value; }
        }

        public string RegularExpression
        {
            get { return regularExpression; }
            set { regularExpression = value; }
        }

        public Engine.Attributes.ValueList ValueList
        {
            get { return valueList; }
            set { valueList = value; }
        }

        public object DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }
    }
}
