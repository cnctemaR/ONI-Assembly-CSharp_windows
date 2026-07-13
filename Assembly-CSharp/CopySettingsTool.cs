using System;
using System.Collections.Generic;
using UnityEngine;

public class CopySettingsTool : DragTool
{
	public static void DestroyInstance()
	{
		CopySettingsTool.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		CopySettingsTool.Instance = this;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	public void SetSourceObject(GameObject sourceGameObject)
	{
		this.sourceGameObject = sourceGameObject;
	}

	protected override void OnDragTool(int cell, int _distFromOrigin)
	{
		if (this.sourceGameObject == null)
		{
			return;
		}
		DebugUtil.DevAssert(Grid.IsValidCell(cell), "DragTool only calls us with valid cells", null);
		KPrefabID kprefabID = CopyBuildingSettings.ResolveTarget(CopyBuildingSettings.ResolveLayer(this.sourceGameObject), cell);
		if (kprefabID != null && kprefabID.gameObject != this.sourceGameObject)
		{
			this.targets.TryAdd(kprefabID.gameObject, kprefabID);
		}
	}

	protected override void OnDragComplete(Vector3 _cursorDown, Vector3 _cursorUp)
	{
		if (this.sourceGameObject != null)
		{
			KPrefabID kprefabID;
			this.sourceGameObject.TryGetComponent<KPrefabID>(out kprefabID);
			CopyBuildingSettings copyBuildingSettings;
			this.sourceGameObject.TryGetComponent<CopyBuildingSettings>(out copyBuildingSettings);
			if (kprefabID != null && copyBuildingSettings != null)
			{
				foreach (KPrefabID kprefabID2 in this.targets.Values)
				{
					CopyBuildingSettings.ApplyCopy(kprefabID2, this.sourceGameObject, kprefabID, copyBuildingSettings);
				}
			}
		}
		this.targets.Clear();
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		this.sourceGameObject = null;
	}

	public static CopySettingsTool Instance;

	public GameObject Placer;

	private GameObject sourceGameObject;

	private readonly Dictionary<GameObject, KPrefabID> targets = new Dictionary<GameObject, KPrefabID>();
}
