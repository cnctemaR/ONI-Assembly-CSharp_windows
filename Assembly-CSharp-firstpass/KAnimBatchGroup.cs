using System;
using System.Collections.Generic;
using UnityEngine;

public class KAnimBatchGroup
{
	public KAnimBatchGroup(HashedString id)
	{
		this.buildByteToFloat.bytes = null;
		this.data = KAnimBatchManager.Instance().GetBatchGroupData(id, false);
		this.materials = new Material[5];
		this.batchID = id;
		KAnimGroupFile.Group group = KAnimGroupFile.GetGroup(id);
		if (group == null)
		{
			return;
		}
		this.maxGroupSize = group.maxGroupSize;
		this.isMultiInstance = group.isMultiInstance;
		if (group.isMultiInstance)
		{
			this.maxGroupSize = 1;
		}
		else if (this.maxGroupSize <= 0)
		{
			this.maxGroupSize = 60;
		}
		this.SetupMeshData(0);
		this.InitialiseStaticData();
	}

	public static void FinalizeTextureCache()
	{
		KAnimBatchGroup.cache.Finalise();
	}

	public int layer { get; private set; }

	private Material CreateMaterial(KAnimBatchGroup.MaterialType material_type)
	{
		Material material;
		switch (material_type)
		{
		case KAnimBatchGroup.MaterialType.Simple:
			material = new Material(Shader.Find("Klei/AnimationSimple"));
			goto IL_0075;
		case KAnimBatchGroup.MaterialType.UI:
			material = new Material(Shader.Find("Klei/BatchedAnimationUI"));
			goto IL_0075;
		case KAnimBatchGroup.MaterialType.Overlay:
			material = new Material(Shader.Find("Klei/AnimationOverlay"));
			goto IL_0075;
		}
		material = new Material(Shader.Find("Klei/BatchedAnimation"));
		IL_0075:
		material.name = "Material:" + this.batchID.ToString();
		return material;
	}

	public Material GetMaterial(KAnimBatchGroup.MaterialType material_type)
	{
		if (this.materials[(int)material_type] == null)
		{
			this.materials[(int)material_type] = this.CreateMaterial(material_type);
		}
		return this.materials[(int)material_type];
	}

	public int maxGroupSize { get; private set; }

	public Mesh mesh { get; private set; }

	public HashedString batchID { get; private set; }

	public KAnimBatchGroup.DataType dataType { get; private set; }

	public void SetDataType(KAnimBatchGroup.DataType type)
	{
		this.dataType = type;
	}

	public KBatchGroupData data { get; private set; }

	public KAnimBatchGroup.KAnimBatchTextureCache.Entry animDataTex { get; private set; }

	public bool InitOK
	{
		get
		{
			return this.texureSize > 0 || this.dataType == KAnimBatchGroup.DataType.AnimOnly;
		}
	}

	public void FreeResources()
	{
		for (int i = 0; i < 5; i++)
		{
			if (this.materials[i] != null)
			{
				global::UnityEngine.Object.Destroy(this.materials[i]);
				this.materials[i] = null;
			}
		}
		if (this.mesh != null)
		{
			global::UnityEngine.Object.Destroy(this.mesh);
			this.mesh = null;
		}
		if (this.animDataTex != null)
		{
			KAnimBatchGroup.cache.Free(this.animDataTex);
			this.animDataTex = null;
		}
		if (this.instances != null)
		{
			foreach (KeyValuePair<int, BatchGroupInstance> keyValuePair in this.instances)
			{
				keyValuePair.Value.DestroyTex();
				keyValuePair.Value.FreeResources();
			}
			this.instances.Clear();
		}
		if (this.data != null)
		{
			this.data.FreeResources();
		}
		this.data = null;
	}

	private int GetBestTextureSize(float cost)
	{
		float num = Mathf.Sqrt(cost);
		return Mathf.CeilToInt(num);
	}

