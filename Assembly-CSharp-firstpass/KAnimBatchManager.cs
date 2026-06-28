using System;
using System.Collections.Generic;
using UnityEngine;

public class KAnimBatchManager
{
	public int dirtyBatchLastFrame { get; private set; }

	public static KAnimBatchManager Instance()
	{
		if (KAnimBatchManager.instance == null)
		{
			KAnimBatchManager.instance = new KAnimBatchManager();
			if (!SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat))
			{
				global::Debug.LogError("Machine does not support RGBAFloat32", null);
			}
		}
		return KAnimBatchManager.instance;
	}

	public static void Destroy()
	{
		if (KAnimBatchManager.instance != null)
		{
			KAnimBatchManager.instance.ready = false;
			foreach (KeyValuePair<BatchGroupKey, KAnimBatchGroup> keyValuePair in KAnimBatchManager.instance.batchGroups)
			{
				keyValuePair.Value.Finalise();
			}
			KAnimBatchManager.instance.batchGroups.Clear();
			KAnimBatchManager.instance.batchGroupData.Clear();
			KAnimBatchManager.instance.activeBatchSets.Clear();
			KAnimBatchManager.instance.batchSets.Clear();
			KAnimBatchManager.instance.inactiveBatchSets.Clear();
			KAnimBatchManager.instance.dirtyBatchLastFrame = 0;
			KAnimBatchManager.instance = null;
			KAnimBatchGroup.FinalizeTextureCache();
		}
	}

	public void ClearMultiInstances()
	{
		List<BatchGroupKey> list = new List<BatchGroupKey>();
		foreach (KeyValuePair<BatchGroupKey, KAnimBatchGroup> keyValuePair in this.batchGroups)
		{
			if (keyValuePair.Value.data != null && keyValuePair.Value.data.isDynamic)
			{
				list.Add(keyValuePair.Key);
			}
			keyValuePair.Value.ClearMuiltiInstanceData();
		}
		foreach (BatchGroupKey batchGroupKey in list)
		{
			if (this.batchGroupData.ContainsKey(batchGroupKey.groupID))
			{
				this.batchGroupData.Remove(batchGroupKey.groupID);
			}
			this.batchGroups[batchGroupKey].Finalise();
			this.batchGroups.Remove(batchGroupKey);
		}
	}

	public bool isReady
	{
		get
		{
			return this.ready;
		}
	}

	public HashSet<BatchSet> GetActiveBatchSets()
	{
		return this.activeBatchSets;
	}

	public KBatchGroupData GetBatchGroupData(HashedString groupID, bool isDynamic = false)
	{
		if (!groupID.isValid || groupID == KAnimBatchManager.NO_BATCH || groupID == KAnimBatchManager.IGNORE)
		{
			return null;
		}
		if (!this.batchGroupData.ContainsKey(groupID))
		{
			this.batchGroupData[groupID] = new KBatchGroupData(groupID, isDynamic);
		}
		return this.batchGroupData[groupID];
	}

	public KAnimBatchGroup GetBatchGroup(KAnimConverter.IAnimConverter controller)
	{
		BatchKey batchKey = new BatchKey(controller);
		return this.GetBatchGroup(batchKey);
	}

	public KAnimBatchGroup GetBatchGroup(BatchKey batch_key)
	{
		BatchGroupKey batchGroupKey = new BatchGroupKey(batch_key);
		KAnimBatchGroup kanimBatchGroup = null;
		if (!this.batchGroups.TryGetValue(batchGroupKey, out kanimBatchGroup))
		{
			kanimBatchGroup = new KAnimBatchGroup(batchGroupKey.groupID, batchGroupKey.materialType);
			this.batchGroups.Add(batchGroupKey, kanimBatchGroup);
		}
		return kanimBatchGroup;
	}

	public static Vector2I CellXYToChunkXY(Vector2I cell_xy)
	{
		return new Vector2I(cell_xy.x / 16, cell_xy.y / 16);
	}

	public void MoveChunk(KAnimConverter.IAnimConverter controller, Vector2I lastChunkXY, Vector2I newChunkXY)
	{
		BatchKey batchKey = new BatchKey(controller, newChunkXY);
		KAnimBatch batch = controller.GetBatch();
		BatchSet batchSet;
		if (!this.batchSets.TryGetValue(batchKey, out batchSet))
		{
			batchSet = new BatchSet(this.GetBatchGroup(batchKey), batchKey, newChunkXY);
			this.batchSets[batchKey] = batchSet;
		}
		batchSet.AddBatch(batch);
	}

	public void Register(KAnimConverter.IAnimConverter controller)
	{
		BatchKey batchKey = new BatchKey(controller);
		Vector2I cellXY = controller.GetCellXY();
		Vector2I vector2I = KAnimBatchManager.CellXYToChunkXY(cellXY);
		BatchSet batchSet;
		if (!this.batchSets.TryGetValue(batchKey, out batchSet))
		{
			batchSet = new BatchSet(this.GetBatchGroup(batchKey), batchKey, vector2I);
			this.batchSets[batchKey] = batchSet;
		}
		batchSet.Add(controller);
	}

	private void AddToActiveBatchSet(BatchSet bs)
	{
		this.inactiveBatchSets.Remove(bs);
		this.activeBatchSets.Add(bs);
		bs.SetActive(true);
	}

	private void AddToInactiveBatchSet(BatchSet bs)
	{
		this.inactiveBatchSets.Add(bs);
		this.activeBatchSets.Remove(bs);
		bs.SetActive(false);
	}

	public void UpdateActiveArea(Vector2I vis_chunk_min, Vector2I vis_chunk_max)
	{
		this.currentActiveArea.SetMinMax(new Vector3((float)vis_chunk_min.x, (float)vis_chunk_min.y), new Vector3((float)vis_chunk_max.x, (float)vis_chunk_max.y));
		foreach (KeyValuePair<BatchKey, BatchSet> keyValuePair in this.batchSets)
		{
			BatchSet value = keyValuePair.Value;
			if (value.key.materialType == KAnimBatchGroup.MaterialType.UI || (value.batchCount > 0 && this.currentActiveArea.Intersects(value.bounds)))
			{
				if (!value.active || !this.activeBatchSets.Contains(value))
				{
					this.AddToActiveBatchSet(value);
				}
			}
			else if (value.active || this.activeBatchSets.Contains(value))
			{
				this.AddToInactiveBatchSet(value);
			}
		}
	}

	public int UpdateDirty(int frame)
	{
		if (!this.ready)
		{
			return 0;
		}
		this.dirtyBatchLastFrame = 0;
		foreach (BatchSet batchSet in this.activeBatchSets)
		{
			try
			{
				this.dirtyBatchLastFrame += batchSet.UpdateDirty(frame);
			}
			catch (Exception ex)
			{
				global::Debug.LogError("KAnimBatchManager.UpdateDirty " + ex.Message + "\n" + ex.StackTrace, null);
			}
		}
		return this.dirtyBatchLastFrame;
	}

	public void Render()
	{
		if (!this.ready)
		{
			return;
		}
		foreach (BatchSet batchSet in this.activeBatchSets)
		{
			if (batchSet.active && batchSet.group.dataType == KAnimBatchGroup.DataType.Default)
			{
				Mesh mesh = batchSet.group.mesh;
				Material material = batchSet.group.material;
				for (int i = 0; i < batchSet.batchCount; i++)
				{
					KAnimBatch batch = batchSet.GetBatch(i);
					if (batch.size != 0 && batch.active)
					{
						if (batch.materialType != KAnimBatchGroup.MaterialType.UI)
						{
							Vector3 zero = Vector3.zero;
							zero.z = batch.position.z;
							int layer = batch.layer;
							Graphics.DrawMesh(mesh, zero, Quaternion.identity, material, layer, null, 0, batch.matProperties);
						}
					}
				}
			}
		}
	}

	public void CompleteInit()
	{
		this.ready = true;
	}

	private const int DEFAULT_BATCH_SIZE = 60;

	public const int CHUNK_SIZE = 16;

	public static HashedString NO_BATCH = new HashedString("NO_BATCH");

	public static HashedString IGNORE = new HashedString("IGNORE");

	public static Vector2 GROUP_SIZE = new Vector2(16f, 16f);

	private bool ready;

	private Bounds currentActiveArea = default(Bounds);

	private Dictionary<HashedString, KBatchGroupData> batchGroupData = new Dictionary<HashedString, KBatchGroupData>();

	private Dictionary<BatchGroupKey, KAnimBatchGroup> batchGroups = new Dictionary<BatchGroupKey, KAnimBatchGroup>();

	private Dictionary<BatchKey, BatchSet> batchSets = new Dictionary<BatchKey, BatchSet>();

	private HashSet<BatchSet> activeBatchSets = new HashSet<BatchSet>();

	private HashSet<BatchSet> inactiveBatchSets = new HashSet<BatchSet>();

	private static KAnimBatchManager instance;
}
