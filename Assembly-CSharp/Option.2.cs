using System;

public static class Option
{
	public static Option<T> Some<T>(T value)
	{
		return new Option<T>(value);
	}

	public static Option.Value_None None
	{
		get
		{
			return default(Option.Value_None);
		}
	}

	public static bool AllHaveValues(params Option.Value_HasValue[] options)
	{
		if (options == null || options.Length == 0)
		{
			return false;
		}
		for (int i = 0; i < options.Length; i++)
		{
			if (!options[i].HasValue)
			{
				return false;
			}
		}
		return true;
	}

	public readonly ref struct Value_None
	{
	}

	public readonly struct Value_HasValue
	{
		public Value_HasValue(bool hasValue)
		{
			this.HasValue = hasValue;
		}

		public readonly bool HasValue;
	}
}
