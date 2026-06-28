using System;
using UnityEngine;

public class PlaceTool : DragTool
{
	protected override void OnPrefabInit()
	{
		PlaceTool.Instance = this;
		this.tooltip = base.GetComponent<ToolTip>();
	}

	protected override void OnActivateTool()
	{
		this.active = true;
		base.OnActivateTool();
		GameObject prefab = Assets.GetPrefab(this.previewTag);
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Front;
		Folder folder = Folder.Placers;
		int num = LayerMask.NameToLayer("Place");
		this.visualizer = GameUtil.KInstantiate(prefab, sceneLayer, folder, null, num);
		KBatchedAnimController component = this.visualizer.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.visibilityType = KAnimControllerBase.VisibilityType.Always;
			component.isMovable = true;
		}
		this.visualizer.SetActive(true);
		this.visualizer.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.Placers).transform;
		this.ShowToolTip();
		BuildToolHoverTextCard component2 = base.GetComponent<BuildToolHoverTextCard>();
		component2.currentDef = null;
		component2.ConfigureHoverScreen();
		component2.UpdateHoverElements(null);
		ResourceRemainingDisplayScreen.instance.ActivateDisplay(this.visualizer);
		if (component == null)
		{
			this.visualizer.SetLayerRecursively(LayerMask.NameToLayer("Place"));
		}
		else
		{
			component.SetLayer(LayerMask.NameToLayer("Place"));
		}
		GridCompositor.Instance.ToggleMajor(true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		this.active = false;
		GridCompositor.Instance.ToggleMajor(false);
		this.HideToolTip();
		ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
		global::UnityEngine.Object.Destroy(this.visualizer);
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound(this.GetDeactivateSound(), false));
		base.OnDeactivateTool(new_tool);
	}

	public void Activate(Placeable source, Tag previewTag)
	{
		this.source = source;
		this.previewTag = previewTag;
		PlayerController.Instance.ActivateTool(this);
	}

	public void Deactivate()
	{
		SelectTool.Instance.Activate();
		this.source = null;
		this.previewTag = Tag.Invalid;
		ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (this.visualizer == null)
		{
			return;
		}
		bool flag = false;
		EntityPreview component = this.visualizer.GetComponent<EntityPreview>();
		if (component.Valid)
		{
			if (DebugHandler.InstantBuildMode)
			{
				this.source.Place(cell);
			}
			else
			{
				this.source.QueuePlacement(cell);
			}
			flag = true;
		}
		if (flag)
		{
			this.Deactivate();
		}
	}

	protected override DragTool.Mode GetMode()
	{
		return DragTool.Mode.Brush;
	}

	private void ShowToolTip()
	{
		ToolTipScreen.Instance.SetToolTip(this.tooltip);
	}

	private void HideToolTip()
	{
		ToolTipScreen.Instance.ClearToolTip(this.tooltip);
	}

	public override void Update()
	{
		if (this.active)
		{
			KBatchedAnimController component = this.visualizer.GetComponent<KBatchedAnimController>();
			if (component != null)
			{
				component.SetLayer(LayerMask.NameToLayer("Place"));
			}
		}
	}

	public override string GetDeactivateSound()
	{
		return "HUD_Click_Deselect";
	}

	[SerializeField]
	private TextStyleSetting tooltipStyle;

	private Tag previewTag;

	private Placeable source;

	private ToolTip tooltip;

	public static PlaceTool Instance;

	private bool active;
}
