/*
 * <Your-Product-Name>
 * Copyright (c) <Year-From>-<Year-To> <Your-Company-Name>
 *
 * Please configure this header in your SonarCloud/SonarQube quality profile.
 * You can also set it in SonarLint.xml additional file for SonarLint or standalone NuGet analyzer.
 */

namespace ChatDAL.Constant
{
    public class BusinessConstants
    {
        public class Common
        {
            public const string CA0100 = "Oops we hit an error, Try again.";
        }

        public class RegistrationStatus
        {
            public const string CA0001 = "Success";

            public const string CA0002 = "Duplicate";

            public const string CA0003 = "Failed";

            public const string CA0004 = "Account Already Exist.";

        }

        public class LoginStatus
        {
            public const string CA0005 = "Failed to login, Try again";

            public const string CA0006 = "Account Not Found";
        }
        public class NotificationTypes
        {
            public const int CA0007 = 1;
        }
        public static class AppConfigrationDetails
        {
            public const string AFZContainer = "AfzContainer";
            public const string AFZStorageConnectionString = "AfzStorageConnectionString";
            public const string AFZStorageUrl = "AfzStorageUrl";
            public const string SharedAccessStartTime = "SharedAccessStartTime";
            public const string SharedAccessExpiryTime = "SharedAccessExpiryTime";
            public const string BlobProfilePolicyName = "BlobProfilePolicyName";
        }
        public static class AttachmentPath
        {
            public const string AttachmentBasePath = "AttachmentBasePath";
            public const string ChatPath = "ChatPath";
            public const string SupportAttachmentPath = "SupportAttachmentPath";
            public const string FolderPath = @"Attachments\SupportAttachments";
            public const string FolderNotes = @"Attachments\Notes";
        }

        public static class Status
        {
            public const int Active = 1;
        }
        
        public static class ChatGPTConnection
        {
            public const string ApiKey = "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";
        }
    }
 
}
