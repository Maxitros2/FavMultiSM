using FavMultiSM.Models;
using FavMultiSM.Models.ApiModels;
using FavMultiSM.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VkNet.Abstractions;
using VkNet.Model.Attachments;

namespace FavMultiSM.Api.Socials
{
    public class VKSender : ISender, ITrancientService
    {
        public VKSender(IVkApi vkApi)
        {
            VkApi = vkApi;
        }

        IVkApi VkApi { get; }

        public async Task<bool> SendMessage(ReSendMessage message, UserAccountData data)
        {
            var vkMessage = new VkNet.Model.RequestParams.MessagesSendParams()
            {
                PeerId = Convert.ToInt64(data.VKId),
                RandomId = new DateTime().Millisecond
            };
            if (message.Text != null)
                vkMessage.Message = message.Text;
            else
                vkMessage.Message = " ";
            if (message.Attachments != null)
            {
                var uploadServer = await VkApi.Photo.GetMessagesUploadServerAsync(Convert.ToInt64(data.VKId));
                var attachmentsList = new List<MediaAttachment>();
                foreach (var photo in message.Attachments)
                {
                    var response = await UploadFile(uploadServer.UploadUrl, photo, photo.Split(".").Last());
                    var attachment =  await VkApi.Photo.SaveMessagesPhotoAsync(response);
                    attachmentsList.Add(attachment.First());
                }
                vkMessage.Attachments = attachmentsList;
            }             
            await VkApi.Messages.SendAsync(vkMessage);
            return true;
        }

        public Task<bool> SendMessage(ReSendMessage message, UserAccountData data, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }
        private async Task<byte[]> GetBytes(string fileUrl)
        {
            using (var webClient = new WebClient())
            {
                return await webClient.DownloadDataTaskAsync(fileUrl);
            }
        }
        private async Task<string> UploadFile(string serverUrl, string file, string fileExtension)
        {
            // Получение массива байтов из файла
            var data = await GetBytes(file);

            // Создание запроса на загрузку файла на сервер
            using (var client = new HttpClient())
            {
                var requestContent = new MultipartFormDataContent();
                var content = new ByteArrayContent(data);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                requestContent.Add(content, "file", $"file.{fileExtension}");

                var response = client.PostAsync(serverUrl, requestContent).Result;
                return Encoding.Default.GetString(await response.Content.ReadAsByteArrayAsync());
            }
        }
    }
}
