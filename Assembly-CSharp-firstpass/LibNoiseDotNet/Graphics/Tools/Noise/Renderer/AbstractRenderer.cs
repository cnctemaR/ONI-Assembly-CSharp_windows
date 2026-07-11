using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public abstract class AbstractRenderer
	{
		public IMap2D<float> NoiseMap
		{
			get
			{
				return this._noiseMap;
			}
			set
			{
				this._noiseMap = value;
			}
		}

		public RendererCallback CallBack
		{
			get
			{
				return this._callBack;
			}
			set
			{
				this._callBack = value;
			}
		}

		public abstract void Render();

		protected RendererCallback _callBack;

		protected IMap2D<float> _noiseMap;
	}
}
