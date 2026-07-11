using System;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Model
{
	public class Line : AbstractModel
	{
		public Line()
		{
		}

		public Line(IModule module)
			: base(module)
		{
		}

		public bool Attenuate
		{
			get
			{
				return this._attenuate;
			}
			set
			{
				this._attenuate = value;
			}
		}

		public void SetStartPoint(float x, float y, float z)
		{
			this._startPosition.x = x;
			this._startPosition.y = y;
			this._startPosition.z = z;
		}

		public void SetEndPoint(float x, float y, float z)
		{
			this._endPosition.x = x;
			this._endPosition.y = y;
			this._endPosition.z = z;
		}

		public float GetValue(float p)
		{
			float num = (this._endPosition.x - this._startPosition.x) * p + this._startPosition.x;
			float num2 = (this._endPosition.y - this._startPosition.y) * p + this._startPosition.y;
			float num3 = (this._endPosition.z - this._startPosition.z) * p + this._startPosition.z;
			float value = ((IModule3D)this._sourceModule).GetValue(num, num2, num3);
			if (this._attenuate)
			{
				return p * (1f - p) * 4f * value;
			}
			return value;
		}

		protected bool _attenuate = true;

		protected Line.Position _startPosition = new Line.Position(0f, 0f, 0f);

		protected Line.Position _endPosition = new Line.Position(0f, 0f, 0f);

		protected struct Position
		{
			public Position(float x, float y, float z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}

			public float x;

			public float y;

			public float z;
		}
	}
}
