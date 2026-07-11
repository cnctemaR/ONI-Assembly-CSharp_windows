using System;

public class Tuple<T, U> : IEquatable<Tuple<T, U>>
{
	public Tuple(T a, U b)
	{
		this.first = a;
		this.second = b;
	}

	public bool Equals(Tuple<T, U> other)
	{
		return this.first.Equals(other.first) && this.second.Equals(other.second);
	}

	public override int GetHashCode()
	{
		return this.first.GetHashCode() ^ this.second.GetHashCode();
	}

	public T first;

	public U second;
}
