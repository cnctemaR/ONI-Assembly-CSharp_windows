using System;
using UnityEngine;

public class RangeVisualizerEffect : MonoBehaviour
{
	private void Start()
	{
		this.material = new Material(Shader.Find("Klei/PostFX/Range"));
		RangeVisualizerEffect.Instance = this;
	}

	public void UpdateEnabled()
	{
		bool flag = false;
		if (SelectTool.Instance.selected != null)
		{
			flag = SelectTool.Instance.selected.GetComponent<RangeVisualizer>() != null;
		}
		if (!flag && BuildTool.Instance.visualizer != null)
		{
			flag = BuildTool.Instance.visualizer.GetComponent<RangeVisualizer>() != null;
		}
		base.enabled = flag;
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
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
			Vector2I vector2I2 = rangeVisualizer.RangeMin;
			Vector2I vector2I3 = rangeVisualizer.RangeMax;
			vector2I2 += vector2I;
			vector2I3 += vector2I;
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
			vector2.x = vector.x;
			vector2.y = vector.y;
			ray = this.myCamera.ViewportPointToRay(Vector3.one);
			num = Mathf.Abs(ray.origin.z / ray.direction.z);
			vector = ray.GetPoint(num);
			vector2.z = vector.x - vector2.x;
			vector2.w = vector.y - vector2.y;
			this.material.SetVector("_UVOffsetScale", vector2);
			Vector4 vector3;
			vector3.x = (float)vector2I2.x;
			vector3.y = (float)vector2I2.y;
			vector3.z = (float)(vector2I3.x + 1);
			vector3.w = (float)(vector2I3.y + 1);
			this.material.SetVector("_RangeParams", vector3);
			this.material.SetColor("_HighlightColor", this.highlightColor);
			Graphics.Blit(src, dest, this.material);
		}
	}

	private Material material;

	private Camera myCamera;

	public Color highlightColor;

	public static RangeVisualizerEffect Instance;
}
