using System;

public class Tuple<T, U, V> : IEquatable<global::Tuple<T, U, V>>
{
	public Tuple(T a, U b, V c)
	{
		this.first = a;
		this.second = b;
		this.third = c;
	}

	public bool Equals(global::Tuple<T, U, V> other)
	{
		return this.first.Equals(other.first) && this.second.Equals(other.second) && this.third.Equals(other.third);
	}

	public override int GetHashCode()
	{
		return this.first.GetHashCode() ^ this.second.GetHashCode() ^ this.third.GetHashCode();
	}

	public T first;

	public U second;

	public V third;
}
