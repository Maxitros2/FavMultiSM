using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Models.ApiModels
{
    public class Attachment
    {
        public Attachment(string url)
        {
            Url = url;
            Type = AttachmentType.WebPhoto;
        }
        public Attachment(string url, AttachmentType type)
        {
            Url = url;
            Type = type;
        }
        public AttachmentType Type { get; set; }
        public string Url { get; set; }
    }
    public enum AttachmentType
    {
        LocalPhoto,
        WebPhoto
    }
}
