using FavMultiSM.Api.Instagram;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Controllers
{
    [Route("api/instagram/callback")]
    public class InstagramController : ControllerBase
    {
        public InstagramController(IConfiguration config, InstagramApi instagramApi)
        {
            Config = config;
            InstagramApi = instagramApi;
        }

        IConfiguration Config { get; }
        InstagramApi InstagramApi { get; }

        [HttpPost]
        [Route("[controller]/setcode")]
        public IActionResult SetInstagramCode(string code, string token)
        {
            if(Config["savecode"]==token)
            {
                if(String.IsNullOrEmpty(code))
                    return BadRequest();
                InstagramApi.Code = code;
                return Ok();
            }
            else
            {
                return Unauthorized();
            }            
        }
    }
}
