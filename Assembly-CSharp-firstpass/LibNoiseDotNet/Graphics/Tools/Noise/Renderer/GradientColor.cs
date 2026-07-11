using System;
using System.Collections.Generic;

namespace LibNoiseDotNet.Graphics.Tools.Noise.Renderer
{
	public class GradientColor
	{
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
			int i;
			for (i = 0; i < this._gradientPoints.Count; i++)
			{
				if (position < this._gradientPoints[i].Position)
				{
					break;
				}
			}
			int num = Libnoise.Clamp(i - 1, 0, this._gradientPoints.Count - 1);
			int num2 = Libnoise.Clamp(i, 0, this._gradientPoints.Count - 1);
			if (num == num2)
			{
				return this._gradientPoints[num2].Color;
			}
			float position2 = this._gradientPoints[num].Position;
			float position3 = this._gradientPoints[num2].Position;
			float num3 = (position - position2) / (position3 - position2);
			return Color.Lerp(this._gradientPoints[num].Color, this._gradientPoints[num2].Color, num3);
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
