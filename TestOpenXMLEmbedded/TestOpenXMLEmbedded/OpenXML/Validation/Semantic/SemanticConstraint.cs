﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace DocumentFormat.OpenXml.Validation.Semantic
{
    /// <summary>
    /// Base class for each semantic constraint category.
    /// </summary>
    internal abstract partial class SemanticConstraint
    {
        public SemanticConstraint(SemanticValidationLevel level)
        {
            SemanticValidationLevel = level;
        }

        public SemanticValidationLevel SemanticValidationLevel { get; }

        public virtual SemanticValidationLevel StateScope => SemanticValidationLevel;

        /// <summary>
        /// Semantic validation logic
        /// </summary>
        /// <param name="context">return null if validation succeed</param>
        /// <returns></returns>
        abstract public ValidationErrorInfo Validate(ValidationContext context);

        protected static OpenXmlPart GetReferencedPart(ValidationContext context, string path)
        {
            if (path == ".")
            {
                return context.Part;
            }

            string[] parts = path.Split('/');

            if (string.IsNullOrEmpty(parts[0]))
            {
                return GetPartThroughPartPath(context.Package.Parts, parts.Skip(1).ToArray()); //absolute path
            }
            else if (parts[0] == "..")
            {
                var refParts = context.Package
                    .GetAllParts()
                    .Where(p => p.Parts.Any(r => r.OpenXmlPart.PackagePart.Uri == context.Part.PackagePart.Uri));

                Debug.Assert(refParts.Count() == 1);

                return refParts.First();
            }
            else
            {
                return GetPartThroughPartPath(context.Part.Parts, parts); //relative path
            }
        }

        protected static XmlQualifiedName GetAttributeQualifiedName(OpenXmlElement element, byte attributeID) => element.Attributes[attributeID].Property.GetQName();

        private static bool CompareBooleanValue(bool value1, string value2)
        {
            if (bool.TryParse(value2, out bool parsedValue))
            {
                return value1 == parsedValue;
            }
            else
            {
                return false;
            }
        }

        protected static bool AttributeValueEquals(OpenXmlSimpleType type, string value, bool ignoreCase)
        {
            if (type is HexBinaryValue hexValue)
            {
                if (!hexValue.HasValue)
                {
                    return true;
                }

                return Convert.ToInt64(hexValue.Value, 16) == Convert.ToInt64(value, 16);
            }

            if (type is BooleanValue boolValue)
            {
                if (!boolValue.HasValue)
                {
                    return false;
                }

                if (CompareBooleanValue(boolValue.Value, value))
                {
                    return true;
                }
            }

            if (type is OnOffValue onOffValue)
            {
                if (!onOffValue.HasValue)
                {
                    return false;
                }

                if (CompareBooleanValue(onOffValue.Value, value))
                {
                    return true;
                }
            }

            if (type is TrueFalseValue trueFalseValue)
            {
                if (!trueFalseValue.HasValue)
                {
                    return false;
                }

                if (CompareBooleanValue(trueFalseValue.Value, value))
                {
                    return true;
                }
            }

            if (type is TrueFalseBlankValue trueFalseBlankValue)
            {
                if (!trueFalseBlankValue.HasValue)
                {
                    return false;
                }

                if (CompareBooleanValue(trueFalseBlankValue.Value, value))
                {
                    return true;
                }
            }

            if (ignoreCase)
            {
                return string.Equals(value, type.InnerText, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return string.Equals(value, type.InnerText, StringComparison.Ordinal);
            }
        }

        protected static bool GetAttrNumVal(OpenXmlSimpleType attributeValue, out double value)
        {
            if (attributeValue is HexBinaryValue hexBinaryValue)
            {
                bool result = long.TryParse(hexBinaryValue.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long val);
                value = val;
                return result;
            }

            return double.TryParse(attributeValue.InnerText, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out value);
        }

        private static OpenXmlPart GetPartThroughPartPath(IEnumerable<IdPartPair> pairs, string[] path)
        {
            OpenXmlPart temp = null;
            var parts = pairs;

            for (int i = 0; i < path.Length; i++)
            {
                var s = parts.Where(p => p.OpenXmlPart.GetType().Name == path[i]).Select(t => t.OpenXmlPart);

                if (s.Count() > 1)
                {
                    throw new System.IO.FileFormatException("Invalid document error: more than one part retrieved for one URI.");
                }

                if (s.Count() == 0)
                {
                    return null;
                }

                temp = s.First();
                parts = temp.Parts;
            }

            return temp;
        }

        protected readonly struct PartHolder<T>
        {
            public PartHolder(T item, OpenXmlPart part)
            {
                Item = item;
                Part = part;
            }

            public T Item { get; }

            public OpenXmlPart Part { get; }
        }
    }
}
