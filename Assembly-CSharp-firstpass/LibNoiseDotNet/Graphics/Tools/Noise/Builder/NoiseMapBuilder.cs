using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Builder
{
	public abstract class NoiseMapBuilder
	{
		public IModule SourceModule
		{
			get
			{
				return this._sourceModule;
			}
			set
			{
				this._sourceModule = value;
			}
		}

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

		public NoiseMapBuilderCallback CallBack
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

		public int Width
		{
			get
			{
				return this._width;
			}
		}

		public int Height
		{
			get
			{
				return this._height;
			}
		}

		public IBuilderFilter Filter
		{
			get
			{
				return this._filter;
			}
			set
			{
				this._filter = value;
			}
		}

		public NoiseMapBuilder()
		{
		}

		public abstract void Build();

		public void SetSize(int width, int height)
		{
			if (width < 0 || height < 0)
			{
				throw new ArgumentException("Dimension must be greater or equal 0");
			}
			this._height = height;
			this._width = width;
		}

		protected IModule _sourceModule;

		protected IMap2D<float> _noiseMap;

		protected NoiseMapBuilderCallback _callBack;

		protected int _width;

		protected int _height;

		protected IBuilderFilter _filter;
	}
}
