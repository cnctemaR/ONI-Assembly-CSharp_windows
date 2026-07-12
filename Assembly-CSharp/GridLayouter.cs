using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GridLayouter
{
	[Conditional("UNITY_EDITOR")]
	private void ValidateImportantFieldsAreSet()
	{
		global::Debug.Assert(this.minCellSize >= 0f, string.Format("[{0} Error] Minimum cell size is invalid. Given: {1}", "GridLayouter", this.minCellSize));
		global::Debug.Assert(this.maxCellSize >= 0f, string.Format("[{0} Error] Maximum cell size is invalid. Given: {1}", "GridLayouter", this.maxCellSize));
		global::Debug.Assert(this.targetGridLayout != null && this.targetGridLayout, string.Format("[{0} Error] Target grid layout is invalid. Given: {1}", "GridLayouter", this.targetGridLayout));
	}

	public void CheckIfShouldResizeGrid()
	{
		Vector2 vector = new Vector2((float)Screen.width, (float)Screen.height);
		if (vector != this.oldScreenSize)
		{
			this.RequestGridResize();
		}
		this.oldScreenSize = vector;
		float @float = KPlayerPrefs.GetFloat(KCanvasScaler.UIScalePrefKey);
		if (@float != this.oldScreenScale)
		{
			this.RequestGridResize();
		}
		this.oldScreenScale = @float;
		this.ResizeGridIfRequested();
	}

	public void RequestGridResize()
	{
		this.framesLeftToResizeGrid = 3;
	}

	private void ResizeGridIfRequested()
	{
		if (this.framesLeftToResizeGrid > 0)
		{
			this.ImmediateSizeGridToScreenResolution();
			this.framesLeftToResizeGrid--;
		}
	}

	public void ImmediateSizeGridToScreenResolution()
	{
		float num = this.targetGridLayout.transform.parent.rectTransform().rect.size.x - (float)this.targetGridLayout.padding.left - (float)this.targetGridLayout.padding.right;
		float x = this.targetGridLayout.spacing.x;
		int num2 = GridLayouter.<ImmediateSizeGridToScreenResolution>g__GetCellCountToFit|10_1(this.maxCellSize, x, num) + 1;
		float num3;
		for (num3 = GridLayouter.<ImmediateSizeGridToScreenResolution>g__GetCellSize|10_0(num, x, num2); num3 < this.minCellSize; num3 = Mathf.Min(this.maxCellSize, GridLayouter.<ImmediateSizeGridToScreenResolution>g__GetCellSize|10_0(num, x, num2)))
		{
			num2--;
			if (num2 <= 0)
			{
				num2 = 1;
				num3 = this.minCellSize;
				break;
			}
		}
		this.targetGridLayout.childAlignment = ((num2 == 1) ? TextAnchor.UpperCenter : TextAnchor.UpperLeft);
		this.targetGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		this.targetGridLayout.constraintCount = num2;
		this.targetGridLayout.cellSize = Vector2.one * num3;
	}

	[CompilerGenerated]
	internal static float <ImmediateSizeGridToScreenResolution>g__GetCellSize|10_0(float workingWidth, float spacingSize, int count)
	{
		return (workingWidth - (spacingSize * (float)count - 1f)) / (float)count;
	}

	[CompilerGenerated]
	internal static int <ImmediateSizeGridToScreenResolution>g__GetCellCountToFit|10_1(float cellSize, float spacingSize, float workingWidth)
	{
		int num = 0;
		for (float num2 = cellSize; num2 < workingWidth; num2 += cellSize + spacingSize)
		{
			num++;
		}
		return num;
	}

	public float minCellSize = -1f;

	public float maxCellSize = -1f;

	public GridLayoutGroup targetGridLayout;

	private Vector2 oldScreenSize;

	private float oldScreenScale;

	private int framesLeftToResizeGrid;
}
