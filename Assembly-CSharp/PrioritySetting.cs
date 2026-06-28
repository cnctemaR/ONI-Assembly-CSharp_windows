using System;

public struct PrioritySetting : IComparable
{
	public PrioritySetting(PriorityScreen.PriorityClass priority_class, int priority_value)
	{
		this.priority_class = priority_class;
		this.priority_value = priority_value;
	}

	public override int GetHashCode()
	{
		return ((int)((int)this.priority_class << 28)).GetHashCode() ^ this.priority_value.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		return obj is PrioritySetting && ((PrioritySetting)obj).priority_class == this.priority_class && ((PrioritySetting)obj).priority_value == this.priority_value;
	}

	public int CompareTo(object obj)
	{
		int num;
		if (!(obj is PrioritySetting))
		{
			num = 1;
		}
		else if (this.priority_class > ((PrioritySetting)obj).priority_class)
		{
			num = 1;
		}
		else if (this.priority_class < ((PrioritySetting)obj).priority_class)
		{
			num = -1;
		}
		else if (this.priority_value > ((PrioritySetting)obj).priority_value)
		{
			num = 1;
		}
		else if (this.priority_value < ((PrioritySetting)obj).priority_value)
		{
			num = -1;
		}
		else
		{
			num = 0;
		}
		return num;
	}

	public PriorityScreen.PriorityClass priority_class;

	public int priority_value;
}
