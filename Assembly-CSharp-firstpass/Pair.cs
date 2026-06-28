using System;

[Serializable]
public struct Pair<T, U> : IEquatable<Pair<T, U>>
{
	public Pair(T a, U b)
	{
		this.first = a;
		this.second = b;
	}

	public bool Equals(Pair<T, U> other)
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
