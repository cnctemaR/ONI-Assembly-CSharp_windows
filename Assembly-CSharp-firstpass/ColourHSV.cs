using System;
using UnityEngine;

public struct ColourHSV
{
	public ColourHSV(float h, float s, float v)
	{
		this.h = h;
		this.s = s;
		this.v = v;
		this.a = 1f;
	}

	public ColourHSV(float h, float s, float v, float a)
	{
		this.h = h;
		this.s = s;
		this.v = v;
		this.a = a;
	}

	public ColourHSV(Color color)
	{
		this.a = 0f;
		float num = Mathf.Min(Mathf.Min(color.r, color.g), color.b);
		float num2 = Mathf.Max(Mathf.Max(color.r, color.g), color.b);
		float num3 = num2 - num;
		this.v = num2;
		if (Mathf.Approximately(num2, 0f))
		{
			this.s = 0f;
			this.h = -1f;
			return;
		}
		this.s = num3 / num2;
		if (Mathf.Approximately(num, num2))
		{
			this.v = num2;
			this.s = 0f;
			this.h = -1f;
			return;
		}
		if (color.r == num2)
		{
			this.h = (color.g - color.b) / num3;
		}
		else if (color.g == num2)
		{
			this.h = 2f + (color.b - color.r) / num3;
		}
		else
		{
			this.h = 4f + (color.r - color.g) / num3;
		}
		this.h *= 60f;
		if (this.h < 0f)
		{
			this.h += 360f;
		}
	}

	public Color ToColor()
	{
		if (this.s == 0f)
		{
			return new Color(this.v, this.v, this.v, this.a);
		}
		float num = this.h / 60f;
		int num2 = (int)Mathf.Floor(num);
		float num3 = num - (float)num2;
		float num4 = this.v;
		float num5 = num4 * (1f - this.s);
		float num6 = num4 * (1f - this.s * num3);
		float num7 = num4 * (1f - this.s * (1f - num3));
		Color color = new Color(0f, 0f, 0f, this.a);
		switch (num2)
		{
		case 0:
			color.r = num4;
			color.g = num7;
			color.b = num5;
			break;
		case 1:
			color.r = num6;
			color.g = num4;
			color.b = num5;
			break;
		case 2:
			color.r = num5;
			color.g = num4;
			color.b = num7;
			break;
		case 3:
			color.r = num5;
			color.g = num6;
			color.b = num4;
			break;
		case 4:
			color.r = num7;
			color.g = num5;
			color.b = num4;
			break;
		default:
			color.r = num4;
			color.g = num5;
			color.b = num6;
			break;
		}
		return color;
	}

	public new string ToString()
	{
		return string.Format("h: {0:0.00}, s: {1:0.00}, v: {2:0.00}, a: {3:0.00}", new object[] { this.h, this.s, this.v, this.a });
	}

	public static Color Random()
	{
		ColourHSV colourHSV = new ColourHSV(global::UnityEngine.Random.Range(0f, 360f), 1f, 1f);
		return colourHSV.ToColor();
	}

	private float h;

	private float s;

	private float v;

	private float a;
}
