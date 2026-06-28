using System;
using System.Collections.Generic;
using UnityEngine;

public class KAnimBatch
{
	public KAnimBatch(KAnimBatchGroup group, BatchSet batchset, int layer, float z, Vector2I idx, KAnimBatchGroup.MaterialType renderTypeOverride)
	{
		this.active = true;
		this.position = new Vector3((float)(idx.x * 32), (float)(idx.y * 32), z);
		this.batchset = batchset;
		this.group = group;
		this.layer = layer;
		this.idx = idx;
		this.batchGroup = group.batchID;
		this.materialType = group.materialType;
		this.instanceID = KAnimBatch.maxInstanceID++;
	}

	public bool dirty
	{
		get
		{
			return this.dirtySet.Count > 0;
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

	public Texture2D dataTex { get; private set; }

	public BatchGroupInstance groupInstace { get; private set; }

	public int GetInstanceID()
	{
		return this.instanceID;
	}

	public void DestroyTex()
	{
		if (this.dataTex != null)
		{
			global::UnityEngine.Object.Destroy(this.dataTex);
			this.dataTex = null;
		}
	}

	~KAnimBatch()
	{
		this.dataTex = null;
	}

	public void Init()
	{
		this.groupInstace = this.group.GetBatchGroupInstance(this);
		Debug.AssertFormat(this.groupInstace != null, "Got null groupInstace from AnimBatchGroup [{0}]", new object[] { this.batchGroup });
		this.dataTex = this.group.CreateTexture();
		Debug.AssertFormat(this.dataTex != null, "Got null data texture from AnimBatchGroup [{0}]", new object[] { this.batchGroup });
		int width = this.dataTex.width;
		if (width == 0)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"Empty group [",
				this.group.batchID,
				"] ",
				this.idx,
				" (probably just anims)"
			}));
			return;
		}
		this.texBytes = new byte[width * width * 4 * 4];
		this.byteToFloat = new KAnimConverter.ByteToFloatConverter
		{
			bytes = this.texBytes
		};
		for (int i = 0; i < width * width; i++)
		{
			this.byteToFloat.floats[i * 4] = -1f;
			this.byteToFloat.floats[i * 4 + 1] = 0f;
			this.byteToFloat.floats[i * 4 + 2] = 0f;
			this.byteToFloat.floats[i * 4 + 3] = 0f;
		}
		this.matProperties = new MaterialPropertyBlock();
		this.matProperties.SetTexture("instanceTex", this.dataTex);
		this.matProperties.SetVector("INSTANCE_TEXEL_SIZE", this.dataTex.texelSize);
		this.matProperties.SetVector("INSTANCE_TEXTURE_SIZE", new Vector2((float)this.dataTex.width, (float)this.dataTex.height));
		this.group.GetDataTextures(this.groupInstace, this.matProperties);
	}

	public bool ContainsPos(Vector3 pos)
	{
		return new Vector2I((int)(pos.x / 32f), (int)(pos.y / 32f)) == this.idx;
	}

	public void ClearRender(KAnimConverter.IAnimConverter controller)
	{
		int num = this.controllers.IndexOf(controller);
		Debug.AssertFormat(num >= 0, "Wrong batch - couldn't find {0}]", new object[] { controller.GetName() });
		if (num >= 0)
		{
			int num2 = num * 48;
			for (int i = 0; i < 48; i++)
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
		if (this.byteToFloat.floats == null || this.byteToFloat.floats.Length == 0)
		{
			return false;
		}
		if (!this.controllers.Contains(controller))
		{
			this.controllers.Add(controller);
			this.currentOffset += 48;
		}
		this.AddToDirty(this.controllers.IndexOf(controller));
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
		int num = this.controllers.IndexOf(controller);
		if (num >= 0)
		{
			if (!this.controllers.Remove(controller))
			{
				Debug.LogError("Failed to remove controller [" + controller.GetName() + "]");
			}
			controller.SetBatch(null);
			this.currentOffset -= 48;
			this.currentOffset = Mathf.Max(0, this.currentOffset);
			for (int i = 0; i < 48; i++)
			{
				this.byteToFloat.floats[this.currentOffset + i] = -1f;
			}
			this.currentOffset = 48 * this.controllers.Count;
			this.ClearDirty();
			for (int j = 0; j < this.controllers.Count; j++)
			{
				this.AddToDirty(j);
			}
		}
		else
		{
			Debug.LogError("Deregister called for [" + controller.GetName() + "] but its not in this batch ");
		}
		if (this.controllers.Count == 0)
		{
			this.batchset.RemoveBatch(this);
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
			Debug.LogError("Setting controller [" + controller.GetName() + "] to dirty but its not in this batch");
			return;
		}
		this.AddToDirty(num);
	}

	private void WriteToByteArray(int index)
	{
		if (this.controllers[index] != null && this.controllers[index] as global::UnityEngine.Object != null)
		{
			this.controllers[index].GetBatchInstanceData().WriteToTexture(this.byteToFloat.floats, index * 48, index);
		}
		else
		{
			this.controllers.RemoveAt(index);
		}
	}

	public void UpdateTexture()
	{
		this.dataTex.LoadRawTextureData(this.texBytes);
		this.dataTex.Apply();
	}

	public int UpdateDirty()
	{
		if (this.dataTex == null || !this.needsWrite)
		{
			return 0;
		}
		this.writtenLastFrame = 0;
		if (this.dirtySet.Count > 0)
		{
			foreach (int num in this.dirtySet)
			{
				this.WriteToByteArray(num);
				this.writtenLastFrame++;
			}
			this.ClearDirty();
		}
		this.UpdateTexture();
		return this.writtenLastFrame;
	}

	private const int BATCH_PHYSICAL_SIZE = 32;

	private List<KAnimConverter.IAnimConverter> controllers = new List<KAnimConverter.IAnimConverter>();

	private HashSet<int> dirtySet = new HashSet<int>();

	private byte[] texBytes;

	private KAnimConverter.ByteToFloatConverter byteToFloat;

	private int currentOffset;

	private Vector2I idx;

	private int instanceID = -1;

	private static int maxInstanceID;

	private bool needsWrite;
}
