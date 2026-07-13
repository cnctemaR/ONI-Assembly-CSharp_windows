using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class KAnimBatchGroup
{
	public static void FinalizeTextureCache()
	{
		KAnimBatchGroup.cache.Finalise();
	}

	public static string GetShaderNameForMaterialType(KAnimBatchGroup.MaterialType material_type)
	{
		switch (material_type)
		{
		case KAnimBatchGroup.MaterialType.Simple:
			return "Klei/AnimationSimple";
		case KAnimBatchGroup.MaterialType.UI:
			return "Klei/BatchedAnimationUI";
		case KAnimBatchGroup.MaterialType.Invisible:
			return "Klei/AnimationInvisible";
		case KAnimBatchGroup.MaterialType.Human:
			return "Klei/BatchedAnimationHuman";
		}
		return "Klei/BatchedAnimation";
	}

	private Material CreateMaterial(KAnimBatchGroup.MaterialType material_type)
	{
		Material material = new Material(Shader.Find(KAnimBatchGroup.GetShaderNameForMaterialType(material_type)));
		material.name = "Material:" + this.batchID.ToString();
		material.SetFloat(KAnimBatchGroup.ShaderProperty_SYMBOLS_PER_BUILD, (float)this.data.maxSymbolsPerBuild);
		material.SetFloat(KAnimBatchGroup.ShaderProperty_ANIM_TEXTURE_START_OFFSET, (float)(this.data.animDataStartOffset / 4));
		material.SetFloat(KAnimBatchGroup.ShaderProperty_SYMBOL_OVERRIDES_PER_BUILD, (float)this.data.symbolFrameInstances.Count);
		return material;
	}

	private Material CreatePostProcesingMaterial(KAnimConverter.PostProcessingEffects effect)
	{
		Material material = new Material(Shader.Find(KAnimConverter.ShaderNameForPostProcessingEffect[effect]));
		material.name = "Material:" + this.batchID.ToString() + "_PST";
		material.SetFloat(KAnimBatchGroup.ShaderProperty_SYMBOLS_PER_BUILD, (float)this.data.maxSymbolsPerBuild);
		material.SetFloat(KAnimBatchGroup.ShaderProperty_ANIM_TEXTURE_START_OFFSET, (float)(this.data.animDataStartOffset / 4));
		material.SetFloat(KAnimBatchGroup.ShaderProperty_SYMBOL_OVERRIDES_PER_BUILD, (float)this.data.symbolFrameInstances.Count);
		return material;
	}

	public Material GetPostProcessingMaterial(KAnimConverter.PostProcessingEffects effectToRender)
	{
		Material material;
		if (!this.postEffectMaterials.TryGetValue(effectToRender, out material))
		{
			material = this.CreatePostProcesingMaterial(effectToRender);
			this.postEffectMaterials[effectToRender] = material;
		}
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

	public KAnimBatchGroup(HashedString id)
	{
		this.data = KAnimBatchManager.Instance().GetBatchGroupData(id);
		this.materials = new Material[6];
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

	public bool InitOK
	{
		get
		{
			return this.size.x > 0;
		}
	}

	public void FreeResources()
	{
		if (this.buildAndAnimTex != null)
		{
			KAnimBatchGroup.cache.Free(this.buildAndAnimTex);
			this.buildAndAnimTex = null;
		}
		for (int i = 0; i < 6; i++)
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

	public static Vector2I GetBestTextureSize(int cost)
	{
		int num = MathUtil.RoundToNextPowerOfTwo(Mathf.CeilToInt(Mathf.Sqrt((float)cost)));
		int num2 = (cost - 1) / num + 1;
		int num3 = 16;
		num2 = Mathf.CeilToInt((float)num2 / (float)num3) * num3;
		return new Vector2I(num, num2);
	}

	private void SetupMeshData()
	{
		global::Debug.Assert(this.maxGroupSize > 0, "Group size must be >0");
		this.maxGroupSize = Mathf.Min(this.maxGroupSize, 30);
		this.mesh = this.BuildMesh(this.maxGroupSize * this.data.maxVisibleSymbols);
		int num = Mathf.CeilToInt((float)(this.maxGroupSize * 28) / 4f);
		this.size = KAnimBatchGroup.GetBestTextureSize(num);
	}

	private int GetBuildDataSize()
	{
		return Mathf.CeilToInt((float)(this.data.GetBuildSymbolFrameCount() * 12) / 4f);
	}

	private int GetAnimDataSize()
	{
		int num = 4;
		List<KAnim.Anim.Frame> animFrames = this.data.GetAnimFrames();
		if (animFrames.Count == 0)
		{
			num += this.data.symbolFrameInstances.Count * 4;
			num += this.data.symbolFrameInstances.Count * 8;
		}
		else
		{
			num += animFrames.Count * 4;
			List<KAnim.Anim.FrameElement> animFrameElements = this.data.GetAnimFrameElements();
			num += animFrameElements.Count * 8;
		}
		return Mathf.CeilToInt((float)num / 4f);
	}

	public void InitBuildAndAnimTex()
	{
		int num = this.GetBuildDataSize() + this.GetAnimDataSize();
		Vector2I bestTextureSize = KAnimBatchGroup.GetBestTextureSize(num);
		this.buildAndAnimTex = KAnimBatchGroup.cache.Get(bestTextureSize.x, bestTextureSize.y, KAnimBatchGroup.ShaderProperty_buildAndAnimTex, KAnimBatchGroup.ShaderProperty_BUILD_AND_ANIM_TEXTURE_SIZE);
		this.buildAndAnimTex.name = "BuildAndAnimData:" + this.batchID.ToString();
		if (num > this.buildAndAnimTex.width * this.buildAndAnimTex.height)
		{
			global::Debug.LogErrorFormat("Texture is the wrong size! {0} <= {1}", new object[]
			{
				num,
				this.buildAndAnimTex.width * this.buildAndAnimTex.height
			});
		}
		NativeArray<float> floatDataPointer = this.buildAndAnimTex.GetFloatDataPointer();
		int num2 = this.data.WriteBuildData(floatDataPointer);
		this.data.WriteAnimData(num2, floatDataPointer);
		this.buildAndAnimTex.texture.Apply(false, true);
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
		Vector3[] array3 = new Vector3[numQuads * 4];
		for (int j = 0; j < numQuads; j++)
		{
			int num3 = j * 4;
			array2[num3] = Vector3.zero;
			array2[num3 + 1] = Vector3.zero;
			array2[num3 + 2] = Vector3.zero;
			array2[num3 + 3] = Vector3.zero;
			int num4 = j / this.data.maxVisibleSymbols;
			int num5 = this.data.maxVisibleSymbols - j % this.data.maxVisibleSymbols - 1;
			array3[num3] = new Vector3((float)num4, (float)num5, 0f);
			array3[num3 + 1] = new Vector3((float)num4, (float)num5, 1f);
			array3[num3 + 2] = new Vector3((float)num4, (float)num5, 2f);
			array3[num3 + 3] = new Vector3((float)num4, (float)num5, 3f);
		}
		mesh.name = "BatchGroup:" + this.batchID.ToString();
		mesh.vertices = array2;
		mesh.SetUVs(0, array3);
		mesh.SetIndices(array, MeshTopology.Triangles, 0);
		mesh.bounds = new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
		return mesh;
	}

	public KAnimBatchGroup.KAnimBatchTextureCache.Entry CreateTexture(string name, int width, int height, int texture_property_id, int texture_size_property_id)
	{
		DebugUtil.Assert(width > 0 && height > 0);
		KAnimBatchGroup.KAnimBatchTextureCache.Entry entry = KAnimBatchGroup.cache.Get(width, height, texture_property_id, texture_size_property_id);
		entry.name = name;
		return entry;
	}

	public KAnimBatchGroup.KAnimBatchTextureCache.Entry CreateTexture()
	{
		if (this.size.x <= 0)
		{
			global::Debug.LogErrorFormat("Need to init AnimBatchGroup [{0}] first!", new object[] { this.batchID });
		}
		return this.CreateTexture("InstanceData:" + this.batchID.ToString(), this.size.x, this.size.y, KAnimBatchGroup.ShaderProperty_instanceTex, KAnimBatchGroup.ShaderProperty_INSTANCE_TEXTURE_SIZE);
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
		matProperties.SetFloat(KAnimBatchGroup.ShaderProperty_ANIM_TEXTURE_START_OFFSET, (float)(this.data.animDataStartOffset / 4));
		for (int i = 0; i < this.data.textures.Count; i++)
		{
			atlases.Add(this.data.textures[i]);
		}
	}

	public static int ShaderProperty_SYMBOLS_PER_BUILD = Shader.PropertyToID("SYMBOLS_PER_BUILD");

	public static int ShaderProperty_ANIM_TEXTURE_START_OFFSET = Shader.PropertyToID("ANIM_TEXTURE_START_OFFSET");

	public static int ShaderProperty_SYMBOL_OVERRIDES_PER_BUILD = Shader.PropertyToID("SYMBOL_OVERRIDES_PER_BUILD");

	private static Color ResetColor = new Color(0f, 0f, 0f, 0f);

	private static KAnimBatchGroup.KAnimBatchTextureCache cache = new KAnimBatchGroup.KAnimBatchTextureCache();

	public int batchCount;

	private Vector2I size = new Vector2I(0, 0);

	private static int ShaderProperty_BUILD_AND_ANIM_TEXTURE_SIZE = Shader.PropertyToID("BUILD_AND_ANIM_TEXTURE_SIZE");

	private static int ShaderProperty_buildAndAnimTex = Shader.PropertyToID("buildAndAnimTex");

	private static int ShaderProperty_INSTANCE_TEXTURE_SIZE = Shader.PropertyToID("INSTANCE_TEXTURE_SIZE");

	private static int ShaderProperty_instanceTex = Shader.PropertyToID("instanceTex");

	private Dictionary<KAnimConverter.PostProcessingEffects, Material> postEffectMaterials = new Dictionary<KAnimConverter.PostProcessingEffects, Material>();

	private Material[] materials;

	public class KAnimBatchTextureCache
	{
		public KAnimBatchGroup.KAnimBatchTextureCache.Entry Get(int float4s_width, int float4s_height, int texture_property_id, int texture_size_property_id)
		{
			Vector2I vector2I = new Vector2I(float4s_width, float4s_height);
			List<KAnimBatchGroup.KAnimBatchTextureCache.Entry> list = null;
			if (!this.unused.TryGetValue(vector2I, out list))
			{
				list = new List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>();
				this.unused.Add(vector2I, list);
			}
			KAnimBatchGroup.KAnimBatchTextureCache.Entry entry;
			if (list.Count > 0)
			{
				int num = list.Count - 1;
				entry = list[num];
				list.RemoveAt(num);
			}
			else
			{
				entry = new KAnimBatchGroup.KAnimBatchTextureCache.Entry(float4s_width, float4s_height);
			}
			entry.texturePropertyId = texture_property_id;
			entry.textureSizePropertyId = texture_size_property_id;
			List<KAnimBatchGroup.KAnimBatchTextureCache.Entry> list2 = null;
			if (!this.inuse.TryGetValue(vector2I, out list2))
			{
				list2 = new List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>();
				this.inuse.Add(vector2I, list2);
			}
			list2.Add(entry);
			entry.cacheIndex = list2.Count - 1;
			return entry;
		}

		public void Free(KAnimBatchGroup.KAnimBatchTextureCache.Entry entry)
		{
			Vector2I vector2I = new Vector2I(entry.texture.width, entry.texture.height);
			int cacheIndex = entry.cacheIndex;
			entry.cacheIndex = -1;
			List<KAnimBatchGroup.KAnimBatchTextureCache.Entry> list = null;
			if (this.inuse.TryGetValue(vector2I, out list))
			{
				int num = list.Count - 1;
				if (num != cacheIndex)
				{
					KAnimBatchGroup.KAnimBatchTextureCache.Entry entry2 = list[num];
					entry2.cacheIndex = cacheIndex;
					list[cacheIndex] = entry2;
					list[num] = null;
					list.RemoveAt(num);
				}
			}
			List<KAnimBatchGroup.KAnimBatchTextureCache.Entry> list2 = null;
			if (this.unused.TryGetValue(vector2I, out list2))
			{
				list2.Add(entry);
			}
		}

		public void Finalise()
		{
			foreach (KeyValuePair<Vector2I, List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>> keyValuePair in this.inuse)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					global::UnityEngine.Object.Destroy(keyValuePair.Value[i].texture);
				}
			}
			this.inuse.Clear();
			foreach (KeyValuePair<Vector2I, List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>> keyValuePair2 in this.unused)
			{
				for (int j = 0; j < keyValuePair2.Value.Count; j++)
				{
					global::UnityEngine.Object.Destroy(keyValuePair2.Value[j].texture);
				}
			}
			this.unused.Clear();
		}

		private Dictionary<Vector2I, List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>> unused = new Dictionary<Vector2I, List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>>();

		private Dictionary<Vector2I, List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>> inuse = new Dictionary<Vector2I, List<KAnimBatchGroup.KAnimBatchTextureCache.Entry>>();

		public class Entry
		{
			public Texture2D texture { get; private set; }

			public Entry(int float4s_width, int float4s_height)
			{
				if (float4s_width <= 0 || !MathUtil.IsPowerOfTwo(float4s_width))
				{
					DebugUtil.DevAssert(false, string.Format("Width ({0}) must be a power of two in order for fast indexing of the texture to work!", float4s_width), null);
				}
				this.texture = new Texture2D(float4s_width, float4s_height, TextureFormat.RGBAFloat, false);
				this.texture.wrapMode = TextureWrapMode.Clamp;
				this.texture.filterMode = FilterMode.Point;
				this.texture.anisoLevel = 0;
				int num = float4s_width * float4s_height;
				NativeArray<Color> rawTextureData = this.texture.GetRawTextureData<Color>();
				for (int i = 0; i < num; i++)
				{
					rawTextureData[i] = KAnimBatchGroup.ResetColor;
				}
				this.texture.Apply();
			}

			public void SetTextureAndSize(MaterialPropertyBlock property_block)
			{
				if (this.width <= 0 || !MathUtil.IsPowerOfTwo(this.width))
				{
					DebugUtil.DevAssert(false, string.Format("Width ({0}) must be a power of two in order for fast indexing of the texture to work!", this.width), null);
				}
				Vector2I vector2I = MathUtil.PowerOfTwoToMaskAndShift(this.width);
				property_block.SetTexture(this.texturePropertyId, this.texture);
				property_block.SetVector(this.textureSizePropertyId, new Vector4(this.texelSize.x, this.texelSize.y, (float)vector2I.x, (float)vector2I.y));
			}

			public NativeArray<byte> GetDataPointer()
			{
				return this.texture.GetRawTextureData<byte>();
			}

			public NativeArray<float> GetFloatDataPointer()
			{
				return this.texture.GetRawTextureData<float>();
			}

			public void Apply()
			{
				this.texture.Apply();
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

			public int texturePropertyId;

			public int textureSizePropertyId;

			public int cacheIndex = -1;
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
		Invisible,
		Human,
		NumMaterials
	}
}
