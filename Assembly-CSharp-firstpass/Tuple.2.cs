using System;
using System.Collections.Generic;

public class Tuple<T, U, V> : IEquatable<global::Tuple<T, U, V>>
{
	public Tuple(T a, U b, V c)
	{
		this.first = a;
		this.second = b;
		this.third = c;
	}

	public override bool Equals(object obj)
	{
		global::Tuple<T, U, V> tuple = obj as global::Tuple<T, U, V>;
		return tuple != null && EqualityComparer<T>.Default.Equals(this.first, tuple.first) && EqualityComparer<U>.Default.Equals(this.second, tuple.second) && EqualityComparer<V>.Default.Equals(this.third, tuple.third);
	}

	public bool Equals(global::Tuple<T, U, V> other)
	{
		return EqualityComparer<T>.Default.Equals(this.first, other.first) && EqualityComparer<U>.Default.Equals(this.second, other.second) && EqualityComparer<V>.Default.Equals(this.third, other.third);
	}

	public override int GetHashCode()
	{
		return ((1888190068 * -1521134295 + EqualityComparer<T>.Default.GetHashCode(this.first)) * -1521134295 + EqualityComparer<U>.Default.GetHashCode(this.second)) * -1521134295 + EqualityComparer<V>.Default.GetHashCode(this.third);
	}

	public T first;

	public U second;

	public V third;
}
