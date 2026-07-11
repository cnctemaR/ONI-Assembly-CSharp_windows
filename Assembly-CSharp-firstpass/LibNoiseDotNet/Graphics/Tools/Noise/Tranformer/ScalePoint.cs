using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Tranformer
{
	public class ScalePoint : TransformerModule, IModule3D, IModule
	{
		public ScalePoint()
		{
		}

		public ScalePoint(IModule source)
		{
			this._sourceModule = source;
		}

		public ScalePoint(IModule source, float x, float y, float z)
			: this(source)
		{
			this._xScale = x;
			this._yScale = y;
			this._zScale = z;
		}

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

		public float XScale
		{
			get
			{
				return this._xScale;
			}
			set
			{
				this._xScale = value;
			}
		}

		public float YScale
		{
			get
			{
				return this._yScale;
			}
			set
			{
				this._yScale = value;
			}
		}

		public float ZScale
		{
			get
			{
				return this._zScale;
			}
			set
			{
				this._zScale = value;
			}
		}

		public float GetValue(float x, float y, float z)
		{
			return ((IModule3D)this._sourceModule).GetValue(x * this._xScale, y * this._yScale, z * this._zScale);
		}

		public const float DEFAULT_POINT_X = 1f;

		public const float DEFAULT_POINT_Y = 1f;

		public const float DEFAULT_POINT_Z = 1f;

		protected IModule _sourceModule;

		protected float _xScale = 1f;

		protected float _yScale = 1f;

		protected float _zScale = 1f;
	}
}
