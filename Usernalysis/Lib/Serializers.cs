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
            output.AppendLine($"Percentage female versus male: {analysis.Gender.PercentFemale:F1}%");
            output.AppendLine($"Percentage of first names that start with A-M [{analysis.FirstName.PercentAtoM:F1}%] versus N-Z [{analysis.FirstName.PercentNtoZ:F1}%]");
            output.AppendLine($"Percentage of last names that start with A-M [{analysis.LastName.PercentAtoM:F1}%] versus N-Z [{analysis.LastName.PercentNtoZ:F1}%]");
            var top10 = Utilities.GetTopSortedFromDictionary<string, decimal>(analysis.StatePercentages);
            output.AppendLine("10 most populous states and the percentage of people in each state:");
            foreach (var state in top10)
            {
                output.AppendLine($"\t{state.Key} {state.Value * 100:F1}%");
            }
            output.AppendLine("10 most populous female states and the percentage of females in each state:");
            top10 = Utilities.GetTopSortedFromDictionary<string, decimal>(analysis.FemaleStatePercentages);
            foreach (var state in top10)
            {
                output.AppendLine($"\t{state.Key} {state.Value * 100:F1}%");
            }
            output.AppendLine("10 most populous male states and the percentage of males in each state:");
            top10 = Utilities.GetTopSortedFromDictionary<string, decimal>(analysis.MaleStatePercentages);
            foreach (var state in top10)
            {
                output.AppendLine($"\t{state.Key} {state.Value * 100:F1}%");
            }
            output.AppendLine("Percentage of people in the following age ranges:");
            foreach (var ageRangePercentage in analysis.AgeRangePercentages)
            {
                output.AppendLine($"\t{ageRangePercentage.Key}: {ageRangePercentage.Value * 100:F1}%");
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
                jsonWriter.WriteValue($"{analysis.Gender.PercentFemale:F1}%");
                jsonWriter.WritePropertyName("male");
                jsonWriter.WriteValue($"{analysis.Gender.PercentMale:F1}%");
                jsonWriter.WriteEndObject();

                jsonWriter.WritePropertyName("name-initial-pct");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("first-name");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("a-m");
                jsonWriter.WriteValue($"{analysis.FirstName.PercentAtoM:F1}%");
                jsonWriter.WritePropertyName("n-z");
                jsonWriter.WriteValue($"{analysis.FirstName.PercentNtoZ:F1}%");
                jsonWriter.WriteEndObject();
                jsonWriter.WritePropertyName("last-name");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("a-m");
                jsonWriter.WriteValue($"{analysis.LastName.PercentAtoM:F1}%");
                jsonWriter.WritePropertyName("n-z");
                jsonWriter.WriteValue($"{analysis.LastName.PercentNtoZ:F1}%");
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
                    jsonWriter.WriteValue($"{state.Value * 100:F1}%");
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
                    jsonWriter.WriteValue($"{state.Value * 100:F1}%");
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
                    jsonWriter.WriteValue($"{state.Value * 100:F1}%");
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
                    jsonWriter.WriteValue($"{ageRangePercentage.Value * 100:F1}%");
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
                xmlWriter.WriteString($"{analysis.Gender.PercentFemale:F1}%");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("male");
                xmlWriter.WriteString($"{analysis.Gender.PercentMale:F1}%");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("name-initial-pct");
                xmlWriter.WriteStartElement("first-name");
                xmlWriter.WriteStartElement("a-m");
                xmlWriter.WriteString($"{analysis.FirstName.PercentAtoM:F1}%");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("n-z");
                xmlWriter.WriteString($"{analysis.FirstName.PercentNtoZ:F1}%");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("last-name");
                xmlWriter.WriteStartElement("a-m");
                xmlWriter.WriteString($"{analysis.LastName.PercentAtoM:F1}%");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("n-z");
                xmlWriter.WriteString($"{analysis.LastName.PercentNtoZ:F1}%");
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
                    xmlWriter.WriteString($"{state.Value * 100:F1}%");
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
                    xmlWriter.WriteString($"{state.Value * 100:F1}%");
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
                    xmlWriter.WriteString($"{state.Value * 100:F1}%");
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
                    xmlWriter.WriteString($"{ageRangePercentage.Value * 100:F1}%");
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndDocument();
                xmlWriter.Close();

                xml = stringWriter.ToString();
            }
            return xml;
        }


    }
}
