using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class KAnimBatch
{
	public int id { get; private set; }

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

	public KAnimBatchGroup.KAnimBatchTextureCache.Entry symbolOverrideInfoTex { get; private set; }

	public bool isSetup { get; private set; }

	public KAnimBatch(KAnimBatchGroup group, int layer, float z, KAnimBatchGroup.MaterialType material_type)
	{
		this.id = KAnimBatch.nextBatchId++;
		this.active = true;
		this.group = group;
		this.layer = layer;
		this.batchGroup = group.batchID;
		this.materialType = material_type;
		this.matProperties = new MaterialPropertyBlock();
		this.position = new Vector3(0f, 0f, z);
		this.symbolInstanceSlots = new KAnimBatch.SymbolInstanceSlot[group.maxGroupSize];
		this.symbolOverrideInfoSlots = new KAnimBatch.SymbolOverrideInfoSlot[group.maxGroupSize];
		this.isSetup = false;
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
		if (this.symbolOverrideInfoTex != null)
		{
			this.group.FreeTexture(this.symbolOverrideInfoTex);
			this.symbolOverrideInfoTex = null;
		}
	}

	public void Init()
	{
		this.dataTex = this.group.CreateTexture();
		if (this.dataTex == null)
		{
			global::Debug.LogErrorFormat("Got null data texture from AnimBatchGroup [{0}]", new object[] { this.batchGroup });
		}
		int bestTextureSize = KAnimBatchGroup.GetBestTextureSize((float)(this.group.data.maxSymbolsPerBuild * this.group.maxGroupSize * 8));
		this.symbolInstanceTex = this.group.CreateTexture("SymbolInstanceTex", bestTextureSize, KAnimBatch.ShaderProperty_symbolInstanceTex, KAnimBatch.ShaderProperty_SYMBOL_INSTANCE_TEXTURE_SIZE);
		int width = this.dataTex.width;
		if (width == 0)
		{
			global::Debug.LogWarning(string.Concat(new string[]
			{
				"Empty group [",
				this.group.batchID.ToString(),
				"] ",
				this.batchset.idx.ToString(),
				" (probably just anims)"
			}));
			return;
		}
		NativeArray<float> floatDataPointer = this.dataTex.GetFloatDataPointer();
		for (int i = 0; i < width * width; i++)
		{
			floatDataPointer[i * 4] = -1f;
			floatDataPointer[i * 4 + 1] = 0f;
			floatDataPointer[i * 4 + 2] = 0f;
			floatDataPointer[i * 4 + 3] = 0f;
		}
		this.isSetup = true;
		if (this.matProperties == null)
		{
			this.matProperties = new MaterialPropertyBlock();
		}
		this.dataTex.SetTextureAndSize(this.matProperties);
		this.symbolInstanceTex.SetTextureAndSize(this.matProperties);
		this.group.GetDataTextures(this.matProperties, this.atlases);
		this.atlases.Apply(this.matProperties);
	}

	public void Clear()
	{
		this.DestroyTex();
		this.controllers.Clear();
		this.dirtySet.Clear();
		this.batchset = null;
		this.group = null;
		this.matProperties = null;
		this.dataTex = null;
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

	public bool Register(KAnimConverter.IAnimConverter controller)
	{
		if (this.dataTex == null || !this.isSetup)
		{
			this.Init();
		}
		if (!this.controllers.Contains(controller))
		{
			this.controllers.Add(controller);
			this.controllersToIdx[controller] = this.controllers.Count - 1;
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
		if (this.controllers.IndexOf(controller) >= 0)
		{
			if (!this.controllers.Remove(controller))
			{
				global::Debug.LogError("Failed to remove controller [" + controller.GetName() + "]");
			}
			controller.SetBatch(null);
			this.currentOffset -= 28;
			this.currentOffset = Mathf.Max(0, this.currentOffset);
			NativeArray<float> floatDataPointer = this.dataTex.GetFloatDataPointer();
			for (int i = 0; i < 28; i++)
			{
				floatDataPointer[this.currentOffset + i] = -1f;
			}
			this.dataTex.Apply();
			this.currentOffset = 28 * this.controllers.Count;
			this.ClearDirty();
			this.controllersToIdx.Clear();
			for (int j = 0; j < this.controllers.Count; j++)
			{
				this.controllersToIdx[this.controllers[j]] = j;
				this.AddToDirty(j);
			}
		}
		else
		{
			global::Debug.LogError("Deregister called for [" + controller.GetName() + "] but its not in this batch ");
		}
		if (this.controllers.Count == 0)
		{
			this.batchset.RemoveBatch(this);
			this.DestroyTex();
		}
	}

	private void ClearDirty()
	{
		this.needsWrite = false;
		this.dirtySet.Clear();
	}

	private void AddToDirty(int dirtyIdx)
	{
		if (!this.dirtySet.Contains(dirtyIdx))
		{
			this.dirtySet.Add(dirtyIdx);
		}
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
		int num;
		if (!this.controllersToIdx.TryGetValue(controller, out num))
		{
			global::Debug.LogError("Setting controller [" + controller.GetName() + "] to dirty but its not in this batch");
			return;
		}
		this.AddToDirty(num);
	}

	private void WriteBatchedAnimInstanceData(int index, KAnimConverter.IAnimConverter controller, NativeArray<byte> data)
	{
		controller.GetBatchInstanceData().WriteToTexture(data, index * 112, index);
	}

	private bool WriteSymbolInstanceData(int index, KAnimConverter.IAnimConverter controller, NativeArray<byte> data)
	{
		bool flag = false;
		KAnimBatch.SymbolInstanceSlot symbolInstanceSlot = this.symbolInstanceSlots[index];
		if (symbolInstanceSlot.symbolInstanceData != controller.symbolInstanceGpuData || symbolInstanceSlot.dataVersion != controller.symbolInstanceGpuData.version)
		{
			controller.symbolInstanceGpuData.WriteToTexture(data, index * 8 * this.group.data.maxSymbolsPerBuild * 4, index);
			symbolInstanceSlot.symbolInstanceData = controller.symbolInstanceGpuData;
			symbolInstanceSlot.dataVersion = controller.symbolInstanceGpuData.version;
			this.symbolInstanceSlots[index] = symbolInstanceSlot;
			flag = true;
		}
		return flag;
	}

	private bool WriteSymbolOverrideInfoTex(int index, KAnimConverter.IAnimConverter controller, NativeArray<byte> data)
	{
		bool flag = false;
		KAnimBatch.SymbolOverrideInfoSlot symbolOverrideInfoSlot = this.symbolOverrideInfoSlots[index];
		if (symbolOverrideInfoSlot.symbolOverrideInfo != controller.symbolOverrideInfoGpuData || symbolOverrideInfoSlot.dataVersion != controller.symbolOverrideInfoGpuData.version)
		{
			controller.symbolOverrideInfoGpuData.WriteToTexture(data, index * 12 * this.group.data.maxSymbolFrameInstancesPerbuild * 4, index);
			symbolOverrideInfoSlot.symbolOverrideInfo = controller.symbolOverrideInfoGpuData;
			symbolOverrideInfoSlot.dataVersion = controller.symbolOverrideInfoGpuData.version;
			this.symbolOverrideInfoSlots[index] = symbolOverrideInfoSlot;
			flag = true;
		}
		return flag;
	}

	public int UpdateDirty(int frame)
	{
		if (!this.needsWrite)
		{
			return 0;
		}
		if (this.dataTex == null || !this.isSetup)
		{
			this.Init();
		}
		this.writtenLastFrame = 0;
		bool flag = false;
		bool flag2 = false;
		NativeArray<byte> dataPointer = this.dataTex.GetDataPointer();
		NativeArray<byte> dataPointer2 = this.symbolInstanceTex.GetDataPointer();
		if (this.dirtySet.Count > 0)
		{
			foreach (int num in this.dirtySet)
			{
				KAnimConverter.IAnimConverter animConverter = this.controllers[num];
				if (animConverter != null && animConverter as global::UnityEngine.Object != null)
				{
					this.WriteBatchedAnimInstanceData(num, animConverter, dataPointer);
					bool flag3 = this.WriteSymbolInstanceData(num, animConverter, dataPointer2);
					flag = flag || flag3;
					if (animConverter.ApplySymbolOverrides())
					{
						if (this.symbolOverrideInfoTex == null)
						{
							int bestTextureSize = KAnimBatchGroup.GetBestTextureSize((float)(this.group.data.maxSymbolFrameInstancesPerbuild * this.group.maxGroupSize * 12));
							this.symbolOverrideInfoTex = this.group.CreateTexture("SymbolOverrideInfoTex", bestTextureSize, KAnimBatch.ShaderProperty_symbolOverrideInfoTex, KAnimBatch.ShaderProperty_SYMBOL_OVERRIDE_INFO_TEXTURE_SIZE);
							this.symbolOverrideInfoTex.SetTextureAndSize(this.matProperties);
							this.matProperties.SetFloat(KAnimBatch.ShaderProperty_SUPPORTS_SYMBOL_OVERRIDING, 1f);
						}
						NativeArray<byte> dataPointer3 = this.symbolOverrideInfoTex.GetDataPointer();
						bool flag4 = this.WriteSymbolOverrideInfoTex(num, animConverter, dataPointer3);
						flag2 = flag2 || flag4;
					}
					int writtenLastFrame = this.writtenLastFrame;
					this.writtenLastFrame = writtenLastFrame + 1;
				}
			}
			if (this.writtenLastFrame != 0)
			{
				this.ClearDirty();
			}
			else
			{
				global::Debug.LogError("dirtySet not written");
			}
		}
		this.dataTex.Apply();
		if (flag)
		{
			this.symbolInstanceTex.Apply();
		}
		if (flag2)
		{
			this.symbolOverrideInfoTex.Apply();
		}
		return this.writtenLastFrame;
	}

	private List<KAnimConverter.IAnimConverter> controllers = new List<KAnimConverter.IAnimConverter>();

	private Dictionary<KAnimConverter.IAnimConverter, int> controllersToIdx = new Dictionary<KAnimConverter.IAnimConverter, int>();

	private List<int> dirtySet = new List<int>();

	private static int nextBatchId;

	private int currentOffset;

	private static int ShaderProperty_SYMBOL_INSTANCE_TEXTURE_SIZE = Shader.PropertyToID("SYMBOL_INSTANCE_TEXTURE_SIZE");

	private static int ShaderProperty_symbolInstanceTex = Shader.PropertyToID("symbolInstanceTex");

	private static int ShaderProperty_SYMBOL_OVERRIDE_INFO_TEXTURE_SIZE = Shader.PropertyToID("SYMBOL_OVERRIDE_INFO_TEXTURE_SIZE");

	private static int ShaderProperty_symbolOverrideInfoTex = Shader.PropertyToID("symbolOverrideInfoTex");

	public static int ShaderProperty_SUPPORTS_SYMBOL_OVERRIDING = Shader.PropertyToID("SUPPORTS_SYMBOL_OVERRIDING");

	public static int ShaderProperty_ANIM_TEXTURE_START_OFFSET = Shader.PropertyToID("ANIM_TEXTURE_START_OFFSET");

	private KAnimBatch.SymbolInstanceSlot[] symbolInstanceSlots;

	private KAnimBatch.SymbolOverrideInfoSlot[] symbolOverrideInfoSlots;

	public KAnimBatch.AtlasList atlases = new KAnimBatch.AtlasList(0);

	private bool needsWrite;

	public struct SymbolInstanceSlot
	{
		public SymbolInstanceGpuData symbolInstanceData;

		public int dataVersion;
	}

	public struct SymbolOverrideInfoSlot
	{
		public SymbolOverrideInfoGpuData symbolOverrideInfo;

		public int dataVersion;
	}

	public class AtlasList
	{
		public AtlasList(int start_idx)
		{
			this.startIdx = start_idx;
		}

		public int Add(Texture2D atlas)
		{
			DebugUtil.Assert(atlas != null, "KAnimBatch Atlas is null");
			DebugUtil.Assert(this.atlases.Count < KAnimBatchManager.instance.atlasNames.Length);
			int num = this.atlases.IndexOf(atlas);
			if (num == -1)
			{
				num = this.atlases.Count;
				this.atlases.Add(atlas);
			}
			return num + this.startIdx;
		}

		public void Apply(MaterialPropertyBlock material_property_block)
		{
			bool flag = false;
			for (int i = 0; i < this.atlases.Count; i++)
			{
				int num = this.startIdx + i;
				if (num >= KAnimBatchManager.instance.atlasNames.Length)
				{
					flag = true;
				}
				else
				{
					material_property_block.SetTexture(KAnimBatchManager.instance.atlasNames[num], this.atlases[i]);
				}
			}
			if (flag && !KAnimBatch.AtlasList.reported_overflow)
			{
				string text = "Atlas overflow: (startIndex=" + this.startIdx.ToString() + ")\n";
				int num2 = 0;
				foreach (Texture2D texture2D in this.atlases)
				{
					text = string.Concat(new string[]
					{
						text,
						(this.startIdx + num2).ToString(),
						": ",
						texture2D.name,
						"\n"
					});
					num2++;
				}
				global::Debug.LogWarning(text);
				KAnimBatch.AtlasList.reported_overflow = true;
			}
		}

		public void Clear(int start_idx)
		{
			this.atlases.Clear();
			this.startIdx = start_idx;
		}

		public int GetAtlasIdx(Texture2D atlas)
		{
			for (int i = 0; i < this.atlases.Count; i++)
			{
				if (this.atlases[i] == atlas)
				{
					return i + this.startIdx;
				}
			}
			return -1;
		}

		public int Count
		{
			get
			{
				return this.atlases.Count;
			}
		}

		private List<Texture2D> atlases = new List<Texture2D>();

		private int startIdx;

		private static bool reported_overflow;
	}
}
