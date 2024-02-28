using CloudBeat.Kit.Common.Models;
using System;
using System.IO;
using System.Reflection;

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
            attachment.Subtype = AttachmentSubTypeEnum.ScreenRecording;
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
			attachment.Subtype = AttachmentSubTypeEnum.ScreenShot;
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

		public static Attachment PrepareScreenRecordingAttachmentFromPath(string filePath)
        {
            var attachment = new Attachment();
            attachment.Type = AttachmentTypeEnum.Video;
            attachment.Subtype = AttachmentSubTypeEnum.ScreenRecording;
            attachment.FileName = Path.GetFileName(filePath);
            attachment.FilePath = filePath;
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
