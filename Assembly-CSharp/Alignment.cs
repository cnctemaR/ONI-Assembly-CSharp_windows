using System;
using UnityEngine;

public readonly struct Alignment
{
	public Alignment(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	public static Alignment Custom(float x, float y)
	{
		return new Alignment(x, y);
	}

	public static Alignment TopLeft()
	{
		return new Alignment(0f, 1f);
	}

	public static Alignment Top()
	{
		return new Alignment(0.5f, 1f);
	}

	public static Alignment TopRight()
	{
		return new Alignment(1f, 1f);
	}

	public static Alignment Left()
	{
		return new Alignment(0f, 0.5f);
	}

	public static Alignment Center()
	{
		return new Alignment(0.5f, 0.5f);
	}

	public static Alignment Right()
	{
		return new Alignment(1f, 0.5f);
	}

	public static Alignment BottomLeft()
	{
		return new Alignment(0f, 0f);
	}

	public static Alignment Bottom()
	{
		return new Alignment(0.5f, 0f);
	}

	public static Alignment BottomRight()
	{
		return new Alignment(1f, 0f);
	}

	public static implicit operator Vector2(Alignment a)
	{
		return new Vector2(a.x, a.y);
	}

	public static implicit operator Alignment(Vector2 v)
	{
		return new Alignment(v.x, v.y);
	}

	public readonly float x;

	public readonly float y;
}
