using InstagramApiSharp.Logger;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FavMultiSM.Api.Instagram
{
    public class InstagramAspLogger : IInstaLogger
    {
        private readonly ILogger logger;

        public InstagramAspLogger(ILogger logger)
        {
            this.logger = logger;
        }
        public void LogException(Exception exception)
        {
            logger.LogError(exception.Message);
        }

        public void LogInfo(string info)
        {
            logger.LogInformation(info);
        }

        public void LogRequest(HttpRequestMessage request)
        {
            logger.LogInformation(request.ToString());
        }

        public void LogRequest(Uri uri)
        {
            logger.LogInformation(uri.ToString());
        }

        public void LogResponse(HttpResponseMessage response)
        {
            logger.LogInformation(response.Content.ToString());
        }
    }
}
