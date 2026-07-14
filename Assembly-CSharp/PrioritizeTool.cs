using System;
using UnityEngine;

public class PrioritizeTool : FilteredDragTool
{
	public static void DestroyInstance()
	{
		PrioritizeTool.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.interceptNumberKeysForPriority = true;
		PrioritizeTool.Instance = this;
		this.visualizer = Util.KInstantiate(this.visualizer, null, null);
		this.viewMode = OverlayModes.Priorities.ID;
		Game.Instance.prioritizableRenderer.currentTool = this;
	}

	public override string GetFilterLayerFromGameObject(GameObject input)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (input.GetComponent<Diggable>())
		{
			flag = true;
		}
		if (input.GetComponent<Constructable>() || (input.GetComponent<Deconstructable>() && input.GetComponent<Deconstructable>().IsMarkedForDeconstruction()))
		{
			flag2 = true;
		}
		if (input.GetComponent<Clearable>() || input.GetComponent<Moppable>() || input.GetComponent<StorageLocker>())
		{
			flag3 = true;
		}
		if (flag2)
		{
			return ToolParameterMenu.FILTERLAYERS.CONSTRUCTION;
		}
		if (flag)
		{
			return ToolParameterMenu.FILTERLAYERS.DIG;
		}
		if (flag3)
		{
			return ToolParameterMenu.FILTERLAYERS.CLEAN;
		}
		return ToolParameterMenu.FILTERLAYERS.OPERATE;
	}

	protected override void GetDefaultFilters(out ToolParameterMenu.ToggleData[] filters)
	{
		filters = new ToolParameterMenu.ToggleData[]
		{
			new ToolParameterMenu.ToggleData(ToolParameterMenu.FILTERLAYERS.ALL, ToolParameterMenu.ToggleState.On, false),
			new ToolParameterMenu.ToggleData(ToolParameterMenu.FILTERLAYERS.CONSTRUCTION, ToolParameterMenu.ToggleState.Off, false),
			new ToolParameterMenu.ToggleData(ToolParameterMenu.FILTERLAYERS.DIG, ToolParameterMenu.ToggleState.Off, false),
			new ToolParameterMenu.ToggleData(ToolParameterMenu.FILTERLAYERS.CLEAN, ToolParameterMenu.ToggleState.Off, false),
			new ToolParameterMenu.ToggleData(ToolParameterMenu.FILTERLAYERS.OPERATE, ToolParameterMenu.ToggleState.Off, false)
		};
	}

	private bool TryPrioritizeGameObject(GameObject target, PrioritySetting priority)
	{
		string filterLayerFromGameObject = this.GetFilterLayerFromGameObject(target);
		if (base.IsActiveLayer(filterLayerFromGameObject))
		{
			Prioritizable component = target.GetComponent<Prioritizable>();
			if (component != null && component.showIcon && component.IsPrioritizable())
			{
				component.SetMasterPriority(priority);
				return true;
			}
		}
		return false;
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		PrioritySetting lastSelectedPriority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority();
		int num = 0;
		for (int i = 0; i < 45; i++)
		{
			GameObject gameObject = Grid.Objects[cell, i];
			if (gameObject != null)
			{
				if (gameObject.GetComponent<Pickupable>())
				{
					ObjectLayerListItem objectLayerListItem = gameObject.GetComponent<Pickupable>().objectLayerListItem;
					while (objectLayerListItem != null)
					{
						GameObject gameObject2 = objectLayerListItem.gameObject;
						objectLayerListItem = objectLayerListItem.nextItem;
						if (!(gameObject2 == null) && !(gameObject2.GetComponent<MinionIdentity>() != null) && this.TryPrioritizeGameObject(gameObject2, lastSelectedPriority))
						{
							num++;
						}
					}
				}
				else if (this.TryPrioritizeGameObject(gameObject, lastSelectedPriority))
				{
					num++;
				}
			}
		}
		if (num > 0)
		{
			PriorityScreen.PlayPriorityConfirmSound(lastSelectedPriority);
		}
	}

	protected override void OnOverlayChanged(HashedString overlay)
	{
		if (!base.IsActive)
		{
			return;
		}
		ToolMenu.Instance.toolParameterMenu.PopulateMenu(this.currentFilters);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		ToolMenu.Instance.PriorityScreen.ShowDiagram(true);
		ToolMenu.Instance.PriorityScreen.Show(true);
		ToolMenu.Instance.PriorityScreen.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ToolMenu.Instance.PriorityScreen.Show(false);
		ToolMenu.Instance.PriorityScreen.ShowDiagram(false);
		ToolMenu.Instance.PriorityScreen.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	public void Update()
	{
		PrioritySetting lastSelectedPriority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority();
		int num = 0;
		if (lastSelectedPriority.priority_class >= PriorityScreen.PriorityClass.high)
		{
			num += 9;
		}
		if (lastSelectedPriority.priority_class >= PriorityScreen.PriorityClass.topPriority)
		{
			num = num;
		}
		num += lastSelectedPriority.priority_value;
		Texture2D texture2D = this.cursors[num - 1];
		MeshRenderer componentInChildren = this.visualizer.GetComponentInChildren<MeshRenderer>();
		if (componentInChildren != null)
		{
			componentInChildren.material.mainTexture = texture2D;
		}
	}

	public GameObject Placer;

	public static PrioritizeTool Instance;

	public Texture2D[] cursors;
}
