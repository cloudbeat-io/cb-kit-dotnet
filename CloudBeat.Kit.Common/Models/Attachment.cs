using System;
namespace CloudBeat.Kit.Common.Models
{
	public class Attachment
	{
        public Attachment()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public AttachmentTypeEnum Type { get; set; }
        public AttachmentSubTypeEnum Subtype { get; set; }
    }
}

