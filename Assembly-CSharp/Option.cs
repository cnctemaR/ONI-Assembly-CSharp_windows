using System;
using System.Collections.Generic;
using System.Diagnostics;
using KSerialization;

[DebuggerDisplay("has_value={hasValue} {value}")]
[Serializable]
public readonly struct Option<T> : IEquatable<Option<T>>, IEquatable<T>
{
	public Option(T value)
	{
		this.value = value;
		this.hasValue = true;
	}

	public bool HasValue
	{
		get
		{
			return this.hasValue;
		}
	}

	public T Value
	{
		get
		{
			return this.Unwrap();
		}
	}

	public T Unwrap()
	{
		if (!this.hasValue)
		{
			throw new Exception("Tried to get a value for a Option<" + typeof(T).FullName + ">, but hasValue is false");
		}
		return this.value;
	}

	public T UnwrapOr(T fallback_value, string warn_on_fallback = null)
	{
		if (!this.hasValue)
		{
			if (warn_on_fallback != null)
			{
				DebugUtil.DevAssert(false, "Failed to unwrap a Option<" + typeof(T).FullName + ">: " + warn_on_fallback, null);
			}
			return fallback_value;
		}
		return this.value;
	}

	public T UnwrapOrElse(Func<T> get_fallback_value_fn, string warn_on_fallback = null)
	{
		if (!this.hasValue)
		{
			if (warn_on_fallback != null)
			{
				DebugUtil.DevAssert(false, "Failed to unwrap a Option<" + typeof(T).FullName + ">: " + warn_on_fallback, null);
			}
			return get_fallback_value_fn();
		}
		return this.value;
	}

	public T UnwrapOrDefault()
	{
		if (!this.hasValue)
		{
			return default(T);
		}
		return this.value;
	}

	public T Expect(string msg_on_fail)
	{
		if (!this.hasValue)
		{
			throw new Exception(msg_on_fail);
		}
		return this.value;
	}

	public bool IsSome()
	{
		return this.hasValue;
	}

	public bool IsNone()
	{
		return !this.hasValue;
	}

	public Option<U> AndThen<U>(Func<T, U> fn)
	{
		if (this.IsNone())
		{
			return Option.None;
		}
		return Option.Maybe<U>(fn(this.value));
	}

	public Option<U> AndThen<U>(Func<T, Option<U>> fn)
	{
		if (this.IsNone())
		{
			return Option.None;
		}
		return fn(this.value);
	}

	public static implicit operator Option<T>(T value)
	{
		return Option.Maybe<T>(value);
	}

	public static explicit operator T(Option<T> option)
	{
		return option.Unwrap();
	}

	public static implicit operator Option<T>(Option.Internal.Value_None value)
	{
		return default(Option<T>);
	}

	public static implicit operator Option.Internal.Value_HasValue(Option<T> value)
	{
		return new Option.Internal.Value_HasValue(value.hasValue);
	}

	public void Deconstruct(out bool hasValue, out T value)
	{
		hasValue = this.hasValue;
		value = this.value;
	}

	public bool Equals(Option<T> other)
	{
		return EqualityComparer<bool>.Default.Equals(this.hasValue, other.hasValue) && EqualityComparer<T>.Default.Equals(this.value, other.value);
	}

	public override bool Equals(object obj)
	{
		if (obj is Option<T>)
		{
			Option<T> option = (Option<T>)obj;
			return this.Equals(option);
		}
		return false;
	}

	public static bool operator ==(Option<T> lhs, Option<T> rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(Option<T> lhs, Option<T> rhs)
	{
		return !(lhs == rhs);
	}

	public override int GetHashCode()
	{
		return (-363764631 * -1521134295 + this.hasValue.GetHashCode()) * -1521134295 + EqualityComparer<T>.Default.GetHashCode(this.value);
	}

	public override string ToString()
	{
		if (!this.hasValue)
		{
			return "None";
		}
		return string.Format("{0}", this.value);
	}

	public static bool operator ==(Option<T> lhs, T rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(Option<T> lhs, T rhs)
	{
		return !(lhs == rhs);
	}

	public static bool operator ==(T lhs, Option<T> rhs)
	{
		return rhs.Equals(lhs);
	}

	public static bool operator !=(T lhs, Option<T> rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(T other)
	{
		return this.HasValue && EqualityComparer<T>.Default.Equals(this.value, other);
	}

	[Serialize]
	private readonly bool hasValue;

	[Serialize]
	private readonly T value;
}
