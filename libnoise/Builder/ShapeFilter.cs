using System;
using LibNoiseDotNet.Graphics.Tools.Noise.Renderer;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Builder
{
	public class ShapeFilter : IBuilderFilter
	{
		public IMap2D<IColor> Shape
		{
			get
			{
				return this._shape;
			}
			set
			{
				this._shape = value;
			}
		}

		public float ConstantValue
		{
			get
			{
				return this._constant;
			}
			set
			{
				this._constant = value;
			}
		}

		public FilterLevel IsFiltered(int x, int y)
		{
			byte greyscaleLevel = this.GetGreyscaleLevel(x, y);
			if (greyscaleLevel == 0)
			{
				return FilterLevel.Constant;
			}
			if (greyscaleLevel == 255)
			{
				return FilterLevel.Source;
			}
			return FilterLevel.Filter;
		}

		public float FilterValue(int x, int y, float source)
		{
			byte greyscaleLevel = this.GetGreyscaleLevel(x, y);
			if (greyscaleLevel == 255)
			{
				return source;
			}
			if (greyscaleLevel == 0)
			{
				return this._constant;
			}
			return Libnoise.Lerp(this._constant, source, (float)greyscaleLevel / 255f);
		}

		protected byte GetGreyscaleLevel(int x, int y)
		{
			if (!this._cache.IsCached(x, y))
			{
				this._cache.Update(x, y, this._shape.GetValue(x, y).Red);
			}
			return this._cache.level;
		}

		public const float DEFAULT_VALUE = -0.5f;

		protected float _constant = -0.5f;

		protected IMap2D<IColor> _shape;

		protected ShapeFilter.LevelCache _cache = new ShapeFilter.LevelCache(-1, -1, 0);

		protected struct LevelCache
		{
			public LevelCache(int x, int y, byte level)
			{
				this.x = x;
				this.y = y;
				this.level = level;
			}

			public bool IsCached(int x, int y)
			{
				return this.x == x && this.y == y;
			}

			public void Update(int x, int y, byte level)
			{
				this.x = x;
				this.y = y;
				this.level = level;
			}

			private int x;

			private int y;

			public byte level;
		}
	}
}
