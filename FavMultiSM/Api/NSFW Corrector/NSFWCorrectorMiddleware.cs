using FavMultiSM.Models.ApiModels;
using FavMultiSM.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Api.NSFW_Corrector
{
    public class NSFWCorrectorMiddleware : ISingletoneService
    {
        private readonly SimpleRectCorrector simpleRectCorrector;
        private readonly NSFWDeepAICorrector nSFWDeepAICorrector;

        public NSFWCorrectorMiddleware(SimpleRectCorrector simpleRectCorrector, NSFWDeepAICorrector nSFWDeepAICorrector)
        {
            this.simpleRectCorrector = simpleRectCorrector;
            this.nSFWDeepAICorrector = nSFWDeepAICorrector;
        }
        public async Task<IEnumerable<Attachment>> GetAttachmentsAsync(IEnumerable<Attachment> attachments)
        {
            List<Attachment> returnAttachmets = new List<Attachment>();
            foreach(Attachment attachment in attachments)
            {
                var result = await nSFWDeepAICorrector.HasNSFW(attachment);
                if(result!=null)
                {
                    returnAttachmets.Add(await simpleRectCorrector.GetCorrrectedAttachments(attachment, result));
                }
                else
                {
                    returnAttachmets.Add(attachment);
                }
            }
            return returnAttachmets;
        }
    }
}
