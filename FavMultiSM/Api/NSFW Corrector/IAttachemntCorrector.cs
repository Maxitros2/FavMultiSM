using FavMultiSM.Models.ApiModels;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Api.NSFW_Corrector
{
    public interface IAttachemntCorrector
    {
        public Task<Attachment> GetCorrrectedAttachments(Attachment attachment, Rectangle[] rectangles);
    }
}
