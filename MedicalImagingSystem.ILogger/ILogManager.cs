using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalImagingSystem.ILogger
{
	public interface ILogManager
	{
		void LogInfo(string message);
		void LogWarning(string message);
		void LogError(string message, Exception? ex = null);
	}
}
