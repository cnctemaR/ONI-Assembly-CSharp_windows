using System;

[Serializable]
public struct EffectorValues
{
	public EffectorValues(int amt, int rad)
	{
		this.amount = amt;
		this.radius = rad;
	}

	public override bool Equals(object obj)
	{
		return obj is EffectorValues && this.Equals((EffectorValues)obj);
	}

	public bool Equals(EffectorValues p)
	{
		return p != null && (this == p || (!(base.GetType() != p.GetType()) && this.amount == p.amount && this.radius == p.radius));
	}

	public override int GetHashCode()
	{
		return this.amount ^ this.radius;
	}

	public static bool operator ==(EffectorValues lhs, EffectorValues rhs)
	{
		if (lhs == null)
		{
			return rhs == null;
		}
		return lhs.Equals(rhs);
	}

	public static bool operator !=(EffectorValues lhs, EffectorValues rhs)
	{
		return !(lhs == rhs);
	}

	public int amount;

	public int radius;
}
