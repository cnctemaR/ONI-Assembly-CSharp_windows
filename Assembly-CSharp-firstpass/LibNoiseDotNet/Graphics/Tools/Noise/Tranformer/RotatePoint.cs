using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Tranformer
{
	public class RotatePoint : TransformerModule, IModule3D, IModule
	{
		public RotatePoint()
		{
		}

		public RotatePoint(IModule source)
		{
			this._sourceModule = source;
		}

		public RotatePoint(IModule source, float xAngle, float yAngle, float zAngle)
		{
			this._sourceModule = source;
			this.SetAngles(xAngle, yAngle, zAngle);
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

		public float XAngle
		{
			get
			{
				return this._xAngle;
			}
			set
			{
				this.SetAngles(value, this._yAngle, this._zAngle);
			}
		}

		public float YAngle
		{
			get
			{
				return this._yAngle;
			}
			set
			{
				this.SetAngles(this._xAngle, value, this._zAngle);
			}
		}

		public float ZAngle
		{
			get
			{
				return this._zAngle;
			}
			set
			{
				this.SetAngles(this._xAngle, this._yAngle, value);
			}
		}

		public void SetAngles(float xAngle, float yAngle, float zAngle)
		{
			float num = (float)Math.Cos((double)(xAngle * 0.017453292f));
			float num2 = (float)Math.Cos((double)(yAngle * 0.017453292f));
			float num3 = (float)Math.Cos((double)(zAngle * 0.017453292f));
			float num4 = (float)Math.Sin((double)(xAngle * 0.017453292f));
			float num5 = (float)Math.Sin((double)(yAngle * 0.017453292f));
			float num6 = (float)Math.Sin((double)(zAngle * 0.017453292f));
			this._x1Matrix = num5 * num4 * num6 + num2 * num3;
			this._y1Matrix = num * num6;
			this._z1Matrix = num5 * num3 - num2 * num4 * num6;
			this._x2Matrix = num5 * num4 * num3 - num2 * num6;
			this._y2Matrix = num * num3;
			this._z2Matrix = -num2 * num4 * num3 - num5 * num6;
			this._x3Matrix = -num5 * num;
			this._y3Matrix = num4;
			this._z3Matrix = num2 * num;
			this._xAngle = xAngle;
			this._yAngle = yAngle;
			this._zAngle = zAngle;
		}

		public float GetValue(float x, float y, float z)
		{
			float num = this._x1Matrix * x + this._y1Matrix * y + this._z1Matrix * z;
			float num2 = this._x2Matrix * x + this._y2Matrix * y + this._z2Matrix * z;
			float num3 = this._x3Matrix * x + this._y3Matrix * y + this._z3Matrix * z;
			return ((IModule3D)this._sourceModule).GetValue(num, num2, num3);
		}

		public const float DEFAULT_ROTATE_X = 0f;

		public const float DEFAULT_ROTATE_Y = 0f;

		public const float DEFAULT_ROTATE_Z = 0f;

		protected IModule _sourceModule;

		protected float _x1Matrix;

		protected float _x2Matrix;

		protected float _x3Matrix;

		protected float _xAngle;

		protected float _y1Matrix;

		protected float _y2Matrix;

		protected float _y3Matrix;

		protected float _yAngle;

		protected float _z1Matrix;

		protected float _z2Matrix;

		protected float _z3Matrix;

		protected float _zAngle;
	}
}
