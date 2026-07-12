using System;
using FMOD;

namespace FMODUnity
{
	public class EventNotFoundException : Exception
	{
		public EventNotFoundException(string path)
			: base("[FMOD] Event not found: '" + path + "'")
		{
			this.Path = path;
		}

		public EventNotFoundException(GUID guid)
		{
			string text = "[FMOD] Event not found: ";
			GUID guid2 = guid;
			base..ctor(text + guid2.ToString());
			this.Guid = guid;
		}

		public EventNotFoundException(EventReference eventReference)
			: base("[FMOD] Event not found: " + eventReference.ToString())
		{
			this.Guid = eventReference.Guid;
		}

		public GUID Guid;

		public string Path;
	}
}
