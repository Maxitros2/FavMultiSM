using FavMultiSM.Api.Users;
using FavMultiSM.Models;
using FavMultiSM.Models.ApiModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.Attachments;

namespace FavMultiSM.Controllers
{
    [Route("api/vk/callback")]
    public class VKController : ControllerBase, IReciver
    {
        private readonly IConfiguration configuration;
        private readonly IVkApi vkApi;
        public VKController(IVkApi vkApi, IConfiguration configuration, MessageProceeder messageProceeder)
        {
            this.configuration = configuration;
            MessageProceeder = messageProceeder;
            this.vkApi = vkApi;
        }

        MessageProceeder MessageProceeder { get; }

        [HttpPost]
        public async Task<IActionResult> Callback([FromBody] IncomeVkMessage incomeMessage)
        {
            if(incomeMessage.Type == "message_new")
            {                
                var msg = Message.FromJson(new VkNet.Utils.VkResponse(incomeMessage.Object));
                var resendMsg = new ReSendMessage() { Text = msg.Text };
                if (msg.Attachments != null)
                {
                    var convertedAttachments = new List<Models.ApiModels.Attachment>();
                    foreach (var attach in msg.Attachments)
                    {                        
                        switch(attach.Instance)
                        {
                            case Photo photo: convertedAttachments.Add(new Models.ApiModels.Attachment(photo.PhotoSrc.ToString()));  break;
                        }
                    }
                    resendMsg.Attachments = convertedAttachments;                   
                }
                if (msg.Text == null)
                    resendMsg.Text = "";
                await MessageProceeder.ProceedVkMessage(resendMsg, msg.PeerId.ToString());
            }
            if (incomeMessage.Type == "confirmation")
            {
                return Ok(configuration["vk:testMessage"]);
            }            
            return Ok("ok");
        }        
    }
}
