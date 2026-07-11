using System;
using System.IO;

namespace System.Media
{
	public class SystemSound
	{
		internal SystemSound(string tag)
		{
			this.resource = typeof(SystemSound).Assembly.GetManifestResourceStream(tag + ".wav");
		}

		public void Play()
		{
			new SoundPlayer(this.resource).Play();
		}

		private Stream resource;
	}
}
