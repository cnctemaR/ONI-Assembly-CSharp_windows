using System;
using System.Collections.Generic;
using UnityEngine;

public class KAnimBatch
{
	public KAnimBatch(KAnimBatchGroup group, int layer, float z, KAnimBatchGroup.MaterialType material_type)
	{
		this.active = true;
		this.group = group;
		this.layer = layer;
		this.batchGroup = group.batchID;
		this.materialType = material_type;
		this.position = new Vector3(0f, 0f, z);
		this.instanceID = KAnimBatch.maxInstanceID++;
		this.symbolInstanceSlots = new KAnimBatch.SymbolInstanceSlot[group.maxGroupSize];
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

	public KAnimBatchGroup.KAnimBatchTextureCache.Entry symbolInstanceTex { get; private set; }

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
		if (this.symbolInstanceTex != null)
		{
			this.group.FreeTexture(this.symbolInstanceTex);
			this.symbolInstanceTex = null;
		}
	}

	public void Init(BatchGroupInstance batch_group_instance)
	{
		this.batchGroupInstance = batch_group_instance;
		this.dataTex = this.group.CreateTexture();
		int bestTextureSize = KAnimBatchGroup.GetBestTextureSize((float)(this.group.data.maxSymbolsPerBuild * this.group.maxGroupSize * 8));
		this.symbolInstanceTex = this.group.CreateTexture("SymbolInstanceTex", bestTextureSize, KAnimBatch.ShaderProperty_symbolInstanceTex, KAnimBatch.ShaderProperty_SYMBOL_INSTANCE_TEXTURE_SIZE);
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
		for (int i = 0; i < width * width; i++)
		{
			this.dataTex.floats[i * 4] = -1f;
			this.dataTex.floats[i * 4 + 1] = 0f;
			this.dataTex.floats[i * 4 + 2] = 0f;
			this.dataTex.floats[i * 4 + 3] = 0f;
		}
		this.matProperties = new MaterialPropertyBlock();
		this.dataTex.SetTextureAndSize(this.matProperties);
		this.symbolInstanceTex.SetTextureAndSize(this.matProperties);
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
			this.position = new Vector3((float)(this.batchset.idx.x * 32), (float)(this.batchset.idx.y * 32), this.position.z);
			this.active = this.batchset.active;
		}
	}

	public bool ContainsPos(Vector3 pos)
	{
		return new Vector2I((int)(pos.x / 32f), (int)(pos.y / 32f)) == this.batchset.idx;
	}

	public void ClearRender(KAnimConverter.IAnimConverter controller)
	{
		int num = this.controllers.IndexOf(controller);
		if (num >= 0)
		{
			int num2 = num * 28;
			for (int i = 0; i < 28; i++)
			{
				this.dataTex.floats[num2 + i] = -1f;
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
		if (this.dataTex == null || this.dataTex.floats.Length == 0)
		{
			this.Init(controller.batchGroupInstance);
		}
		if (!this.controllers.Contains(controller))
		{
			this.controllers.Add(controller);
			this.currentOffset += 28;
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

	public void OverrideZ(float z)
	{
		this.position = new Vector3(this.position.x, this.position.y, z);
	}

	public void SetLayer(int layer)
	{
		this.layer = layer;
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
			this.currentOffset -= 28;
			this.currentOffset = Mathf.Max(0, this.currentOffset);
			for (int i = 0; i < 28; i++)
			{
				this.dataTex.floats[this.currentOffset + i] = -1f;
			}
			this.currentOffset = 28 * this.controllers.Count;
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

	private bool WriteToByteArray(int index)
	{
		bool flag = false;
		KAnimConverter.IAnimConverter animConverter = this.controllers[index];
		if (animConverter != null && animConverter as global::UnityEngine.Object != null)
		{
			animConverter.GetBatchInstanceData().WriteToTexture(this.dataTex.bytes, index * 112, index);
			KAnimBatch.SymbolInstanceSlot symbolInstanceSlot = this.symbolInstanceSlots[index];
			if (symbolInstanceSlot.symbolInstanceData != animConverter.symbolInstanceGpuData || symbolInstanceSlot.dataVersion != animConverter.symbolInstanceGpuData.version)
			{
				animConverter.symbolInstanceGpuData.WriteToTexture(this.symbolInstanceTex.bytes, index * 8 * this.group.data.maxSymbolsPerBuild * 4, index);
				symbolInstanceSlot.symbolInstanceData = animConverter.symbolInstanceGpuData;
				symbolInstanceSlot.dataVersion = animConverter.symbolInstanceGpuData.version;
				this.symbolInstanceSlots[index] = symbolInstanceSlot;
				flag = true;
			}
		}
		else
		{
			this.controllers.RemoveAt(index);
		}
		return flag;
	}

	public void UpdateTexture(bool is_symbol_instance_data_dirty)
	{
		this.dataTex.LoadRawTextureData(this.dataTex.bytes);
		this.dataTex.Apply();
		if (is_symbol_instance_data_dirty)
		{
			this.symbolInstanceTex.LoadRawTextureData(this.symbolInstanceTex.bytes);
			this.symbolInstanceTex.Apply();
		}
	}

	public int UpdateDirty(int frame)
	{
		if (!this.needsWrite)
		{
			return 0;
		}
		if (this.dataTex == null || this.dataTex.floats.Length == 0)
		{
			this.Init(this.batchGroupInstance);
		}
		this.writtenLastFrame = 0;
		bool flag = false;
		if (this.dirtySet.Count > 0)
		{
			foreach (int num in this.dirtySet)
			{
				bool flag2 = this.WriteToByteArray(num);
				flag = flag || flag2;
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
		this.UpdateTexture(flag);
		return this.writtenLastFrame;
	}

	private List<KAnimConverter.IAnimConverter> controllers = new List<KAnimConverter.IAnimConverter>();

	private HashSet<int> dirtySet = new HashSet<int>();

	private int currentOffset;

	private int instanceID = -1;

	private static int maxInstanceID = 0;

	private static int ShaderProperty_SYMBOL_INSTANCE_TEXTURE_SIZE = Shader.PropertyToID("SYMBOL_INSTANCE_TEXTURE_SIZE");

	private static int ShaderProperty_symbolInstanceTex = Shader.PropertyToID("symbolInstanceTex");

	private KAnimBatch.SymbolInstanceSlot[] symbolInstanceSlots;

	private bool needsWrite;

	public struct SymbolInstanceSlot
	{
		public SymbolInstanceGpuData symbolInstanceData;

		public int dataVersion;
	}
}
