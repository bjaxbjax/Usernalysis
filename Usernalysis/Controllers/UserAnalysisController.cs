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
            }
            var calc = Calculators.PercentageFemale(users);
            analysisOutput.AppendLine($"Percentage female versus male: {Calculators.PercentageFemale(users) * 100}%");
            return analysisOutput.ToString();
        }

    }
}
