using System;
using UnityEngine;

public interface ICellSelectionProxy
{
	Element Element { get; }

	void OnObjectSelected(object o);

	public static bool IsSelectionProxy(GameObject go)
	{
		return CellSelectionObject.IsSelectionObject(go) || BackwallSelectionObject.IsBackwallSelectionObject(go);
	}

	public const float CELL_SELECTION_Z_OFFSET = -0.6f;

	public const float BACKWALL_SELECTION_Z_OFFSET = -0.5f;
}
