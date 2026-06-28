using System;
using System.Collections.Generic;
using UnityEngine;

public class KAnimBatch
{
	public KAnimBatch(KAnimBatchGroup group, int layer, float z, KAnimBatchGroup.MaterialType renderTypeOverride)
	{
		this.active = true;
		this.group = group;
		this.layer = layer;
		this.batchGroup = group.batchID;
		this.materialType = group.materialType;
		this.position = new Vector3(0f, 0f, z);
		this.instanceID = KAnimBatch.maxInstanceID++;
	}

	public bool dirty
	{
		get
		{
			return this.dirtySet.Count > 0;
		}
	}

	public int dirtyCount
	{
		get
		{
			return this.dirtySet.Count;
		}
	}

	public bool active { get; private set; }

	public int size
	{
		get
		{
			return this.controllers.Count;
		}
	}

	public Vector3 position { get; private set; }

	public int layer { get; private set; }

	public List<KAnimConverter.IAnimConverter> Controllers
	{
		get
		{
			return this.controllers;
		}
	}

	public KAnimBatchGroup.MaterialType materialType { get; private set; }

	public HashedString batchGroup { get; private set; }

	public BatchSet batchset { get; private set; }

	public KAnimBatchGroup group { get; private set; }

	public int writtenLastFrame { get; private set; }

	public MaterialPropertyBlock matProperties { get; private set; }

	public KAnimBatchGroup.KAnimBatchTextureCache.Entry dataTex { get; private set; }

	public BatchGroupInstance batchGroupInstance { get; private set; }

	public int GetInstanceID()
	{
		return this.instanceID;
	}

	public void DestroyTex()
	{
		if (this.dataTex != null)
		{
			this.group.FreeTexture(this.dataTex);
			this.dataTex = null;
		}
	}

	~KAnimBatch()
	{
		this.dataTex = null;
		this.byteToFloat.bytes = null;
	}

	public void Init()
	{
		this.batchGroupInstance = this.group.GetBatchGroupInstance(this);
		this.dataTex = this.group.CreateTexture();
		int width = this.dataTex.width;
		if (width == 0)
		{
			global::Debug.LogWarning(string.Concat(new object[]
			{
				"Empty group [",
				this.group.batchID,
				"] ",
				this.batchset.idx,
				" (probably just anims)"
			}), null);
			return;
		}
		this.byteToFloat = new KAnimConverter.ByteToFloatConverter
		{
			bytes = this.dataTex.bytes
		};
		for (int i = 0; i < width * width; i++)
		{
			this.byteToFloat.floats[i * 4] = -1f;
			this.byteToFloat.floats[i * 4 + 1] = 0f;
			this.byteToFloat.floats[i * 4 + 2] = 0f;
			this.byteToFloat.floats[i * 4 + 3] = 0f;
		}
		this.matProperties = new MaterialPropertyBlock();
		this.matProperties.SetTexture("instanceTex", this.dataTex.texture);
		this.matProperties.SetVector("INSTANCE_TEXEL_SIZE", this.dataTex.texelSize);
		this.matProperties.SetVector("INSTANCE_TEXTURE_SIZE", new Vector2((float)this.dataTex.width, (float)this.dataTex.height));
		this.group.GetDataTextures(this.batchGroupInstance, this.matProperties);
	}

	public void Clear()
	{
		this.DestroyTex();
		this.controllers.Clear();
		this.dirtySet.Clear();
		this.batchset = null;
		this.group = null;
		this.batchGroupInstance = null;
		this.matProperties = null;
		this.dataTex = null;
		this.byteToFloat.bytes = null;
		this.batchGroupInstance = null;
	}

	public void SetBatchSet(BatchSet newBatchSet)
	{
		if (this.batchset != null && this.batchset != newBatchSet)
		{
			this.batchset.RemoveBatch(this);
		}
		this.batchset = newBatchSet;
		if (this.batchset != null)
		{
			this.position = new Vector3((float)(this.batchset.idx.x * 16), (float)(this.batchset.idx.y * 16), this.position.z);
			this.active = this.batchset.active;
		}
	}

	public bool ContainsPos(Vector3 pos)
	{
		return new Vector2I((int)(pos.x / 16f), (int)(pos.y / 16f)) == this.batchset.idx;
	}

	public void ClearRender(KAnimConverter.IAnimConverter controller)
	{
		int num = this.controllers.IndexOf(controller);
		if (num >= 0)
		{
			int num2 = num * 64;
			for (int i = 0; i < 64; i++)
			{
				this.byteToFloat.floats[num2 + i] = -1f;
			}
			this.dirtySet.Remove(num);
			this.AddToClear();
		}
	}

	private void AddToClear()
	{
		this.needsWrite = true;
		this.batchset.SetDirty();
	}

