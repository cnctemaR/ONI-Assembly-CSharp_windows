using System;

namespace Mono.Cecil.Cil
{
	public sealed class ImageDebugHeaderEntry
	{
		public ImageDebugDirectory Directory
		{
			get
			{
				return this.directory;
			}
			internal set
			{
				this.directory = value;
			}
		}

		public byte[] Data
		{
			get
			{
				return this.data;
			}
		}

		public ImageDebugHeaderEntry(ImageDebugDirectory directory, byte[] data)
		{
			this.directory = directory;
			this.data = data ?? Empty<byte>.Array;
		}

		private ImageDebugDirectory directory;

		private readonly byte[] data;
	}
}
