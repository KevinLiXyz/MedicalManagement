using MedicalImagingSystem.ILogger;
using Microsoft.Extensions.Logging;

namespace MedicalImagingSystem.Logger
{
	public class LogManager: ILogManager
	{
		private readonly Microsoft.Extensions.Logging.ILogger _logger;
		public LogManager(Microsoft.Extensions.Logging.ILogger logger)
		{
			_logger = logger;
		}
		public void LogInfo(string message)
		{
			_logger.LogInformation(message);
		}

		public void LogWarning(string message)
		{
			_logger.LogWarning(message);
		}

		public void LogError(string message, Exception? ex = null)
		{
			_logger.LogError(message, ex);
		}
	}
}
