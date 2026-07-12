using System;
using System.Collections.Generic;

public class Tuple<T, U> : IEquatable<global::Tuple<T, U>>
{
	public Tuple(T a, U b)
	{
		this.first = a;
		this.second = b;
	}

	public override bool Equals(object obj)
	{
		global::Tuple<T, U> tuple = obj as global::Tuple<T, U>;
		return tuple != null && EqualityComparer<T>.Default.Equals(this.first, tuple.first) && EqualityComparer<U>.Default.Equals(this.second, tuple.second);
	}

	public bool Equals(global::Tuple<T, U> other)
	{
		return EqualityComparer<T>.Default.Equals(this.first, other.first) && EqualityComparer<U>.Default.Equals(this.second, other.second);
	}

	public override int GetHashCode()
	{
		return (405212230 * -1521134295 + EqualityComparer<T>.Default.GetHashCode(this.first)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(this.second);
	}

	public T first;

	public U second;
}