	private void SetupMeshData(int layer)
	{
		this.layer = layer;
		this.maxGroupSize = Mathf.Min(this.maxGroupSize, 60);
		this.mesh = this.BuildMesh(this.maxGroupSize * this.data.maxVisibleSymbols);
		float num = (float)(this.maxGroupSize * 64) / 4f;
		this.texureSize = this.GetBestTextureSize(num);
	}

	public BatchGroupInstance GetBatchGroupInstance(global::UnityEngine.Object obj)
	{
		if (this.instances == null || (obj == null && this.isMultiInstance))
		{
			return null;
		}
		int num = -1;
		if (this.isMultiInstance)
		{
			num = obj.GetInstanceID();
			if (!this.instances.ContainsKey(num))
			{
				this.instances.Add(num, new BatchGroupInstance(this));
				this.InitBuild(this.instances[num]);
			}
		}
		return this.instances[num];
	}

	public void FreeBatchGroupInstance(global::UnityEngine.Object obj, BatchGroupInstance batch_group_instance)
	{
		if (this.isMultiInstance)
		{
			int instanceID = obj.GetInstanceID();
			this.instances.Remove(instanceID);
			batch_group_instance.FreeResources();
		}
	}

	public bool isMultiInstance
	{
		get
		{
			return this._isMultiInstance;
		}
		set
		{
			this._isMultiInstance = value;
			this.instances = new Dictionary<int, BatchGroupInstance>();
			if (!this.isMultiInstance)
			{
				this.instances.Add(-1, new BatchGroupInstance(this));
			}
		}
	}

	public void ClearMuiltiInstanceData()
	{
		if (this.isMultiInstance && this.instances.Count > 1)
		{
			BatchGroupInstance batchGroupInstance = null;
			foreach (KeyValuePair<int, BatchGroupInstance> keyValuePair in this.instances)
			{
				if (keyValuePair.Key != -1)
				{
					keyValuePair.Value.FreeResources();
				}
				else
				{
					batchGroupInstance = keyValuePair.Value;
				}
			}
			this.instances.Clear();
			if (batchGroupInstance != null)
			{
				this.instances.Add(-1, batchGroupInstance);
			}
		}
	}

	private void InitialiseStaticData()
	{
		if (!this.isMultiInstance)
		{
			this.InitBuild(this.instances[-1]);
		}
		this.InitAnim();
	}

	private void InitAnim()
	{
		if (this.dataType == KAnimBatchGroup.DataType.DontRender)
		{
			return;
		}
		int num = 4;
		List<KAnim.Anim.Frame> animFrames = this.data.GetAnimFrames();
		if (animFrames.Count == 0)
		{
			num += this.data.symbolFrameInstances.Count * 4;
			num += this.data.symbolFrameInstances.Count * 16;
		}
		else
		{
			num += animFrames.Count * 4;
			List<KAnim.Anim.FrameElement> animFrameElements = this.data.GetAnimFrameElements();
			num += animFrameElements.Count * 16;
		}
		float num2 = (float)num / 4f;
		int bestTextureSize = this.GetBestTextureSize(num2);
		this.animDataTex = null;
		this.animDataTex = KAnimBatchGroup.cache.Get(bestTextureSize);
		this.animDataTex.name = "AnimData:" + this.batchID.ToString();
		int num3 = bestTextureSize * bestTextureSize * 4 * 4;
		KAnimConverter.ByteToFloatConverter byteToFloatConverter = new KAnimConverter.ByteToFloatConverter
		{
			bytes = new byte[num3]
		};
		this.data.WriteAnimData(byteToFloatConverter.floats);
		this.animDataTex.LoadRawTextureData(byteToFloatConverter.bytes);
		this.animDataTex.Apply();
	}

	public void InitBuild(BatchGroupInstance instance)
	{
		if (this.dataType == KAnimBatchGroup.DataType.DontRender)
		{
			return;
		}
		int num = this.data.GetBuildSymbolFrameCount() * 32;
		float num2 = (float)num / 4f;
		int bestTextureSize = this.GetBestTextureSize(num2);
		if (instance.buildTex == null || bestTextureSize != instance.buildTex.width)
		{
			instance.DestroyTex();
			instance.buildTex = KAnimBatchGroup.cache.Get(bestTextureSize);
			instance.buildTex.name = "BuildData:" + this.batchID.ToString();
			this.buildByteToFloat.bytes = instance.buildTex.bytes;
		}
		this.data.WriteBuildData(instance, this.buildByteToFloat.floats);
		instance.buildTex.LoadRawTextureData(this.buildByteToFloat.bytes);
		instance.buildTex.Apply();
	}

