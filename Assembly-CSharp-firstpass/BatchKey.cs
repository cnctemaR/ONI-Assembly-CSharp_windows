using System;
using UnityEngine;

public struct BatchKey
{
	public BatchKey(KAnimConverter.IAnimConverter controller)
	{
		this._layer = controller.GetLayer();
		this._groupID = controller.GetBatchGroupID(false);
		this._materialType = controller.GetMaterialType();
		Vector3 position = controller.GetPosition();
		this._z = position.z;
		this._idx = KAnimBatchManager.GetBatchIndex(position);
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

	private float _z;

	private int _layer;

	private KAnimBatchGroup.MaterialType _materialType;

	private HashedString _groupID;

	private Vector2I _idx;
}
