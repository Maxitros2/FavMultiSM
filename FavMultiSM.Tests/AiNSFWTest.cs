using FavMultiSM.Api.NSFW_Corrector;
using FavMultiSM.Models.ApiModels;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FavMultiSM.Tests
{    
    
    public class AiNSFWTest
    {
        readonly Attachment NSWFImage = new Attachment("https://sun9-7.userapi.com/impg/6MqIpT1VenqG9UccoO8n8n8xAnawfKh_tUH-8Q/62RsdNPy5cM.jpg?size=687x1280&quality=96&sign=1f2371e9480d479ca453b728222ddb62&type=album");
        readonly Rectangle[] result = new Rectangle[] { new Rectangle(143, 687, 139, 181), new Rectangle(314, 673, 131, 187) };        
        [Fact]
        public async void TestNSFWDeepAI()
        {
            NSFWDeepAICorrector nSFWDeepAICorrector = new NSFWDeepAICorrector();
            var resultAi = await nSFWDeepAICorrector.HasNSFW(NSWFImage);
            Assert.Equal(new Rectangle[] { new Rectangle(143, 687, 139, 181), new Rectangle(314, 673, 131, 187) }, resultAi);
        }
        [Fact]
        public async void TestSimpleRectCensore()
        {
            SimpleRectCorrector simpleRectCorrector = new SimpleRectCorrector();
            var attach = await simpleRectCorrector.GetCorrrectedAttachments(NSWFImage, result);
            Assert.Equal(AttachmentType.LocalPhoto, attach.Type);
            Assert.True(File.Exists(attach.Url));

        }
    }
}
        

