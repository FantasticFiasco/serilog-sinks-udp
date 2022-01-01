// Copyright 2015-2022 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.IO;
using System.Text;

namespace Serilog.Sinks.Udp.Private
{
    /// <remarks>
    /// The methods in this class where influenced by
    /// https://weblog.west-wind.com/posts/2018/Nov/30/Returning-an-XML-Encoded-String-in-NET.
    /// </remarks>
    internal class XmlSerializer
    {
        private const char LtCharacter = '<';
        private const char GtCharacter = '>';
        private const char AmpCharacter = '&';
        private const char QuotCharacter = '\"';
        private const char AposCharacter = '\'';
        private const char LfCharacter = '\n';
        private const char CrCharacter = '\r';
        private const char TabCharacter = '\t';

        private const string LfString = "\n";
        private const string CrString = "\r";
        private const string TabString = "\t";

        private const string SerializedLt = "&lt;";
        private const string SerializedGt = "&gt;";
        private const string SerializedAmp = "&amp;";
        private const string SerializedQuot = "&quot;";
        private const string SerializedApos = "&apos;";
        private const string SerializedLf = "&#xA;";
        private const string SerializedCr = "&#xD;";
        private const string SerializedTab = "&#x9;";

        internal void SerializeXmlValue(TextWriter output, string text, bool isAttribute)
        {
            foreach (var character in text)
            {
                output.Write(SerializeXmlValue(character, isAttribute));
            }
        }

        internal string SerializeXmlValue(string text, bool isAttribute)
        {
            var builder = new StringBuilder();

            foreach (var character in text)
            {
                builder.Append(SerializeXmlValue(character, isAttribute));
            }

            return builder.ToString();
        }

        private static string SerializeXmlValue(char character, bool isAttribute)
        {
            if (character == LtCharacter)
            {
                return SerializedLt;
            }

            if (character == GtCharacter)
            {
                return SerializedGt;
            }

            if (character == AmpCharacter)
            {
                return SerializedAmp;
            }

            // Special handling for quotes
            if (isAttribute && character == QuotCharacter)
            {
                return SerializedQuot;
            }

            if (isAttribute && character == AposCharacter)
            {
                return SerializedApos;
            }

            // Legal sub-chr32 characters
            if (character == LfCharacter)
            {
                return isAttribute ? SerializedLf : LfString;
            }

            if (character == CrCharacter)
            {
                return isAttribute ? SerializedCr : CrString;
            }

            if (character == TabCharacter)
            {
                return isAttribute ? SerializedTab : TabString;
            }

            return character.ToString();
        }
    }
}
