﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using DocumentFormat.OpenXml.Framework;
using DocumentFormat.OpenXml.Validation;
using System;

namespace DocumentFormat.OpenXml
{
    /// <summary>
    /// Defines an OfficeAvailabilityAttribute class to indicate whether the property is available in a specific version of an Office application.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Field)]
    public sealed class OfficeAvailabilityAttribute : Attribute, IOpenXmlSimpleTypeValidator
    {
        /// <summary>
        /// Gets the Office version of the available property.
        /// </summary>
        public FileFormatVersions OfficeVersion { get; }

        /// <summary>
        /// Initializes a new instance of the OfficeAvailabilityAttribute class.
        /// </summary>
        /// <param name="officeVersion">The Office version where this class or property is available.
        /// If there is more than one version, use bitwise OR to specify multiple versions.</param>
        public OfficeAvailabilityAttribute(FileFormatVersions officeVersion)
        {
            OfficeVersion = officeVersion;
        }

        void IOpenXmlSimpleTypeValidator.Validate(ValidatorContext context)
        {
            if (!context.Version.AtLeast(OfficeVersion) && context.Value?.HasValue == true && !context.McContext.IsIgnorableNs(context.QName.Namespace))
            {
                context.CreateError(
                    id: "Sch_UndeclaredAttribute",
                    description: SR.Format(ValidationResources.Sch_UndeclaredAttribute, context.QName),
                    errorType: ValidationErrorType.Schema);
            }
        }
    }
}
