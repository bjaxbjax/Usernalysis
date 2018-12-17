using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Usernalysis.Models;

namespace Usernalysis.Lib
{
    public class Serializers
    {
        public static string ToPlaintext(UserAnalysisModel analysis)
        {
            var output = new StringBuilder();
            output.AppendLine($"Percentage female versus male: {FormatPercentage(analysis.Gender.PercentFemale)}%");
            output.AppendLine($"Percentage of first names that start with A-M [{FormatPercentage(analysis.FirstName.PercentAtoM)}%] versus N-Z [{FormatPercentage(analysis.FirstName.PercentNtoZ)}%]");
            output.AppendLine($"Percentage of last names that start with A-M [{FormatPercentage(analysis.LastName.PercentAtoM)}%] versus N-Z [{FormatPercentage(analysis.LastName.PercentNtoZ)}%]");
            var top10 = Utilities.GetTopSortedFromDictionary<string, decimal>(analysis.StatePercentages);
            output.AppendLine("10 most populous states and the percentage of people in each state:");
            foreach (var state in top10)
            {
                output.AppendLine($"\t{state.Key} {FormatPercentage(state.Value)}%");
            }
            output.AppendLine("10 most populous female states and the percentage of females in each state:");
            top10 = Utilities.GetTopSortedFromDictionary<string, decimal>(analysis.FemaleStatePercentages);
            foreach (var state in top10)
            {
                output.AppendLine($"\t{state.Key} {FormatPercentage(state.Value)}%");
            }
            output.AppendLine("10 most populous male states and the percentage of males in each state:");
            top10 = Utilities.GetTopSortedFromDictionary<string, decimal>(analysis.MaleStatePercentages);
            foreach (var state in top10)
            {
                output.AppendLine($"\t{state.Key} {FormatPercentage(state.Value)}%");
            }
            output.AppendLine("Percentage of people in the following age ranges:");
            foreach (var ageRangePercentage in analysis.AgeRangePercentages)
            {
                output.AppendLine($"\t{ageRangePercentage.Key}: {FormatPercentage(ageRangePercentage.Value)}%");
            }
            return output.ToString();
        }

