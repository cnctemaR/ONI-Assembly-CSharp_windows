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

	public static void CreateInstance()
	{
		Singleton<KAnimBatchManager>.CreateInstance();
	}

	public static KAnimBatchManager Instance()
	{
		if (!SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat))
		{
			global::Debug.LogError("Machine does not support RGBAFloat32", null);
		}
		return KAnimBatchManager.instance;
	}

	public static void DestroyInstance()
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
			KAnimBatchManager.instance.dirtyBatchLastFrame = 0;
			KAnimBatchGroup.FinalizeTextureCache();
		}
		Singleton<KAnimBatchManager>.DestroyInstance();
	}

	public bool isReady
	{
		get
		{
			return this.ready;
		}
	}

	public KBatchGroupData GetBatchGroupData(HashedString groupID)
	{
		if (!groupID.IsValid || groupID == KAnimBatchManager.NO_BATCH || groupID == KAnimBatchManager.IGNORE)
		{
			return null;
		}
		KBatchGroupData kbatchGroupData = null;
		if (!this.batchGroupData.TryGetValue(groupID, out kbatchGroupData))
		{
			kbatchGroupData = new KBatchGroupData(groupID);
			this.batchGroupData[groupID] = kbatchGroupData;
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

	public static Vector2I ControllerToChunkXY(KAnimConverter.IAnimConverter controller)
	{
		Vector2I cellXY = controller.GetCellXY();
		return KAnimBatchManager.CellXYToChunkXY(cellXY);
	}

	public void Register(KAnimConverter.IAnimConverter controller)
	{
		if (!this.isReady)
		{
			global::Debug.LogError(string.Format("Batcher isnt finished setting up, controller [{0}] is registering too early.", controller.GetName()), null);
		}
		BatchKey batchKey = BatchKey.Create(controller);
		Vector2I vector2I = KAnimBatchManager.ControllerToChunkXY(controller);
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
		this.activeBatchSets.Add(bs);
		bs.SetActive(true);
	}

	private void AddToInactiveBatchSet(BatchSet bs)
	{
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
				if (!value.active)
				{
					this.AddToActiveBatchSet(value);
				}
			}
			else if (value.active)
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
			this.dirtyBatchLastFrame += batchSet.UpdateDirty(frame);
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
			DebugUtil.Assert(batchSet != null, "Assert!");
			DebugUtil.Assert(batchSet.group != null, "Assert!");
			if (batchSet.active)
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

	public void CompleteInit()
	{
		this.ready = true;
	}

	private const int DEFAULT_BATCH_SIZE = 60;

	public const int CHUNK_SIZE = 32;

	public static HashedString NO_BATCH = new HashedString("NO_BATCH");

	public static HashedString IGNORE = new HashedString("IGNORE");

	public static Vector2 GROUP_SIZE = new Vector2(32f, 32f);

	private bool ready;

	private Bounds currentActiveArea = default(Bounds);

	private Dictionary<HashedString, KBatchGroupData> batchGroupData = new Dictionary<HashedString, KBatchGroupData>();

	private Dictionary<BatchGroupKey, KAnimBatchGroup> batchGroups = new Dictionary<BatchGroupKey, KAnimBatchGroup>();

	private Dictionary<BatchKey, BatchSet> batchSets = new Dictionary<BatchKey, BatchSet>();

	private HashSet<BatchSet> activeBatchSets = new HashSet<BatchSet>();

	public int[] atlasNames = new int[]
	{
		Shader.PropertyToID("atlas0"),
		Shader.PropertyToID("atlas1"),
		Shader.PropertyToID("atlas2"),
		Shader.PropertyToID("atlas3"),
		Shader.PropertyToID("atlas4"),
		Shader.PropertyToID("atlas5"),
		Shader.PropertyToID("atlas6"),
		Shader.PropertyToID("atlas7"),
		Shader.PropertyToID("atlas8"),
		Shader.PropertyToID("atlas9"),
		Shader.PropertyToID("atlas10"),
		Shader.PropertyToID("atlas11")
	};
}
