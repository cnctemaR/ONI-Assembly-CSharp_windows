using System;

public struct PrioritySetting : IComparable<PrioritySetting>
{
	public override int GetHashCode()
	{
		return ((int)((int)this.priority_class << 28)).GetHashCode() ^ this.priority_value.GetHashCode();
	}

	public static bool operator ==(PrioritySetting lhs, PrioritySetting rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(PrioritySetting lhs, PrioritySetting rhs)
	{
		return !lhs.Equals(rhs);
	}

	public static bool operator <=(PrioritySetting lhs, PrioritySetting rhs)
	{
		return lhs.CompareTo(rhs) <= 0;
	}

	public static bool operator >=(PrioritySetting lhs, PrioritySetting rhs)
	{
		return lhs.CompareTo(rhs) >= 0;
	}

	public static bool operator <(PrioritySetting lhs, PrioritySetting rhs)
	{
		return lhs.CompareTo(rhs) < 0;
	}

	public static bool operator >(PrioritySetting lhs, PrioritySetting rhs)
	{
		return lhs.CompareTo(rhs) > 0;
	}

	public override bool Equals(object obj)
	{
		return obj is PrioritySetting && ((PrioritySetting)obj).priority_class == this.priority_class && ((PrioritySetting)obj).priority_value == this.priority_value;
	}

	public int CompareTo(PrioritySetting other)
	{
		if (this.priority_class > other.priority_class)
		{
			return 1;
		}
		if (this.priority_class < other.priority_class)
		{
			return -1;
		}
		if (this.priority_value > other.priority_value)
		{
			return 1;
		}
		if (this.priority_value < other.priority_value)
		{
			return -1;
		}
		return 0;
	}

	public PrioritySetting(PriorityScreen.PriorityClass priority_class, int priority_value)
	{
		this.priority_class = priority_class;
		this.priority_value = priority_value;
	}

	public PriorityScreen.PriorityClass priority_class;

	public int priority_value;
}
