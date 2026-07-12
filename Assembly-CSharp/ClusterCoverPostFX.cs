using System;
using UnityEngine;

public class ClusterCoverPostFX : MonoBehaviour
{
	private void Awake()
	{
		if (this.shader != null)
		{
			this.material = new Material(this.shader);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.SetupUVs();
		Graphics.Blit(source, destination, this.material, 0);
	}

	private void SetupUVs()
	{
		if (this.myCamera == null)
		{
			this.myCamera = base.GetComponent<Camera>();
			if (this.myCamera == null)
			{
				return;
			}
		}
		Ray ray = this.myCamera.ViewportPointToRay(Vector3.zero);
		float num = Mathf.Abs(ray.origin.z / ray.direction.z);
		Vector3 vector = ray.GetPoint(num);
		Vector4 vector2;
		vector2.x = vector.x / Grid.WidthInMeters;
		vector2.y = vector.y / Grid.HeightInMeters;
		ray = this.myCamera.ViewportPointToRay(Vector3.one);
		num = Mathf.Abs(ray.origin.z / ray.direction.z);
		vector = ray.GetPoint(num);
		vector2.z = vector.x / Grid.WidthInMeters - vector2.x;
		vector2.w = vector.y / Grid.HeightInMeters - vector2.y;
		this.material.SetVector("_UVOffsetScale", vector2);
		Vector4 vector3;
		if (ClusterManager.Instance != null && !CameraController.Instance.ignoreClusterFX)
		{
			WorldContainer activeWorld = ClusterManager.Instance.activeWorld;
			Vector2I worldOffset = activeWorld.WorldOffset;
			Vector2I worldSize = activeWorld.WorldSize;
			vector3 = new Vector4((float)worldOffset.x / (float)Grid.WidthInCells, (float)worldOffset.y / (float)Grid.HeightInCells, (float)(worldSize.x + worldOffset.x) / (float)Grid.WidthInCells, (float)(worldSize.y + worldOffset.y) / (float)Grid.HeightInCells);
			this.material.SetFloat("_HideSurface", ClusterManager.Instance.activeWorld.FullyEnclosedBorder ? 1f : 0f);
		}
		else
		{
			vector3 = new Vector4(0f, 0f, (float)Grid.WidthInCells, (float)Grid.HeightInCells);
			this.material.SetFloat("_HideSurface", 0f);
		}
		this.material.SetVector("_CameraWorldInfo", vector3);
	}

	[SerializeField]
	private Shader shader;

	private Material material;

	private Camera myCamera;
}
