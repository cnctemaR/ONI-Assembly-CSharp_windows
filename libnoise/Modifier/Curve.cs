using System;
using System.Collections.Generic;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Modifier
{
	public class Curve : ModifierModule, IModule3D, IModule
	{
		public Curve()
		{
		}

		public Curve(IModule source)
			: base(source)
		{
		}

		public void AddControlPoint(float input, float output)
		{
			this.AddControlPoint(new ControlPoint(input, output));
		}

		public void AddControlPoint(ControlPoint point)
		{
			if (this._controlPoints.Contains(point))
			{
				throw new ArgumentException(string.Format("Cannont insert ControlPoint({0}, {1}) : Each control point is required to contain a unique input value", point.Input, point.Output));
			}
			this._controlPoints.Add(point);
			this.SortControlPoints();
		}

		public int CountControlPoints()
		{
			return this._controlPoints.Count;
		}

		public IList<ControlPoint> getControlPoints()
		{
			return this._controlPoints.AsReadOnly();
		}

		public void ClearControlPoints()
		{
			this._controlPoints.Clear();
		}

		public float GetValue(float x, float y, float z)
		{
			float value = ((IModule3D)this._sourceModule).GetValue(x, y, z);
			int num = 0;
			while (num < this._controlPoints.Count && value >= this._controlPoints[num].Input)
			{
				num++;
			}
			int num2 = Libnoise.Clamp(num - 2, 0, this._controlPoints.Count - 1);
			int num3 = Libnoise.Clamp(num - 1, 0, this._controlPoints.Count - 1);
			int num4 = Libnoise.Clamp(num, 0, this._controlPoints.Count - 1);
			int num5 = Libnoise.Clamp(num + 1, 0, this._controlPoints.Count - 1);
			if (num3 == num4)
			{
				return this._controlPoints[num3].Output;
			}
			float input = this._controlPoints[num3].Input;
			float input2 = this._controlPoints[num4].Input;
			float num6 = (value - input) / (input2 - input);
			return Libnoise.Cerp(this._controlPoints[num2].Output, this._controlPoints[num3].Output, this._controlPoints[num4].Output, this._controlPoints[num5].Output, num6);
		}

		protected void SortControlPoints()
		{
			this._controlPoints.Sort(delegate(ControlPoint p1, ControlPoint p2)
			{
				if (p1.Input > p2.Input)
				{
					return 1;
				}
				if (p1.Input < p2.Input)
				{
					return -1;
				}
				return 0;
			});
		}

		protected List<ControlPoint> _controlPoints = new List<ControlPoint>(4);
	}
}
