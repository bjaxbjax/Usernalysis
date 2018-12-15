using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Usernalysis.Models;
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
            //analysisOutput.AppendLine();

            var model = Analyze(users);
            analysisOutput.AppendLine($"Percentage female versus male: {model.Gender.PercentFemale:F1}%");
            analysisOutput.AppendLine($"Percentage of first names that start with A-M [{model.FirstName.PercentAtoM:F1}%] versus N-Z [{model.FirstName.PercentNtoZ:F1}%]");
            analysisOutput.AppendLine($"Percentage of last names that start with A-M [{model.LastName.PercentAtoM:F1}%] versus N-Z [{model.LastName.PercentNtoZ:F1}%]");
            var top10 = GetTopSortedFromDictionary<string, decimal>(model.StatePercentages);
            analysisOutput.AppendLine("10 most populous states and the percentage of people in the state:");
            foreach (var state in top10)
            {
                analysisOutput.AppendLine($"\t{state.Key} {state.Value * 100:F1}%");
            }
            analysisOutput.AppendLine("10 most populous female states and the percentage of total females in the state:");
            top10 = GetTopSortedFromDictionary<string, decimal>(model.FemaleStatePercentages);
            foreach (var state in top10)
            {
                analysisOutput.AppendLine($"\t{state.Key} {state.Value * 100:F1}%");
            }
            analysisOutput.AppendLine("10 most populous male states and the percentage of total males in the state:");
            top10 = GetTopSortedFromDictionary<string, decimal>(model.MaleStatePercentages);
            foreach (var state in top10)
            {
                analysisOutput.AppendLine($"\t{state.Key} {state.Value * 100:F1}%");
            }
            analysisOutput.AppendLine("Percentage of people in the following age ranges:");
            foreach (var ageRangePercentage in model.AgeRangePercentages)
            {
                analysisOutput.AppendLine($"\t{ageRangePercentage.Key} {ageRangePercentage.Value * 100:F1}%");
            }

            return analysisOutput.ToString();
        }

        private UserAnalysisModel Analyze(IList<UserModel> users)
        {
            UserAnalysisModel model = new UserAnalysisModel();

            var femalePercentage = Calculators.PercentageFemale(users) * 100;
            var malePercentage = 100 - femalePercentage;
            model.Gender = (femalePercentage, malePercentage);

            var firstNameLeftPercentage = Calculators.PercentageFirstNameMidpoint(users) * 100;
            var firstNameRightPercentage = 100 - firstNameLeftPercentage;
            model.FirstName = (firstNameLeftPercentage, firstNameRightPercentage);

            var lastNameLeftPercentage = Calculators.PercentageLastNameMidpoint(users) * 100;
            var lastNameRightPercentage = 100 - lastNameLeftPercentage;
            model.LastName = (lastNameLeftPercentage, lastNameRightPercentage);

            model.StatePercentages = Calculators.PercentagePeopleInState(users);
            model.FemaleStatePercentages = Calculators.PercentageFemalesInState(users);
            model.MaleStatePercentages = Calculators.PercentageMalesInState(users);
            model.AgeRangePercentages = Calculators.PercentageAgeRanges(users);

            return model;
        }

        private IList<KeyValuePair<T, U>> GetTopSortedFromDictionary<T, U>(IDictionary<T, U> dictionary, int topN = 10)
        {
            var result = new List<KeyValuePair<T, U>>();
            var query = (from entry in dictionary
                         orderby entry.Value descending
                         select entry).Take(topN);
            foreach (var entry in query)
            {
                result.Add(new KeyValuePair<T, U>(entry.Key, entry.Value));
            }
            return result;
        }
    }
}
