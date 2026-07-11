using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public abstract class AbstractImageRenderer : AbstractRenderer
	{
		public IMap2D<IColor> Image
		{
			get
			{
				return this._image;
			}
			set
			{
				this._image = value;
			}
		}

		protected IMap2D<IColor> _image;
	}
}