	private Mesh BuildMesh(int numQuads)
	{
		Mesh mesh = new Mesh();
		int[] array = new int[numQuads * 6];
		for (int i = 0; i < numQuads; i++)
		{
			int num = i * 6;
			int num2 = i * 4;
			array[num] = num2;
			array[num + 1] = num2 + 1;
			array[num + 2] = num2 + 2;
			array[num + 3] = num2 + 1;
			array[num + 4] = num2 + 2;
			array[num + 5] = num2 + 3;
		}
		Vector3[] array2 = new Vector3[numQuads * 4];
		Vector2[] array3 = new Vector2[numQuads * 4];
		Vector4[] array4 = new Vector4[numQuads * 4];
		for (int j = 0; j < numQuads; j++)
		{
			int num3 = j * 4;
			array2[num3] = Vector3.zero;
			array2[num3 + 1] = Vector3.zero;
			array2[num3 + 2] = Vector3.zero;
			array2[num3 + 3] = Vector3.zero;
			Vector2 vector = new Vector2((float)(j / this.data.maxVisibleSymbols), (float)(this.data.maxVisibleSymbols - j % this.data.maxVisibleSymbols - 1));
			array3[num3] = vector;
			array3[num3 + 1] = vector;
			array3[num3 + 2] = vector;
			array3[num3 + 3] = vector;
			array4[num3] = new Vector4(0f, (float)num3, (float)j);
			array4[num3 + 1] = new Vector4(1f, (float)num3, (float)j);
			array4[num3 + 2] = new Vector4(2f, (float)num3, (float)j);
			array4[num3 + 3] = new Vector4(3f, (float)num3, (float)j);
		}
		mesh.name = "BatchGroup:" + this.batchID.ToString();
		mesh.vertices = array2;
		mesh.SetUVs(0, new List<Vector2>(array3));
		mesh.SetUVs(1, new List<Vector4>(array4));
		mesh.SetIndices(array, MeshTopology.Triangles, 0);
		mesh.bounds = new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
		return mesh;
	}

	public KAnimBatchGroup.KAnimBatchTextureCache.Entry CreateTexture()
	{
		if (this.dataType == KAnimBatchGroup.DataType.DontRender)
		{
			return null;
		}
		KAnimBatchGroup.KAnimBatchTextureCache.Entry entry = KAnimBatchGroup.cache.Get(this.texureSize);
		entry.name = "InstanceData:" + this.batchID.ToString();
		return entry;
	}

	public void FreeTexture(KAnimBatchGroup.KAnimBatchTextureCache.Entry entry)
	{
		KAnimBatchGroup.cache.Free(entry);
	}

	public void GetDataTextures(BatchGroupInstance instance, MaterialPropertyBlock matProperties)
	{
		matProperties.SetFloat("MAX_VISIBLE_SYMBOLS", (float)this.data.maxVisibleSymbols);
		if (this.animDataTex != null)
		{
			matProperties.SetTexture("animTex", this.animDataTex.texture);
			matProperties.SetVector("ANIM_TEXEL_SIZE", this.animDataTex.texelSize);
			matProperties.SetVector("ANIM_TEXTURE_SIZE", new Vector2((float)this.animDataTex.width, (float)this.animDataTex.height));
		}
		if (instance.buildTex != null)
		{
			matProperties.SetTexture("buildTex", instance.buildTex.texture);
			matProperties.SetVector("BUILD_TEXEL_SIZE", instance.buildTex.texelSize);
			matProperties.SetVector("BUILD_TEXTURE_SIZE", new Vector2((float)instance.buildTex.width, (float)instance.buildTex.height));
		}
		for (int i = 0; i < this.data.textures.Count; i++)
		{
			Texture2D texture2D = this.data.textures[i];
			if (texture2D != null)
			{
				matProperties.SetTexture("atlas" + i, texture2D);
			}
		}
		for (int j = 0; j < instance.textures.Count; j++)
		{
			Texture2D texture2D2 = instance.textures[j];
			if (texture2D2 != null)
			{
				matProperties.SetTexture("atlas" + (this.data.textures.Count + j), texture2D2);
			}
		}
	}

