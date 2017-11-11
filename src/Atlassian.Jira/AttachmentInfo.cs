using System;

namespace Atlassian.Jira
{
    /// <summary>
    /// Information about an attachment to be uploaded
    /// </summary>
    public class UploadAttachmentInfo
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }

        public UploadAttachmentInfo(string name, byte[] data)
        {
            this.Name = name;
            this.Data = data;
        }
    }
}