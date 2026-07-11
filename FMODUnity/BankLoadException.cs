using System;
using FMOD;

namespace FMODUnity
{
	public class BankLoadException : Exception
	{
		public BankLoadException(string path, RESULT result)
			: base(string.Format("[FMOD] Could not load bank '{0}' : {1} : {2}", path, result.ToString(), Error.String(result)))
		{
			this.Path = path;
			this.Result = result;
		}

		public BankLoadException(string path, string error)
			: base(string.Format("[FMOD] Could not load bank '{0}' : {1}", path, error))
		{
			this.Path = path;
			this.Result = RESULT.ERR_INTERNAL;
		}

		public string Path;

		public RESULT Result;
	}
}
