using System;
using System.Collections.Generic;
using System.Text;

namespace HTCPlatform.Core.Attributes
{
    public class SwaggerDefaultValueAttribute : Attribute
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public SwaggerDefaultValueAttribute(object value)
        {
            this.Value = value;
        }

        public SwaggerDefaultValueAttribute(string name, object value) : this(value)
        {
            this.Name = name;
        }
    }
}
