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

            var output = Serializers.ToPlaintext(model);

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

    }
}
