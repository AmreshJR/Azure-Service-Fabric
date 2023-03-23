/*
 * <Your-Product-Name>
 * Copyright (c) <Year-From>-<Year-To> <Your-Company-Name>
 *
 * Please configure this header in your SonarCloud/SonarQube quality profile.
 * You can also set it in SonarLint.xml additional file for SonarLint or standalone NuGet analyzer.
 */

using System;

namespace ChatDAL.DTO.Chat
{
    public class DtoAttachment
    {
        public string FilePath { get; set; }
        public string FilePathOrginal { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public int StatusId { get; set; }
        public int? DocumentTypeId { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string GuidFileName { get; set; }
        public long? UploadDocumentsId { get; set; }
        public long? DocumentId { get; set; }
        public string Description { get; set; }
        public Stream FileContent { get; set; }
    }
}
