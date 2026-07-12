using System;

public static class Result
{
	public static Result.Internal.Value_Ok<T> Ok<T>(T value)
	{
		return new Result.Internal.Value_Ok<T>(value);
	}

	public static Result.Internal.Value_Err<T> Err<T>(T value)
	{
		return new Result.Internal.Value_Err<T>(value);
	}

	public static class Internal
	{
		public readonly struct Value_Ok<T>
		{
			public Value_Ok(T value)
			{
				this.value = value;
			}

			public readonly T value;
		}

		public readonly struct Value_Err<T>
		{
			public Value_Err(T value)
			{
				this.value = value;
			}

			public readonly T value;
		}
	}
}
