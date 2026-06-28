using System;
using System.Collections.Generic;
using UnityEngine;

public class KAnimBatchManager
{
	public int dirtyBatchLastFrame { get; private set; }

	public Bounds GetCurrentActiveArea()
	{
		return this.currentActiveArea;
	}

	public static KAnimBatchManager instance
	{
		get
		{
			return Singleton<KAnimBatchManager>.Instance;
		}
	}

	public static KAnimBatchManager Instance()
	{
		if (!KAnimBatchManager.created)
		{
			KAnimBatchManager.created = true;
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
				keyValuePair.Value.FreeResources();
			}
			KAnimBatchManager.instance.batchGroups.Clear();
			foreach (KeyValuePair<HashedString, KBatchGroupData> keyValuePair2 in KAnimBatchManager.instance.batchGroupData)
			{
				if (keyValuePair2.Value != null)
				{
					keyValuePair2.Value.FreeResources();
				}
			}
			KAnimBatchManager.instance.batchGroupData.Clear();
			foreach (KeyValuePair<BatchKey, BatchSet> keyValuePair3 in KAnimBatchManager.instance.batchSets)
			{
				if (keyValuePair3.Value != null)
				{
					keyValuePair3.Value.Clear();
				}
			}
			KAnimBatchManager.instance.batchSets.Clear();
			KAnimBatchManager.instance.activeBatchSets.Clear();
			KAnimBatchManager.instance.inactiveBatchSets.Clear();
			KAnimBatchManager.instance.dirtyBatchLastFrame = 0;
			KAnimBatchGroup.FinalizeTextureCache();
		}
		Singleton<KAnimBatchManager>.Destroy();
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
			this.batchGroups[batchGroupKey].FreeResources();
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
		KBatchGroupData kbatchGroupData;
		if (!groupID.isValid || groupID == KAnimBatchManager.NO_BATCH || groupID == KAnimBatchManager.IGNORE)
		{
			kbatchGroupData = null;
		}
		else
		{
			if (!this.batchGroupData.ContainsKey(groupID))
			{
				this.batchGroupData[groupID] = new KBatchGroupData(groupID, isDynamic);
			}
			kbatchGroupData = this.batchGroupData[groupID];
		}
		return kbatchGroupData;
	}

	public KAnimBatchGroup GetBatchGroup(BatchGroupKey group_key)
	{
		KAnimBatchGroup kanimBatchGroup = null;
		if (!this.batchGroups.TryGetValue(group_key, out kanimBatchGroup))
		{
			kanimBatchGroup = new KAnimBatchGroup(group_key.groupID);
			this.batchGroups.Add(group_key, kanimBatchGroup);
		}
		return kanimBatchGroup;
	}

	public static Vector2I CellXYToChunkXY(Vector2I cell_xy)
	{
		return new Vector2I(cell_xy.x / 32, cell_xy.y / 32);
	}

	public void MoveChunk(KAnimConverter.IAnimConverter controller, Vector2I lastChunkXY, Vector2I newChunkXY)
	{
		BatchKey batchKey = BatchKey.Create(controller, newChunkXY);
		KAnimBatch batch = controller.GetBatch();
		BatchSet batchSet;
		if (!this.batchSets.TryGetValue(batchKey, out batchSet))
		{
			batchSet = new BatchSet(this.GetBatchGroup(new BatchGroupKey(batchKey.groupID)), batchKey, newChunkXY);
			this.batchSets[batchKey] = batchSet;
		}
		batchSet.AddBatch(batch);
	}

	public void Register(KAnimConverter.IAnimConverter controller)
	{
		if (!this.isReady)
		{
			global::Debug.LogError(string.Format("Batcher isnt finished setting up, controller [{0}] is registering too early.", controller.GetName()), null);
		}
		BatchKey batchKey = BatchKey.Create(controller);
		Vector2I cellXY = controller.GetCellXY();
		Vector2I vector2I = KAnimBatchManager.CellXYToChunkXY(cellXY);
		BatchSet batchSet;
		if (!this.batchSets.TryGetValue(batchKey, out batchSet))
		{
			batchSet = new BatchSet(this.GetBatchGroup(new BatchGroupKey(batchKey.groupID)), batchKey, vector2I);
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
		int num;
		if (!this.ready)
		{
			num = 0;
		}
		else
		{
			this.dirtyBatchLastFrame = 0;
			foreach (BatchSet batchSet in this.activeBatchSets)
			{
				this.dirtyBatchLastFrame += batchSet.UpdateDirty(frame);
			}
			num = this.dirtyBatchLastFrame;
		}
		return num;
	}

	public void Render()
	{
		if (this.ready)
		{
			foreach (BatchSet batchSet in this.activeBatchSets)
			{
				if (batchSet.active && batchSet.group.dataType == KAnimBatchGroup.DataType.Default)
				{
					Mesh mesh = batchSet.group.mesh;
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
								Graphics.DrawMesh(mesh, zero, Quaternion.identity, batchSet.group.GetMaterial(batch.materialType), layer, null, 0, batch.matProperties);
							}
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

	public const int CHUNK_SIZE = 32;

	public static HashedString NO_BATCH = new HashedString("NO_BATCH");

	public static HashedString IGNORE = new HashedString("IGNORE");

	public static Vector2 GROUP_SIZE = new Vector2(32f, 32f);

	private bool ready = false;

	private Bounds currentActiveArea = default(Bounds);

	private Dictionary<HashedString, KBatchGroupData> batchGroupData = new Dictionary<HashedString, KBatchGroupData>();

	private Dictionary<BatchGroupKey, KAnimBatchGroup> batchGroups = new Dictionary<BatchGroupKey, KAnimBatchGroup>();

	private Dictionary<BatchKey, BatchSet> batchSets = new Dictionary<BatchKey, BatchSet>();

	private HashSet<BatchSet> activeBatchSets = new HashSet<BatchSet>();

	private HashSet<BatchSet> inactiveBatchSets = new HashSet<BatchSet>();

	private static bool created = false;
}
