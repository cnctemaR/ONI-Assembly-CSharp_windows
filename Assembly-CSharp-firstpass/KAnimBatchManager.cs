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
			KAnimBatchManager.instance.currentActiveArea = default(Bounds);
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
			KAnimBatchManager.instance.dirtyBatchLastFrame = 0;
			KAnimBatchManager.instance = null;
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

	public KBatchGroupData GetBatchGroupData(HashedString groupID)
	{
		if (!groupID.isValid || groupID == KAnimBatchManager.NO_BATCH || groupID == KAnimBatchManager.IGNORE)
		{
			return null;
		}
		if (!this.batchGroupData.ContainsKey(groupID))
		{
			this.batchGroupData[groupID] = new KBatchGroupData(groupID);
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

	public static Vector2I GetBatchIndex(Vector3 pos)
	{
		return new Vector2I
		{
			x = (int)(pos.x / 16f),
			y = (int)(pos.y / 16f)
		};
	}

	public void Register(KAnimConverter.IAnimConverter controller)
	{
		Debug.AssertFormat(this.isReady, "Batcher isnt finished setting up, controller [{0}] is registering too early.", new object[] { controller.GetName() });
		BatchKey batchKey = new BatchKey(controller);
		Vector3 position = controller.GetPosition();
		if (position.z == 0f)
		{
		}
		Vector2I batchIndex = KAnimBatchManager.GetBatchIndex(position);
		BatchSet batchSet;
		if (!this.batchSets.TryGetValue(batchKey, out batchSet))
		{
			batchSet = new BatchSet(this.GetBatchGroup(batchKey), batchKey, batchIndex);
			this.batchSets[batchKey] = batchSet;
		}
		batchSet.Add(controller);
	}

	private Bounds GetVisibleBounds(Vector2I visible_area_min, Vector2I visible_area_max)
	{
		Vector2I vector2I = new Vector2I((visible_area_min.x - 1) / 16, (visible_area_min.y - 1) / 16);
		Vector2I vector2I2 = new Vector2I((visible_area_max.x + 16 - 1) / 16, (visible_area_max.y + 16 - 1) / 16);
		Bounds bounds = default(Bounds);
		bounds.SetMinMax(new Vector3((float)vector2I.x, (float)vector2I.y), new Vector3((float)vector2I2.x, (float)vector2I2.y));
		return bounds;
	}

	public Bounds currentActiveArea { get; private set; }

	public void UpdateActiveArea(Vector2I visible_area_min, Vector2I visible_area_max)
	{
		this.currentActiveArea = this.GetVisibleBounds(visible_area_min, visible_area_max);
		foreach (KeyValuePair<BatchKey, BatchSet> keyValuePair in this.batchSets)
		{
			BatchSet value = keyValuePair.Value;
			if (value.key.materialType == KAnimBatchGroup.MaterialType.UI || (value.batches.Count > 0 && this.currentActiveArea.Intersects(value.bounds)))
			{
				this.activeBatchSets.Add(value);
				value.SetActive(true);
			}
			else
			{
				this.activeBatchSets.Remove(value);
				value.SetActive(false);
			}
		}
	}

	public int UpdateDirty()
	{
		if (!this.ready)
		{
			return 0;
		}
		this.dirtyBatchLastFrame = 0;
		foreach (BatchSet batchSet in this.activeBatchSets)
		{
			this.dirtyBatchLastFrame += batchSet.UpdateDirty();
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
				for (int i = 0; i < batchSet.batches.Count; i++)
				{
					KAnimBatch kanimBatch = batchSet.batches[i];
					if (kanimBatch.size != 0 && kanimBatch.active)
					{
						if (kanimBatch.materialType != KAnimBatchGroup.MaterialType.UI)
						{
							Vector3 zero = Vector3.zero;
							zero.z = kanimBatch.position.z;
							int layer = kanimBatch.layer;
							Graphics.DrawMesh(mesh, zero, Quaternion.identity, material, layer, null, 0, kanimBatch.matProperties);
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

	private const int GROUP_WIDTH = 16;

	private const int GROUP_HEIGHT = 16;

	public static HashedString NO_BATCH = new HashedString("NO_BATCH");

	public static HashedString IGNORE = new HashedString("IGNORE");

	private bool ready;

	private Dictionary<HashedString, KBatchGroupData> batchGroupData = new Dictionary<HashedString, KBatchGroupData>();

	private Dictionary<BatchGroupKey, KAnimBatchGroup> batchGroups = new Dictionary<BatchGroupKey, KAnimBatchGroup>();

	private Dictionary<BatchKey, BatchSet> batchSets = new Dictionary<BatchKey, BatchSet>();

	private HashSet<BatchSet> activeBatchSets = new HashSet<BatchSet>();

	private static KAnimBatchManager instance;

	public static Vector2 GROUP_SIZE = new Vector2(16f, 16f);
}
