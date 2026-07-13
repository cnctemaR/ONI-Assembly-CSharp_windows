using System;

namespace UnityEngine.Networking
{
	public struct DownloadedTextureParams
	{
		public static DownloadedTextureParams Default
		{
			get
			{
				return new DownloadedTextureParams
				{
					flags = (DownloadedTextureFlags.Readable | DownloadedTextureFlags.MipmapChain),
					mipmapCount = -1
				};
			}
		}

		public bool readable
		{
			get
			{
				return this.flags.HasFlag(DownloadedTextureFlags.Readable);
			}
			set
			{
				this.SetFlags(DownloadedTextureFlags.Readable, value);
			}
		}

		public bool mipmapChain
		{
			get
			{
				return this.flags.HasFlag(DownloadedTextureFlags.MipmapChain);
			}
			set
			{
				this.SetFlags(DownloadedTextureFlags.MipmapChain, value);
			}
		}

		public bool linearColorSpace
		{
			get
			{
				return this.flags.HasFlag(DownloadedTextureFlags.LinearColorSpace);
			}
			set
			{
				this.SetFlags(DownloadedTextureFlags.LinearColorSpace, value);
			}
		}

		private void SetFlags(DownloadedTextureFlags flgs, bool add)
		{
			if (add)
			{
				this.flags |= flgs;
			}
			else
			{
				this.flags &= ~flgs;
			}
		}

		public DownloadedTextureFlags flags;

		public int mipmapCount;
	}
}
