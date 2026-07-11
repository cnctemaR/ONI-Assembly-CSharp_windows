using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Cache : ModifierModule, IModule3D, IModule
	{
		public new IModule SourceModule
		{
			get
			{
				return this._sourceModule;
			}
			set
			{
				this._isCached = false;
				this._sourceModule = value;
			}
		}

		public Cache()
		{
		}

		public Cache(IModule source)
			: base(source)
		{
		}

		public float GetValue(float x, float y, float z)
		{
			if (!this._isCached || x != this._xCache || y != this._yCache || z != this._zCache)
			{
				this._cachedValue = ((IModule3D)this._sourceModule).GetValue(x, y, z);
				this._xCache = x;
				this._yCache = y;
				this._zCache = z;
				this._isCached = true;
			}
			return this._cachedValue;
		}

		protected float _cachedValue;

		protected bool _isCached;

		protected float _xCache;

		protected float _yCache;

		protected float _zCache;
	}
}
