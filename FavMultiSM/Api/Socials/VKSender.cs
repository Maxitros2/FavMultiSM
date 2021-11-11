using FavMultiSM.Models;
using FavMultiSM.Models.ApiModels;
using FavMultiSM.Registration;
using System;
using System.Collections.Generic;
using System.IO;
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
                foreach (var attachment in message.Attachments)
                {
                    switch (attachment.Type)
                    {
                        case AttachmentType.LocalPhoto:
                            {
                                var response = await UploadFile(uploadServer.UploadUrl, attachment.Url, ".jpeg", false);
                                var vkAttachment = await VkApi.Photo.SaveMessagesPhotoAsync(response);
                                attachmentsList.Add(vkAttachment.First());                                
                            }
                            break;
                        case AttachmentType.WebPhoto:
                            {
                                var response = await UploadFile(uploadServer.UploadUrl, attachment.Url, ".jpg", true);
                                var vkAttachment = await VkApi.Photo.SaveMessagesPhotoAsync(response);
                                attachmentsList.Add(vkAttachment.First());
                            }
                            break;
                    }                    
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
        private async Task<byte[]> GetBytesExternal(string fileUrl)
        {
            using (var webClient = new WebClient())
            {
                return await webClient.DownloadDataTaskAsync(fileUrl);
            }
        }
        private async Task<byte[]> GetBytes(string filePath)
        {
            return await File.ReadAllBytesAsync(filePath);
        }
        private async Task<string> UploadFile(string serverUrl, string file, string fileExtension, bool isExternal)
        {
            // Получение массива байтов из файла
            var data = isExternal?await GetBytesExternal(file):await GetBytes(file);

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
