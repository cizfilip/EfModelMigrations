﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations.Operations.Mapping.Model
{
    public class PropertySelectorParameter : IEfFluentApiMethodParameter
    {
        public string ClassName { get; private set; }

        public string[] PropertyNames { get; private set; }

        public PropertySelectorParameter(string className, string propertyName)
            :this(className, new[] { propertyName })
        {
        }

        public PropertySelectorParameter(string className, string[] propertyNames)
        {
            this.ClassName = className;
            this.PropertyNames = propertyNames;
        }
    }
}