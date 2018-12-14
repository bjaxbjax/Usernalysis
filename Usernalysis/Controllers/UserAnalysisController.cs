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
            var ret = new StringBuilder();
            foreach (var result in results)
            {
                var user = result.ToObject<UserModel>();
                ret.Append($"{user.Name.Title}. {user.Name.First} {user.Name.Last} - {user.Gender} - {user.Location.State} - {user.Dob.Date}");
            }
            return ret.ToString();
        }

    }
}
