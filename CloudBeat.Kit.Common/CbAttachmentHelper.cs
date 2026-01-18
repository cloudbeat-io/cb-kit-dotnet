using CloudBeat.Kit.Common.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace CloudBeat.Kit.Common
{
    internal static class CbAttachmentHelper
    {
        public const string CB_ATTACHMENTS_DIR_NAME = ".cb-attachments";

        public static Attachment PrepareScreenRecordingAttachment(string base64VideoData)
        {
            byte[] data = Convert.FromBase64String(base64VideoData);
            var attachment = new Attachment();
            attachment.Type = AttachmentTypeEnum.Video;
            attachment.Subtype = AttachmentSubTypeEnum.Screencast;
            attachment.FileName = $"{Guid.NewGuid()}.mp4";
            attachment.FilePath = GetAttachmentFilePath(attachment.FileName);
            
            try {
                System.IO.File.WriteAllBytes(attachment.FilePath, data);
                return attachment;
            }
            catch {
                return null;
            }
        }

		public static Attachment PrepareScreenshotAttachment(string base64Data)
		{
			byte[] data = Convert.FromBase64String(base64Data);
			var attachment = new Attachment();
			attachment.Type = AttachmentTypeEnum.Screenshot;
			attachment.Subtype = AttachmentSubTypeEnum.Screenshot;
			attachment.FileName = $"{Guid.NewGuid()}.png";
			attachment.FilePath = GetAttachmentFilePath(attachment.FileName);

			try
			{
				System.IO.File.WriteAllBytes(attachment.FilePath, data);
				return attachment;
			}
			catch
			{
				return null;
			}
		}
		
		public static Attachment PreparePageSourceAttachment(string pageSource, string mimeType = "text/plain")
		{
			var attachment = new Attachment
			{
				Type = AttachmentTypeEnum.Snapshot,
				MimeType = mimeType
			};
			if (mimeType == "text/html")
				attachment.Subtype = AttachmentSubTypeEnum.Html;
			else if (mimeType == "text/xml" || mimeType == "application/xml")
				attachment.Subtype = AttachmentSubTypeEnum.Xml;
			else
				attachment.Subtype = AttachmentSubTypeEnum.Text;
			attachment.FileName = $"{Guid.NewGuid()}.png";
			attachment.FilePath = GetAttachmentFilePath(attachment.FileName);

			try
			{
				System.IO.File.WriteAllBytes(attachment.FilePath, Encoding.UTF8.GetBytes(pageSource));
				return attachment;
			}
			catch
			{
				return null;
			}
		}

		public static Attachment PrepareScreenRecordingAttachmentFromPath(string filePath)
        {
            var attachment = new Attachment
            {
	            Type = AttachmentTypeEnum.Video,
	            Subtype = AttachmentSubTypeEnum.Screencast,
	            FileName = Path.GetFileName(filePath),
	            FilePath = filePath
            };
            return attachment;
        }

        public static string GetAttachmentFilePath(string fileName)
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            var cwd = Path.GetDirectoryName(assembly.Location);
            var attachmentsDirPath = Path.Combine(cwd, CB_ATTACHMENTS_DIR_NAME);
            if (!Directory.Exists(attachmentsDirPath))
                Directory.CreateDirectory(attachmentsDirPath);
            return Path.Combine(attachmentsDirPath, fileName);
        }
    }
}