	private static KAnimBatchGroup.KAnimBatchTextureCache cache = new KAnimBatchGroup.KAnimBatchTextureCache();

	public int batchCount;

	private int texureSize;

	private Dictionary<int, BatchGroupInstance> instances;

	private bool _isMultiInstance;

	private KAnimConverter.ByteToFloatConverter buildByteToFloat;

	private Material[] materials;

	public class KAnimBatchTextureCache
	{
		public KAnimBatchGroup.KAnimBatchTextureCache.Entry Get(int size)
		{
			if (!this.unused.ContainsKey(size))
			{
				this.unused.Add(size, new List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>());
			}
			KAnimBatchGroup.KAnimBatchTextureCache.Entry entry;
			if (this.unused[size].Count > 0)
			{
				entry = this.unused[size][0];
				this.unused[size].RemoveAt(0);
			}
			else
			{
				entry = new KAnimBatchGroup.KAnimBatchTextureCache.Entry(size);
			}
			if (!this.inuse.ContainsKey(size))
			{
				this.inuse.Add(size, new List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>());
			}
			this.inuse[size].Add(entry);
			return entry;
		}

		public void Free(KAnimBatchGroup.KAnimBatchTextureCache.Entry entry)
		{
			int width = entry.texture.width;
			if (this.inuse.ContainsKey(width))
			{
				this.inuse[width].Remove(entry);
			}
			if (this.unused.ContainsKey(width))
			{
				this.unused[width].Add(entry);
			}
		}

		public void Finalise()
		{
			foreach (KeyValuePair<int, List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>> keyValuePair in this.inuse)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					global::UnityEngine.Object.Destroy(keyValuePair.Value[i].texture);
				}
			}
			this.inuse.Clear();
			foreach (KeyValuePair<int, List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>> keyValuePair2 in this.unused)
			{
				for (int j = 0; j < keyValuePair2.Value.Count; j++)
				{
					global::UnityEngine.Object.Destroy(keyValuePair2.Value[j].texture);
				}
			}
			this.unused.Clear();
		}

		private Dictionary<int, List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>> unused = new Dictionary<int, List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>>();

		private Dictionary<int, List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>> inuse = new Dictionary<int, List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>>();

		public class Entry
		{
			public Entry(int size)
			{
				this.texture = new Texture2D(size, size, TextureFormat.RGBAFloat, false);
				this.texture.wrapMode = TextureWrapMode.Clamp;
				this.texture.filterMode = FilterMode.Point;
				this.texture.anisoLevel = 0;
				this.bytes = new byte[size * size * 4 * 4];
			}

			public Texture2D texture { get; private set; }

			public byte[] bytes { get; private set; }

			public void Apply()
			{
				this.texture.Apply();
			}

			public void LoadRawTextureData(byte[] bytes)
			{
				this.texture.LoadRawTextureData(bytes);
			}

			public int width
			{
				get
				{
					return this.texture.width;
				}
			}

			public int height
			{
				get
				{
					return this.texture.height;
				}
			}

			public Vector2 texelSize
			{
				get
				{
					return this.texture.texelSize;
				}
			}

			public string name
			{
				get
				{
					return this.texture.name;
				}
				set
				{
					this.texture.name = value;
				}
			}
		}
	}

	public enum RendererType
	{
		Default,
		UI,
		StaticBatch,
		DontRender,
		AnimOnly
	}

	public enum MaterialType
	{
		Default,
		Simple,
		Placer,
		UI,
		Overlay,
		NumMaterials
	}

	public enum DataType
	{
		Default,
		UI,
		DontRender,
		AnimOnly
	}
}
