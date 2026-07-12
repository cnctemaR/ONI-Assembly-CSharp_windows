using System;
using System.Collections.Generic;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpacePOISimpleInfoPanel : SimpleInfoPanel
{
	public SpacePOISimpleInfoPanel(SimpleInfoScreen simpleInfoScreen)
		: base(simpleInfoScreen)
	{
	}

	public override void Refresh(CollapsibleDetailContentPanel spacePOIPanel, GameObject selectedTarget)
	{
		spacePOIPanel.SetTitle(UI.CLUSTERMAP.POI.TITLE);
		if (selectedTarget == null)
		{
			spacePOIPanel.gameObject.SetActive(false);
			return;
		}
		HarvestablePOIClusterGridEntity harvestablePOIClusterGridEntity = ((selectedTarget == null) ? null : selectedTarget.GetComponent<HarvestablePOIClusterGridEntity>());
		Clustercraft component = selectedTarget.GetComponent<Clustercraft>();
		ArtifactPOIConfigurator artifactPOIConfigurator = selectedTarget.GetComponent<ArtifactPOIConfigurator>();
		if (harvestablePOIClusterGridEntity == null && component == null && artifactPOIConfigurator == null)
		{
			spacePOIPanel.gameObject.SetActive(false);
			return;
		}
		if (harvestablePOIClusterGridEntity == null && artifactPOIConfigurator == null && component != null)
		{
			RocketModuleCluster rocketModuleCluster = null;
			CraftModuleInterface craftModuleInterface = null;
			RocketSimpleInfoPanel.GetRocketStuffFromTarget(selectedTarget, ref rocketModuleCluster, ref component, ref craftModuleInterface);
			if (component != null)
			{
				foreach (ClusterGridEntity clusterGridEntity in ClusterGrid.Instance.GetEntitiesOnCell(component.GetMyWorldLocation()))
				{
					HarvestablePOIClusterGridEntity harvestablePOIClusterGridEntity2 = clusterGridEntity as HarvestablePOIClusterGridEntity;
					if (harvestablePOIClusterGridEntity2 != null)
					{
						harvestablePOIClusterGridEntity = harvestablePOIClusterGridEntity2;
						artifactPOIConfigurator = harvestablePOIClusterGridEntity2.GetComponent<ArtifactPOIConfigurator>();
						break;
					}
				}
			}
		}
		bool flag = harvestablePOIClusterGridEntity != null || artifactPOIConfigurator != null;
		spacePOIPanel.gameObject.SetActive(flag);
		if (!flag)
		{
			return;
		}
		HarvestablePOIStates.Instance instance = ((harvestablePOIClusterGridEntity == null) ? null : harvestablePOIClusterGridEntity.GetSMI<HarvestablePOIStates.Instance>());
		this.RefreshMassHeader(instance, selectedTarget, spacePOIPanel);
		this.RefreshElements(instance, selectedTarget, spacePOIPanel);
		this.RefreshArtifacts(artifactPOIConfigurator, selectedTarget, spacePOIPanel);
	}

	private void RefreshMassHeader(HarvestablePOIStates.Instance harvestable, GameObject selectedTarget, CollapsibleDetailContentPanel spacePOIPanel)
	{
		if (this.massHeader == null)
		{
			this.massHeader = Util.KInstantiateUI(this.simpleInfoRoot.iconLabelRow, spacePOIPanel.Content.gameObject, true);
		}
		this.massHeader.SetActive(harvestable != null);
		if (harvestable == null)
		{
			return;
		}
		HierarchyReferences component = this.massHeader.GetComponent<HierarchyReferences>();
		Sprite sprite = Assets.GetSprite("icon_asteroid_type");
		if (sprite != null)
		{
			component.GetReference<Image>("Icon").sprite = sprite;
		}
		component.GetReference<LocText>("NameLabel").text = UI.CLUSTERMAP.POI.MASS_REMAINING;
		component.GetReference<LocText>("ValueLabel").text = GameUtil.GetFormattedMass(harvestable.poiCapacity, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
		component.GetReference<LocText>("ValueLabel").alignment = TextAlignmentOptions.MidlineRight;
	}

	private void RefreshElements(HarvestablePOIStates.Instance harvestable, GameObject selectedTarget, CollapsibleDetailContentPanel spacePOIPanel)
	{
		foreach (KeyValuePair<Tag, GameObject> keyValuePair in this.elementRows)
		{
			if (keyValuePair.Value != null)
			{
				keyValuePair.Value.SetActive(false);
			}
		}
		if (harvestable == null)
		{
			return;
		}
		Dictionary<SimHashes, float> elementsWithWeights = harvestable.configuration.GetElementsWithWeights();
		float num = 0f;
		List<KeyValuePair<SimHashes, float>> list = new List<KeyValuePair<SimHashes, float>>();
		foreach (KeyValuePair<SimHashes, float> keyValuePair2 in elementsWithWeights)
		{
			num += keyValuePair2.Value;
			list.Add(keyValuePair2);
		}
		list.Sort((KeyValuePair<SimHashes, float> a, KeyValuePair<SimHashes, float> b) => b.Value.CompareTo(a.Value));
		foreach (KeyValuePair<SimHashes, float> keyValuePair3 in list)
		{
			SimHashes key = keyValuePair3.Key;
			Tag tag = key.CreateTag();
			if (!this.elementRows.ContainsKey(key.CreateTag()))
			{
				this.elementRows.Add(tag, Util.KInstantiateUI(this.simpleInfoRoot.iconLabelRow, spacePOIPanel.Content.gameObject, true));
			}
			this.elementRows[tag].SetActive(true);
			HierarchyReferences component = this.elementRows[tag].GetComponent<HierarchyReferences>();
			global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(tag, "ui", false);
			component.GetReference<Image>("Icon").sprite = uisprite.first;
			component.GetReference<Image>("Icon").color = uisprite.second;
			component.GetReference<LocText>("NameLabel").text = ElementLoader.GetElement(tag).name;
			component.GetReference<LocText>("ValueLabel").text = GameUtil.GetFormattedPercent(keyValuePair3.Value / num * 100f, GameUtil.TimeSlice.None);
			component.GetReference<LocText>("ValueLabel").alignment = TextAlignmentOptions.MidlineRight;
		}
	}

	private void RefreshRocketsAtThisLocation(HarvestablePOIStates.Instance harvestable, GameObject selectedTarget, CollapsibleDetailContentPanel spacePOIPanel)
	{
		if (this.rocketsHeader == null)
		{
			this.rocketsSpacer = Util.KInstantiateUI(this.simpleInfoRoot.spacerRow, spacePOIPanel.Content.gameObject, true);
			this.rocketsHeader = Util.KInstantiateUI(this.simpleInfoRoot.iconLabelRow, spacePOIPanel.Content.gameObject, true);
			HierarchyReferences component = this.rocketsHeader.GetComponent<HierarchyReferences>();
			Sprite sprite = Assets.GetSprite("ic_rocket");
			if (sprite != null)
			{
				component.GetReference<Image>("Icon").sprite = sprite;
				component.GetReference<Image>("Icon").color = Color.black;
			}
			component.GetReference<LocText>("NameLabel").text = UI.CLUSTERMAP.POI.ROCKETS_AT_THIS_LOCATION;
			component.GetReference<LocText>("ValueLabel").text = "";
		}
		this.rocketsSpacer.rectTransform().SetAsLastSibling();
		this.rocketsHeader.rectTransform().SetAsLastSibling();
		foreach (KeyValuePair<Clustercraft, GameObject> keyValuePair in this.rocketRows)
		{
			keyValuePair.Value.SetActive(false);
		}
		bool flag = true;
		for (int i = 0; i < Components.Clustercrafts.Count; i++)
		{
			Clustercraft clustercraft = Components.Clustercrafts[i];
			if (!this.rocketRows.ContainsKey(clustercraft))
			{
				GameObject gameObject = Util.KInstantiateUI(this.simpleInfoRoot.iconLabelRow, spacePOIPanel.Content.gameObject, true);
				this.rocketRows.Add(clustercraft, gameObject);
			}
			bool flag2 = clustercraft.Location == selectedTarget.GetComponent<KMonoBehaviour>().GetMyWorldLocation();
			flag = flag && !flag2;
			this.rocketRows[clustercraft].SetActive(flag2);
			if (flag2)
			{
				HierarchyReferences component2 = this.rocketRows[clustercraft].GetComponent<HierarchyReferences>();
				component2.GetReference<Image>("Icon").sprite = clustercraft.GetUISprite();
				component2.GetReference<Image>("Icon").color = Color.grey;
				component2.GetReference<LocText>("NameLabel").text = clustercraft.Name;
				component2.GetReference<LocText>("ValueLabel").text = "";
				component2.GetReference<LocText>("ValueLabel").alignment = TextAlignmentOptions.MidlineRight;
				this.rocketRows[clustercraft].rectTransform().SetAsLastSibling();
			}
		}
		this.rocketsHeader.SetActive(!flag);
		this.rocketsSpacer.SetActive(this.rocketsHeader.activeSelf);
	}

	private void RefreshArtifacts(ArtifactPOIConfigurator artifactConfigurator, GameObject selectedTarget, CollapsibleDetailContentPanel spacePOIPanel)
	{
		if (this.artifactsSpacer == null)
		{
			this.artifactsSpacer = Util.KInstantiateUI(this.simpleInfoRoot.spacerRow, spacePOIPanel.Content.gameObject, true);
			this.artifactRow = Util.KInstantiateUI(this.simpleInfoRoot.iconLabelRow, spacePOIPanel.Content.gameObject, true);
		}
		this.artifactsSpacer.rectTransform().SetAsLastSibling();
		this.artifactRow.rectTransform().SetAsLastSibling();
		ArtifactPOIStates.Instance smi = artifactConfigurator.GetSMI<ArtifactPOIStates.Instance>();
		smi.configuration.GetArtifactID();
		HierarchyReferences component = this.artifactRow.GetComponent<HierarchyReferences>();
		component.GetReference<LocText>("NameLabel").text = UI.CLUSTERMAP.POI.ARTIFACTS;
		component.GetReference<LocText>("ValueLabel").alignment = TextAlignmentOptions.MidlineRight;
		component.GetReference<Image>("Icon").sprite = Assets.GetSprite("ic_artifacts");
		component.GetReference<Image>("Icon").color = Color.black;
		if (smi.CanHarvestArtifact())
		{
			component.GetReference<LocText>("ValueLabel").text = UI.CLUSTERMAP.POI.ARTIFACTS_AVAILABLE;
			return;
		}
		component.GetReference<LocText>("ValueLabel").text = UI.CLUSTERMAP.POI.ARTIFACTS_DEPLETED;
	}

	private Dictionary<Tag, GameObject> elementRows = new Dictionary<Tag, GameObject>();

	private Dictionary<Clustercraft, GameObject> rocketRows = new Dictionary<Clustercraft, GameObject>();

	private GameObject massHeader;

	private GameObject rocketsSpacer;

	private GameObject rocketsHeader;

	private GameObject artifactsSpacer;

	private GameObject artifactRow;
}
