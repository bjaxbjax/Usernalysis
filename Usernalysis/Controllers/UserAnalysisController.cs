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
        private enum FileFormat { Text, Json, Xml };
        private const FileFormat DEFAULT_FILE_FORMAT = FileFormat.Text;

        [HttpGet]
        public string Get()
        {
            FileFormat format = DetermineFileFormat();

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
            foreach (var result in results)
            {
                var user = result.ToObject<UserModel>();
                users.Add(user);
            }

            var model = Analyze(users);

            var output = ToPlaintext(model);

            return output;
        }

        private FileFormat DetermineFileFormat()
        {
            // Give priority to "Accept:" header value
            if (Request.Headers.Keys.Contains("Accept"))
            {
                foreach(var acceptEntry in Request.Headers["Accept"])
                {
                    var accepts = acceptEntry.Split(',');
                    foreach(var accept in accepts)
                    {
                        switch (accept.Trim().ToLower())
                        {
                            case "text/plain":
                                return FileFormat.Text;
                            case "application/json":
                                return FileFormat.Json;
                            case "application/xml":
                                return FileFormat.Xml;
                            default:
                                break;
                        }
                    }
                }
            }

            if (Request.Query.ContainsKey("format"))
            {
                foreach(var queryStringEntry in Request.Query["format"])
                {
                    switch (queryStringEntry.Trim().ToLower())
                    {
                        case "text":
                        case "txt":
                            return FileFormat.Text;
                        case "json":
                            return FileFormat.Json;
                        case "xml":
                            return FileFormat.Xml;
                        default:
                            break;
                    }
                }
            }

            return DEFAULT_FILE_FORMAT;
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

        private string ToPlaintext(UserAnalysisModel analysis)
        {
            var output = new StringBuilder();
            output.AppendLine($"Percentage female versus male: {analysis.Gender.PercentFemale:F1}%");
            output.AppendLine($"Percentage of first names that start with A-M [{analysis.FirstName.PercentAtoM:F1}%] versus N-Z [{analysis.FirstName.PercentNtoZ:F1}%]");
            output.AppendLine($"Percentage of last names that start with A-M [{analysis.LastName.PercentAtoM:F1}%] versus N-Z [{analysis.LastName.PercentNtoZ:F1}%]");
            var top10 = GetTopSortedFromDictionary<string, decimal>(analysis.StatePercentages);
            output.AppendLine("10 most populous states and the percentage of people in each state:");
            foreach (var state in top10)
            {
                output.AppendLine($"\t{state.Key} {state.Value * 100:F1}%");
            }
            output.AppendLine("10 most populous female states and the percentage of females in each state:");
            top10 = GetTopSortedFromDictionary<string, decimal>(analysis.FemaleStatePercentages);
            foreach (var state in top10)
            {
                output.AppendLine($"\t{state.Key} {state.Value * 100:F1}%");
            }
            output.AppendLine("10 most populous male states and the percentage of males in each state:");
            top10 = GetTopSortedFromDictionary<string, decimal>(analysis.MaleStatePercentages);
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
