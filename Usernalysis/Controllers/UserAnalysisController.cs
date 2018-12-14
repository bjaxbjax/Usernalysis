using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Usernalysis.Models.User;
using Usernalysis.Lib;

namespace Usernalysis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAnalysisController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            string json = new StreamReader(Request.Body).ReadToEnd();
            if (string.IsNullOrEmpty(json))
            {
                return "ERROR: Json required.";
            }
            JObject parse = null;
            try
            {
                parse = JObject.Parse(json);
            }
            catch (Exception)
            {
                return "ERROR: Could not parse json.";
            }
            if(parse["results"] == null)
            {
                return "ERROR: No results found.";
            }

            var results = parse["results"].Children();
            var users = new List<UserModel>();
            var analysisOutput = new StringBuilder();
            foreach (var result in results)
            {
                var user = result.ToObject<UserModel>();
                users.Add(user);
                //analysisOutput.AppendLine($"{user.Name.First} {user.Name.Last} - Dob: {user.Dob.Date} - Age: {user.Dob.Age}");
            }
            analysisOutput.AppendLine();
            var pct = Calculators.PercentageFemale(users);
            analysisOutput.AppendLine($"Percentage female versus male: {pct * 100:F1}%");
            pct = Calculators.PercentageFirstNameMidpoint(users);
            analysisOutput.AppendLine($"Percentage of first names that start with A-M [{pct * 100:F1}%] versus N-Z [{100 - (pct * 100):F1}%]");
            pct = Calculators.PercentageLastNameMidpoint(users);
            analysisOutput.AppendLine($"Percentage of first names that start with A-M [{pct * 100:F1}%] versus N-Z [{100 - (pct * 100):F1}%]");
            var top10Pct = (from statePct in Calculators.PercentagePeopleInState(users)
                            select statePct).Take(10);
            analysisOutput.AppendLine("10 most populous states and the percentage of people in the state:");
            foreach (var state in top10Pct)
            {
                analysisOutput.AppendLine($"\t{state.Key} {state.Value * 100:F1}%");
            }

            analysisOutput.AppendLine("10 most populous female states and the percentage of total females in the state:");
            top10Pct = (from statePct in Calculators.PercentageFemalesInState(users)
                        select statePct).Take(10);
            foreach (var state in top10Pct)
            {
                analysisOutput.AppendLine($"\t{state.Key} {state.Value * 100:F1}%");
            }
            analysisOutput.AppendLine("10 most populous male states and the percentage of total males in the state:");
            top10Pct = (from statePct in Calculators.PercentageMalesInState(users)
                        select statePct).Take(10);
            foreach (var state in top10Pct)
            {
                analysisOutput.AppendLine($"\t{state.Key} {state.Value * 100:F1}%");
            }

            analysisOutput.AppendLine("Percentage of people in the following age ranges:");
            var ageRangePercentages = Calculators.PercentageAgeRanges(users);
            foreach(var ageRangePercentage in ageRangePercentages)
            {
                analysisOutput.AppendLine($"\t{ageRangePercentage.Key} {ageRangePercentage.Value * 100:F1}%");
            }

            return analysisOutput.ToString();
        }

    }
}
