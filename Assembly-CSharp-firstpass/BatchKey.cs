using System;

public struct BatchKey : IEquatable<BatchKey>
{
	private BatchKey(KAnimConverter.IAnimConverter controller)
	{
		this._layer = controller.GetLayer();
		this._groupID = controller.GetBatchGroupID(false);
		this._materialType = controller.GetMaterialType();
		this._z = controller.GetZ();
		this._idx = KAnimBatchManager.CellXYToChunkXY(controller.GetCellXY());
		this._hash = 0;
	}

	private BatchKey(KAnimConverter.IAnimConverter controller, Vector2I idx)
	{
		this = new BatchKey(controller);
		this._idx = idx;
	}

	private void CalculateHash()
	{
		this._hash = this._z.GetHashCode() ^ this._layer.GetHashCode() ^ this._materialType.GetHashCode() ^ this._groupID.GetHashCode() ^ this._idx.GetHashCode();
	}

	public static BatchKey Create(KAnimConverter.IAnimConverter controller, Vector2I idx)
	{
		BatchKey batchKey = new BatchKey(controller, idx);
		batchKey.CalculateHash();
		return batchKey;
	}

	public static BatchKey Create(KAnimConverter.IAnimConverter controller)
	{
		BatchKey batchKey = new BatchKey(controller);
		batchKey.CalculateHash();
		return batchKey;
	}

	public bool Equals(BatchKey other)
	{
		return this._z == other._z && this._layer == other._layer && this._materialType == other._materialType && this._groupID == other._groupID && this._idx == other._idx;
	}

	public override int GetHashCode()
	{
		return this._hash;
	}

	public float z
	{
		get
		{
			return this._z;
		}
	}

	public int layer
	{
		get
		{
			return this._layer;
		}
	}

	public HashedString groupID
	{
		get
		{
			return this._groupID;
		}
	}

	public Vector2I idx
	{
		get
		{
			return this._idx;
		}
	}

	public KAnimBatchGroup.MaterialType materialType
	{
		get
		{
			return this._materialType;
		}
	}

	public int hash
	{
		get
		{
			return this._hash;
		}
	}

	public override string ToString()
	{
		return string.Concat(new object[]
		{
			"[",
			this.idx.x,
			",",
			this.idx.y,
			"] [",
			this.groupID.HashValue,
			"] [",
			this.layer,
			"] [",
			this.z,
			"]",
			this.materialType.ToString()
		});
	}

	private float _z;

	private int _layer;

	private KAnimBatchGroup.MaterialType _materialType;

	private HashedString _groupID;

	private Vector2I _idx;

	private int _hash;
}
