using System;
using System.Collections.Generic;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class GradientColor
	{
		public static GradientColor GRAYSCALE
		{
			get
			{
				GradientColor gradientColor = new GradientColor();
				gradientColor.AddGradientPoint(-1f, Color.BLACK);
				gradientColor.AddGradientPoint(1f, Color.WHITE);
				return gradientColor;
			}
		}

		public static GradientColor EMPTY
		{
			get
			{
				GradientColor gradientColor = new GradientColor();
				gradientColor.AddGradientPoint(-1f, Color.TRANSPARENT);
				gradientColor.AddGradientPoint(1f, Color.TRANSPARENT);
				return gradientColor;
			}
		}

		public static GradientColor TERRAIN
		{
			get
			{
				GradientColor gradientColor = new GradientColor();
				gradientColor.AddGradientPoint(-1f, new Color(0, 0, 128, byte.MaxValue));
				gradientColor.AddGradientPoint(-0.25f, new Color(0, 0, byte.MaxValue, byte.MaxValue));
				gradientColor.AddGradientPoint(0f, new Color(0, 128, byte.MaxValue, byte.MaxValue));
				gradientColor.AddGradientPoint(0.0625f, new Color(240, 240, 64, byte.MaxValue));
				gradientColor.AddGradientPoint(0.125f, new Color(32, 160, 0, byte.MaxValue));
				gradientColor.AddGradientPoint(0.375f, new Color(224, 224, 0, byte.MaxValue));
				gradientColor.AddGradientPoint(0.75f, new Color(128, 128, 128, byte.MaxValue));
				gradientColor.AddGradientPoint(1f, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue));
				return gradientColor;
			}
		}

		public GradientColor()
		{
		}

		public GradientColor(IColor color)
		{
			this.AddGradientPoint(-1f, color);
			this.AddGradientPoint(1f, color);
		}

		public GradientColor(IColor start, IColor end)
		{
			this.AddGradientPoint(-1f, start);
			this.AddGradientPoint(1f, end);
		}

		public void AddGradientPoint(float position, IColor color)
		{
			this.AddGradientPoint(new GradientPoint(position, color));
		}

		public void AddGradientPoint(GradientPoint point)
		{
			if (this._gradientPoints.Contains(point))
			{
				throw new ArgumentException(string.Format("Cannont insert GradientPoint({0}, {1}) : Each GradientPoint is required to contain a unique position", point.Position, point.Color));
			}
			this._gradientPoints.Add(point);
			this._gradientPoints.Sort(delegate(GradientPoint p1, GradientPoint p2)
			{
				if (p1.Position > p2.Position)
				{
					return 1;
				}
				if (p1.Position < p2.Position)
				{
					return -1;
				}
				return 0;
			});
		}

		public void Clear()
		{
			this._gradientPoints.Clear();
		}

		public IColor GetColor(float position)
		{
			int num = 0;
			while (num < this._gradientPoints.Count && position >= this._gradientPoints[num].Position)
			{
				num++;
			}
			int num2 = Libnoise.Clamp(num - 1, 0, this._gradientPoints.Count - 1);
			int num3 = Libnoise.Clamp(num, 0, this._gradientPoints.Count - 1);
			if (num2 == num3)
			{
				return this._gradientPoints[num3].Color;
			}
			float position2 = this._gradientPoints[num2].Position;
			float position3 = this._gradientPoints[num3].Position;
			float num4 = (position - position2) / (position3 - position2);
			return Color.Lerp(this._gradientPoints[num2].Color, this._gradientPoints[num3].Color, num4);
		}

		public int CountGradientPoints()
		{
			return this._gradientPoints.Count;
		}

		public IList<GradientPoint> getGradientPoints()
		{
			return this._gradientPoints.AsReadOnly();
		}

		protected List<GradientPoint> _gradientPoints = new List<GradientPoint>(10);
	}
}
