using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Resources
{
	[ComVisible(true)]
	[Serializable]
	public class MissingSatelliteAssemblyException : SystemException
	{
		public MissingSatelliteAssemblyException()
			: base(Locale.GetText("The satellite assembly was not found for the required culture."))
		{
		}

		public MissingSatelliteAssemblyException(string message)
			: base(message)
		{
		}

		public MissingSatelliteAssemblyException(string message, string cultureName)
			: base(message)
		{
			this.culture = cultureName;
		}

		protected MissingSatelliteAssemblyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public MissingSatelliteAssemblyException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public string CultureName
		{
			get
			{
				return this.culture;
			}
		}

		private string culture;
	}
}