	public int GetIndex(KAnimConverter.IAnimConverter controller)
	{
		return this.controllers.IndexOf(controller);
	}

	public bool Register(KAnimConverter.IAnimConverter controller)
	{
		if (this.dataTex == null || this.byteToFloat.floats == null || this.byteToFloat.floats.Length == 0)
		{
			this.Init();
		}
		if (!this.controllers.Contains(controller))
		{
			this.controllers.Add(controller);
			this.currentOffset += 64;
		}
		this.AddToDirty(this.controllers.IndexOf(controller));
		KAnimBatch batch = controller.GetBatch();
		if (batch != null)
		{
			batch.Deregister(controller);
		}
		controller.SetBatch(this);
		return true;
	}

	public void DebugClearNulls()
	{
		this.controllers.RemoveAll((KAnimConverter.IAnimConverter p) => p == null || p as global::UnityEngine.Object == null);
	}

	public void OverrideZ(float z)
	{
		this.position = new Vector3(this.position.x, this.position.y, z);
	}

	public void SetLayer(int layer)
	{
		this.layer = layer;
	}

	public void ResetLayer()
	{
		this.layer = this.batchset.key.layer;
	}

	public void Deregister(KAnimConverter.IAnimConverter controller)
	{
		if (App.IsExiting)
		{
			return;
		}
		int num = this.controllers.IndexOf(controller);
		if (num >= 0)
		{
			if (!this.controllers.Remove(controller))
			{
				global::Debug.LogError("Failed to remove controller [" + controller.GetName() + "]", null);
			}
			controller.SetBatch(null);
			this.currentOffset -= 64;
			this.currentOffset = Mathf.Max(0, this.currentOffset);
			for (int i = 0; i < 64; i++)
			{
				this.byteToFloat.floats[this.currentOffset + i] = -1f;
			}
			this.currentOffset = 64 * this.controllers.Count;
			this.ClearDirty();
			for (int j = 0; j < this.controllers.Count; j++)
			{
				this.AddToDirty(j);
			}
		}
		else
		{
			global::Debug.LogError("Deregister called for [" + controller.GetName() + "] but its not in this batch ", null);
		}
		if (this.controllers.Count == 0)
		{
			this.batchset.RemoveBatch(this);
			this.DestroyTex();
		}
	}

	private void ClearDirty()
	{
		this.dirtySet.Clear();
	}

	private void AddToDirty(int dirtyIdx)
	{
		this.dirtySet.Add(dirtyIdx);
		this.batchset.SetDirty();
		this.needsWrite = true;
	}

	public void Activate()
	{
		this.active = true;
	}

	public void Deactivate()
	{
		this.active = false;
	}

	public void SetDirty(KAnimConverter.IAnimConverter controller)
	{
		int num = this.controllers.IndexOf(controller);
		if (num < 0)
		{
			global::Debug.LogError("Setting controller [" + controller.GetName() + "] to dirty but its not in this batch", null);
			return;
		}
		this.AddToDirty(num);
	}

	private void WriteToByteArray(int index)
	{
		if (this.controllers[index] != null && this.controllers[index] as global::UnityEngine.Object != null)
		{
			this.controllers[index].GetBatchInstanceData().WriteToTexture(this.byteToFloat.floats, index * 64, index);
		}
		else
		{
			this.controllers.RemoveAt(index);
		}
	}

	public void UpdateTexture()
	{
		this.dataTex.LoadRawTextureData(this.dataTex.bytes);
		this.dataTex.Apply();
	}

	public int UpdateDirty(int frame)
	{
		if (!this.needsWrite)
		{
			return 0;
		}
		if (this.dataTex == null || this.byteToFloat.floats == null || this.byteToFloat.floats.Length == 0)
		{
			this.Init();
		}
		this.writtenLastFrame = 0;
		if (this.dirtySet.Count > 0)
		{
			HashSet<int>.Enumerator enumerator = this.dirtySet.GetEnumerator();
			while (enumerator.MoveNext())
			{
				try
				{
					this.WriteToByteArray(enumerator.Current);
				}
				catch (Exception ex)
				{
					global::Debug.LogError("WriteToByteArray: " + ex.Message + "\n" + ex.StackTrace, null);
				}
				this.writtenLastFrame++;
			}
			if (this.writtenLastFrame != 0)
			{
				this.ClearDirty();
			}
			else
			{
				global::Debug.LogError("dirtySet not written", null);
			}
		}
		this.UpdateTexture();
		return this.writtenLastFrame;
	}

	private List<KAnimConverter.IAnimConverter> controllers = new List<KAnimConverter.IAnimConverter>();

	private HashSet<int> dirtySet = new HashSet<int>();

	private KAnimConverter.ByteToFloatConverter byteToFloat;

	private int currentOffset;

	private int instanceID = -1;

	private static int maxInstanceID;

	private bool needsWrite;
}
