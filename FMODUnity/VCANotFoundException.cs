using System;

namespace FMODUnity
{
	public class VCANotFoundException : Exception
	{
		public VCANotFoundException(string path)
			: base("[FMOD] VCA not found '" + path + "'")
		{
			this.Path = path;
		}

		public string Path;
	}
}
