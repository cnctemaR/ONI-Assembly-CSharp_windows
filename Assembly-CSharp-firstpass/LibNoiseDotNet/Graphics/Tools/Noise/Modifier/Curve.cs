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
			int i;
			for (i = 0; i < this._controlPoints.Count; i++)
			{
				if (value < this._controlPoints[i].Input)
				{
					break;
				}
			}
			int num = Libnoise.Clamp(i - 2, 0, this._controlPoints.Count - 1);
			int num2 = Libnoise.Clamp(i - 1, 0, this._controlPoints.Count - 1);
			int num3 = Libnoise.Clamp(i, 0, this._controlPoints.Count - 1);
			int num4 = Libnoise.Clamp(i + 1, 0, this._controlPoints.Count - 1);
			if (num2 == num3)
			{
				return this._controlPoints[num2].Output;
			}
			float input = this._controlPoints[num2].Input;
			float input2 = this._controlPoints[num3].Input;
			float num5 = (value - input) / (input2 - input);
			return Libnoise.Cerp(this._controlPoints[num].Output, this._controlPoints[num2].Output, this._controlPoints[num3].Output, this._controlPoints[num4].Output, num5);
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
