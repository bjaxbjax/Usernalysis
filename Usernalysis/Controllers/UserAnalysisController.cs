﻿using System;
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
        public ContentResult Get()
        {
            var result = new ContentResult();
            UserAnalysisModel model = null;
            try
            {
                var users = GetJsonCollectionFromRequest<UserModel>(Request, "results");
                model = Analyze(users);
            }
            catch (Exception ex)
            {
                result.Content = $"ERROR: {ex.Message}";
                return result;
            }


            switch (DetermineFileFormat(Request))
            {
                case FileFormat.Json:
                    Response.ContentType = "application/json";
                    result.Content = Serializers.ToJson(model);
                    break;
                case FileFormat.Xml:
                    Response.ContentType = "application/xml";
                    result.Content = Serializers.ToXml(model);
                    break;
                case FileFormat.Text:
                default:
                    result.ContentType = "text/plain";
                    result.Content = Serializers.ToPlaintext(model);
                    break;
            }

            return result;
        }

        private IList<T> GetJsonCollectionFromRequest<T>(HttpRequest request, string key)
        {
            var collection = new List<T>();
            var bodyStr = new StreamReader(request.Body).ReadToEnd();
            if (string.IsNullOrEmpty(bodyStr))
            {
                throw new ApplicationException("Json required in request body.");
            }
            try
            {
                var json = JObject.Parse(bodyStr);
                if (!json.ContainsKey(key))
                {
                    throw new ApplicationException($"Missing '{key}' key in json.");
                }
                var results = json[key].Children();
                foreach (var result in results)
                {
                    var user = result.ToObject<T>();
                    collection.Add(user);
                }
            }
            catch (JsonReaderException ex)
            {
                throw new ApplicationException("Invalid json in request body.", ex);
            }
            return collection;
        }

        private FileFormat DetermineFileFormat(HttpRequest request)
        {
            // Give priority to "Accept:" header value
            if (request.Headers.Keys.Contains("Accept"))
            {
                foreach(var acceptEntry in request.Headers["Accept"])
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
