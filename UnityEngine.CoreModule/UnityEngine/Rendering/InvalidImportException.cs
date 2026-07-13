using System;

namespace UnityEngine.Rendering
{
	public class InvalidImportException : Exception
	{
		public InvalidImportException(string message)
			: base(message)
		{
		}
	}
}
