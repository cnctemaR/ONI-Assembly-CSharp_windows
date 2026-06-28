using System;
using UnityEngine;
using UnityEngine.Rendering;

public class KAnimMeshRenderer : KAnimRenderer
{
	public static KAnimMeshRenderer CreateInstance(Material base_material, Transform parent, global::System.Action becameVisible, global::System.Action becameInvisible)
	{
		GameObject gameObject = new GameObject();
		KAnimMeshRenderer kanimMeshRenderer = gameObject.AddComponent<KAnimMeshRenderer>();
		kanimMeshRenderer.baseMaterial = new Material(base_material);
		kanimMeshRenderer.baseMaterial.name = parent.name;
		KAnimRenderer.InitializeInstance(kanimMeshRenderer, parent, becameVisible, becameInvisible);
		kanimMeshRenderer.meshRenderer = kanimMeshRenderer.Root.gameObject.AddComponent<MeshRenderer>();
		kanimMeshRenderer.meshRenderer.receiveShadows = false;
		kanimMeshRenderer.meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		kanimMeshRenderer.meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		kanimMeshRenderer.MeshFilter = kanimMeshRenderer.Root.gameObject.AddComponent<MeshFilter>();
		kanimMeshRenderer.mesh = new Mesh();
		kanimMeshRenderer.mesh.name = "KAnimMeshRenderer";
		kanimMeshRenderer.mesh.MarkDynamic();
		kanimMeshRenderer.MeshFilter.sharedMesh = kanimMeshRenderer.mesh;
		return kanimMeshRenderer;
	}

	public override void Commit(Bounds bounds, ref Matrix4x4 root_matrix)
	{
		base.Commit(bounds, ref root_matrix);
		this.meshRenderer.material = this.atlasMaterial;
		this.mesh.Clear();
		this.mesh.SetVertices(base.vertices);
		this.mesh.SetUVs(KAnimRenderer.UVS_UV, base.UVs);
		this.mesh.SetUVs(KAnimRenderer.MESH_PARAMS_UV, base.materialMeshParams);
		this.mesh.SetColors(base.colors);
		this.mesh.subMeshCount = 1;
		this.mesh.bounds = bounds;
		this.mesh.SetTriangles(this.triangles, 0);
	}

	public override void AddShaderVector(int property_id, Vector4 value)
	{
		base.AddShaderVector(property_id, value);
		this.meshRenderer.SetPropertyBlock(this.materialPropertyBlock);
	}

	public override void RemoveShaderVector(int property_id)
	{
		base.RemoveShaderVector(property_id);
		this.meshRenderer.SetPropertyBlock(this.materialPropertyBlock);
	}

	private MeshRenderer meshRenderer;

	private MeshFilter MeshFilter;

	private Mesh mesh;
}
