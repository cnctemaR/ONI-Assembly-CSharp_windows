using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class ScannerNetworkVisualizerEffect : VisualizerEffect
{
	protected override void SetupMaterial()
	{
		this.material = new Material(Shader.Find("Klei/PostFX/ScannerNetwork"));
	}

	protected override void SetupOcclusionTex()
	{
		this.OcclusionTex = new Texture2D(512, 1, TextureFormat.RGFloat, false);
		this.OcclusionTex.filterMode = FilterMode.Point;
		this.OcclusionTex.wrapMode = TextureWrapMode.Clamp;
	}

	protected override void OnPostRender()
	{
		ScannerNetworkVisualizer scannerNetworkVisualizer = null;
		if (SelectTool.Instance.selected != null)
		{
			scannerNetworkVisualizer = SelectTool.Instance.selected.GetComponent<ScannerNetworkVisualizer>();
		}
		if (scannerNetworkVisualizer == null && BuildTool.Instance.visualizer != null)
		{
			scannerNetworkVisualizer = BuildTool.Instance.visualizer.GetComponent<ScannerNetworkVisualizer>();
		}
		if (scannerNetworkVisualizer != null)
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			ScannerNetworkVisualizerEffect.FindWorldBounds(out vector2I, out vector2I2);
			if (vector2I2.x - vector2I.x > this.OcclusionTex.width)
			{
				return;
			}
			NativeArray<float> pixelData = this.OcclusionTex.GetPixelData<float>(0);
			for (int i = 0; i < this.OcclusionTex.width; i++)
			{
				pixelData[2 * i] = 0f;
				pixelData[2 * i + 1] = 0f;
			}
			int num = 0;
			List<ScannerNetworkVisualizer> items = Components.ScannerVisualizers.GetItems(scannerNetworkVisualizer.GetMyWorldId());
			for (int j = 0; j < items.Count; j++)
			{
				ScannerNetworkVisualizer scannerNetworkVisualizer2 = items[j];
				if (scannerNetworkVisualizer2 != scannerNetworkVisualizer)
				{
					ScannerNetworkVisualizerEffect.ComputeVisibility(scannerNetworkVisualizer2, pixelData, vector2I, vector2I2, ref num);
				}
			}
			ScannerNetworkVisualizerEffect.ComputeVisibility(scannerNetworkVisualizer, pixelData, vector2I, vector2I2, ref num);
			this.OcclusionTex.Apply(false, false);
			Vector2I vector2I3 = vector2I;
			Vector2I vector2I4 = vector2I2;
			if (this.myCamera == null)
			{
				this.myCamera = base.GetComponent<Camera>();
				if (this.myCamera == null)
				{
					return;
				}
			}
			Ray ray = this.myCamera.ViewportPointToRay(Vector3.zero);
			float num2 = Mathf.Abs(ray.origin.z / ray.direction.z);
			Vector3 vector = ray.GetPoint(num2);
			Vector4 vector2;
			vector2.x = vector.x;
			vector2.y = vector.y;
			ray = this.myCamera.ViewportPointToRay(Vector3.one);
			num2 = Mathf.Abs(ray.origin.z / ray.direction.z);
			vector = ray.GetPoint(num2);
			vector2.z = vector.x - vector2.x;
			vector2.w = vector.y - vector2.y;
			this.material.SetVector("_UVOffsetScale", vector2);
			Vector4 vector3;
			vector3.x = (float)vector2I3.x;
			vector3.y = (float)vector2I3.y;
			vector3.z = (float)vector2I4.x;
			vector3.w = (float)vector2I4.y;
			this.material.SetVector("_RangeParams", vector3);
			this.material.SetColor("_HighlightColor", this.highlightColor);
			this.material.SetColor("_HighlightColor2", this.highlightColor2);
			Vector4 vector4;
			vector4.x = 1f / (float)this.OcclusionTex.width;
			vector4.y = 1f / (float)this.OcclusionTex.height;
			vector4.z = 0f;
			vector4.w = 0f;
			this.material.SetVector("_OcclusionParams", vector4);
			this.material.SetTexture("_OcclusionTex", this.OcclusionTex);
			Vector4 vector5;
			vector5.x = (float)Grid.WidthInCells;
			vector5.y = (float)Grid.HeightInCells;
			vector5.z = 1f / (float)Grid.WidthInCells;
			vector5.w = 1f / (float)Grid.HeightInCells;
			this.material.SetVector("_WorldParams", vector5);
			GL.PushMatrix();
			this.material.SetPass(0);
			GL.LoadOrtho();
			GL.Begin(5);
			GL.Color(Color.white);
			GL.Vertex3(0f, 0f, 0f);
			GL.Vertex3(0f, 1f, 0f);
			GL.Vertex3(1f, 0f, 0f);
			GL.Vertex3(1f, 1f, 0f);
			GL.End();
			GL.PopMatrix();
			if (this.LastVisibleColumnCount != num)
			{
				SoundEvent.PlayOneShot(GlobalAssets.GetSound("RangeVisualization_movement", false), scannerNetworkVisualizer.transform.GetPosition(), 1f);
				this.LastVisibleColumnCount = num;
			}
		}
	}

	private static void ComputeVisibility(ScannerNetworkVisualizer scan, NativeArray<float> pixels, Vector2I world_min, Vector2I world_max, ref int visible_column_count)
	{
		Vector2I vector2I = Grid.PosToXY(scan.transform.GetPosition());
		int rangeMin = scan.RangeMin;
		int rangeMax = scan.RangeMax;
		Vector2I vector2I2 = vector2I + scan.OriginOffset;
		bool flag = true;
		for (int i = 0; i >= rangeMin; i--)
		{
			int num = vector2I2.x + i;
			int num2 = vector2I2.y + Mathf.Abs(i);
			ScannerNetworkVisualizerEffect.ComputeVisibility(num, num2, pixels, world_min, world_max, ref flag);
			if (flag)
			{
				visible_column_count++;
			}
		}
		flag = true;
		for (int j = 0; j <= rangeMax; j++)
		{
			int num3 = vector2I2.x + j;
			int num4 = vector2I2.y + Mathf.Abs(j);
			ScannerNetworkVisualizerEffect.ComputeVisibility(num3, num4, pixels, world_min, world_max, ref flag);
			if (flag)
			{
				visible_column_count++;
			}
		}
	}

	private static void ComputeVisibility(int x_abs, int y_abs, NativeArray<float> pixels, Vector2I world_min, Vector2I world_max, ref bool visible)
	{
		int num = x_abs - world_min.x;
		if (x_abs < world_min.x || x_abs > world_max.x || y_abs < world_min.y || y_abs >= world_max.y)
		{
			return;
		}
		int num2 = Grid.XYToCell(x_abs, y_abs);
		visible &= ScannerNetworkVisualizerEffect.HasSkyVisibility(num2);
		if (pixels[2 * num] == 2f)
		{
			if (visible)
			{
				pixels[2 * num + 1] = Mathf.Min(pixels[2 * num + 1], (float)(y_abs + 1));
			}
			return;
		}
		pixels[2 * num] = (float)(visible ? 2 : 1);
		if (pixels[2 * num] == 1f && pixels[2 * num + 1] != 0f)
		{
			pixels[2 * num + 1] = Mathf.Min(pixels[2 * num + 1], (float)(y_abs + 1));
			return;
		}
		pixels[2 * num + 1] = (float)(y_abs + 1);
	}

	private static void FindWorldBounds(out Vector2I world_min, out Vector2I world_max)
	{
		if (ClusterManager.Instance != null)
		{
			WorldContainer activeWorld = ClusterManager.Instance.activeWorld;
			world_min = activeWorld.WorldOffset;
			world_max = activeWorld.WorldOffset + activeWorld.WorldSize;
			return;
		}
		world_min.x = 0;
		world_min.y = 0;
		world_max.x = Grid.WidthInCells;
		world_max.y = Grid.HeightInCells;
	}

	private static bool HasSkyVisibility(int cell)
	{
		return Grid.ExposedToSunlight[cell] >= 1;
	}

	public Color highlightColor = new Color(0f, 1f, 0.8f, 1f);

	public Color highlightColor2 = new Color(1f, 0.32f, 0f, 1f);

	private int LastVisibleColumnCount;
}
