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
			SoundPlayer soundPlayer = new SoundPlayer(this.resource);
			soundPlayer.Play();
		}

		private Stream resource;
	}
}
