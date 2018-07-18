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
using System.Reflection;

namespace Engine.Effects
{
    public class VisualProperties
    {
        protected System.Type activityType;
        protected string activityName;

        protected int attributeCount;

        protected System.Reflection.PropertyInfo[] properties;

        protected Dictionary<string, VisualPropertyItem> visualPropertyItems;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">The name of the activity (effect or tool)</param>
        /// <param name="type">The type of the activity (effect or tool)</param>
        public VisualProperties(string name, Type type)
        {
            activityName = name;
            activityType = type;

            visualPropertyItems = new Dictionary<string, VisualPropertyItem>();
        }

        public void Fill()
        {
            visualPropertyItems = new Dictionary<string, VisualPropertyItem>();

            properties = activityType.GetProperties();

            // properties (marked with attributes) are stuff to be displayed in the UI
            foreach (System.Reflection.PropertyInfo pi in properties)
            {
                GetAttributes(pi);
            }
        }

        protected void GetAttributes(PropertyInfo pi)
        {
            VisualPropertyItem vpi = new VisualPropertyItem();
            // if not set at all, do not add vpi to dictionary
            bool propertySet = false;

            vpi.ActualPropertyName = pi.Name;

            foreach (Attribute a in pi.GetCustomAttributes())
            {
                string typename = a.GetType().FullName;

                switch (typename)
                {
                    case "Engine.Attributes.Meta.DisplayNameAttribute":
                        vpi.DisplayName = ((Engine.Attributes.Meta.DisplayNameAttribute)a).DisplayName;
                        propertySet = true;
                        break;

                    case "Engine.Attributes.Meta.DisplayControlTypeAttribute":
                        vpi.DisplayControlType = ((Engine.Attributes.Meta.DisplayControlTypeAttribute)a).ControlType;
                        propertySet = true;
                        break;

                    case "Engine.Attributes.Meta.RangeAttribute":
                        vpi.RangeMinimumValue = ((Engine.Attributes.Meta.RangeAttribute)a).MinValue;
                        vpi.RangeMaximumValue = ((Engine.Attributes.Meta.RangeAttribute)a).MaxValue;
                        propertySet = true;
                        break;
                    case "Engine.Attributes.Meta.DataTypeAttribute":
                        vpi.DataType = ((Engine.Attributes.Meta.DataTypeAttribute)a).DataType;
                        propertySet = true;
                        break;
                    case "Engine.Attributes.Meta.ValidatorAttribute":
                        vpi.ValidatorType = ((Engine.Attributes.Meta.ValidatorAttribute)a).ValidatorType;
                        vpi.RegularExpression = ((Engine.Attributes.Meta.ValidatorAttribute)a).RegularExpression;
                        break;

                    case "Engine.Attributes.Meta.ValueListAttribute":
                        Type t = ((Engine.Attributes.Meta.ValueListAttribute)a).ValueListType;
                        vpi.ValueList = (Engine.Attributes.ValueList)Activator.CreateInstance(t) ;
                        break;

                    case "Engine.Attributes.Meta.DefaultValueAttribute":
                        vpi.DefaultValue = ((Engine.Attributes.Meta.DefaultValueAttribute)a).DefaultValue;
                        break;
                    default:
                        throw new ArgumentException(String.Format("The type \"{0}\" is not supported in '{1}'.", typename, activityType.Name));
                }
            }

            if (propertySet)
            {
                visualPropertyItems.Add(pi.Name, vpi);
            }
        }

        public Dictionary<string, VisualPropertyItem> GetItems()
        {
            return visualPropertyItems;
        }

        public int Count
        {
            get { return visualPropertyItems.Count; }
        }
    }
}
