using System;

public struct BatchGroupKey
{
	public BatchGroupKey(BatchKey batchKey)
	{
		this._groupID = batchKey.groupID;
		this._materialType = batchKey.materialType;
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
}
