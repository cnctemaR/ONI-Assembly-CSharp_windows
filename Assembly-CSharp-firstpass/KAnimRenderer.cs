using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KAnimRenderer : MonoBehaviour
{
	private protected List<Vector3> vertices { protected get; private set; }

	private protected List<Vector2> UVs { protected get; private set; }

	private protected List<Vector4> materialMeshParams { protected get; private set; }

	private protected List<Vector4> temperatureParams { protected get; private set; }

	private protected List<Color32> colors { protected get; private set; }

	private void Awake()
	{
		this.Clear();
	}

	public void SetOffset(Vector3 offset)
	{
		this.Offset = offset;
	}

	protected static void InitializeInstance(KAnimRenderer cmp, Transform parent, global::System.Action becameVisible, global::System.Action becameInvisible)
	{
		GameObject gameObject = cmp.gameObject;
		gameObject.transform.name = "KAnimRendererRoot";
		gameObject.transform.parent = parent;
		gameObject.layer = parent.gameObject.layer;
		cmp.Root = gameObject.transform;
		cmp.Root.transform.localPosition = Vector3.zero;
		cmp.Root.transform.rotation = Quaternion.identity;
		cmp.Root.transform.localScale = Vector3.one;
		cmp.RootMatrixId = Shader.PropertyToID("_RootMatrix");
		cmp.becameVisible = becameVisible;
		cmp.becameInvisible = becameInvisible;
		if (cmp.baseMaterial != null)
		{
			cmp.atlasMaterial = new Material(cmp.baseMaterial);
		}
	}

	public void Reserve(int max_vertices, int max_indices)
	{
		this.vertices = new List<Vector3>(max_vertices);
		this.UVs = new List<Vector2>(max_vertices);
		this.materialMeshParams = new List<Vector4>(max_vertices);
		this.temperatureParams = new List<Vector4>(max_vertices);
		this.colors = new List<Color32>(max_vertices);
	}

	public void Break()
	{
		this.IncrementDrawIdx();
	}

	protected void IncrementDrawIdx()
	{
		this.drawIdx++;
		this.ReserveDrawBuffers();
	}

	protected void ReserveDrawBuffers()
	{
		int num = this.GetAtlasCount();
		if (this.drawIdx >= num)
		{
			num++;
			Texture[] array = new Texture[num];
			bool[] array2 = new bool[num];
			for (int i = 0; i < num; i++)
			{
				if (i != num - 1)
				{
					array[i] = this.atlases[i];
					array2[i] = this.ActiveDraw[i];
				}
			}
			this.atlases = array;
			this.ActiveDraw = array2;
		}
	}

	private int GetAtlasCount()
	{
		return this.atlases.Length;
	}

	private int GetAtlasIndex(Texture atlas)
	{
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < this.atlases.Length; i++)
		{
			if (num2 == -1 && this.atlases[i] == null)
			{
				num2 = i;
			}
			else if (atlas == this.atlases[i])
			{
				num = i;
				break;
			}
		}
		if (num == -1 && num2 != -1)
		{
			num = num2;
			if (this.atlasMaterial == null)
			{
				this.atlasMaterial = new Material(this.baseMaterial);
			}
			this.atlases[num] = atlas;
			if (num == 0)
			{
				this.atlasMaterial.mainTexture = atlas;
			}
			else
			{
				this.atlasMaterial.SetTexture("_Tex" + num, atlas);
			}
		}
		return num;
	}

	public virtual int SetAtlas(Texture atlas)
	{
		int num;
		if (this.drawIdx == -1)
		{
			this.IncrementDrawIdx();
			num = this.GetAtlasIndex(atlas);
		}
		else
		{
			num = this.GetAtlasIndex(atlas);
			if (num == -1)
			{
				this.ReserveDrawBuffers();
				num = this.GetAtlasIndex(atlas);
			}
			if (atlas != this.atlases[this.drawIdx] && this.ActiveDraw[this.drawIdx])
			{
				this.IncrementDrawIdx();
			}
		}
		this.ActiveDraw[this.drawIdx] = true;
		return num;
	}

	public bool IsEmpty()
	{
		return this.vertices.Count == 0;
	}

	public void Clear()
	{
		this.drawIdx = -1;
		this.quadIdx = 0;
		this.Offset = Vector3.zero;
		int atlasCount = this.GetAtlasCount();
		for (int i = 0; i < atlasCount; i++)
		{
			this.ActiveDraw[i] = false;
		}
		this.triangles.Clear();
		if (this.vertices != null)
		{
			this.vertices.Clear();
		}
		if (this.colors != null)
		{
			this.colors.Clear();
		}
		if (this.UVs != null)
		{
			this.UVs.Clear();
		}
		if (this.materialMeshParams != null)
		{
			this.materialMeshParams.Clear();
		}
		if (this.temperatureParams != null)
		{
			this.temperatureParams.Clear();
		}
		this.OnMeshChanged();
	}

	public void AddVertex(Vector3 position, Vector2 uv, Vector2 mesh_param, Vector2 material_param, Color32 color, Color32 temperature)
	{
		this.vertices.Add(position + this.Offset);
		this.colors.Add(color);
		this.UVs.Add(uv);
		this.materialMeshParams.Add(new Vector4(mesh_param.x, (float)this.UVs.Count, material_param.x, material_param.y));
		this.temperatureParams.Add(new Vector4((float)temperature.r, (float)temperature.g, (float)temperature.b, (float)temperature.a));
		this.OnMeshChanged();
	}

	protected virtual void OnMeshChanged()
	{
	}

	public void AddQuad()
	{
		this.triangles.Add(this.quadIdx);
		this.triangles.Add(this.quadIdx + 1);
		this.triangles.Add(this.quadIdx + 2);
		this.triangles.Add(this.quadIdx + 1);
		this.triangles.Add(this.quadIdx + 3);
		this.triangles.Add(this.quadIdx + 2);
		this.quadIdx += 4;
	}

	public virtual void Commit(Bounds bounds, ref Matrix4x4 root_matrix)
	{
		if (this.atlasMaterial == null)
		{
			this.atlasMaterial = new Material(this.baseMaterial);
		}
		this.atlasMaterial.SetMatrix(this.RootMatrixId, root_matrix);
	}

	public virtual void AddShaderVector(int property_id, Vector4 value)
	{
		if (this.shaderVectors == null)
		{
			this.shaderVectors = new Dictionary<int, Vector4>();
		}
		bool flag = false;
		Vector4 vector;
		if (this.shaderVectors.TryGetValue(property_id, out vector))
		{
			if (value != vector)
			{
				flag = true;
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			this.shaderVectors[property_id] = value;
			this.BuildPropertyBlock();
		}
	}

	public virtual void RemoveShaderVector(int property_id)
	{
		if (this.shaderVectors == null)
		{
			return;
		}
		if (this.shaderVectors.ContainsKey(property_id))
		{
			this.shaderVectors.Remove(property_id);
			this.BuildPropertyBlock();
		}
	}

	private void BuildPropertyBlock()
	{
		if (base.gameObject.GetInstanceID() == KAnimRenderer.DEBUG_INSTANCE_ID)
		{
			foreach (KeyValuePair<int, Vector4> keyValuePair in this.shaderVectors)
			{
				Output.Log(new object[]
				{
					KAnimRenderer.DEBUG_INSTANCE_ID,
					keyValuePair.Key,
					keyValuePair.Value
				});
			}
		}
		if (this.shaderVectors.Count == 0)
		{
			this.materialPropertyBlock = null;
		}
		else
		{
			if (this.materialPropertyBlock == null)
			{
				this.materialPropertyBlock = new MaterialPropertyBlock();
			}
			else
			{
				this.materialPropertyBlock.Clear();
			}
			foreach (KeyValuePair<int, Vector4> keyValuePair2 in this.shaderVectors)
			{
				this.materialPropertyBlock.SetVector(keyValuePair2.Key, keyValuePair2.Value);
			}
		}
	}

	private void OnBecameInvisible()
	{
		if (this.becameInvisible != null && base.gameObject != null)
		{
			this.becameInvisible();
		}
	}

	private void OnBecameVisible()
	{
		if (this.becameVisible != null && base.gameObject != null)
		{
			this.becameVisible();
		}
	}

	public void WriteFrameCapture(string name, StreamWriter file)
	{
		if (!base.enabled)
		{
			return;
		}
		int num = this.drawIdx;
		if (this.atlases != null)
		{
			num = this.atlases.Length;
		}
		int num2 = 0;
		if (this.vertices != null)
		{
			num2 = this.vertices.Count / 4;
		}
		if (num > 0)
		{
			file.WriteLine(string.Concat(new object[] { "\"", name, "\",", num, ",", num2 }));
		}
	}

	protected Transform Root;

	protected Material baseMaterial;

	protected Material atlasMaterial;

	protected List<int> triangles = new List<int>();

	protected Texture[] atlases = new Texture[0];

	protected bool[] ActiveDraw = new bool[0];

	public MaterialPropertyBlock materialPropertyBlock;

	private int quadIdx;

	private Vector3 Offset;

	public static int UVS_UV;

	public static int MESH_PARAMS_UV = 1;

	protected int drawIdx = -1;

	private global::System.Action becameVisible;

	private global::System.Action becameInvisible;

	private int RootMatrixId;

	private Dictionary<int, Vector4> shaderVectors;

	public static int DEBUG_INSTANCE_ID = -1;
}
