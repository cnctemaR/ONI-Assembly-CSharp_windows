using System;

public struct BatchKey
{
	public BatchKey(KAnimConverter.IAnimConverter controller)
	{
		this._layer = controller.GetLayer();
		this._groupID = controller.GetBatchGroupID(false);
		this._materialType = controller.GetMaterialType();
		this._z = controller.GetZ();
		this._idx = KAnimBatchManager.CellXYToChunkXY(controller.GetCellXY());
	}

	public BatchKey(KAnimConverter.IAnimConverter controller, Vector2I idx)
	{
		this = new BatchKey(controller);
		this._idx = idx;
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
}
