using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Tranformer
{
	public class TranslatePoint : TransformerModule, IModule3D, IModule
	{
		public TranslatePoint()
		{
		}

		public TranslatePoint(IModule source)
		{
			this._sourceModule = source;
		}

		public TranslatePoint(IModule source, float x, float y, float z)
			: this(source)
		{
			this._xTranslate = x;
			this._yTranslate = y;
			this._zTranslate = z;
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

		public float XTranslate
		{
			get
			{
				return this._xTranslate;
			}
			set
			{
				this._xTranslate = value;
			}
		}

		public float YTranslate
		{
			get
			{
				return this._yTranslate;
			}
			set
			{
				this._yTranslate = value;
			}
		}

		public float ZTranslate
		{
			get
			{
				return this._zTranslate;
			}
			set
			{
				this._zTranslate = value;
			}
		}

		public float GetValue(float x, float y, float z)
		{
			return ((IModule3D)this._sourceModule).GetValue(x + this._xTranslate, y + this._yTranslate, z + this._zTranslate);
		}

		public const float DEFAULT_TRANSLATE_X = 1f;

		public const float DEFAULT_TRANSLATE_Y = 1f;

		public const float DEFAULT_TRANSLATE_Z = 1f;

		protected IModule _sourceModule;

		protected float _xTranslate = 1f;

		protected float _yTranslate = 1f;

		protected float _zTranslate = 1f;
	}
}
