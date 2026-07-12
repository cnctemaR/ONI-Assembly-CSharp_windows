using System;
using System.Collections.Generic;
using System.Diagnostics;
using KSerialization;

[DebuggerDisplay("has_value={hasValue} {value}")]
[Serializable]
public readonly struct Option<T> : IEquatable<Option<T>>, IEquatable<T>
{
	public bool HasValue
	{
		get
		{
			return this.hasValue;
		}
	}

	public bool IsNone
	{
		get
		{
			return !this.hasValue;
		}
	}

	public T Value
	{
		get
		{
			if (this.IsNone)
			{
				throw new Exception("Tried to get a value for a Option<" + typeof(T).Name + ">, but IsNone is true");
			}
			return this.value;
		}
	}

	public Option(T value)
	{
		this.value = value;
		this.hasValue = true;
	}

	public static implicit operator Option<T>(T value)
	{
		return new Option<T>(value);
	}

	public static implicit operator T(Option<T> option)
	{
		return option.Value;
	}

	public static implicit operator Option<T>(Option.Value_None value)
	{
		return default(Option<T>);
	}

	public static implicit operator Option.Value_HasValue(Option<T> value)
	{
		return new Option.Value_HasValue(value.hasValue);
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

	public Option<U> IfHasValue<U>(Func<T, Option<U>> fn)
	{
		if (this.IsNone)
		{
			return Option.None;
		}
		return fn(this.Value);
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
		return lhs.Equals(rhs);
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
