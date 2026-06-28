using System;
using FMOD;

namespace FMODUnity
{
	public class SystemNotInitializedException : Exception
	{
		public SystemNotInitializedException(RESULT result, string location)
			: base(string.Format("FMOD Studio initialization failed : {2} : {0} : {1}", result.ToString(), Error.String(result), location))
		{
			this.Result = result;
			this.Location = location;
		}

		public SystemNotInitializedException(Exception inner)
			: base("FMOD Studio initialization failed", inner)
		{
		}

		public RESULT Result;

		public string Location;
	}
}
