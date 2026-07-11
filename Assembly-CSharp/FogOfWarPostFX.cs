using System;
using UnityEngine;

public class FogOfWarPostFX : MonoBehaviour
{
	private void Awake()
	{
		base.enabled = SystemInfo.supportsImageEffects;
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
	}

	[SerializeField]
	private Shader shader;

	private Material material;

	private Camera myCamera;
}
