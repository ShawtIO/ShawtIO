using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Links.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UAParser;

namespace Links.Controllers
{
    [Route("r")]
    [ApiController]
    [AllowAnonymous]
    public class RedirectController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RedirectController(IDataRepository dataRepository, IHttpContextAccessor httpContextAccessor)
        {
            _dataRepository = dataRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("{url}", Name = "RedirectToLink")]
        public IActionResult Get(string url)
        {
            int id = ShortUrl.Decode(url);
            string originalUrl = _dataRepository.GetLink(id);
            if (string.IsNullOrEmpty(originalUrl))
                return NotFound();
            else
            {
                //TODO: Log stats before redirecting                     
                string ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                ipAddress = ipAddress == "::1" ? _httpContextAccessor.HttpContext.Connection.LocalIpAddress.ToString() : ipAddress;
                string userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"];
                //TODO: Get IP address, user agent, browser, OS etc.                
                (string browser, string os, string device) = GetUserAgentDetails(userAgent);
                _dataRepository.UpdateAccessStats(id, ipAddress, DateTime.UtcNow, userAgent, browser, os, device);
                originalUrl = !originalUrl.ToUpper().StartsWith("HTTP") ? $"http://{originalUrl}" : originalUrl; //DevSkim: ignore DS137138
                return Redirect(originalUrl);
            }
        }

        private (string, string, string) GetUserAgentDetails(string userAgent)
        {            
            var uaParser = Parser.GetDefault();
            ClientInfo clientInfo = uaParser.Parse(userAgent);
            var browser = $"{clientInfo.UserAgent.ToString()}";
            var os = clientInfo.OS.ToString();
            var device = clientInfo.Device.Family;
            return (browser, os, device);
        }
    }
}