using Microsoft.Extensions.Logging;
using NLog;
using NotificationService.DTO.Common;
using System;
using System.Diagnostics;

namespace NotificationService.Logger
{
    public class Log
    {
        #region Log Error
        private static NLog.Logger objNlog = LogManager.GetCurrentClassLogger();
        /// <param name="strLogType">Error Type</param>
        /// <param name="strSource">Source of the error occured component</param>
        /// <param name="strMsgFromSys">System generated error message</param>
        /// <param name="strMsgFromObject">Custom error message</param>        
        /// 
        private readonly ILogger<Log> _logger;

        public Log(ILogger<Log> logger)
        {
            _logger = logger;
        }

        public void LogToTables(string strLogType, string strSource, string strMsgFromSys, string strMsgFromObject)
        {
            try
            {
                _logger.LogError("Error Source :: " + strSource + "Error Message :: " + strMsgFromSys + " :: " + strMsgFromObject);
            }
            catch (Exception)
            {
                Debug.WriteLine(DtoLog.Error);
            }
        }
        private static void LogError(string strLogType, string strSource, string strMsgFromSys, string strMsgFromObject)
        {
            try
            {
                objNlog.Error(strLogType + "Source :: " + strSource + "|Error Message :: " + strMsgFromSys + " :: " + strMsgFromObject);
            }
            catch (Exception)
            {
                Debug.WriteLine(DtoLog.Error);
            }
        }

        public static string GetErrorMessage(Exception ex)
        {
            string message = ((ex.InnerException != null) ? ex.Message + GetErrorMessage(ex.InnerException) : ex.Message);
            return message;
        }
        #endregion

        #region Common Log      
        public static void WriteError(string folderPath, string methodName, Exception expMessage)
        {
            try
            {
                LogError(DtoLog.Error, folderPath, GetErrorMessage(expMessage), "Problem in " + methodName + " method");
            }
            catch (Exception ex)
            {
               Debug.WriteLine(DtoLog.Error);
            }
        }
        public static void WriteInfo(string message)
        {
            try
            {
                    objNlog.Info("Information :: " + message);
            }
            catch (Exception)
            {
                Debug.WriteLine(DtoLog.Error);
            }
        }


        #endregion        
    }
}
