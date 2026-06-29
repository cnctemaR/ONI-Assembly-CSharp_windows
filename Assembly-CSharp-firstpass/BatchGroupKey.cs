using System;

public struct BatchGroupKey : IEquatable<BatchGroupKey>
{
	public BatchGroupKey(HashedString group_id)
	{
		this._groupID = group_id;
	}

	public bool Equals(BatchGroupKey other)
	{
		return this._groupID == other._groupID;
	}

	public override int GetHashCode()
	{
		return this._groupID.HashValue;
	}

	public HashedString groupID
	{
		get
		{
			return this._groupID;
		}
	}

	private HashedString _groupID;
}
