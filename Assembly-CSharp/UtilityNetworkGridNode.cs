using System;

public struct UtilityNetworkGridNode : IEquatable<UtilityNetworkGridNode>
{
	public bool Equals(UtilityNetworkGridNode other)
	{
		return this.connections == other.connections && this.networkIdx == other.networkIdx;
	}

	public override bool Equals(object obj)
	{
		return ((UtilityNetworkGridNode)obj).Equals(this);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static bool operator ==(UtilityNetworkGridNode x, UtilityNetworkGridNode y)
	{
		return x.Equals(y);
	}

	public static bool operator !=(UtilityNetworkGridNode x, UtilityNetworkGridNode y)
	{
		return !x.Equals(y);
	}

	public UtilityConnections connections;

	public int networkIdx;

	public const int InvalidNetworkIdx = -1;
}
