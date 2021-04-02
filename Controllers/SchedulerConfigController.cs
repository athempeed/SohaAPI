using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Intuit.Ipp.Diagnostics;
using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebApplication10.Model;
using WebApplication10.Model.Interfaces;
using WebApplication10.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication10.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulerConfigController : ControllerBase
    {

        private IAppSettings _config;
        private ILogger<SchedulerConfigController> _logger;

        string path = Path.Combine(Model.Constants.RootPath, "response", "response.json");
        public SchedulerConfigController(ILogger<SchedulerConfigController> logger, IAppSettings appSettings)
        {
            _config = appSettings;
            _logger = logger;
        }

        // GET: api/<LoginController>
        [HttpGet]
        public string Get()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(path))
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        sb.Append(sr.ReadToEnd());
                    }
                }

                return sb.ToString();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "error occuered");
                
            }
            return String.Empty;
        }

        [HttpGet("Tokens")]
        public string GetTokens()
        {
            try
            {
                var tokenPath = Path.Combine(Model.Constants.RootPath, "response", "QBToken.json");
                var d = Common.ReadJson<QBToken>(tokenPath);

                if (d != null)
                {
                    return JsonConvert.SerializeObject(d);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "error :Tokens");
            }
            

            return string.Empty;
        }


        [HttpPost("Tokens")]
        public bool GetTokens(QBToken callback)
        {
            try
            {
                var tokenPath = Path.Combine(Model.Constants.RootPath, "response", "QBToken.json");
                if (callback != null)
                {
                    Common.SaveJson(tokenPath, JsonConvert.SerializeObject(callback));

                    return true;
                }

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "error opccured");
            }


            return false;
            
        }


        // GET api/<LoginController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }



        [HttpGet("callback")]        
        public async Task<string> Callback()
        {
            try
            {
                _logger.LogInformation("Callback");

                path = Path.Combine(Model.Constants.RootPath, "response", "response.json");
                //var filepath = _appSettings.CallbackConfig.Path;
                var queryString = Request.QueryString.ToString();

                var arr = queryString.Split("&");

                var callback = new CallbackResponse
                {
                    Code = arr[0].Replace("?", "").Replace("code=", ""),
                    State = arr[1].Replace("state=", ""),
                    RealmId = arr[2].Replace("realmId=", "")
                };
                GenerateTokens(callback);
                var data = JsonConvert.SerializeObject(callback);
                
                Common.SaveJson(path, data);
                //Debug.WriteLine(bodyContent);
                return "successully saved code";
            }
            catch(Exception ex)
            {
                _logger.LogError("Error occured:", ex);
                return null;
            }
        }

        public void SaveToStorage(string data)
        {
            BlobService services = new BlobService();
            services.UploadFileToBlobAsync("QBOTokens.json", data).Wait();
        }

        [HttpGet("Realm")]
        public async  Task<string> GetRealmID()
        {

            var jsonData = Common.ReadJson<CallbackResponse>(path);
            if (jsonData != null)
            {

                return JsonConvert.SerializeObject(jsonData);
                
            }

            return "";

        }


        private void GenerateTokens(CallbackResponse callback)
        {
            try
            {

                _logger.LogError($"GenerateTokens:{_config.QbConfig}");
                var oAuth2Client = new OAuth2Client(_config.QbConfig.ClientID, _config.QbConfig.ClientSecret, _config.QbConfig.RedirectUri, _config.QbConfig.Environment);
                var response = oAuth2Client.GetBearerTokenAsync(callback.Code).Result;
                var curDateTime = DateTime.Now;
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("status is ok.");
                    var token = new QBToken
                    {
                        AccessToken = response.AccessToken,
                        RefreshToken = response.RefreshToken,
                        RefreshTokenExpiresIn = curDateTime.AddSeconds(response.RefreshTokenExpiresIn),
                        AccessTokenExpiresIn = curDateTime.AddSeconds(response.AccessTokenExpiresIn)
                    };
                    SaveToStorage(JsonConvert.SerializeObject(token));
                    var tokenPath = Path.Combine(Model.Constants.RootPath, "response", "QBToken.json"); 
                    Common.SaveJson(tokenPath, JsonConvert.SerializeObject(token));
                    
                }
                else
                {
                    _logger.LogError($"Error: {response.Error}, Error Description:{response.ErrorDescription}");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error occured:{JsonConvert.SerializeObject(ex)}",ex);
            }
        }



        




        // POST api/<LoginController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<LoginController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LoginController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
