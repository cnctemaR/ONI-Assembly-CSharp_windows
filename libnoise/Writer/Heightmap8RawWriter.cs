using System;
using System.IO;
using LibNoiseDotNet.Graphics.Tools.Noise.Renderer;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Writer
{
	public class Heightmap8RawWriter : AbstractWriter
	{
		public Heightmap8 Heightmap
		{
			get
			{
				return this._heightmap;
			}
			set
			{
				this._heightmap = value;
			}
		}

		public override void WriteFile()
		{
			if (this._heightmap == null)
			{
				throw new ArgumentException("An heightmap must be provided");
			}
			base.OpenFile();
			try
			{
				this._writer.Write(this._heightmap.Share());
			}
			catch (Exception ex)
			{
				throw new IOException("Unknown IO exception", ex);
			}
			base.CloseFile();
		}

		protected Heightmap8 _heightmap;
	}
}
