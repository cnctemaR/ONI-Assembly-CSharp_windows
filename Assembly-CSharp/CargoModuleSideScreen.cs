using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class CargoModuleSideScreen : SideScreenContent, ISimEveryTick
{
	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		base.ConsumeMouseScroll = true;
	}

	public override float GetSortKey()
	{
		return 21f;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<Clustercraft>() != null && this.GetCollectionModules(target.GetComponent<Clustercraft>()).Length != 0;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		this.targetCraft = target.GetComponent<Clustercraft>();
		this.RefreshModulePanel(this.targetCraft);
	}

	private IHexCellCollector[] GetCollectionModules(Clustercraft craft)
	{
		List<IHexCellCollector> list = new List<IHexCellCollector>();
		foreach (Ref<RocketModuleCluster> @ref in craft.ModuleInterface.ClusterModules)
		{
			IHexCellCollector hexCellCollector = @ref.Get().GetComponent<IHexCellCollector>();
			if (hexCellCollector == null)
			{
				hexCellCollector = @ref.Get().GetSMI<IHexCellCollector>();
			}
			if (hexCellCollector != null)
			{
				list.Add(hexCellCollector);
			}
		}
		return list.ToArray();
	}

	private void RefreshModulePanel(Clustercraft module)
	{
		this.ClearModules();
		foreach (IHexCellCollector hexCellCollector in this.GetCollectionModules(module))
		{
			GameObject gameObject = Util.KInstantiateUI(this.modulePanelPrefab, this.moduleContentContainer, true);
			this.modulePanels.Add(hexCellCollector, gameObject);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<Image>("icon").sprite = hexCellCollector.GetUISprite();
			component.GetReference<LocText>("label").SetText(hexCellCollector.GetProperName());
		}
		this.RefreshProgressBars();
		this.scrollRectLayout.preferredHeight = (this.scrollRectLayout.minHeight = Mathf.Min((float)this.modulePanels.Count, 2.5f) * this.modulePanelPrefab.GetComponent<RectTransform>().rect.height);
	}

	private void ClearModules()
	{
		foreach (KeyValuePair<IHexCellCollector, GameObject> keyValuePair in this.modulePanels)
		{
			Util.KDestroyGameObject(keyValuePair.Value.gameObject);
		}
		this.modulePanels.Clear();
	}

	private void RefreshProgressBars()
	{
		if (this.targetCraft.IsNullOrDestroyed())
		{
			return;
		}
		if (ClusterMapSelectTool.Instance.GetSelected() == null || !this.IsValidForTarget(ClusterMapSelectTool.Instance.GetSelected().gameObject))
		{
			return;
		}
		foreach (KeyValuePair<IHexCellCollector, GameObject> keyValuePair in this.modulePanels)
		{
			HierarchyReferences component = keyValuePair.Value.GetComponent<HierarchyReferences>();
			GenericUIProgressBar reference = component.GetReference<GenericUIProgressBar>("gatheringProgressBar");
			float num = 4f;
			float num2 = keyValuePair.Key.GetCapacity() - keyValuePair.Key.GetMassStored();
			if (keyValuePair.Key.CheckIsCollecting())
			{
				float num3 = keyValuePair.Key.TimeInState() % num;
				if (num2 > 0f)
				{
					reference.SetFillPercentage(num3 / num);
					reference.label.SetText(UI.UISIDESCREENS.CARGOMODULESIDESCREEN.GATHERING_IN_PROGRESS);
				}
			}
			else if (num2 == 0f)
			{
				reference.SetFillPercentage(0f);
				reference.label.SetText(UI.UISIDESCREENS.CARGOMODULESIDESCREEN.GATHERING_FULL);
			}
			else
			{
				reference.SetFillPercentage(0f);
				reference.label.SetText(UI.UISIDESCREENS.CARGOMODULESIDESCREEN.GATHERING_STOPPED);
			}
			GenericUIProgressBar reference2 = component.GetReference<GenericUIProgressBar>("capacityProgressBar");
			float num4 = keyValuePair.Key.GetMassStored() / keyValuePair.Key.GetCapacity();
			reference2.SetFillPercentage(num4);
			reference2.label.SetText(keyValuePair.Key.GetCapacityBarText());
		}
	}

	public void SimEveryTick(float dt)
	{
		this.RefreshProgressBars();
	}

	private Clustercraft targetCraft;

	private Dictionary<IHexCellCollector, GameObject> modulePanels = new Dictionary<IHexCellCollector, GameObject>();

	public GameObject moduleContentContainer;

	public GameObject modulePanelPrefab;

	[SerializeField]
	private LayoutElement scrollRectLayout;
}
