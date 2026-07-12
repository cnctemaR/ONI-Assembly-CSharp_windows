using System;
using UnityEngine;

public class DevToolWarmthZonesVisualizer : DevTool
{
	private void SetupColors()
	{
		if (this.colors == null)
		{
			this.colors = new Color[3];
			for (int i = 1; i <= 3; i++)
			{
				this.colors[i - 1] = this.CreateColorForWarmthValue(i);
			}
		}
	}

	private Color CreateColorForWarmthValue(int warmValue)
	{
		float num = (float)Mathf.Clamp(warmValue, 1, 3) / 3f;
		Color color = this.WARM_CELL_COLOR * num;
		color.a = this.WARM_CELL_COLOR.a;
		return color;
	}

	private Color GetBorderColor(int warmValue)
	{
		int num = Mathf.Clamp(warmValue, 0, 3);
		return this.colors[num];
	}

	private Color GetFillColor(int warmValue)
	{
		Color borderColor = this.GetBorderColor(warmValue);
		borderColor.a = 0.3f;
		return borderColor;
	}

	protected override void RenderTo(DevPanel panel)
	{
		this.SetupColors();
		foreach (int num in WarmthProvider.WarmCells.Keys)
		{
			if (Grid.IsValidCell(num) && WarmthProvider.IsWarmCell(num))
			{
				int warmthValue = WarmthProvider.GetWarmthValue(num);
				Option<ValueTuple<Vector2, Vector2>> screenRect = new DevToolEntityTarget.ForSimCell(num).GetScreenRect();
				string text = warmthValue.ToString();
				DevToolEntity.DrawScreenRect(screenRect.Unwrap(), text, this.GetBorderColor(warmthValue - 1), this.GetFillColor(warmthValue - 1), new Option<DevToolUtil.TextAlignment>(DevToolUtil.TextAlignment.Center));
			}
		}
	}

	private const int MAX_COLOR_VARIANTS = 3;

	private Color WARM_CELL_COLOR = Color.red;

	private Color[] colors;
}
