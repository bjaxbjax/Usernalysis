using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    }
}
