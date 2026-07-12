using System;
using System.Runtime.Serialization;

namespace System.Resources
{
	[Serializable]
	public class MissingSatelliteAssemblyException : SystemException
	{
		public MissingSatelliteAssemblyException()
			: base("Resource lookup fell back to the ultimate fallback resources in a satellite assembly, but that satellite either was not found or could not be loaded. Please consider reinstalling or repairing the application.")
		{
			base.HResult = -2146233034;
		}

		public MissingSatelliteAssemblyException(string message)
			: base(message)
		{
			base.HResult = -2146233034;
		}

		public MissingSatelliteAssemblyException(string message, string cultureName)
			: base(message)
		{
			base.HResult = -2146233034;
			this._cultureName = cultureName;
		}

		public MissingSatelliteAssemblyException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233034;
		}

		protected MissingSatelliteAssemblyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public string CultureName
		{
			get
			{
				return this._cultureName;
			}
		}

		private string _cultureName;
	}
}
