using System;

public struct BatchKey : IEquatable<BatchKey>
{
	private BatchKey(KAnimConverter.IAnimConverter controller)
	{
		this._layer = controller.GetLayer();
		this._groupID = controller.GetBatchGroupID(false);
		this._materialType = controller.GetMaterialType();
		this._z = controller.GetZ();
		this._idx = KAnimBatchManager.ControllerToChunkXY(controller);
		this._hash = 0;
	}

	private BatchKey(KAnimConverter.IAnimConverter controller, Vector2I idx)
	{
		this = new BatchKey(controller);
		this._idx = idx;
	}

	private void CalculateHash()
	{
		this._hash = this._z.GetHashCode() ^ this._layer ^ (int)this._materialType ^ this._groupID.HashValue ^ this._idx.GetHashCode();
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
		string[] array = new string[12];
		array[0] = "[";
		int num = 1;
		Vector2I vector2I = this.idx;
		array[num] = vector2I.x.ToString();
		array[2] = ",";
		int num2 = 3;
		vector2I = this.idx;
		array[num2] = vector2I.y.ToString();
		array[4] = "] [";
		array[5] = this.groupID.HashValue.ToString();
		array[6] = "] [";
		array[7] = this.layer.ToString();
		array[8] = "] [";
		array[9] = this.z.ToString();
		array[10] = "]";
		array[11] = this.materialType.ToString();
		return string.Concat(array);
	}

	private float _z;

	private int _layer;

	private KAnimBatchGroup.MaterialType _materialType;

	private HashedString _groupID;

	private Vector2I _idx;

	private int _hash;
}
