using System;
using System.Collections.Generic;
using UnityEngine;

public class BatchSet
{
	public BatchSet(KAnimBatchGroup group, BatchKey key, Vector2I spacialIdx)
	{
		this.idx = spacialIdx;
		this.key = key;
		this.dirty = true;
		this.active = true;
		this.group = group;
		this.bounds = new Bounds(new Vector2((float)this.idx.x + 0.5f, (float)this.idx.y + 0.5f), new Vector2(1f, 1f));
		this.batches = new List<KAnimBatch>();
	}

	public KAnimBatchGroup group { get; private set; }

	public List<KAnimBatch> batches { get; private set; }

	public Bounds bounds { get; private set; }

	public Vector2I idx { get; private set; }

	public BatchKey key { get; private set; }

	public bool dirty { get; private set; }

	public bool active { get; private set; }

	public int dirtyBatchLastFrame { get; private set; }

	public void Add(KAnimConverter.IAnimConverter controller)
	{
		if (this.group.dataType == KAnimBatchGroup.DataType.DontRender || this.group.dataType == KAnimBatchGroup.DataType.AnimOnly)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"Cant add [",
				controller.GetName(),
				"] to group [",
				this.group.batchID,
				"]"
			}));
			return;
		}
		int layer = controller.GetLayer();
		if (layer != this.key.layer)
		{
			Debug.LogError("Registering with wrong batch set (layer) " + controller.GetName());
		}
		HashedString batchGroupID = controller.GetBatchGroupID(false);
		if (!(batchGroupID == this.key.groupID))
		{
			Debug.LogError("Registering with wrong batch set (groupID) " + controller.GetName());
		}
		KAnimBatchGroup.MaterialType materialType = controller.GetMaterialType();
		for (int i = 0; i < this.batches.Count; i++)
		{
			if (this.batches[i].size < this.group.maxGroupSize && this.batches[i].materialType == materialType)
			{
				if (this.batches[i].Register(controller))
				{
					this.SetDirty();
				}
				return;
			}
		}
		KAnimBatch kanimBatch = new KAnimBatch(this.group, this, layer, controller.GetPosition().z, this.idx, materialType);
		kanimBatch.Init();
		this.group.batchCount++;
		this.batches.Add(kanimBatch);
		this.batches.Sort((KAnimBatch b0, KAnimBatch b1) => b0.position.z.CompareTo(b1.position.z));
		kanimBatch.Register(controller);
		this.SetDirty();
	}

	public void RemoveBatch(KAnimBatch batch)
	{
		if (this.group == batch.group && batch.size == 0 && this.group.batchCount > 1)
		{
			this.group.batchCount--;
			this.batches.Remove(batch);
			batch.DestroyTex();
		}
	}

	public void SetDirty()
	{
		this.dirty = true;
	}

	public void SetActive(bool isActive)
	{
		if (isActive != this.active)
		{
			if (this.active)
			{
				for (int i = 0; i < this.batches.Count; i++)
				{
					this.batches[i].Deactivate();
				}
			}
			else
			{
				for (int j = 0; j < this.batches.Count; j++)
				{
					this.batches[j].Activate();
				}
				this.SetDirty();
			}
		}
		this.active = isActive;
	}

	public int UpdateDirty()
	{
		this.dirtyBatchLastFrame = 0;
		if (this.dirty)
		{
			for (int i = 0; i < this.batches.Count; i++)
			{
				this.dirtyBatchLastFrame += this.batches[i].UpdateDirty();
			}
			this.dirty = false;
		}
		return this.dirtyBatchLastFrame;
	}
}
