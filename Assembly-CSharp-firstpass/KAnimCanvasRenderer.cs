using System;
using System.Collections.Generic;
using UnityEngine;

public class KAnimCanvasRenderer : KAnimRenderer
{
	public static KAnimCanvasRenderer CreateInstance(Material base_material, Transform parent, global::System.Action becameVisible, global::System.Action becameInvisible)
	{
		GameObject gameObject = new GameObject();
		KAnimCanvasRenderer kanimCanvasRenderer = gameObject.AddComponent<KAnimCanvasRenderer>();
		KAnimCanvasRenderer.Configure(kanimCanvasRenderer, base_material, parent, becameVisible, becameInvisible);
		return kanimCanvasRenderer;
	}

	protected static void Configure(KAnimCanvasRenderer cmp, Material base_material, Transform parent, global::System.Action becameVisible, global::System.Action becameInvisible)
	{
		cmp.baseMaterial = new Material(base_material);
		cmp.baseMaterial.name = parent.name;
		KAnimRenderer.InitializeInstance(cmp, parent, becameVisible, becameInvisible);
		CanvasRenderer canvasRenderer = parent.gameObject.GetComponent<CanvasRenderer>();
		if (canvasRenderer == null)
		{
			canvasRenderer = parent.gameObject.AddComponent<CanvasRenderer>();
		}
		KBatchedAnimCanvasRenderer kbatchedAnimCanvasRenderer = parent.gameObject.GetComponent<KBatchedAnimCanvasRenderer>();
		if (kbatchedAnimCanvasRenderer == null)
		{
			kbatchedAnimCanvasRenderer = parent.gameObject.AddComponent<KBatchedAnimCanvasRenderer>();
		}
		RectTransform rectTransform = parent.gameObject.GetComponent<RectTransform>();
		if (rectTransform == null)
		{
			rectTransform = parent.gameObject.AddComponent<RectTransform>();
		}
		cmp.rootRectTransform = rectTransform;
		cmp.SetStretch(cmp.rootRectTransform);
		GameObject gameObject = new GameObject();
		gameObject.name = "KAnimCanvasRenderer";
		cmp.canvases = gameObject.AddComponent<CanvasRenderer>();
		cmp.rectTransforms = gameObject.AddComponent<RectTransform>();
		cmp.rectTransforms.SetParent(cmp.rootRectTransform);
		cmp.rectTransforms.SetLocalPosition(Vector3.zero);
		cmp.rectTransforms.localScale = Vector3.one;
		cmp.SetStretch(cmp.rectTransforms);
		cmp.canvases.Clear();
		cmp.canvases.materialCount = 1;
		cmp.canvases.SetMaterial(cmp.atlasMaterial, 0);
		cmp.atlasMaterial.SetFloat("_IsUI", 1f);
		cmp.mesh.name = cmp.transform.parent.name;
	}

	private void Awake()
	{
		if (this.baseMaterial == null)
		{
			KAnimCanvasRenderer.Configure(this, this.defaultMat, base.transform.parent, null, null);
		}
	}

	private void SetStretch(RectTransform rect)
	{
		rect.anchorMin = new Vector2(0f, 0f);
		rect.anchorMax = new Vector2(1f, 1f);
		rect.offsetMin = new Vector2(0f, 0f);
		rect.offsetMax = new Vector2(0f, 0f);
	}

	public Vector3 GetUIVertex(int idx, float width, float height, ref Matrix4x4 root_matrix)
	{
		Vector3 vector = root_matrix * base.vertices[idx];
		Vector3 vector2 = new Vector3(vector.x * width, vector.y * height - 0.5f * height, vector.z);
		return vector2;
	}

	public List<Vector3> GetUIVertices(float width, float height, ref Matrix4x4 root_matrix)
	{
		int count = this.triangles.Count;
		int num = 4 * count / 6;
		List<Vector3> list = new List<Vector3>(num);
		for (int i = 0; i < num / 4; i++)
		{
			list.Add(this.GetUIVertex(i * 4, width, height, ref root_matrix));
			list.Add(this.GetUIVertex(i * 4 + 1, width, height, ref root_matrix));
			list.Add(this.GetUIVertex(i * 4 + 2, width, height, ref root_matrix));
			list.Add(this.GetUIVertex(i * 4 + 3, width, height, ref root_matrix));
		}
		return list;
	}

	public override void Commit(Bounds bounds, ref Matrix4x4 root_matrix)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		base.Commit(bounds, ref identity);
		List<Vector3> uivertices = this.GetUIVertices(this.rootRectTransform.rect.width, this.rootRectTransform.rect.height, ref root_matrix);
		this.mesh.name = base.name;
		this.mesh.Clear();
		this.mesh.subMeshCount = 1;
		this.mesh.bounds = bounds;
		this.mesh.SetVertices(uivertices);
		this.mesh.SetUVs(KAnimRenderer.UVS_UV, base.UVs);
		this.mesh.SetUVs(KAnimRenderer.MESH_PARAMS_UV, base.materialMeshParams);
		this.mesh.SetColors(base.colors);
		this.mesh.SetTriangles(this.triangles, 0);
		this.canvases.SetMesh(this.mesh);
	}

	public void SetMaterial(Material newMat)
	{
		if (this.baseMaterial != newMat)
		{
			this.baseMaterial = newMat;
		}
	}

	public bool CheckMaterialEqual(Material toMaterial)
	{
		return this.baseMaterial == toMaterial;
	}

	private CanvasRenderer canvases = new CanvasRenderer();

	private RectTransform rectTransforms = new RectTransform();

	private Mesh mesh = new Mesh();

	private RectTransform rootRectTransform;

	public Material defaultMat;
}
