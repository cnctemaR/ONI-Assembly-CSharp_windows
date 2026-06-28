using System;

public struct BatchGroupKey : IEquatable<BatchGroupKey>
{
	public BatchGroupKey(BatchKey batchKey)
	{
		this._groupID = batchKey.groupID;
		this._materialType = batchKey.materialType;
		this._hash = this._materialType.GetHashCode() ^ this._groupID.GetHashCode();
	}

	public bool Equals(BatchGroupKey other)
	{
		return this._materialType == other._materialType && this._groupID == other._groupID;
	}

	public override int GetHashCode()
	{
		return this._hash;
	}

	public HashedString groupID
	{
		get
		{
			return this._groupID;
		}
	}

	public KAnimBatchGroup.MaterialType materialType
	{
		get
		{
			return this._materialType;
		}
	}

	private KAnimBatchGroup.MaterialType _materialType;

	private HashedString _groupID;

	private int _hash;
}
