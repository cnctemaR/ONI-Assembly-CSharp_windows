using System;
using System.Collections.Generic;
using ProcGen;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/AsteroidDescriptorPanel")]
public class AsteroidDescriptorPanel : KMonoBehaviour
{
	public bool HasDescriptors()
	{
		return this.labels.Count > 0;
	}

	public void EnableClusterDetails(bool setActive)
	{
		this.clusterNameLabel.gameObject.SetActive(setActive);
		this.clusterDifficultyLabel.gameObject.SetActive(setActive);
	}

	public void SetClusterDetailLabels(ColonyDestinationAsteroidBeltData cluster)
	{
		StringEntry stringEntry;
		Strings.TryGet(cluster.properName, out stringEntry);
		this.clusterNameLabel.SetText((stringEntry == null) ? "" : string.Format(WORLDS.SURVIVAL_CHANCE.CLUSTERNAME, stringEntry.String));
		int num = Mathf.Clamp(cluster.difficulty, 0, ColonyDestinationAsteroidBeltData.survivalOptions.Count - 1);
		global::Tuple<string, string, string> tuple = ColonyDestinationAsteroidBeltData.survivalOptions[num];
		string text = string.Format(WORLDS.SURVIVAL_CHANCE.TITLE, tuple.first, tuple.third);
		text = text.Trim(new char[] { '\n' });
		this.clusterDifficultyLabel.SetText(text);
	}

	public void SetParameterDescriptors(IList<AsteroidDescriptor> descriptors)
	{
		for (int i = 0; i < this.parameterWidgets.Count; i++)
		{
			global::UnityEngine.Object.Destroy(this.parameterWidgets[i]);
		}
		this.parameterWidgets.Clear();
		for (int j = 0; j < descriptors.Count; j++)
		{
			GameObject gameObject = global::Util.KInstantiateUI(this.prefabParameterWidget, base.gameObject, true);
			gameObject.GetComponent<LocText>().SetText(descriptors[j].text);
			ToolTip component = gameObject.GetComponent<ToolTip>();
			if (!string.IsNullOrEmpty(descriptors[j].tooltip))
			{
				component.SetSimpleTooltip(descriptors[j].tooltip);
			}
			this.parameterWidgets.Add(gameObject);
		}
	}

	public void SetTraitDescriptors(IList<AsteroidDescriptor> descriptors)
	{
		for (int i = 0; i < this.traitWidgets.Count; i++)
		{
			global::UnityEngine.Object.Destroy(this.traitWidgets[i]);
		}
		this.traitWidgets.Clear();
		for (int j = 0; j < descriptors.Count; j++)
		{
			GameObject gameObject = global::Util.KInstantiate(this.prefabTraitWidget, base.gameObject, null);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("NameLabel").SetText("<b>" + descriptors[j].text + "</b>");
			component.GetReference<Image>("Icon").color = descriptors[j].associatedColor;
			LocText reference = component.GetReference<LocText>("DescLabel");
			if (!string.IsNullOrEmpty(descriptors[j].tooltip))
			{
				reference.SetText(descriptors[j].tooltip);
			}
			else
			{
				reference.gameObject.SetActive(false);
			}
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.SetActive(true);
			this.traitWidgets.Add(gameObject);
		}
	}

	public void EnableClusterLocationLabels(bool enable)
	{
		this.startingAsteroidRowContainer.transform.parent.gameObject.SetActive(enable);
		this.nearbyAsteroidRowContainer.transform.parent.gameObject.SetActive(enable);
		this.distantAsteroidRowContainer.transform.parent.gameObject.SetActive(enable);
	}

