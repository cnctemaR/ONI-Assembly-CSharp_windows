using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class KAnimBatchGroup
{
	public KAnimBatchGroup(HashedString id)
	{
		this.data = KAnimBatchManager.Instance().GetBatchGroupData(id);
		this.materials = new Material[5];
		this.batchID = id;
		KAnimGroupFile.Group group = KAnimGroupFile.GetGroup(id);
		if (group == null)
		{
			return;
		}
		this.maxGroupSize = group.maxGroupSize;
		if (this.maxGroupSize <= 0)
		{
			this.maxGroupSize = 30;
		}
		this.SetupMeshData();
		this.InitBuildAndAnimTex();
	}

	public static void FinalizeTextureCache()
	{
		KAnimBatchGroup.cache.Finalise();
	}

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
		material.SetFloat(KAnimBatchGroup.ShaderProperty_SYMBOLS_PER_BUILD, (float)this.data.maxSymbolsPerBuild);
		material.SetFloat(KAnimBatchGroup.ShaderProperty_ANIM_TEXTURE_START_OFFSET, (float)this.data.animDataStartOffset);
		material.SetFloat(KAnimBatchGroup.ShaderProperty_SYMBOL_OVERRIDES_PER_BUILD, (float)this.data.symbolFrameInstances.Count);
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

	public KBatchGroupData data { get; private set; }

	public KAnimBatchGroup.KAnimBatchTextureCache.Entry buildAndAnimTex { get; private set; }

	public bool InitOK
	{
		get
		{
			return this.float4sPerSide > 0;
		}
	}

	public void FreeResources()
	{
		if (this.buildAndAnimTex != null)
		{
			KAnimBatchGroup.cache.Free(this.buildAndAnimTex);
			this.buildAndAnimTex = null;
		}
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
		if (this.data != null)
		{
			this.data.FreeResources();
		}
		this.data = null;
	}

	public static int GetBestTextureSize(float cost)
	{
		float num = Mathf.Sqrt(cost);
		int num2 = Mathf.CeilToInt(num);
		int num3 = 32;
		float num4 = (float)num2 / (float)num3;
		int num5 = Mathf.CeilToInt(num4);
		return num5 * num3;
	}

	private void SetupMeshData()
	{
		this.maxGroupSize = Mathf.Min(this.maxGroupSize, 30);
		this.mesh = this.BuildMesh(this.maxGroupSize * this.data.maxVisibleSymbols);
		float num = (float)(this.maxGroupSize * 28) / 4f;
		this.float4sPerSide = KAnimBatchGroup.GetBestTextureSize(num);
	}

	private float GetBuildDataSize()
	{
		int num = this.data.GetBuildSymbolFrameCount() * 16;
		return (float)num / 4f;
	}

	private float GetAnimDataSize()
	{
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
		return (float)num / 4f;
	}

	public void InitBuildAndAnimTex()
	{
		float num = this.GetBuildDataSize() + this.GetAnimDataSize();
		int bestTextureSize = KAnimBatchGroup.GetBestTextureSize(num);
		this.buildAndAnimTex = KAnimBatchGroup.cache.Get(bestTextureSize, KAnimBatchGroup.ShaderProperty_buildAndAnimTex, KAnimBatchGroup.ShaderProperty_BUILD_AND_ANIM_TEXTURE_SIZE);
		this.buildAndAnimTex.name = "BuildAndAnimData:" + this.batchID.ToString();
		if (num > (float)(this.buildAndAnimTex.width * this.buildAndAnimTex.height))
		{
			global::Debug.LogErrorFormat("Texture is the wrong size! {0} <= {1}", new object[]
			{
				num,
				this.buildAndAnimTex.width * this.buildAndAnimTex.height
			});
		}
		int num2 = this.data.WriteBuildData(this.data.symbolFrameInstances, this.buildAndAnimTex.floats);
		this.data.WriteAnimData(num2, this.buildAndAnimTex.floats);
		this.buildAndAnimTex.LoadRawTextureData();
		this.buildAndAnimTex.Apply();
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

	public KAnimBatchGroup.KAnimBatchTextureCache.Entry CreateTexture(string name, int size_in_floats, int texture_property_id, int texture_size_property_id)
	{
		DebugUtil.Assert(size_in_floats > 0);
		KAnimBatchGroup.KAnimBatchTextureCache.Entry entry = KAnimBatchGroup.cache.Get(size_in_floats, texture_property_id, texture_size_property_id);
		entry.name = name;
		return entry;
	}

	public KAnimBatchGroup.KAnimBatchTextureCache.Entry CreateTexture()
	{
		if (this.float4sPerSide <= 0)
		{
			global::Debug.LogErrorFormat("Need to init AnimBatchGroup [{0}] first!", new object[] { this.batchID });
		}
		return this.CreateTexture("InstanceData:" + this.batchID.ToString(), this.float4sPerSide, KAnimBatchGroup.ShaderProperty_instanceTex, KAnimBatchGroup.ShaderProperty_INSTANCE_TEXTURE_SIZE);
	}

	public void FreeTexture(KAnimBatchGroup.KAnimBatchTextureCache.Entry entry)
	{
		KAnimBatchGroup.cache.Free(entry);
	}

	public void GetDataTextures(MaterialPropertyBlock matProperties, KAnimBatch.AtlasList atlases)
	{
		if (this.buildAndAnimTex != null)
		{
			this.buildAndAnimTex.SetTextureAndSize(matProperties);
		}
		matProperties.SetFloat(KAnimBatchGroup.ShaderProperty_ANIM_TEXTURE_START_OFFSET, (float)this.data.animDataStartOffset);
		for (int i = 0; i < this.data.textures.Count; i++)
		{
			atlases.Add(this.data.textures[i]);
		}
	}

	public static int ShaderProperty_SYMBOLS_PER_BUILD = Shader.PropertyToID("SYMBOLS_PER_BUILD");

	public static int ShaderProperty_ANIM_TEXTURE_START_OFFSET = Shader.PropertyToID("ANIM_TEXTURE_START_OFFSET");

	public static int ShaderProperty_SYMBOL_OVERRIDES_PER_BUILD = Shader.PropertyToID("SYMBOL_OVERRIDES_PER_BUILD");

	private static KAnimBatchGroup.KAnimBatchTextureCache cache = new KAnimBatchGroup.KAnimBatchTextureCache();

	public int batchCount;

	private int float4sPerSide;

	private static int ShaderProperty_BUILD_AND_ANIM_TEXTURE_SIZE = Shader.PropertyToID("BUILD_AND_ANIM_TEXTURE_SIZE");

	private static int ShaderProperty_buildAndAnimTex = Shader.PropertyToID("buildAndAnimTex");

	private static int ShaderProperty_INSTANCE_TEXTURE_SIZE = Shader.PropertyToID("INSTANCE_TEXTURE_SIZE");

	private static int ShaderProperty_instanceTex = Shader.PropertyToID("instanceTex");

	private Material[] materials;

	public class KAnimBatchTextureCache
	{
		public KAnimBatchGroup.KAnimBatchTextureCache.Entry Get(int float4s_per_side, int texture_property_id, int texture_size_property_id)
		{
			if (!this.unused.ContainsKey(float4s_per_side))
			{
				this.unused.Add(float4s_per_side, new List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>());
			}
			KAnimBatchGroup.KAnimBatchTextureCache.Entry entry;
			if (this.unused[float4s_per_side].Count > 0)
			{
				entry = this.unused[float4s_per_side][0];
				this.unused[float4s_per_side].RemoveAt(0);
			}
			else
			{
				entry = new KAnimBatchGroup.KAnimBatchTextureCache.Entry(float4s_per_side);
			}
			entry.texturePropertyId = texture_property_id;
			entry.textureSizePropertyId = texture_size_property_id;
			if (!this.inuse.ContainsKey(float4s_per_side))
			{
				this.inuse.Add(float4s_per_side, new List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>());
			}
			this.inuse[float4s_per_side].Add(entry);
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
			public Entry(int float4s_per_side)
			{
				this.texture = new Texture2D(float4s_per_side, float4s_per_side, TextureFormat.RGBAFloat, false);
				this.texture.wrapMode = TextureWrapMode.Clamp;
				this.texture.filterMode = FilterMode.Point;
				this.texture.anisoLevel = 0;
				this.floatConverter = new KAnimBatchGroup.KAnimBatchTextureCache.Entry.ByteToFloatConverter
				{
					bytes = new byte[float4s_per_side * float4s_per_side * 4 * 4]
				};
			}

			public Texture2D texture { get; private set; }

			public byte[] bytes
			{
				get
				{
					return this.floatConverter.bytes;
				}
			}

			public float[] floats
			{
				get
				{
					return this.floatConverter.floats;
				}
			}

			public void SetTextureAndSize(MaterialPropertyBlock property_block)
			{
				property_block.SetTexture(this.texturePropertyId, this.texture);
				property_block.SetVector(this.textureSizePropertyId, new Vector4(this.texelSize.x, this.texelSize.y, (float)this.width, (float)this.height));
			}

			public void Apply()
			{
				this.texture.Apply();
			}

			public void LoadRawTextureData()
			{
				this.texture.LoadRawTextureData(this.bytes);
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

			private KAnimBatchGroup.KAnimBatchTextureCache.Entry.ByteToFloatConverter floatConverter;

			public int texturePropertyId;

			public int textureSizePropertyId;

			[StructLayout(LayoutKind.Explicit)]
			public struct ByteToFloatConverter
			{
				[FieldOffset(0)]
				public byte[] bytes;

				[FieldOffset(0)]
				public float[] floats;
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
}
