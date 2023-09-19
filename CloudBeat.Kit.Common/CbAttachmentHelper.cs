using CloudBeat.Kit.Common.Models;
using OpenQA.Selenium.Chrome;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using static System.Net.WebRequestMethods;

namespace CloudBeat.Kit.Common
{
    internal static class CbAttachmentHelper
    {
        public const string CB_RESULT_DIR_NAME = ".cb-results";
        public const string CB_ATTACHMENTS_DIR_NAME = "attachments";

        public static Attachment PrepareScreenRecordingAttachment(string base64VideoData)
        {
            byte[] data = Convert.FromBase64String(base64VideoData);
            var attachment = new Attachment();
            attachment.Type = AttachmentTypeEnum.Video;
            attachment.Subtype = AttachmentSubTypeEnum.ScreenRecording;
            attachment.FileName = $"{Guid.NewGuid().ToString()}.mp4";
            string filePath = GetAttachmentFilePath(attachment.FileName);
            
            try {
                System.IO.File.WriteAllBytes(filePath, data);
                return attachment;
            }
            catch {
                return null;
            }
        }

        public static string GetAttachmentFilePath(string fileName)
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            var cwd = System.IO.Path.GetDirectoryName(assembly.Location);
            var attachmentsDirPath = Path.Combine(cwd, CB_RESULT_DIR_NAME, CB_ATTACHMENTS_DIR_NAME);
            if (!Directory.Exists(attachmentsDirPath))
                Directory.CreateDirectory(attachmentsDirPath);
            return Path.Combine(attachmentsDirPath, fileName);
        }
    }
}