	public void RefreshAsteroidLines(ColonyDestinationAsteroidBeltData cluster, AsteroidDescriptorPanel selectedAsteroidDetailsPanel)
	{
		foreach (KeyValuePair<global::ProcGen.World, GameObject> keyValuePair in this.asteroidLines)
		{
			if (!keyValuePair.Value.IsNullOrDestroyed())
			{
				global::UnityEngine.Object.Destroy(keyValuePair.Value);
			}
		}
		this.asteroidLines.Clear();
		this.SpawnAsteroidLine(cluster.GetStartWorld, this.startingAsteroidRowContainer, cluster);
		for (int i = 0; i < cluster.worlds.Count; i++)
		{
			global::ProcGen.World world = cluster.worlds[i];
			WorldPlacement worldPlacement = null;
			for (int j = 0; j < cluster.Layout.worldPlacements.Count; j++)
			{
				if (cluster.Layout.worldPlacements[j].world == world.filePath)
				{
					worldPlacement = cluster.Layout.worldPlacements[j];
					break;
				}
			}
			this.SpawnAsteroidLine(world, (worldPlacement.locationType == WorldPlacement.LocationType.InnerCluster) ? this.nearbyAsteroidRowContainer : this.distantAsteroidRowContainer, cluster);
		}
		using (Dictionary<global::ProcGen.World, GameObject>.Enumerator enumerator = this.asteroidLines.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<global::ProcGen.World, GameObject> line = enumerator.Current;
				MultiToggle component = line.Value.GetComponent<MultiToggle>();
				component.onClick = (global::System.Action)Delegate.Combine(component.onClick, new global::System.Action(delegate
				{
					this.SelectAsteroidInCluster(line.Key, cluster, selectedAsteroidDetailsPanel);
				}));
			}
		}
		this.SelectAsteroidInCluster(cluster.GetStartWorld, cluster, selectedAsteroidDetailsPanel);
	}

	private void SelectAsteroidInCluster(global::ProcGen.World asteroid, ColonyDestinationAsteroidBeltData cluster, AsteroidDescriptorPanel selectedAsteroidDetailsPanel)
	{
		foreach (KeyValuePair<global::ProcGen.World, GameObject> keyValuePair in this.asteroidLines)
		{
			keyValuePair.Value.GetComponent<MultiToggle>().ChangeState((keyValuePair.Key == asteroid) ? 1 : 0);
			if (keyValuePair.Key == asteroid)
			{
				this.SetSelectedAsteroid(keyValuePair.Key, selectedAsteroidDetailsPanel, cluster.GenerateTraitDescriptors(keyValuePair.Key));
			}
		}
	}

	private void SpawnAsteroidLine(global::ProcGen.World asteroid, GameObject parentContainer, ColonyDestinationAsteroidBeltData cluster)
	{
		if (this.asteroidLines.ContainsKey(asteroid))
		{
			return;
		}
		GameObject gameObject = global::Util.KInstantiateUI(this.prefabAsteroidLine, parentContainer.gameObject, true);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		Image reference = component.GetReference<Image>("Icon");
		LocText reference2 = component.GetReference<LocText>("Label");
		RectTransform reference3 = component.GetReference<RectTransform>("TraitsRow");
		LocText reference4 = component.GetReference<LocText>("TraitLabel");
		ToolTip component2 = gameObject.GetComponent<ToolTip>();
		Sprite uisprite = ColonyDestinationAsteroidBeltData.GetUISprite(asteroid.asteroidIcon);
		reference.sprite = uisprite;
		StringEntry stringEntry;
		Strings.TryGet(asteroid.name, out stringEntry);
		reference2.SetText(stringEntry.String);
		List<WorldTrait> worldTraits = cluster.GetWorldTraits(asteroid);
		reference4.gameObject.SetActive(worldTraits.Count == 0);
		reference4.SetText(UI.FRONTEND.COLONYDESTINATIONSCREEN.NO_TRAITS);
		RectTransform reference5 = component.GetReference<RectTransform>("TraitIconPrefab");
		foreach (WorldTrait worldTrait in worldTraits)
		{
			global::Util.KInstantiateUI(reference5.gameObject, reference3.gameObject, true).GetComponent<Image>().color = global::Util.ColorFromHex(worldTrait.colorHex);
		}
		string text = "";
		if (worldTraits.Count > 0)
		{
			for (int i = 0; i < worldTraits.Count; i++)
			{
				StringEntry stringEntry2;
				Strings.TryGet(worldTraits[i].name, out stringEntry2);
				StringEntry stringEntry3;
				Strings.TryGet(worldTraits[i].description, out stringEntry3);
				text = string.Concat(new string[]
				{
					text,
					"<color=#",
					worldTraits[i].colorHex,
					">",
					stringEntry2.String,
					"</color>\n",
					stringEntry3.String
				});
				if (i != worldTraits.Count - 1)
				{
					text += "\n\n";
				}
			}
		}
		else
		{
			text = UI.FRONTEND.COLONYDESTINATIONSCREEN.NO_TRAITS;
		}
		component2.SetSimpleTooltip(text);
		this.asteroidLines.Add(asteroid, gameObject);
	}

	private void SetSelectedAsteroid(global::ProcGen.World asteroid, AsteroidDescriptorPanel detailPanel, List<AsteroidDescriptor> traitDescriptors)
	{
		detailPanel.SetTraitDescriptors(traitDescriptors);
		detailPanel.selectedAsteroidIcon.sprite = ColonyDestinationAsteroidBeltData.GetUISprite(asteroid.asteroidIcon);
		detailPanel.selectedAsteroidIcon.gameObject.SetActive(true);
		StringEntry stringEntry;
		Strings.TryGet(asteroid.name, out stringEntry);
		detailPanel.selectedAsteroidLabel.SetText(stringEntry.String);
		StringEntry stringEntry2;
		Strings.TryGet(asteroid.description, out stringEntry2);
		detailPanel.selectedAsteroidDescription.SetText(stringEntry2.String);
	}

	[Header("Destination Details")]
	[SerializeField]
	private GameObject customLabelPrefab;

	[SerializeField]
	private GameObject prefabTraitWidget;

	[SerializeField]
	private GameObject prefabParameterWidget;

	[SerializeField]
	private GameObject startingAsteroidRowContainer;

	[SerializeField]
	private GameObject nearbyAsteroidRowContainer;

	[SerializeField]
	private GameObject distantAsteroidRowContainer;

	[SerializeField]
	private LocText clusterNameLabel;

	[SerializeField]
	private LocText clusterDifficultyLabel;

	private List<GameObject> labels = new List<GameObject>();

	[Header("Selected Asteroid Details")]
	private GameObject SpacedOutContentContainer;

	public Image selectedAsteroidIcon;

	public LocText selectedAsteroidLabel;

	public LocText selectedAsteroidDescription;

	[SerializeField]
	private GameObject prefabAsteroidLine;

	private Dictionary<global::ProcGen.World, GameObject> asteroidLines = new Dictionary<global::ProcGen.World, GameObject>();

	private List<GameObject> traitWidgets = new List<GameObject>();

	private List<GameObject> parameterWidgets = new List<GameObject>();
}
