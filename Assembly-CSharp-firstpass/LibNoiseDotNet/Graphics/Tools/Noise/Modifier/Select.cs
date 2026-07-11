using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Select : SelectorModule, IModule3D, IModule
	{
		public Select()
		{
		}

		public Select(IModule controlModule, IModule rightModule, IModule leftModule, float lower, float upper, float edge)
		{
			this._controlModule = controlModule;
			this._leftModule = leftModule;
			this._rightModule = rightModule;
			this.SetBounds(lower, upper);
			this.EdgeFalloff = edge;
		}

		public float LowerBound
		{
			get
			{
				return this._lowerBound;
			}
		}

		public float UpperBound
		{
			get
			{
				return this._upperBound;
			}
		}

		public float EdgeFalloff
		{
			get
			{
				return this._edgeFalloff;
			}
			set
			{
				float num = this._upperBound - this._lowerBound;
				this._edgeFalloff = ((value <= num / 2f) ? value : (num / 2f));
			}
		}

		public IModule LeftModule
		{
			get
			{
				return this._leftModule;
			}
			set
			{
				this._leftModule = value;
			}
		}

		public IModule RightModule
		{
			get
			{
				return this._rightModule;
			}
			set
			{
				this._rightModule = value;
			}
		}

		public IModule ControlModule
		{
			get
			{
				return this._controlModule;
			}
			set
			{
				this._controlModule = value;
			}
		}

		public void SetBounds(float lower, float upper)
		{
			this._lowerBound = lower;
			this._upperBound = upper;
			this.EdgeFalloff = this._edgeFalloff;
		}

		public float GetValue(float x, float y, float z)
		{
			float value = ((IModule3D)this._controlModule).GetValue(x, y, z);
			if ((double)this._edgeFalloff > 0.0)
			{
				if (value < this._lowerBound - this._edgeFalloff)
				{
					return ((IModule3D)this._leftModule).GetValue(x, y, z);
				}
				if (value < this._lowerBound + this._edgeFalloff)
				{
					float num = this._lowerBound - this._edgeFalloff;
					float num2 = this._lowerBound + this._edgeFalloff;
					float num3 = Libnoise.SCurve3((value - num) / (num2 - num));
					return Libnoise.Lerp(((IModule3D)this._leftModule).GetValue(x, y, z), ((IModule3D)this._leftModule).GetValue(x, y, z), num3);
				}
				if (value < this._upperBound - this._edgeFalloff)
				{
					return ((IModule3D)this._leftModule).GetValue(x, y, z);
				}
				if (value < this._upperBound + this._edgeFalloff)
				{
					float num4 = this._upperBound - this._edgeFalloff;
					float num5 = this._upperBound + this._edgeFalloff;
					float num3 = Libnoise.SCurve3((value - num4) / (num5 - num4));
					return Libnoise.Lerp(((IModule3D)this._leftModule).GetValue(x, y, z), ((IModule3D)this._leftModule).GetValue(x, y, z), num3);
				}
				return ((IModule3D)this._leftModule).GetValue(x, y, z);
			}
			else
			{
				if (value < this._lowerBound || value > this._upperBound)
				{
					return ((IModule3D)this._leftModule).GetValue(x, y, z);
				}
				return ((IModule3D)this._leftModule).GetValue(x, y, z);
			}
		}

		public const float DEFAULT_FALL_OFF = -1f;

		public const float DEFAULT_LOWER_BOUND = -1f;

		public const float DEFAULT_UPPER_BOUND = 1f;

		protected float _lowerBound = -1f;

		protected float _upperBound = 1f;

		protected float _edgeFalloff = -1f;

		protected IModule _controlModule;

		protected IModule _rightModule;

		protected IModule _leftModule;
	}
}
