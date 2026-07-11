using System;

namespace FMODUnity
{
	public class BusNotFoundException : Exception
	{
		public BusNotFoundException(string path)
			: base("[FMOD] Bus not found '" + path + "'")
		{
			this.Path = path;
		}

		public string Path;
	}
}
