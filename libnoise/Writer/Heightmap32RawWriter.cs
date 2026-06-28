using System;
using System.IO;
using LibNoiseDotNet.Graphics.Tools.Noise.Renderer;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Writer
{
	public class Heightmap32RawWriter : AbstractWriter
	{
		public Heightmap32 Heightmap
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
			float[] array = this._heightmap.Share();
			try
			{
				for (int i = 0; i < array.Length; i++)
				{
					this._writer.Write(array[i]);
				}
			}
			catch (Exception ex)
			{
				throw new IOException("Unknown IO exception", ex);
			}
			base.CloseFile();
		}

		protected Heightmap32 _heightmap;
	}
}
