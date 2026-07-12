using System;

public readonly struct Result<TSuccess, TError>
{
	private Result(TSuccess successValue, TError errorValue)
	{
		this.successValue = successValue;
		this.errorValue = errorValue;
	}

	public bool IsOk()
	{
		return this.successValue.IsSome();
	}

	public bool IsErr()
	{
		return this.errorValue.IsSome() || this.successValue.IsNone();
	}

	public TSuccess Unwrap()
	{
		if (this.successValue.IsSome())
		{
			return this.successValue.Unwrap();
		}
		if (this.errorValue.IsSome())
		{
			throw new Exception("Tried to unwrap result that is an Err()");
		}
		throw new Exception("Tried to unwrap result that isn't initialized with an Err() or Ok() value");
	}

	public Option<TSuccess> Ok()
	{
		return this.successValue;
	}

	public Option<TError> Err()
	{
		return this.errorValue;
	}

	public static implicit operator Result<TSuccess, TError>(Result.Internal.Value_Ok<TSuccess> value)
	{
		return new Result<TSuccess, TError>(value.value, default(TError));
	}

	public static implicit operator Result<TSuccess, TError>(Result.Internal.Value_Err<TError> value)
	{
		return new Result<TSuccess, TError>(default(TSuccess), value.value);
	}

	private readonly Option<TSuccess> successValue;

	private readonly Option<TError> errorValue;
}
