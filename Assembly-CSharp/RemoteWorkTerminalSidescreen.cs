using System;
using System.Collections.Generic;
using System.Linq;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class RemoteWorkTerminalSidescreen : SideScreenContent
{
	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		this.rowPrefab.SetActive(false);
		if (show)
		{
			this.RefreshOptions(null);
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<RemoteWorkTerminal>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		this.targetTerminal = target.GetComponent<RemoteWorkTerminal>();
		this.RefreshOptions(null);
		this.uiRefreshSubHandle = target.Subscribe(1980521255, new Action<object>(this.RefreshOptions));
	}

	public override void ClearTarget()
	{
		if (this.uiRefreshSubHandle != -1 && this.targetTerminal != null)
		{
			this.targetTerminal.gameObject.Unsubscribe(this.uiRefreshSubHandle);
			this.uiRefreshSubHandle = -1;
		}
	}

	private void RefreshOptions(object data = null)
	{
		int num = 0;
		this.SetRow(num++, UI.UISIDESCREENS.GEOTUNERSIDESCREEN.NOTHING, Assets.GetSprite("action_building_disabled"), null);
		foreach (RemoteWorkerDock remoteWorkerDock in Components.RemoteWorkerDocks.GetItems(this.targetTerminal.GetMyWorldId()))
		{
			remoteWorkerDock.GetProperName();
			Sprite first = Def.GetUISprite(remoteWorkerDock.gameObject, "ui", false).first;
			int num2 = num++;
			string text = UI.StripLinkFormatting(remoteWorkerDock.GetProperName());
			global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(remoteWorkerDock.gameObject, "ui", false);
			this.SetRow(num2, text, (uisprite != null) ? uisprite.first : null, remoteWorkerDock);
		}
		for (int i = num; i < this.rowContainer.childCount; i++)
		{
			this.rowContainer.GetChild(i).gameObject.SetActive(false);
		}
	}

	private void ClearRows()
	{
		for (int i = this.rowContainer.childCount - 1; i >= 0; i--)
		{
			Util.KDestroyGameObject(this.rowContainer.GetChild(i));
		}
		this.rows.Clear();
	}

	private void SetRow(int idx, string name, Sprite icon, RemoteWorkerDock dock)
	{
		dock == null;
		GameObject gameObject;
		if (idx < this.rowContainer.childCount)
		{
			gameObject = this.rowContainer.GetChild(idx).gameObject;
		}
		else
		{
			gameObject = Util.KInstantiateUI(this.rowPrefab, this.rowContainer.gameObject, true);
		}
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		LocText reference = component.GetReference<LocText>("label");
		reference.text = name;
		reference.ApplySettings();
		Image reference2 = component.GetReference<Image>("icon");
		reference2.sprite = icon;
		reference2.color = Color.white;
		ToolTip toolTip = gameObject.GetComponentsInChildren<ToolTip>().First<ToolTip>();
		toolTip.SetSimpleTooltip(UI.UISIDESCREENS.REMOTE_WORK_TERMINAL_SIDE_SCREEN.DOCK_TOOLTIP);
		toolTip.enabled = dock != null;
		MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
		component2.ChangeState((this.targetTerminal.FutureDock == dock) ? 1 : 0);
		component2.onClick = delegate
		{
			this.targetTerminal.FutureDock = dock;
			this.RefreshOptions(null);
		};
		component2.onDoubleClick = delegate
		{
			CameraController.Instance.CameraGoTo((dock == null) ? this.targetTerminal.transform.GetPosition() : dock.transform.GetPosition(), 2f, true);
			return true;
		};
	}

	private RemoteWorkTerminal targetTerminal;

	public GameObject rowPrefab;

	public RectTransform rowContainer;

	public Dictionary<object, GameObject> rows = new Dictionary<object, GameObject>();

	private int uiRefreshSubHandle = -1;
}
