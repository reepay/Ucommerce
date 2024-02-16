using System;
using System.Globalization;
using System.IO;
using System.Text;
using Kruso.Reepay.Extensions.Models;
using Kruso.Reepay.Extensions.Services.Interfaces;
using Ucommerce.Infrastructure.Logging;

namespace Kruso.Reepay.Extensions.Services
{
	public class ReepayLogger<T> : IReepayLogger<T>
	{
		private readonly ILoggingService _defaultLoggingService;

		private readonly string _logDirectory;
		private readonly string _reepayLogFilename;

		public ReepayLogger(ILoggingService defaultLoggingService)
		{
			_defaultLoggingService = defaultLoggingService;
			_logDirectory = @"\App_Data\Logs\";
			_reepayLogFilename = "Reepay.txt";

			Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + _logDirectory);
		}

		public void LogInfo(string line)
		{
			using (var file = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + _logDirectory + _reepayLogFilename, true, Encoding.UTF8))
			{
				file.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) + ": INFO : " + line);
			}
		}

		public void LogWarning(string line)
		{
			_defaultLoggingService.Information<ReepayLogger<T>>("WARN : " + line);
			using (var file = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + _logDirectory + _reepayLogFilename, true, Encoding.UTF8))
			{
				file.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) + ": WARN : " + line);
			}
		}

		public void LogException(Exception exception, string message = null)
		{
			if (message == null) message = string.Empty;

			var logLine = "Exception: ";
			logLine += message;
			logLine += Environment.NewLine + exception.ToString();
			using (var file = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + _logDirectory + _reepayLogFilename, true, Encoding.UTF8))
			{
				file.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) + ": ERROR : " + logLine);
			}
			_defaultLoggingService.Error<ReepayLogger<T>>(exception, "Exception occurred in the Reepay Ucommerce app: " + message);
		}

		public void LogReepayException(ReepayException exception, string message = null)
		{
			if (message == null) message = string.Empty;
			if (exception != null && exception.ReepayError != null)
			{
				message += $"{exception.ReepayError.Code} {exception.ReepayError.Message} {exception.ReepayError.Error} {exception.ReepayError.Transaction_error} {exception.ReepayError.Http_status} {exception.ReepayError.Http_reason} {exception.ReepayError.Path} {exception.ReepayError.Request_id} {exception.ReepayError.TimeStamp}";
			}

			LogException(exception, message);
		}
	}
}
