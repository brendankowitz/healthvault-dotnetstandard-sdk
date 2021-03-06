﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a respiratory rate and a display value
    /// associated with the measurement.
    /// </summary>
    ///
    /// <remarks>
    /// In HealthVault, respiratory rate measurements have values and display values.
    /// All values are stored in a base unit of breaths per minute.
    /// An application can take a respiratory rate value using any scale the application
    /// chooses and can store the user-entered value as the display value,
    /// but the respiratory value must be converted to breaths/min to be stored in HealthVault.
    /// </remarks>
    ///
    public class RespiratoryRateMeasurement : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="RespiratoryRateMeasurement"/> class
        /// with empty values.
        /// </summary>
        ///
        public RespiratoryRateMeasurement()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RespiratoryRateMeasurement"/> class
        /// with the specified value in breaths per minute.
        /// </summary>
        ///
        /// <param name="breathsPerMinute">
        /// The respiratory rate in breaths per minute.
        /// </param>
        ///
        public RespiratoryRateMeasurement(double breathsPerMinute)
            : base(breathsPerMinute)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RespiratoryRateMeasurement"/> class with
        /// the specified value in breaths per minute and optional display value.
        /// </summary>
        ///
        /// <param name="breathsPerMinute">
        /// The respiratory rate in breaths per minute.
        /// </param>
        ///
        /// <param name="displayValue">
        /// The display value of the respiratory rate. This should contain the
        /// exact respiratory rate as entered by the user even if it uses some
        /// other unit of measure besides breaths per minute. The display value
        /// <see cref="DisplayValue.Units"/> and
        /// <see cref="DisplayValue.UnitsCode"/>
        /// represents the unit of measure for the user-entered value.
        /// </param>
        ///
        public RespiratoryRateMeasurement(double breathsPerMinute, DisplayValue displayValue)
            : base(breathsPerMinute, displayValue)
        {
        }

        /// <summary>
        /// Verifies the value is a legal respiratory rate value in breaths/min.
        /// </summary>
        ///
        /// <param name="value">
        /// The respiratory rate measurement.
        /// </param>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        protected override void AssertMeasurementValue(double value)
        {
            if (value < 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), Resources.RespiratoryRateNotPositive);
            }
        }

        /// <summary>
        /// Populates the data for the respiratory rate from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the respiratory rate.
        /// </param>
        ///
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            Value = navigator.SelectSingleNode("breaths-per-minute").ValueAsDouble;
        }

        /// <summary>
        /// Writes the respiratory rate to the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the respiratory rate to.
        /// </param>
        ///
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "breaths-per-minute",
                XmlConvert.ToString(Value));
        }

        /// <summary>
        /// Gets a string representation of the respiratory rate in the base units.
        /// </summary>
        ///
        /// <returns>
        /// The respiratory rate as a string in the base units.
        /// </returns>
        ///
        protected override string GetValueString(double value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
