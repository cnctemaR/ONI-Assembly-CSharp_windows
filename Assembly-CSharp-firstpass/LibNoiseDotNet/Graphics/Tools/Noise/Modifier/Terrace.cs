using System;
using System.Collections.Generic;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Terrace : ModifierModule, IModule3D, IModule
	{
		public Terrace()
		{
		}

		public Terrace(IModule source)
			: base(source)
		{
		}

		public Terrace(IModule source, bool invert)
			: base(source)
		{
			this._invert = invert;
		}

		public bool Invert
		{
			get
			{
				return this._invert;
			}
			set
			{
				this._invert = value;
			}
		}

		public void AddControlPoint(float input)
		{
			if (this._controlPoints.Contains(input))
			{
				throw new ArgumentException(string.Format("Cannont insert ControlPoint({0}) : Each control point is required to contain a unique input value", input));
			}
			this._controlPoints.Add(input);
			this.SortControlPoints();
		}

		public int CountControlPoints()
		{
			return this._controlPoints.Count;
		}

		public IList<float> getControlPoints()
		{
			return this._controlPoints.AsReadOnly();
		}

		public void ClearControlPoints()
		{
			this._controlPoints.Clear();
		}

		public void MakeControlPoints(int controlPointCount)
		{
			if (controlPointCount < 2)
			{
				throw new ArgumentException("Two or more control points must be specified.");
			}
			this.ClearControlPoints();
			float num = 2f / ((float)controlPointCount - 1f);
			float num2 = -1f;
			for (int i = 0; i < controlPointCount; i++)
			{
				this.AddControlPoint(num2);
				num2 += num;
			}
		}

		public float GetValue(float x, float y, float z)
		{
			float value = ((IModule3D)this._sourceModule).GetValue(x, y, z);
			int i;
			for (i = 0; i < this._controlPoints.Count; i++)
			{
				if (value < this._controlPoints[i])
				{
					break;
				}
			}
			int num = Libnoise.Clamp(i - 1, 0, this._controlPoints.Count - 1);
			int num2 = Libnoise.Clamp(i, 0, this._controlPoints.Count - 1);
			if (num == num2)
			{
				return this._controlPoints[num2];
			}
			float num3 = this._controlPoints[num];
			float num4 = this._controlPoints[num2];
			float num5 = (value - num3) / (num4 - num3);
			if (this._invert)
			{
				num5 = 1f - num5;
				Libnoise.SwapValues(ref num3, ref num4);
			}
			num5 *= num5;
			return Libnoise.Lerp(num3, num4, num5);
		}

		protected void SortControlPoints()
		{
			this._controlPoints.Sort(delegate(float p1, float p2)
			{
				if (p1 > p2)
				{
					return 1;
				}
				if (p1 < p2)
				{
					return -1;
				}
				return 0;
			});
		}

		protected List<float> _controlPoints = new List<float>(2);

		protected bool _invert;
	}
}
