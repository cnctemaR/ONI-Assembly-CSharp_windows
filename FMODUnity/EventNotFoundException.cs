using System;

namespace FMODUnity
{
	public class EventNotFoundException : Exception
	{
		public EventNotFoundException(string path)
			: base("[FMOD] Event not found '" + path + "'")
		{
			this.Path = path;
		}

		public EventNotFoundException(Guid guid)
			: base("[FMOD] Event not found " + guid.ToString("b"))
		{
			this.Guid = guid;
		}

		public Guid Guid;

		public string Path;
	}
}
