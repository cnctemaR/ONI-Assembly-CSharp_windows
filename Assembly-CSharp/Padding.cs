using System;

public readonly struct Padding
{
	public float Width
	{
		get
		{
			return this.left + this.right;
		}
	}

	public float Height
	{
		get
		{
			return this.top + this.bottom;
		}
	}

	public Padding(float left, float right, float top, float bottom)
	{
		this.top = top;
		this.bottom = bottom;
		this.left = left;
		this.right = right;
	}

	public static Padding All(float padding)
	{
		return new Padding(padding, padding, padding, padding);
	}

	public static Padding Symmetric(float horizontal, float vertical)
	{
		return new Padding(horizontal, horizontal, vertical, vertical);
	}

	public static Padding Only(float left = 0f, float right = 0f, float top = 0f, float bottom = 0f)
	{
		return new Padding(left, right, top, bottom);
	}

	public static Padding Vertical(float vertical)
	{
		return new Padding(0f, 0f, vertical, vertical);
	}

	public static Padding Horizontal(float horizontal)
	{
		return new Padding(horizontal, horizontal, 0f, 0f);
	}

	public static Padding Top(float amount)
	{
		return new Padding(0f, 0f, amount, 0f);
	}

	public static Padding Left(float amount)
	{
		return new Padding(amount, 0f, 0f, 0f);
	}

	public static Padding Bottom(float amount)
	{
		return new Padding(0f, 0f, 0f, amount);
	}

	public static Padding Right(float amount)
	{
		return new Padding(0f, amount, 0f, 0f);
	}

	public static Padding operator +(Padding a, Padding b)
	{
		return new Padding(a.left + b.left, a.right + b.right, a.top + b.top, a.bottom + b.bottom);
	}

	public static Padding operator -(Padding a, Padding b)
	{
		return new Padding(a.left - b.left, a.right - b.right, a.top - b.top, a.bottom - b.bottom);
	}

	public static Padding operator *(float f, Padding p)
	{
		return p * f;
	}

	public static Padding operator *(Padding p, float f)
	{
		return new Padding(p.left * f, p.right * f, p.top * f, p.bottom * f);
	}

	public static Padding operator /(Padding p, float f)
	{
		return new Padding(p.left / f, p.right / f, p.top / f, p.bottom / f);
	}

	public readonly float top;

	public readonly float bottom;

	public readonly float left;

	public readonly float right;
}
