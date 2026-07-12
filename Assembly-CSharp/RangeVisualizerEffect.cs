using System;
using Unity.Collections;
using UnityEngine;

public class RangeVisualizerEffect : MonoBehaviour
{
	private void Start()
	{
		this.material = new Material(Shader.Find("Klei/PostFX/Range"));
	}

	private void OnPostRender()
	{
		RangeVisualizer rangeVisualizer = null;
		Vector2I vector2I = new Vector2I(0, 0);
		if (SelectTool.Instance.selected != null)
		{
			Grid.PosToXY(SelectTool.Instance.selected.transform.GetPosition(), out vector2I.x, out vector2I.y);
			rangeVisualizer = SelectTool.Instance.selected.GetComponent<RangeVisualizer>();
		}
		if (rangeVisualizer == null && BuildTool.Instance.visualizer != null)
		{
			Grid.PosToXY(BuildTool.Instance.visualizer.transform.GetPosition(), out vector2I.x, out vector2I.y);
			rangeVisualizer = BuildTool.Instance.visualizer.GetComponent<RangeVisualizer>();
		}
		if (rangeVisualizer != null)
		{
			if (this.OcclusionTex == null)
			{
				this.OcclusionTex = new Texture2D(64, 64, TextureFormat.Alpha8, false);
				this.OcclusionTex.filterMode = FilterMode.Point;
				this.OcclusionTex.wrapMode = TextureWrapMode.Clamp;
			}
			Vector2I vector2I2;
			Vector2I vector2I3;
			this.FindWorldBounds(out vector2I2, out vector2I3);
			Vector2I rangeMin = rangeVisualizer.RangeMin;
			Vector2I rangeMax = rangeVisualizer.RangeMax;
			Vector2I vector2I4 = rangeVisualizer.OriginOffset;
			Rotatable rotatable;
			if (rangeVisualizer.TryGetComponent<Rotatable>(out rotatable))
			{
				vector2I4 = rotatable.GetRotatedOffset(vector2I4);
				Vector2I rotatedOffset = rotatable.GetRotatedOffset(rangeMin);
				Vector2I rotatedOffset2 = rotatable.GetRotatedOffset(rangeMax);
				rangeMin.x = ((rotatedOffset.x < rotatedOffset2.x) ? rotatedOffset.x : rotatedOffset2.x);
				rangeMin.y = ((rotatedOffset.y < rotatedOffset2.y) ? rotatedOffset.y : rotatedOffset2.y);
				rangeMax.x = ((rotatedOffset.x > rotatedOffset2.x) ? rotatedOffset.x : rotatedOffset2.x);
				rangeMax.y = ((rotatedOffset.y > rotatedOffset2.y) ? rotatedOffset.y : rotatedOffset2.y);
			}
			Vector2I vector2I5 = vector2I + vector2I4;
			int width = this.OcclusionTex.width;
			NativeArray<byte> pixelData = this.OcclusionTex.GetPixelData<byte>(0);
			int num = 0;
			if (rangeVisualizer.TestLineOfSight)
			{
				Func<int, bool> <>9__0;
				for (int m = 0; m <= rangeMax.y - rangeMin.y; m++)
				{
					int num2 = vector2I5.y + rangeMin.y + m;
					for (int j = 0; j <= rangeMax.x - rangeMin.x; j++)
					{
						int num3 = vector2I5.x + rangeMin.x + j;
						Grid.XYToCell(num3, num2);
						bool flag;
						if (num3 > vector2I2.x && num3 < vector2I3.x && num2 > vector2I2.y && (num2 < vector2I3.y || rangeVisualizer.AllowLineOfSightInvalidCells))
						{
							int x = vector2I5.x;
							int y = vector2I5.y;
							int num4 = num3;
							int num5 = num2;
							Func<int, bool> blockingCb = rangeVisualizer.BlockingCb;
							Func<int, bool> func;
							if (rangeVisualizer.BlockingVisibleCb != null)
							{
								func = rangeVisualizer.BlockingVisibleCb;
							}
							else if ((func = <>9__0) == null)
							{
								func = (<>9__0 = (int i) => rangeVisualizer.BlockingTileVisible);
							}
							flag = Grid.TestLineOfSight(x, y, num4, num5, blockingCb, func, rangeVisualizer.AllowLineOfSightInvalidCells);
						}
						else
						{
							flag = false;
						}
						bool flag2 = flag;
						pixelData[m * width + j] = (flag2 ? byte.MaxValue : 0);
						if (flag2)
						{
							num++;
						}
					}
				}
			}
			else
			{
				for (int k = 0; k <= rangeMax.y - rangeMin.y; k++)
				{
					int num6 = vector2I5.y + rangeMin.y + k;
					for (int l = 0; l <= rangeMax.x - rangeMin.x; l++)
					{
						int num7 = vector2I5.x + rangeMin.x + l;
						int num8 = Grid.XYToCell(num7, num6);
						bool flag3 = num7 > vector2I2.x && num7 < vector2I3.x && num6 > vector2I2.y && num6 < vector2I3.y && rangeVisualizer.BlockingCb(num8);
						pixelData[k * width + l] = (flag3 ? 0 : byte.MaxValue);
						if (!flag3)
						{
							num++;
						}
					}
				}
			}
			this.OcclusionTex.Apply(false, false);
			Vector2I vector2I6 = rangeMin + vector2I5;
			Vector2I vector2I7 = rangeMax + vector2I5;
			if (this.myCamera == null)
			{
				this.myCamera = base.GetComponent<Camera>();
				if (this.myCamera == null)
				{
					return;
				}
			}
			Ray ray = this.myCamera.ViewportPointToRay(Vector3.zero);
			float num9 = Mathf.Abs(ray.origin.z / ray.direction.z);
			Vector3 vector = ray.GetPoint(num9);
			Vector4 vector2;
			vector2.x = vector.x;
			vector2.y = vector.y;
			ray = this.myCamera.ViewportPointToRay(Vector3.one);
			num9 = Mathf.Abs(ray.origin.z / ray.direction.z);
			vector = ray.GetPoint(num9);
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
			if (this.LastVisibleTileCount != num)
			{
				SoundEvent.PlayOneShot(GlobalAssets.GetSound("RangeVisualization_movement", false), rangeVisualizer.transform.GetPosition(), 1f);
				this.LastVisibleTileCount = num;
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

	private Texture2D OcclusionTex;

	private int LastVisibleTileCount;
}
