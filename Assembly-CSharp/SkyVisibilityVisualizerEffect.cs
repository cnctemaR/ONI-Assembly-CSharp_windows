using System;
using Unity.Collections;
using UnityEngine;

public class SkyVisibilityVisualizerEffect : MonoBehaviour
{
	private void Start()
	{
		this.material = new Material(Shader.Find("Klei/PostFX/SkyVisibility"));
	}

	private void OnPostRender()
	{
		SkyVisibilityVisualizer skyVisibilityVisualizer = null;
		Vector2I vector2I = new Vector2I(0, 0);
		if (SelectTool.Instance.selected != null)
		{
			Grid.PosToXY(SelectTool.Instance.selected.transform.GetPosition(), out vector2I.x, out vector2I.y);
			skyVisibilityVisualizer = SelectTool.Instance.selected.GetComponent<SkyVisibilityVisualizer>();
		}
		if (skyVisibilityVisualizer == null && BuildTool.Instance.visualizer != null)
		{
			Grid.PosToXY(BuildTool.Instance.visualizer.transform.GetPosition(), out vector2I.x, out vector2I.y);
			skyVisibilityVisualizer = BuildTool.Instance.visualizer.GetComponent<SkyVisibilityVisualizer>();
		}
		if (skyVisibilityVisualizer != null)
		{
			if (skyVisibilityVisualizer.SkipOnModuleInteriors && ClusterManager.Instance != null)
			{
				WorldContainer myWorld = skyVisibilityVisualizer.GetMyWorld();
				if (myWorld != null && myWorld.IsModuleInterior)
				{
					return;
				}
			}
			if (this.OcclusionTex == null)
			{
				this.OcclusionTex = new Texture2D(64, 1, TextureFormat.RGFloat, false);
				this.OcclusionTex.filterMode = FilterMode.Point;
				this.OcclusionTex.wrapMode = TextureWrapMode.Clamp;
			}
			Vector2I vector2I2;
			Vector2I vector2I3;
			this.FindWorldBounds(out vector2I2, out vector2I3);
			int rangeMin = skyVisibilityVisualizer.RangeMin;
			int rangeMax = skyVisibilityVisualizer.RangeMax;
			Vector2I originOffset = skyVisibilityVisualizer.OriginOffset;
			Vector2I vector2I4 = vector2I + originOffset;
			NativeArray<float> pixelData = this.OcclusionTex.GetPixelData<float>(0);
			int num = 0;
			bool flag = true;
			int num2 = vector2I4.x + rangeMin;
			int num3 = vector2I4.x + rangeMax;
			bool flag2 = true;
			for (int i = vector2I4.x; i >= num2; i--)
			{
				int num4 = vector2I4.y + (vector2I4.x - i) * skyVisibilityVisualizer.ScanVerticalStep;
				int num5 = Grid.XYToCell(i, num4);
				flag2 &= i > vector2I2.x && i < vector2I3.x && num4 > vector2I2.y && num4 < vector2I3.y && skyVisibilityVisualizer.SkyVisibilityCb(num5);
				int num6 = i - num2;
				if (!skyVisibilityVisualizer.AllOrNothingVisibility)
				{
					pixelData[2 * num6] = (float)(flag2 ? 1 : 0);
				}
				pixelData[2 * num6 + 1] = (float)(num4 + 1);
				if (flag2)
				{
					num++;
				}
			}
			flag = flag && flag2;
			Vector2I vector2I5 = vector2I4;
			if (skyVisibilityVisualizer.TwoWideOrgin)
			{
				vector2I5.x++;
			}
			flag2 = true;
			for (int j = vector2I5.x; j <= num3; j++)
			{
				int num7 = vector2I5.y + (j - vector2I5.x) * skyVisibilityVisualizer.ScanVerticalStep;
				int num8 = Grid.XYToCell(j, num7);
				flag2 &= j > vector2I2.x && j < vector2I3.x && num7 > vector2I2.y && num7 < vector2I3.y && skyVisibilityVisualizer.SkyVisibilityCb(num8);
				int num9 = j - num2;
				if (!skyVisibilityVisualizer.AllOrNothingVisibility)
				{
					pixelData[2 * num9] = (float)(flag2 ? 1 : 0);
				}
				pixelData[2 * num9 + 1] = (float)(num7 + 1);
				if (flag2)
				{
					num++;
				}
			}
			flag = flag && flag2;
			if (skyVisibilityVisualizer.AllOrNothingVisibility)
			{
				for (int k = 0; k <= rangeMax - rangeMin; k++)
				{
					pixelData[2 * k] = (float)(flag ? 1 : 0);
				}
			}
			this.OcclusionTex.Apply(false, false);
			Vector2I vector2I6 = vector2I4 + new Vector2I(rangeMin, 0);
			Vector2I vector2I7 = new Vector2I(vector2I4.x + rangeMax, vector2I3.y);
			if (this.myCamera == null)
			{
				this.myCamera = base.GetComponent<Camera>();
				if (this.myCamera == null)
				{
					return;
				}
			}
			Ray ray = this.myCamera.ViewportPointToRay(Vector3.zero);
			float num10 = Mathf.Abs(ray.origin.z / ray.direction.z);
			Vector3 vector = ray.GetPoint(num10);
			Vector4 vector2;
			vector2.x = vector.x;
			vector2.y = vector.y;
			ray = this.myCamera.ViewportPointToRay(Vector3.one);
			num10 = Mathf.Abs(ray.origin.z / ray.direction.z);
			vector = ray.GetPoint(num10);
			vector2.z = vector.x - vector2.x;
			vector2.w = vector.y - vector2.y;
			this.material.SetVector("_UVOffsetScale", vector2);
			Vector4 vector3;
			vector3.x = (float)vector2I6.x;
			vector3.y = (float)vector2I6.y;
			vector3.z = (float)(vector2I7.x + 1);
			vector3.w = (float)(vector2I7.y + 1);
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
				SoundEvent.PlayOneShot(GlobalAssets.GetSound("RangeVisualization_movement", false), skyVisibilityVisualizer.transform.GetPosition(), 1f);
				this.LastVisibleColumnCount = num;
			}
		}
	}

	private void FindWorldBounds(out Vector2I world_min, out Vector2I world_max)
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

	private Material material;

	private Camera myCamera;

	public Color highlightColor = new Color(0f, 1f, 0.8f, 1f);

	public Color highlightColor2 = new Color(1f, 0.32f, 0f, 1f);

	private Texture2D OcclusionTex;

	private int LastVisibleColumnCount;
}
