using System;

public static class BoxedBools
{
	public static Boxed<bool> Box(bool b)
	{
		if (!b)
		{
			return BoxedBools.False;
		}
		return BoxedBools.True;
	}

	public static readonly Boxed<bool> True = new Boxed<bool>
	{
		value = true
	};

	public static readonly Boxed<bool> False = new Boxed<bool>
	{
		value = false
	};
}
