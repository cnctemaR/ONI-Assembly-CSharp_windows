using System;
using System.Collections.Generic;
using UnityEngine;

public class BatchSet
{
	public BatchSet(KAnimBatchGroup batchGroup, BatchKey batchKey, Vector2I spacialIdx)
	{
		this.idx = spacialIdx;
		this.key = batchKey;
		this.dirty = true;
		this.active = true;
		this.group = batchGroup;
		this.bounds = new Bounds(new Vector2((float)this.idx.x + 0.5f, (float)this.idx.y + 0.5f), Vector2.one);
		this.batches = new List<KAnimBatch>();
	}

	public KAnimBatchGroup group { get; private set; }

	private protected List<KAnimBatch> batches { protected get; private set; }

	public Bounds bounds { get; private set; }

	public Vector2I idx { get; private set; }

	public BatchKey key { get; private set; }

	public bool dirty { get; private set; }

	public bool active { get; private set; }

	public int batchCount
	{
		get
		{
			return this.batches.Count;
		}
	}

	public int dirtyBatchLastFrame { get; private set; }

	public void Clear()
	{
		this.group = null;
		for (int i = 0; i < this.batches.Count; i++)
		{
			if (this.batches[i] != null)
			{
				this.batches[i].Clear();
			}
		}
		this.batches.Clear();
	}

	public KAnimBatch GetBatch(int idx)
	{
		return this.batches[idx];
	}

	public void Add(KAnimConverter.IAnimConverter controller)
	{
		if (this.group.dataType == KAnimBatchGroup.DataType.DontRender || this.group.dataType == KAnimBatchGroup.DataType.AnimOnly)
		{
			global::Debug.LogWarning(string.Concat(new object[]
			{
				"Cant add [",
				controller.GetName(),
				"] to group [",
				this.group.batchID,
				"]"
			}), null);
			return;
		}
		int layer = controller.GetLayer();
		if (layer != this.key.layer)
		{
			global::Debug.LogError("Registering with wrong batch set (layer) " + controller.GetName(), null);
		}
		HashedString batchGroupID = controller.GetBatchGroupID(false);
		if (!(batchGroupID == this.key.groupID))
		{
			global::Debug.LogError("Registering with wrong batch set (groupID) " + controller.GetName(), null);
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
		KAnimBatch kanimBatch = new KAnimBatch(this.group, layer, controller.GetZ(), materialType);
		kanimBatch.Init();
		this.AddBatch(kanimBatch);
		kanimBatch.Register(controller);
	}

	public void RemoveBatch(KAnimBatch batch)
	{
		if (this.batches.Contains(batch))
		{
			this.group.batchCount--;
			this.batches.Remove(batch);
			batch.SetBatchSet(null);
		}
	}

	public void AddBatch(KAnimBatch batch)
	{
		if (batch.batchset != this)
		{
			if (batch.batchset != null)
			{
				batch.batchset.RemoveBatch(batch);
			}
			batch.SetBatchSet(this);
			if (!this.batches.Contains(batch))
			{
				this.group.batchCount++;
				this.batches.Add(batch);
				this.batches.Sort((KAnimBatch b0, KAnimBatch b1) => b0.position.z.CompareTo(b1.position.z));
			}
		}
		this.SetDirty();
	}

	public void SetDirty()
	{
		this.dirty = true;
	}

	public void SetActive(bool isActive)
	{
		if (isActive != this.active)
		{
			this.batches.RemoveAll((KAnimBatch b) => b == null);
			if (!isActive)
			{
				for (int i = 0; i < this.batches.Count; i++)
				{
					if (this.batches[i] != null)
					{
						this.batches[i].Deactivate();
					}
				}
			}
			else
			{
				for (int j = 0; j < this.batches.Count; j++)
				{
					if (this.batches[j] != null)
					{
						this.batches[j].Activate();
					}
				}
				this.SetDirty();
			}
		}
		this.active = isActive;
	}

	public int lastDirtyFrame { get; private set; }

	public int UpdateDirty(int frame)
	{
		this.dirtyBatchLastFrame = 0;
		if (this.dirty)
		{
			for (int i = 0; i < this.batches.Count; i++)
			{
				try
				{
					this.dirtyBatchLastFrame += this.batches[i].UpdateDirty(frame);
				}
				catch (Exception ex)
				{
					global::Debug.LogError("BatchSet.UpdateDirty: " + ex.Message + "\n" + ex.StackTrace, null);
				}
			}
			this.lastDirtyFrame = frame;
			this.dirty = false;
		}
		return this.dirtyBatchLastFrame;
	}
}
