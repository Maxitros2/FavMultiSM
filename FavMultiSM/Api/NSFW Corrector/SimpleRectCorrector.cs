using FavMultiSM.Models.ApiModels;
using FavMultiSM.Registration;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;

namespace FavMultiSM.Api.NSFW_Corrector
{
    public class SimpleRectCorrector : IAttachemntCorrector, ISingletoneService
    {
        private readonly string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        public async Task<Attachment> GetCorrrectedAttachments(Attachment attachment, SixLabors.ImageSharp.Rectangle[] rectangles)
        {
            string name = attachment.Url.Split("/").Last().Substring(0,8);
            string filepath = Path.Combine(path, "N"+ name);
            using (var client = new WebClient())
            {
                await client.DownloadFileTaskAsync(new Uri(attachment.Url), filepath);
            }
            using (Image image = await Image.LoadAsync(filepath))
            {
                foreach (var rect in rectangles)
                    image.Mutate(x => x.Fill(SixLabors.ImageSharp.Color.Red, rect));
                await image.SaveAsJpegAsync(name + ".jpeg");
                attachment.Type = AttachmentType.LocalPhoto;
                attachment.Url = name + ".jpeg";
            }           
            await Task.Run(()=>File.Delete(filepath));
            return attachment;
        }
    }
}
