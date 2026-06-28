using System;
using System.Collections.Generic;
using UnityEngine;

public class KAnimBatchGroup
{
	public KAnimBatchGroup(HashedString id, KAnimBatchGroup.MaterialType materialType)
	{
		this.buildByteToFloat.bytes = null;
		this.data = KAnimBatchManager.Instance().GetBatchGroupData(id);
		this.batchID = id;
		this.ResetMaterial(materialType);
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

	public Material material { get; private set; }

	public int layer { get; private set; }

	public void ResetMaterial()
	{
		this.ResetMaterial(this.materialType);
	}

	public void ResetMaterial(KAnimBatchGroup.MaterialType matType)
	{
		if (this.material != null)
		{
			global::UnityEngine.Object.Destroy(this.material);
			this.material = null;
		}
		this.materialType = matType;
		switch (this.materialType)
		{
		case KAnimBatchGroup.MaterialType.Simple:
			this.material = new Material(Shader.Find("Klei/AnimationSimple"));
			goto IL_00B8;
		case KAnimBatchGroup.MaterialType.UI:
			this.material = new Material(Shader.Find("Klei/BatchedAnimationUI"));
			goto IL_00B8;
		case KAnimBatchGroup.MaterialType.Overlay:
			this.material = new Material(Shader.Find("Klei/AnimationOverlay"));
			goto IL_00B8;
		}
		this.material = new Material(Shader.Find("Klei/BatchedAnimation"));
		IL_00B8:
		this.material.name = "Material:" + this.batchID.ToString();
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

	public Texture2D animDataTex { get; private set; }

	public bool InitOK
	{
		get
		{
			return this.texureSize > 0 || this.dataType == KAnimBatchGroup.DataType.AnimOnly;
		}
	}

	public void Finalise()
	{
		if (this.material != null)
		{
			global::UnityEngine.Object.Destroy(this.material);
			this.material = null;
		}
		if (this.mesh != null)
		{
			global::UnityEngine.Object.Destroy(this.mesh);
			this.mesh = null;
		}
		if (this.animDataTex != null)
		{
			global::UnityEngine.Object.Destroy(this.animDataTex);
			this.animDataTex = null;
		}
		if (this.instances != null)
		{
			foreach (KeyValuePair<int, BatchGroupInstance> keyValuePair in this.instances)
			{
				keyValuePair.Value.DestroyTex();
			}
			this.instances.Clear();
		}
	}

	private int GetBestTextureSize(float cost)
	{
		float num = Mathf.Sqrt(cost);
		return Mathf.CeilToInt(num);
	}

	private void SetupMeshData(int layer)
	{
		Debug.Assert(this.maxGroupSize > 0, "Group size must be >0");
		this.layer = layer;
		this.maxGroupSize = Mathf.Min(this.maxGroupSize, 60);
		this.mesh = this.BuildMesh(this.maxGroupSize * this.data.maxVisibleSymbols);
		float num = (float)(this.maxGroupSize * 48) / 4f;
		this.texureSize = this.GetBestTextureSize(num);
	}

	public BatchGroupInstance GetBatchGroupInstance(KAnimBatch obj)
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
		this.animDataTex = new Texture2D(bestTextureSize, bestTextureSize, TextureFormat.RGBAFloat, false);
		this.animDataTex.wrapMode = TextureWrapMode.Clamp;
		this.animDataTex.filterMode = FilterMode.Point;
		this.animDataTex.anisoLevel = 0;
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

	public void Rebuild(BatchGroupInstance instance, MaterialPropertyBlock matProperties)
	{
		this.InitBuild(instance);
		matProperties.SetTexture("buildTex", instance.buildTex);
	}

	public void InitBuild(BatchGroupInstance instance)
	{
		if (this.dataType == KAnimBatchGroup.DataType.DontRender)
		{
			return;
		}
		List<KAnim.Build.SymbolFrameInstance> buildSymbolFrameInstances = this.data.GetBuildSymbolFrameInstances();
		int num = buildSymbolFrameInstances.Count * 32;
		float num2 = (float)num / 4f;
		int bestTextureSize = this.GetBestTextureSize(num2);
		if (instance.buildTex == null || bestTextureSize != instance.buildTex.width)
		{
			instance.buildTex = new Texture2D(bestTextureSize, bestTextureSize, TextureFormat.RGBAFloat, false);
			instance.buildTex.wrapMode = TextureWrapMode.Clamp;
			instance.buildTex.filterMode = FilterMode.Point;
			instance.buildTex.anisoLevel = 0;
			instance.buildTex.name = "BuildData:" + this.batchID.ToString();
			this.buildByteToFloat.bytes = null;
		}
		Debug.AssertFormat(num2 <= (float)(instance.buildTex.width * instance.buildTex.height), "Build texture is the wrong size! {0} <= {1}", new object[]
		{
			num2,
			instance.buildTex.width * instance.buildTex.height
		});
		int num3 = instance.buildTex.width * instance.buildTex.height * 4 * 4;
		Debug.AssertFormat(num3 > 0, "Init build failure for [{0}]", new object[] { this.batchID });
		if (this.buildByteToFloat.bytes == null)
		{
			this.buildByteToFloat = new KAnimConverter.ByteToFloatConverter
			{
				bytes = new byte[num3]
			};
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

	public Texture2D CreateTexture()
	{
		if (this.dataType == KAnimBatchGroup.DataType.DontRender)
		{
			return null;
		}
		Debug.AssertFormat(this.texureSize > 0, "Need to init AnimBatchGroup [{0}] first!", new object[] { this.batchID });
		return new Texture2D(this.texureSize, this.texureSize, TextureFormat.RGBAFloat, false)
		{
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point,
			anisoLevel = 0,
			name = "InstanceData:" + this.batchID.ToString()
		};
	}

	public void GetDataTextures(BatchGroupInstance instance, MaterialPropertyBlock matProperties)
	{
		Debug.AssertFormat(instance != null, "Got null Group Instace from AnimBatchGroup [{0}]", new object[] { this.batchID });
		matProperties.SetFloat("MAX_VISIBLE_SYMBOLS", (float)this.data.maxVisibleSymbols);
		if (this.animDataTex != null)
		{
			matProperties.SetTexture("animTex", this.animDataTex);
			matProperties.SetVector("ANIM_TEXEL_SIZE", this.animDataTex.texelSize);
			matProperties.SetVector("ANIM_TEXTURE_SIZE", new Vector2((float)this.animDataTex.width, (float)this.animDataTex.height));
		}
		if (instance.buildTex != null)
		{
			matProperties.SetTexture("buildTex", instance.buildTex);
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
				matProperties.SetTexture("atlas" + this.data.textures.Count + j, texture2D2);
			}
		}
	}

	public KAnimBatchGroup.MaterialType materialType;

	public int batchCount;

	private int texureSize;

	private Dictionary<int, BatchGroupInstance> instances;

	private bool _isMultiInstance;

	private KAnimConverter.ByteToFloatConverter buildByteToFloat;

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
		Overlay
	}

	public enum DataType
	{
		Default,
		UI,
		DontRender,
		AnimOnly
	}
}
