// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Defines a single health or medical issue/problem.
    /// </summary>
    public class ConditionEntry : ItemBase
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="ConditionEntry"/> class
        /// with default values.
        /// </summary>
        ///
        public ConditionEntry()
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="ConditionEntry"/> class
        /// with name.
        /// </summary>
        ///
        /// <param name="name">
        /// The name or description of a condition.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <b> null </b>.
        /// </exception>
        ///
        public ConditionEntry(CodableValue name)
        {
            Name = name;
        }

        /// <summary>
        /// Populates this <see cref="Condition"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the condition data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> is <b> null </b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // name
            _name = new CodableValue();
            _name.ParseXml(navigator.SelectSingleNode("name"));

            // onset-date
            _onsetDate =
                XPathHelper.GetOptNavValue<ApproximateDate>(navigator, "onset-date");

            // resolution-date
            _resolutionDate =
                XPathHelper.GetOptNavValue<ApproximateDate>(navigator, "resolution-date");

            // resolution
            if (navigator.SelectSingleNode("resolution") != null)
            {
                _resolution = navigator.SelectSingleNode("resolution").Value;
            }

            // occurrence
            _occurrence =
                XPathHelper.GetOptNavValue<CodableValue>(navigator, "occurrence");

            // severity
            _severity =
                XPathHelper.GetOptNavValue<CodableValue>(navigator, "severity");
        }

        /// <summary>
        /// Writes the condition data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the condition item.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the condition data to.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> is <b> null </b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b> null </b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="Name"/> is <b> null </b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_name, Resources.ConditionEntryNameNotSet);

            // <condition>
            writer.WriteStartElement(nodeName);

            // name
            _name.WriteXml("name", writer);

            // onset-date
            XmlWriterHelper.WriteOpt(
                writer,
                "onset-date",
                _onsetDate);

            // resolution-date
            XmlWriterHelper.WriteOpt(
                writer,
                "resolution-date",
                _resolutionDate);

            // resolution
            XmlWriterHelper.WriteOptString(
                writer,
                "resolution",
                _resolution);

            // occurrence
            XmlWriterHelper.WriteOpt(
                writer,
                "occurrence",
                _occurrence);

            // severity
            XmlWriterHelper.WriteOpt(
                writer,
                "severity",
                _severity);

            // </conditon>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name or description of a medical condition entry.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is <b>null</b>.
        /// </exception>
        ///
        public CodableValue Name
        {
            get { return _name; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Name), Resources.ConditionEntryNameMandatory);
                _name = value;
            }
        }

        private CodableValue _name;

        /// <summary>
        /// Gets or sets the date of onset or the first diagnosis.
        /// </summary>
        ///
        /// <remarks>
        /// The onset date should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public ApproximateDate OnsetDate
        {
            get { return _onsetDate; }
            set { _onsetDate = value; }
        }

        private ApproximateDate _onsetDate;

        /// <summary>
        /// Gets or sets the date the condition resolved (or for multiple acute
        /// episodes, the last date the condition resolved).
        /// </summary>
        ///
        /// <remarks>
        /// The resolution date should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public ApproximateDate ResolutionDate
        {
            get { return _resolutionDate; }
            set { _resolutionDate = value; }
        }

        private ApproximateDate _resolutionDate;

        /// <summary>
        /// Gets or sets the resolution which is a statement of how the condition
        /// was resolved.
        /// </summary>
        ///
        /// <remarks>
        /// The resolution of a condition should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Resolution
        {
            get { return _resolution; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Resolution");
                _resolution = value;
            }
        }

        private string _resolution;

        /// <summary>
        /// Gets or sets the description of how often the condition occurs.
        /// </summary>
        ///
        /// <remarks>
        /// Examples of occurrence include acute, chronic. The occurrence of condition
        /// should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public CodableValue Occurrence
        {
            get { return _occurrence; }
            set { _occurrence = value; }
        }

        private CodableValue _occurrence;

        /// <summary>
        /// Gets or sets the severity of a condition.
        /// </summary>
        ///
        /// <remarks>
        /// The severity should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public CodableValue Severity
        {
            get { return _severity; }
            set { _severity = value; }
        }

        private CodableValue _severity;

        /// <summary>
        /// Gets a string of the name or description of the condition item.
        /// </summary>
        ///
        public override string ToString()
        {
            return _name.ToString();
        }
    }
}
