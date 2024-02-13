using System;
using Kruso.Reepay.Extensions.Models;

namespace Kruso.Reepay.Extensions.Services.Interfaces
{
	public interface IReepayLogger<T>
	{
		void LogInfo(string line);
		void LogWarning(string line);
		void LogException(Exception exception, string message = null);
		void LogReepayException(ReepayException exception, string message = null);
	}
}
