using DeepAI;
using FavMultiSM.Models.ApiModels;
using FavMultiSM.Registration;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Api.NSFW_Corrector
{
    public class NSFWDeepAICorrector : INSFWCorrector, ISingletoneService
    {
        public NSFWDeepAICorrector()
        {
            api = new DeepAI_API(apiKey: "a14b67e7-ab39-40e6-98ec-e4adfea8b4ea");
        }
        DeepAI_API api { get; }

        public async Task<Rectangle[]> HasNSFW(Attachment attachment)
        {
            StandardApiResponse resp;
            List<Rectangle> rectangles = new List<Rectangle>();
            switch (attachment.Type)
            {
                case AttachmentType.LocalPhoto:
                    break;
                case AttachmentType.WebPhoto:
                    await Task.Run(() =>
                    {
                        resp = api.callStandardApi("nsfw-detector", new
                        {
                            image = attachment.Url,
                        });
                        foreach (var detection in JObject.Parse(api.objectAsJsonString(resp)).SelectToken("output.detections").Children())
                        {
                            if (detection.Value<double>("confidence") >= 0.5)
                            {
                                int[] recArray = (detection.SelectToken("bounding_box") as JArray).ToObject<int[]>();
                                rectangles.Add(new Rectangle(recArray[0], recArray[1], recArray[2], recArray[3]));
                            }
                        }                       
                    });
                    break;
            }
            if (rectangles.Count > 0)
                return rectangles.ToArray();
            return null;
        }
    }
}