        public static string ToJson(UserAnalysisModel analysis)
        {
            var json = string.Empty;
            var serializer = new JsonSerializer();
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                jsonWriter.WriteStartObject();

                jsonWriter.WritePropertyName("gender-pct");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("female");
                jsonWriter.WriteValue(DecimalToPercent(analysis.Gender.PercentFemale));
                jsonWriter.WritePropertyName("male");
                jsonWriter.WriteValue(DecimalToPercent(analysis.Gender.PercentMale));
                jsonWriter.WriteEndObject();

                jsonWriter.WritePropertyName("name-initial-pct");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("first-name");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("a-m");
                jsonWriter.WriteValue(DecimalToPercent(analysis.FirstName.PercentAtoM));
                jsonWriter.WritePropertyName("n-z");
                jsonWriter.WriteValue(DecimalToPercent(analysis.FirstName.PercentNtoZ));
                jsonWriter.WriteEndObject();
                jsonWriter.WritePropertyName("last-name");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("a-m");
                jsonWriter.WriteValue(DecimalToPercent(analysis.LastName.PercentAtoM));
                jsonWriter.WritePropertyName("n-z");
                jsonWriter.WriteValue(DecimalToPercent(analysis.LastName.PercentNtoZ));
                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();

                jsonWriter.WritePropertyName("state-population-top-10");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("total");
                jsonWriter.WriteStartArray();
                var top10 = Utilities.GetTopSortedFromDictionary<string, decimal>(analysis.StatePercentages);
                foreach (var state in top10)
                {
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName("state");
                    jsonWriter.WriteValue(state.Key);
                    jsonWriter.WritePropertyName("pct");
                    jsonWriter.WriteValue(DecimalToPercent(state.Value));
                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteEndArray();
                jsonWriter.WritePropertyName("female");
                jsonWriter.WriteStartArray();
                top10 = Utilities.GetTopSortedFromDictionary<string, decimal>(analysis.FemaleStatePercentages);
                foreach (var state in top10)
                {
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName("state");
                    jsonWriter.WriteValue(state.Key);
                    jsonWriter.WritePropertyName("pct");
                    jsonWriter.WriteValue(DecimalToPercent(state.Value));
                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteEndArray();
                jsonWriter.WritePropertyName("male");
                jsonWriter.WriteStartArray();
                top10 = Utilities.GetTopSortedFromDictionary<string, decimal>(analysis.MaleStatePercentages);
                foreach (var state in top10)
                {
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName("state");
                    jsonWriter.WriteValue(state.Key);
                    jsonWriter.WritePropertyName("pct");
                    jsonWriter.WriteValue(DecimalToPercent(state.Value));
                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteEndArray();
                jsonWriter.WriteEndObject();

                jsonWriter.WritePropertyName("age-ranges");
                jsonWriter.WriteStartArray();
                foreach (var ageRangePercentage in analysis.AgeRangePercentages)
                {
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName("age");
                    jsonWriter.WriteValue(ageRangePercentage.Key);
                    jsonWriter.WritePropertyName("pct");
                    jsonWriter.WriteValue(DecimalToPercent(ageRangePercentage.Value));
                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteEndArray();

                jsonWriter.WriteEndObject();
                json = stringWriter.ToString();
            }
            return json;
        }

        public static string ToXml(UserAnalysisModel analysis)
        {
            var xml = string.Empty;
            using (var stringWriter = new StringWriter())
            {
                var xmlWriter = XmlWriter.Create(stringWriter);
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("useranalysis");

                xmlWriter.WriteStartElement("gender-pct");
                xmlWriter.WriteStartElement("female");
                xmlWriter.WriteString($"{FormatPercentage(analysis.Gender.PercentFemale)}");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("male");
                xmlWriter.WriteString($"{FormatPercentage(analysis.Gender.PercentMale)}");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("name-initial-pct");
                xmlWriter.WriteStartElement("first-name");
                xmlWriter.WriteStartElement("a-m");
                xmlWriter.WriteString($"{FormatPercentage(analysis.FirstName.PercentAtoM)}");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("n-z");
                xmlWriter.WriteString($"{FormatPercentage(analysis.FirstName.PercentNtoZ)}");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("last-name");
                xmlWriter.WriteStartElement("a-m");
                xmlWriter.WriteString($"{FormatPercentage(analysis.LastName.PercentAtoM)}");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("n-z");
                xmlWriter.WriteString($"{FormatPercentage(analysis.LastName.PercentNtoZ)}");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("state-population-top-10");
                xmlWriter.WriteStartElement("total");
                var top10 = Utilities.GetTopSortedFromDictionary<string, decimal>(analysis.StatePercentages);
                foreach (var state in top10)
                {
                    xmlWriter.WriteStartElement("state");
                    xmlWriter.WriteStartElement("name");
                    xmlWriter.WriteString(state.Key);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("pct");
                    xmlWriter.WriteString($"{FormatPercentage(state.Value)}");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("female");
                top10 = Utilities.GetTopSortedFromDictionary<string, decimal>(analysis.FemaleStatePercentages);
                foreach (var state in top10)
                {
                    xmlWriter.WriteStartElement("state");
                    xmlWriter.WriteStartElement("name");
                    xmlWriter.WriteString(state.Key);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("pct");
                    xmlWriter.WriteString($"{FormatPercentage(state.Value)}");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("male");
                top10 = Utilities.GetTopSortedFromDictionary<string, decimal>(analysis.MaleStatePercentages);
                foreach (var state in top10)
                {
                    xmlWriter.WriteStartElement("state");
                    xmlWriter.WriteStartElement("name");
                    xmlWriter.WriteString(state.Key);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("pct");
                    xmlWriter.WriteString($"{FormatPercentage(state.Value)}");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("age-ranges");
                foreach (var ageRangePercentage in analysis.AgeRangePercentages)
                {
                    xmlWriter.WriteStartElement($"age");
                    xmlWriter.WriteAttributeString("range", ageRangePercentage.Key);
                    xmlWriter.WriteString($"{FormatPercentage(ageRangePercentage.Value)}");
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndDocument();
                xmlWriter.Close();

                xml = stringWriter.ToString();
            }
            return xml;
        }

        private static string FormatPercentage(decimal pct)
        {
            var percentage_format = "{0:0.0#}";
            return string.Format(percentage_format, pct * 100);
        }

        private static decimal DecimalToPercent(decimal dec)
        {
            return dec * 100;
        }
    }
}
