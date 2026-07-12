using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class AnimEventHandlerManager : KMonoBehaviour
{
	public static AnimEventHandlerManager Instance { get; private set; }

	public static void DestroyInstance()
	{
		AnimEventHandlerManager.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		AnimEventHandlerManager.Instance = this;
		this.handlers = new List<AnimEventHandler>();
	}

	public void Add(AnimEventHandler handler)
	{
		this.handlers.Add(handler);
	}

	public void Remove(AnimEventHandler handler)
	{
		this.handlers.Remove(handler);
	}

	private bool IsVisibleToZoom()
	{
		return !(Game.MainCamera == null) && Game.MainCamera.orthographicSize < 40f;
	}

	public void LateUpdate()
	{
		if (!this.IsVisibleToZoom())
		{
			return;
		}
		AnimEventHandlerManager.<>c__DisplayClass11_0 CS$<>8__locals1;
		Grid.GetVisibleCellRangeInActiveWorld(out CS$<>8__locals1.min, out CS$<>8__locals1.max, 4, 1.5f);
		foreach (AnimEventHandler animEventHandler in this.handlers)
		{
			if (AnimEventHandlerManager.<LateUpdate>g__IsVisible|11_0(animEventHandler, ref CS$<>8__locals1))
			{
				animEventHandler.UpdateOffset();
			}
		}
	}

	[CompilerGenerated]
	internal static bool <LateUpdate>g__IsVisible|11_0(AnimEventHandler handler, ref AnimEventHandlerManager.<>c__DisplayClass11_0 A_1)
	{
		int num;
		int num2;
		Grid.CellToXY(handler.GetCachedCell(), out num, out num2);
		return num >= A_1.min.x && num2 >= A_1.min.y && num < A_1.max.x && num2 < A_1.max.y;
	}

	private const float HIDE_DISTANCE = 40f;

	private List<AnimEventHandler> handlers;
}
