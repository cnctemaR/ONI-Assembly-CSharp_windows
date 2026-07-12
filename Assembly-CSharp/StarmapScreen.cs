using System;
using System.Collections;
using System.Collections.Generic;
using Database;
using FMOD.Studio;
using STRINGS;
using TMPro;
using TUNING;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StarmapScreen : KModalScreen
{
	public override float GetSortKey()
	{
		if (base.isEditing)
		{
			return 50f;
		}
		return 20f;
	}

	public static void DestroyInstance()
	{
		StarmapScreen.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.ConsumeMouseScroll = true;
		this.rocketDetailsStatus = global::UnityEngine.Object.Instantiate<BreakdownList>(this.breakdownListPrefab, this.rocketDetailsContainer);
		this.rocketDetailsStatus.SetTitle(UI.STARMAP.LISTTITLES.MISSIONSTATUS);
		this.rocketDetailsStatus.SetIcon(this.rocketDetailsStatusIcon);
		this.rocketDetailsStatus.gameObject.name = "rocketDetailsStatus";
		this.rocketDetailsChecklist = global::UnityEngine.Object.Instantiate<BreakdownList>(this.breakdownListPrefab, this.rocketDetailsContainer);
		this.rocketDetailsChecklist.SetTitle(UI.STARMAP.LISTTITLES.LAUNCHCHECKLIST);
		this.rocketDetailsChecklist.SetIcon(this.rocketDetailsChecklistIcon);
		this.rocketDetailsChecklist.gameObject.name = "rocketDetailsChecklist";
		this.rocketDetailsRange = global::UnityEngine.Object.Instantiate<BreakdownList>(this.breakdownListPrefab, this.rocketDetailsContainer);
		this.rocketDetailsRange.SetTitle(UI.STARMAP.LISTTITLES.MAXRANGE);
		this.rocketDetailsRange.SetIcon(this.rocketDetailsRangeIcon);
		this.rocketDetailsRange.gameObject.name = "rocketDetailsRange";
		this.rocketDetailsMass = global::UnityEngine.Object.Instantiate<BreakdownList>(this.breakdownListPrefab, this.rocketDetailsContainer);
		this.rocketDetailsMass.SetTitle(UI.STARMAP.LISTTITLES.MASS);
		this.rocketDetailsMass.SetIcon(this.rocketDetailsMassIcon);
		this.rocketDetailsMass.gameObject.name = "rocketDetailsMass";
		this.rocketThrustWidget = global::UnityEngine.Object.Instantiate<RocketThrustWidget>(this.rocketThrustWidget, this.rocketDetailsContainer);
		this.rocketDetailsStorage = global::UnityEngine.Object.Instantiate<BreakdownList>(this.breakdownListPrefab, this.rocketDetailsContainer);
		this.rocketDetailsStorage.SetTitle(UI.STARMAP.LISTTITLES.STORAGE);
		this.rocketDetailsStorage.SetIcon(this.rocketDetailsStorageIcon);
		this.rocketDetailsStorage.gameObject.name = "rocketDetailsStorage";
		this.rocketDetailsFuel = global::UnityEngine.Object.Instantiate<BreakdownList>(this.breakdownListPrefab, this.rocketDetailsContainer);
		this.rocketDetailsFuel.SetTitle(UI.STARMAP.LISTTITLES.FUEL);
		this.rocketDetailsFuel.SetIcon(this.rocketDetailsFuelIcon);
		this.rocketDetailsFuel.gameObject.name = "rocketDetailsFuel";
		this.rocketDetailsOxidizer = global::UnityEngine.Object.Instantiate<BreakdownList>(this.breakdownListPrefab, this.rocketDetailsContainer);
		this.rocketDetailsOxidizer.SetTitle(UI.STARMAP.LISTTITLES.OXIDIZER);
		this.rocketDetailsOxidizer.SetIcon(this.rocketDetailsOxidizerIcon);
		this.rocketDetailsOxidizer.gameObject.name = "rocketDetailsOxidizer";
		this.rocketDetailsDupes = global::UnityEngine.Object.Instantiate<BreakdownList>(this.breakdownListPrefab, this.rocketDetailsContainer);
		this.rocketDetailsDupes.SetTitle(UI.STARMAP.LISTTITLES.PASSENGERS);
		this.rocketDetailsDupes.SetIcon(this.rocketDetailsDupesIcon);
		this.rocketDetailsDupes.gameObject.name = "rocketDetailsDupes";
		this.destinationDetailsAnalysis = global::UnityEngine.Object.Instantiate<BreakdownList>(this.breakdownListPrefab, this.destinationDetailsContainer);
		this.destinationDetailsAnalysis.SetTitle(UI.STARMAP.LISTTITLES.ANALYSIS);
		this.destinationDetailsAnalysis.SetIcon(this.destinationDetailsAnalysisIcon);
		this.destinationDetailsAnalysis.gameObject.name = "destinationDetailsAnalysis";
		this.destinationDetailsAnalysis.SetDescription(string.Format(UI.STARMAP.ANALYSIS_DESCRIPTION, 0));
		this.destinationAnalysisProgressBar = global::UnityEngine.Object.Instantiate<GameObject>(this.progressBarPrefab.gameObject, this.destinationDetailsContainer).GetComponent<GenericUIProgressBar>();
		this.destinationAnalysisProgressBar.SetMaxValue((float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE);
		this.destinationDetailsResearch = global::UnityEngine.Object.Instantiate<BreakdownList>(this.breakdownListPrefab, this.destinationDetailsContainer);
		this.destinationDetailsResearch.SetTitle(UI.STARMAP.LISTTITLES.RESEARCH);
		this.destinationDetailsResearch.SetIcon(this.destinationDetailsResearchIcon);
		this.destinationDetailsResearch.gameObject.name = "destinationDetailsResearch";
		this.destinationDetailsResearch.SetDescription(string.Format(UI.STARMAP.RESEARCH_DESCRIPTION, 0));
		this.destinationDetailsMass = global::UnityEngine.Object.Instantiate<BreakdownList>(this.breakdownListPrefab, this.destinationDetailsContainer);
		this.destinationDetailsMass.SetTitle(UI.STARMAP.LISTTITLES.DESTINATION_MASS);
		this.destinationDetailsMass.SetIcon(this.destinationDetailsMassIcon);
		this.destinationDetailsMass.gameObject.name = "destinationDetailsMass";
		this.destinationDetailsComposition = global::UnityEngine.Object.Instantiate<BreakdownList>(this.breakdownListPrefab, this.destinationDetailsContainer);
		this.destinationDetailsComposition.SetTitle(UI.STARMAP.LISTTITLES.WORLDCOMPOSITION);
		this.destinationDetailsComposition.SetIcon(this.destinationDetailsCompositionIcon);
		this.destinationDetailsComposition.gameObject.name = "destinationDetailsComposition";
		this.destinationDetailsResources = global::UnityEngine.Object.Instantiate<BreakdownList>(this.breakdownListPrefab, this.destinationDetailsContainer);
		this.destinationDetailsResources.SetTitle(UI.STARMAP.LISTTITLES.RESOURCES);
		this.destinationDetailsResources.SetIcon(this.destinationDetailsResourcesIcon);
		this.destinationDetailsResources.gameObject.name = "destinationDetailsResources";
		this.destinationDetailsArtifacts = global::UnityEngine.Object.Instantiate<BreakdownList>(this.breakdownListPrefab, this.destinationDetailsContainer);
		this.destinationDetailsArtifacts.SetTitle(UI.STARMAP.LISTTITLES.ARTIFACTS);
		this.destinationDetailsArtifacts.SetIcon(this.destinationDetailsArtifactsIcon);
		this.destinationDetailsArtifacts.gameObject.name = "destinationDetailsArtifacts";
		this.LoadPlanets();
		this.selectionUpdateHandle = Game.Instance.Subscribe(-1503271301, new Action<object>(this.OnSelectableChanged));
		this.titleBarLabel.text = UI.STARMAP.TITLE;
		this.button.onClick += delegate
		{
			ManagementMenu.Instance.ToggleStarmap();
		};
		this.launchButton.play_sound_on_click = false;
		MultiToggle multiToggle = this.launchButton;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			if (this.currentLaunchConditionManager != null && this.selectedDestination != null)
			{
				KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Click", false));
				this.LaunchRocket(this.currentLaunchConditionManager);
				return;
			}
			KFMOD.PlayUISound(GlobalAssets.GetSound("Negative", false));
		}));
		this.launchButton.ChangeState(1);
		MultiToggle multiToggle2 = this.showRocketsButton;
		multiToggle2.onClick = (global::System.Action)Delegate.Combine(multiToggle2.onClick, new global::System.Action(delegate
		{
			this.OnSelectableChanged(null);
		}));
		this.SelectDestination(null);
		SpacecraftManager.instance.Subscribe(532901469, delegate(object data)
		{
			this.RefreshAnalyzeButton();
			this.UpdateDestinationStates();
		});
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.selectionUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(this.selectionUpdateHandle);
		}
		base.StopAllCoroutines();
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().MENUStarmapSnapshot);
			MusicManager.instance.PlaySong("Music_Starmap", false);
			this.UpdateDestinationStates();
			this.Refresh(null);
		}
		else
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUStarmapSnapshot, STOP_MODE.ALLOWFADEOUT);
			MusicManager.instance.StopSong("Music_Starmap", true, STOP_MODE.ALLOWFADEOUT);
		}
		this.OnSelectableChanged((SelectTool.Instance.selected == null) ? null : SelectTool.Instance.selected.gameObject);
		this.forceScrollDown = true;
	}

	private void UpdateDestinationStates()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 1;
		foreach (SpaceDestination spaceDestination in SpacecraftManager.instance.destinations)
		{
			num = Mathf.Max(num, spaceDestination.OneBasedDistance);
			if (spaceDestination.AnalysisState() == SpacecraftManager.DestinationAnalysisState.Complete)
			{
				num2 = Mathf.Max(num2, spaceDestination.OneBasedDistance);
			}
		}
		for (int i = num2; i < num; i++)
		{
			bool flag = false;
			using (List<SpaceDestination>.Enumerator enumerator = SpacecraftManager.instance.destinations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.distance == i)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				break;
			}
			num3++;
		}
		using (Dictionary<SpaceDestination, StarmapPlanet>.Enumerator enumerator2 = this.planetWidgets.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				KeyValuePair<SpaceDestination, StarmapPlanet> KVP = enumerator2.Current;
				SpaceDestination key = KVP.Key;
				StarmapPlanet planet = KVP.Value;
				Color color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
				Color color2 = new Color(0.75f, 0.75f, 0.75f, 0.75f);
				if (KVP.Key.distance >= num2 + num3)
				{
					planet.SetUnknownBGActive(false, Color.white);
					planet.SetSprite(Assets.GetSprite("unknown_far"), color);
				}
				else
				{
					planet.SetAnalysisActive(SpacecraftManager.instance.GetStarmapAnalysisDestinationID() == KVP.Key.id);
					bool flag2 = SpacecraftManager.instance.GetDestinationAnalysisState(key) == SpacecraftManager.DestinationAnalysisState.Complete;
					SpaceDestinationType destinationType = key.GetDestinationType();
					planet.SetLabel(flag2 ? (destinationType.Name + "\n<color=#979798> " + GameUtil.GetFormattedDistance((float)KVP.Key.OneBasedDistance * 10000f * 1000f) + "</color>") : (UI.STARMAP.UNKNOWN_DESTINATION + "\n" + string.Format(UI.STARMAP.ANALYSIS_AMOUNT.text, GameUtil.GetFormattedPercent(100f * (SpacecraftManager.instance.GetDestinationAnalysisScore(KVP.Key) / (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE), GameUtil.TimeSlice.None))));
					planet.SetSprite(flag2 ? Assets.GetSprite(destinationType.spriteName) : Assets.GetSprite("unknown"), flag2 ? Color.white : color2);
					planet.SetUnknownBGActive(SpacecraftManager.instance.GetDestinationAnalysisState(KVP.Key) != SpacecraftManager.DestinationAnalysisState.Complete, color2);
					planet.SetFillAmount(SpacecraftManager.instance.GetDestinationAnalysisScore(KVP.Key) / (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE);
					List<int> spacecraftsForDestination = SpacecraftManager.instance.GetSpacecraftsForDestination(key);
					planet.SetRocketIcons(spacecraftsForDestination.Count, this.rocketIconPrefab);
					bool flag3 = this.currentLaunchConditionManager != null && key == SpacecraftManager.instance.GetSpacecraftDestination(this.currentLaunchConditionManager);
					planet.ShowAsCurrentRocketDestination(flag3);
					planet.SetOnClick(delegate
					{
						if (this.currentLaunchConditionManager == null)
						{
							this.SelectDestination(KVP.Key);
							return;
						}
						if (SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this.currentLaunchConditionManager).state == Spacecraft.MissionState.Grounded)
						{
							this.SelectDestination(KVP.Key);
						}
					});
					planet.SetOnEnter(delegate
					{
						planet.ShowLabel(true);
					});
					planet.SetOnExit(delegate
					{
						planet.ShowLabel(false);
					});
				}
			}
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		StarmapScreen.Instance = this;
	}

	private string DisplayDistance(float distance)
	{
		return global::Util.FormatWholeNumber(distance) + " " + UI.UNITSUFFIXES.DISTANCE.KILOMETER;
	}

	private string DisplayDestinationMass(SpaceDestination selectedDestination)
	{
		return GameUtil.GetFormattedMass(selectedDestination.AvailableMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}");
	}

	private string DisplayTotalStorageCapacity(CommandModule command)
	{
		float num = 0f;
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(command.GetComponent<AttachableBuilding>()))
		{
			CargoBay component = gameObject.GetComponent<CargoBay>();
			if (component != null)
			{
				num += component.storage.Capacity();
			}
		}
		return GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}");
	}

	private string StorageCapacityTooltip(CommandModule command, SpaceDestination dest)
	{
		string text = "";
		bool flag = dest != null && SpacecraftManager.instance.GetDestinationAnalysisState(dest) == SpacecraftManager.DestinationAnalysisState.Complete;
		if (dest != null && flag)
		{
			if (dest.AvailableMass <= ConditionHasMinimumMass.CargoCapacity(dest, command))
			{
				text = text + UI.STARMAP.LAUNCHCHECKLIST.INSUFFICENT_MASS_TOOLTIP + UI.HORIZONTAL_BR_RULE;
			}
			text = text + string.Format(UI.STARMAP.LAUNCHCHECKLIST.RESOURCE_MASS_TOOLTIP, dest.GetDestinationType().Name, GameUtil.GetFormattedMass(dest.AvailableMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"), GameUtil.GetFormattedMass(ConditionHasMinimumMass.CargoCapacity(dest, command), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}")) + "\n\n";
		}
		float num = ((dest != null) ? dest.AvailableMass : 0f);
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(command.GetComponent<AttachableBuilding>()))
		{
			CargoBay component = gameObject.GetComponent<CargoBay>();
			if (component != null)
			{
				if (flag)
				{
					float availableResourcesPercentage = dest.GetAvailableResourcesPercentage(component.storageType);
					float num2 = Mathf.Min(component.storage.Capacity(), availableResourcesPercentage * num);
					num -= num2;
					text = string.Concat(new string[]
					{
						text,
						component.gameObject.GetProperName(),
						" ",
						string.Format(UI.STARMAP.STORAGESTATS.STORAGECAPACITY, GameUtil.GetFormattedMass(Mathf.Min(num2, component.storage.Capacity()), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"), GameUtil.GetFormattedMass(component.storage.Capacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}")),
						"\n"
					});
				}
				else
				{
					text = string.Concat(new string[]
					{
						text,
						component.gameObject.GetProperName(),
						" ",
						string.Format(UI.STARMAP.STORAGESTATS.STORAGECAPACITY, GameUtil.GetFormattedMass(0f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"), GameUtil.GetFormattedMass(component.storage.Capacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}")),
						"\n"
					});
				}
			}
		}
		return text;
	}

	private void LoadPlanets()
	{
		foreach (SpaceDestination spaceDestination in Game.Instance.spacecraftManager.destinations)
		{
			if ((float)spaceDestination.OneBasedDistance * 10000f > this.planetsMaxDistance)
			{
				this.planetsMaxDistance = (float)spaceDestination.OneBasedDistance * 10000f;
			}
			while (this.planetRows.Count < spaceDestination.distance + 1)
			{
				GameObject gameObject = global::Util.KInstantiateUI(this.rowPrefab, this.rowsContiner.gameObject, true);
				gameObject.rectTransform().SetAsFirstSibling();
				this.planetRows.Add(gameObject);
				gameObject.GetComponentInChildren<Image>().color = this.distanceColors[this.planetRows.Count % this.distanceColors.Length];
				gameObject.GetComponentInChildren<LocText>().text = this.DisplayDistance((float)(this.planetRows.Count + 1) * 10000f);
			}
			GameObject gameObject2 = global::Util.KInstantiateUI(this.planetPrefab.gameObject, this.planetRows[spaceDestination.distance], true);
			this.planetWidgets.Add(spaceDestination, gameObject2.GetComponent<StarmapPlanet>());
		}
		this.UpdateDestinationStates();
	}

	private void UnselectAllPlanets()
	{
		if (this.animateSelectedPlanetRoutine != null)
		{
			base.StopCoroutine(this.animateSelectedPlanetRoutine);
		}
		foreach (KeyValuePair<SpaceDestination, StarmapPlanet> keyValuePair in this.planetWidgets)
		{
			keyValuePair.Value.SetSelectionActive(false);
			keyValuePair.Value.ShowAsCurrentRocketDestination(false);
		}
	}

	private void SelectPlanet(StarmapPlanet planet)
	{
		planet.SetSelectionActive(true);
		if (this.animateSelectedPlanetRoutine != null)
		{
			base.StopCoroutine(this.animateSelectedPlanetRoutine);
		}
		this.animateSelectedPlanetRoutine = base.StartCoroutine(this.AnimatePlanetSelection(planet));
	}

	private IEnumerator AnimatePlanetSelection(StarmapPlanet planet)
	{
		for (;;)
		{
			planet.AnimateSelector(Time.unscaledTime);
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	private void Update()
	{
		this.PositionPlanetWidgets();
		if (this.forceScrollDown)
		{
			this.ScrollToBottom();
			this.forceScrollDown = false;
		}
	}

	private void ScrollToBottom()
	{
		RectTransform rectTransform = this.Map.GetComponentInChildren<VerticalLayoutGroup>().rectTransform();
		rectTransform.SetLocalPosition(new Vector3(rectTransform.localPosition.x, rectTransform.rect.height - this.Map.rect.height, rectTransform.localPosition.z));
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.CheckBlockedInput())
		{
			if (!e.Consumed)
			{
				e.Consumed = true;
				return;
			}
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private bool CheckBlockedInput()
	{
		if (global::UnityEngine.EventSystems.EventSystem.current != null)
		{
			GameObject currentSelectedGameObject = global::UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
			if (currentSelectedGameObject != null)
			{
				foreach (KeyValuePair<Spacecraft, HierarchyReferences> keyValuePair in this.listRocketRows)
				{
					EditableTitleBar component = keyValuePair.Value.GetReference<RectTransform>("EditableTitle").GetComponent<EditableTitleBar>();
					if (currentSelectedGameObject == component.inputField.gameObject)
					{
						return true;
					}
				}
				return false;
			}
		}
		return false;
	}

	private void PositionPlanetWidgets()
	{
		float num = this.rowPrefab.GetComponent<RectTransform>().rect.height / 2f;
		foreach (KeyValuePair<SpaceDestination, StarmapPlanet> keyValuePair in this.planetWidgets)
		{
			keyValuePair.Value.rectTransform().anchoredPosition = new Vector2(keyValuePair.Value.transform.parent.rectTransform().sizeDelta.x * keyValuePair.Key.startingOrbitPercentage, -num);
		}
	}

	private void OnSelectableChanged(object data)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (this.rocketConditionEventHandler != -1)
		{
			base.Unsubscribe(this.rocketConditionEventHandler);
		}
		if (data != null)
		{
			this.currentSelectable = ((GameObject)data).GetComponent<KSelectable>();
			this.currentCommandModule = this.currentSelectable.GetComponent<CommandModule>();
			this.currentLaunchConditionManager = this.currentSelectable.GetComponent<LaunchConditionManager>();
			if (this.currentCommandModule != null && this.currentLaunchConditionManager != null)
			{
				SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(this.currentLaunchConditionManager);
				this.SelectDestination(spacecraftDestination);
				this.rocketConditionEventHandler = this.currentLaunchConditionManager.Subscribe(1655598572, new Action<object>(this.Refresh));
				this.ShowRocketDetailsPanel();
			}
			else
			{
				this.currentSelectable = null;
				this.currentCommandModule = null;
				this.currentLaunchConditionManager = null;
				this.ShowRocketListPanel();
			}
		}
		else
		{
			this.currentSelectable = null;
			this.currentCommandModule = null;
			this.currentLaunchConditionManager = null;
			this.ShowRocketListPanel();
		}
		this.Refresh(null);
	}

	private void ShowRocketListPanel()
	{
		this.listPanel.SetActive(true);
		this.rocketPanel.SetActive(false);
		this.launchButton.ChangeState(1);
		this.UpdateDistanceOverlay(null);
		this.UpdateMissionOverlay(null);
	}

	private void ShowRocketDetailsPanel()
	{
		this.listPanel.SetActive(false);
		this.rocketPanel.SetActive(true);
		this.ValidateTravelAbility();
		this.UpdateDistanceOverlay(null);
		this.UpdateMissionOverlay(null);
	}

	private void LaunchRocket(LaunchConditionManager lcm)
	{
		SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(lcm);
		if (spacecraftDestination == null)
		{
			return;
		}
		lcm.Launch(spacecraftDestination);
		this.ClearRocketListPanel();
		this.FillRocketListPanel();
		this.ShowRocketListPanel();
		this.Refresh(null);
	}

	private void OnStartedTitlebarEditing()
	{
		base.isEditing = true;
		KScreenManager.Instance.RefreshStack();
	}

	private void OnEndEditing(string data)
	{
		base.isEditing = false;
	}

	private void FillRocketListPanel()
	{
		this.ClearRocketListPanel();
		List<Spacecraft> spacecraft = SpacecraftManager.instance.GetSpacecraft();
		if (spacecraft.Count == 0)
		{
			this.listHeaderStatusLabel.text = UI.STARMAP.NO_ROCKETS_TITLE;
			this.listNoRocketText.gameObject.SetActive(true);
		}
		else
		{
			this.listHeaderStatusLabel.text = string.Format(UI.STARMAP.ROCKET_COUNT, spacecraft.Count);
			this.listNoRocketText.gameObject.SetActive(false);
		}
		using (List<Spacecraft>.Enumerator enumerator = spacecraft.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				StarmapScreen.<>c__DisplayClass114_0 CS$<>8__locals1 = new StarmapScreen.<>c__DisplayClass114_0();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.rocket = enumerator.Current;
				HierarchyReferences hierarchyReferences = global::Util.KInstantiateUI<HierarchyReferences>(this.listRocketTemplate.gameObject, this.rocketListContainer.gameObject, true);
				BreakdownList component = hierarchyReferences.GetComponent<BreakdownList>();
				MultiToggle component2 = hierarchyReferences.GetComponent<MultiToggle>();
				EditableTitleBar component3 = hierarchyReferences.GetReference<RectTransform>("EditableTitle").GetComponent<EditableTitleBar>();
				component3.OnStartedEditing += this.OnStartedTitlebarEditing;
				component3.inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnEndEditing));
				MultiToggle component4 = hierarchyReferences.GetReference<RectTransform>("LaunchRocketButton").GetComponent<MultiToggle>();
				MultiToggle component5 = hierarchyReferences.GetReference<RectTransform>("LandRocketButton").GetComponent<MultiToggle>();
				HierarchyReferences component6 = hierarchyReferences.GetReference<RectTransform>("ProgressBar").GetComponent<HierarchyReferences>();
				LaunchConditionManager launchConditionManager = CS$<>8__locals1.rocket.launchConditions;
				CommandModule component7 = launchConditionManager.GetComponent<CommandModule>();
				MinionStorage component8 = launchConditionManager.GetComponent<MinionStorage>();
				component3.SetTitle(CS$<>8__locals1.rocket.rocketName);
				component3.OnNameChanged += delegate(string newName)
				{
					CS$<>8__locals1.rocket.SetRocketName(newName);
				};
				component2.onEnter = (global::System.Action)Delegate.Combine(component2.onEnter, new global::System.Action(delegate
				{
					LaunchConditionManager launchConditions = CS$<>8__locals1.rocket.launchConditions;
					CS$<>8__locals1.<>4__this.UpdateDistanceOverlay(launchConditions);
					CS$<>8__locals1.<>4__this.UpdateMissionOverlay(launchConditions);
				}));
				component2.onExit = (global::System.Action)Delegate.Combine(component2.onExit, new global::System.Action(delegate
				{
					this.UpdateDistanceOverlay(null);
					this.UpdateMissionOverlay(null);
				}));
				component2.onClick = (global::System.Action)Delegate.Combine(component2.onClick, new global::System.Action(delegate
				{
					CS$<>8__locals1.<>4__this.OnSelectableChanged(CS$<>8__locals1.rocket.launchConditions.gameObject);
				}));
				component4.play_sound_on_click = false;
				MultiToggle multiToggle = component4;
				multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
				{
					if (launchConditionManager != null)
					{
						KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Click", false));
						CS$<>8__locals1.<>4__this.LaunchRocket(launchConditionManager);
						return;
					}
					KFMOD.PlayUISound(GlobalAssets.GetSound("Negative", false));
				}));
				if ((DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive) && SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(launchConditionManager).state != Spacecraft.MissionState.Grounded)
				{
					component5.gameObject.SetActive(true);
					component5.transform.SetAsLastSibling();
					component5.play_sound_on_click = false;
					MultiToggle multiToggle2 = component5;
					multiToggle2.onClick = (global::System.Action)Delegate.Combine(multiToggle2.onClick, new global::System.Action(delegate
					{
						if (launchConditionManager != null)
						{
							KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Click", false));
							SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(launchConditionManager).ForceComplete();
							CS$<>8__locals1.<>4__this.ClearRocketListPanel();
							CS$<>8__locals1.<>4__this.FillRocketListPanel();
							CS$<>8__locals1.<>4__this.ShowRocketListPanel();
							CS$<>8__locals1.<>4__this.Refresh(null);
							return;
						}
						KFMOD.PlayUISound(GlobalAssets.GetSound("Negative", false));
					}));
				}
				else
				{
					component5.gameObject.SetActive(false);
				}
				BreakdownListRow breakdownListRow = component.AddRow();
				string text = UI.STARMAP.MISSION_STATUS.GROUNDED;
				global::Tuple<string, BreakdownListRow.Status> textForState = StarmapScreen.GetTextForState(CS$<>8__locals1.rocket.state);
				text = textForState.first;
				BreakdownListRow.Status second = textForState.second;
				breakdownListRow.ShowStatusData(UI.STARMAP.ROCKETSTATUS.STATUS, text, second);
				breakdownListRow.SetHighlighted(true);
				if (component8 != null)
				{
					List<MinionStorage.Info> storedMinionInfo = component8.GetStoredMinionInfo();
					BreakdownListRow breakdownListRow2 = component.AddRow();
					int count = storedMinionInfo.Count;
					breakdownListRow2.ShowStatusData(UI.STARMAP.LISTTITLES.PASSENGERS, count.ToString(), (count == 0) ? BreakdownListRow.Status.Red : BreakdownListRow.Status.Green);
				}
				if (CS$<>8__locals1.rocket.state == Spacecraft.MissionState.Grounded)
				{
					string text2 = "";
					List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(launchConditionManager.GetComponent<AttachableBuilding>());
					foreach (GameObject gameObject in attachedNetwork)
					{
						text2 = text2 + gameObject.GetProperName() + "\n";
					}
					BreakdownListRow breakdownListRow3 = component.AddRow();
					breakdownListRow3.ShowData(UI.STARMAP.LISTTITLES.MODULES, attachedNetwork.Count.ToString());
					breakdownListRow3.AddTooltip(text2);
					component.AddRow().ShowData(UI.STARMAP.LISTTITLES.MAXRANGE, this.DisplayDistance(component7.rocketStats.GetRocketMaxDistance()));
					BreakdownListRow breakdownListRow4 = component.AddRow();
					breakdownListRow4.ShowData(UI.STARMAP.LISTTITLES.STORAGE, this.DisplayTotalStorageCapacity(component7));
					breakdownListRow4.AddTooltip(this.StorageCapacityTooltip(component7, this.selectedDestination));
					BreakdownListRow breakdownListRow5 = component.AddRow();
					if (this.selectedDestination != null)
					{
						if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
						{
							bool flag = this.selectedDestination.AvailableMass >= ConditionHasMinimumMass.CargoCapacity(this.selectedDestination, component7);
							breakdownListRow5.ShowStatusData(UI.STARMAP.LISTTITLES.DESTINATION_MASS, this.DisplayDestinationMass(this.selectedDestination), flag ? BreakdownListRow.Status.Default : BreakdownListRow.Status.Yellow);
							breakdownListRow5.AddTooltip(this.StorageCapacityTooltip(component7, this.selectedDestination));
						}
						else
						{
							breakdownListRow5.ShowStatusData(UI.STARMAP.LISTTITLES.DESTINATION_MASS, UI.STARMAP.COMPOSITION_UNDISCOVERED_AMOUNT, BreakdownListRow.Status.Default);
						}
					}
					else
					{
						breakdownListRow5.ShowStatusData(UI.STARMAP.DESTINATIONSELECTION.NOTSELECTED, "", BreakdownListRow.Status.Red);
						breakdownListRow5.AddTooltip(UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.NOTSELECTED);
					}
					component4.GetComponent<RectTransform>().SetAsLastSibling();
					component4.gameObject.SetActive(true);
					component6.gameObject.SetActive(false);
				}
				else
				{
					float duration = CS$<>8__locals1.rocket.GetDuration();
					float timeLeft = CS$<>8__locals1.rocket.GetTimeLeft();
					float num = ((duration == 0f) ? 0f : (1f - timeLeft / duration));
					component.AddRow().ShowData(UI.STARMAP.ROCKETSTATUS.TIMEREMAINING, global::Util.FormatOneDecimalPlace(timeLeft / 600f) + " / " + GameUtil.GetFormattedCycles(duration, "F1", false));
					component6.gameObject.SetActive(true);
					RectTransform reference = component6.GetReference<RectTransform>("ProgressImage");
					TMP_Text component9 = component6.GetReference<RectTransform>("ProgressText").GetComponent<LocText>();
					reference.transform.localScale = new Vector3(num, 1f, 1f);
					component9.text = GameUtil.GetFormattedPercent(num * 100f, GameUtil.TimeSlice.None);
					component6.GetComponent<RectTransform>().SetAsLastSibling();
					component4.gameObject.SetActive(false);
				}
				this.listRocketRows.Add(CS$<>8__locals1.rocket, hierarchyReferences);
			}
		}
		this.UpdateRocketRowsTravelAbility();
	}

	public static global::Tuple<string, BreakdownListRow.Status> GetTextForState(Spacecraft.MissionState state)
	{
		switch (state)
		{
		case Spacecraft.MissionState.Grounded:
			return new global::Tuple<string, BreakdownListRow.Status>(UI.STARMAP.MISSION_STATUS.GROUNDED, BreakdownListRow.Status.Green);
		case Spacecraft.MissionState.Launching:
			return new global::Tuple<string, BreakdownListRow.Status>(UI.STARMAP.MISSION_STATUS.LAUNCHING, BreakdownListRow.Status.Yellow);
		case Spacecraft.MissionState.Underway:
			return new global::Tuple<string, BreakdownListRow.Status>(UI.STARMAP.MISSION_STATUS.UNDERWAY, BreakdownListRow.Status.Red);
		case Spacecraft.MissionState.WaitingToLand:
			return new global::Tuple<string, BreakdownListRow.Status>(UI.STARMAP.MISSION_STATUS.WAITING_TO_LAND, BreakdownListRow.Status.Yellow);
		case Spacecraft.MissionState.Landing:
			return new global::Tuple<string, BreakdownListRow.Status>(UI.STARMAP.MISSION_STATUS.LANDING, BreakdownListRow.Status.Yellow);
		}
		return new global::Tuple<string, BreakdownListRow.Status>(UI.STARMAP.MISSION_STATUS.DESTROYED, BreakdownListRow.Status.Red);
	}

	private void ClearRocketListPanel()
	{
		this.listHeaderStatusLabel.text = UI.STARMAP.NO_ROCKETS_TITLE;
		foreach (KeyValuePair<Spacecraft, HierarchyReferences> keyValuePair in this.listRocketRows)
		{
			global::UnityEngine.Object.Destroy(keyValuePair.Value.gameObject);
		}
		this.listRocketRows.Clear();
	}

	private void FillChecklist(LaunchConditionManager launchConditionManager)
	{
		foreach (ProcessCondition processCondition in launchConditionManager.GetLaunchConditionList())
		{
			BreakdownListRow breakdownListRow = this.rocketDetailsChecklist.AddRow();
			string statusMessage = processCondition.GetStatusMessage(ProcessCondition.Status.Ready);
			ProcessCondition.Status status = processCondition.EvaluateCondition();
			BreakdownListRow.Status status2 = BreakdownListRow.Status.Green;
			if (status == ProcessCondition.Status.Failure)
			{
				status2 = BreakdownListRow.Status.Red;
			}
			else if (status == ProcessCondition.Status.Warning)
			{
				status2 = BreakdownListRow.Status.Yellow;
			}
			breakdownListRow.ShowCheckmarkData(statusMessage, "", status2);
			if (status != ProcessCondition.Status.Ready)
			{
				breakdownListRow.SetHighlighted(true);
			}
			breakdownListRow.AddTooltip(processCondition.GetStatusTooltip(status));
		}
	}

	private void SelectDestination(SpaceDestination destination)
	{
		this.selectedDestination = destination;
		this.UnselectAllPlanets();
		if (this.selectedDestination != null)
		{
			this.SelectPlanet(this.planetWidgets[this.selectedDestination]);
			if (this.currentLaunchConditionManager != null)
			{
				SpacecraftManager.instance.SetSpacecraftDestination(this.currentLaunchConditionManager, this.selectedDestination);
			}
			this.ShowDestinationPanel();
			this.UpdateRocketRowsTravelAbility();
		}
		else
		{
			this.ClearDestinationPanel();
		}
		if (this.rangeRowTotal != null && this.selectedDestination != null && this.currentCommandModule != null)
		{
			this.rangeRowTotal.SetStatusColor(this.currentCommandModule.conditions.reachable.CanReachSpacecraftDestination(this.selectedDestination) ? BreakdownListRow.Status.Green : BreakdownListRow.Status.Red);
		}
		this.UpdateDestinationStates();
		this.Refresh(null);
	}

	private void UpdateRocketRowsTravelAbility()
	{
		foreach (KeyValuePair<Spacecraft, HierarchyReferences> keyValuePair in this.listRocketRows)
		{
			Spacecraft key = keyValuePair.Key;
			LaunchConditionManager launchConditions = key.launchConditions;
			CommandModule component = launchConditions.GetComponent<CommandModule>();
			MultiToggle component2 = keyValuePair.Value.GetReference<RectTransform>("LaunchRocketButton").GetComponent<MultiToggle>();
			bool flag = key.state == Spacecraft.MissionState.Grounded;
			SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(launchConditions);
			bool flag2 = spacecraftDestination != null && component.conditions.reachable.CanReachSpacecraftDestination(spacecraftDestination);
			bool flag3 = launchConditions.CheckReadyToLaunch();
			component2.ChangeState((flag && flag2 && flag3) ? 0 : 1);
		}
	}

	private void RefreshAnalyzeButton()
	{
		if (this.selectedDestination == null)
		{
			this.analyzeButton.ChangeState(1);
			this.analyzeButton.onClick = null;
			this.analyzeButton.GetComponentInChildren<LocText>().text = UI.STARMAP.NO_ANALYZABLE_DESTINATION_SELECTED;
			return;
		}
		if (this.selectedDestination.AnalysisState() == SpacecraftManager.DestinationAnalysisState.Complete)
		{
			if (DebugHandler.InstantBuildMode)
			{
				this.analyzeButton.ChangeState(0);
				this.analyzeButton.onClick = delegate
				{
					this.selectedDestination.TryCompleteResearchOpportunity();
					this.ShowDestinationPanel();
				};
				this.analyzeButton.GetComponentInChildren<LocText>().text = UI.STARMAP.ANALYSIS_COMPLETE + " (debug research)";
				return;
			}
			this.analyzeButton.ChangeState(1);
			this.analyzeButton.onClick = null;
			this.analyzeButton.GetComponentInChildren<LocText>().text = UI.STARMAP.ANALYSIS_COMPLETE;
			return;
		}
		else
		{
			this.analyzeButton.ChangeState(0);
			if (this.selectedDestination.id == SpacecraftManager.instance.GetStarmapAnalysisDestinationID())
			{
				this.analyzeButton.GetComponentInChildren<LocText>().text = UI.STARMAP.SUSPEND_DESTINATION_ANALYSIS;
				this.analyzeButton.onClick = delegate
				{
					SpacecraftManager.instance.SetStarmapAnalysisDestinationID(-1);
				};
				return;
			}
			this.analyzeButton.GetComponentInChildren<LocText>().text = UI.STARMAP.ANALYZE_DESTINATION;
			this.analyzeButton.onClick = delegate
			{
				if (DebugHandler.InstantBuildMode)
				{
					SpacecraftManager.instance.SetStarmapAnalysisDestinationID(this.selectedDestination.id);
					SpacecraftManager.instance.EarnDestinationAnalysisPoints(this.selectedDestination.id, 99999f);
					this.ShowDestinationPanel();
					return;
				}
				SpacecraftManager.instance.SetStarmapAnalysisDestinationID(this.selectedDestination.id);
			};
			return;
		}
	}

	private void Refresh(object data = null)
	{
		this.FillRocketListPanel();
		this.RefreshAnalyzeButton();
		this.ShowDestinationPanel();
		if (this.currentCommandModule != null && this.currentLaunchConditionManager != null)
		{
			this.FillRocketPanel();
			if (this.selectedDestination != null)
			{
				this.ValidateTravelAbility();
				return;
			}
		}
		else
		{
			this.ClearRocketPanel();
		}
	}

	private void ClearRocketPanel()
	{
		this.rocketHeaderStatusLabel.text = UI.STARMAP.ROCKETSTATUS.NONE;
		this.rocketDetailsChecklist.ClearRows();
		this.rocketDetailsMass.ClearRows();
		this.rocketDetailsRange.ClearRows();
		this.rocketThrustWidget.gameObject.SetActive(false);
		this.rocketDetailsStorage.ClearRows();
		this.rocketDetailsFuel.ClearRows();
		this.rocketDetailsOxidizer.ClearRows();
		this.rocketDetailsDupes.ClearRows();
		this.rocketDetailsStatus.ClearRows();
		this.currentRocketHasLiquidContainer = false;
		this.currentRocketHasGasContainer = false;
		this.currentRocketHasSolidContainer = false;
		this.currentRocketHasEntitiesContainer = false;
		LayoutRebuilder.ForceRebuildLayoutImmediate(this.rocketDetailsContainer);
	}

	private void FillRocketPanel()
	{
		this.ClearRocketPanel();
		this.rocketHeaderStatusLabel.text = UI.STARMAP.STATUS;
		this.UpdateDistanceOverlay(null);
		this.UpdateMissionOverlay(null);
		this.FillChecklist(this.currentLaunchConditionManager);
		this.UpdateRangeDisplay();
		this.UpdateMassDisplay();
		this.UpdateOxidizerDisplay();
		this.UpdateStorageDisplay();
		this.UpdateFuelDisplay();
		LayoutRebuilder.ForceRebuildLayoutImmediate(this.rocketDetailsContainer);
	}

	private void UpdateRangeDisplay()
	{
		this.rocketDetailsRange.AddRow().ShowData(UI.STARMAP.ROCKETSTATS.TOTAL_OXIDIZABLE_FUEL, GameUtil.GetFormattedMass(this.currentCommandModule.rocketStats.GetTotalOxidizableFuel(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
		this.rocketDetailsRange.AddRow().ShowData(UI.STARMAP.ROCKETSTATS.ENGINE_EFFICIENCY, GameUtil.GetFormattedEngineEfficiency(this.currentCommandModule.rocketStats.GetEngineEfficiency()));
		this.rocketDetailsRange.AddRow().ShowData(UI.STARMAP.ROCKETSTATS.OXIDIZER_EFFICIENCY, GameUtil.GetFormattedPercent(this.currentCommandModule.rocketStats.GetAverageOxidizerEfficiency(), GameUtil.TimeSlice.None));
		float num = this.currentCommandModule.rocketStats.GetBoosterThrust() * 1000f;
		if (num != 0f)
		{
			this.rocketDetailsRange.AddRow().ShowData(UI.STARMAP.ROCKETSTATS.SOLID_BOOSTER, GameUtil.GetFormattedDistance(num));
		}
		BreakdownListRow breakdownListRow = this.rocketDetailsRange.AddRow();
		breakdownListRow.ShowStatusData(UI.STARMAP.ROCKETSTATS.TOTAL_THRUST, GameUtil.GetFormattedDistance(this.currentCommandModule.rocketStats.GetTotalThrust() * 1000f), BreakdownListRow.Status.Green);
		breakdownListRow.SetImportant(true);
		float num2 = -(this.currentCommandModule.rocketStats.GetTotalThrust() - this.currentCommandModule.rocketStats.GetRocketMaxDistance());
		this.rocketThrustWidget.gameObject.SetActive(true);
		BreakdownListRow breakdownListRow2 = this.rocketDetailsRange.AddRow();
		breakdownListRow2.ShowStatusData(UI.STARMAP.ROCKETSTATUS.WEIGHTPENALTY, this.DisplayDistance(num2), BreakdownListRow.Status.Red);
		breakdownListRow2.SetHighlighted(true);
		this.rocketDetailsRange.AddCustomRow(this.rocketThrustWidget.gameObject);
		this.rocketThrustWidget.Draw(this.currentCommandModule);
		BreakdownListRow breakdownListRow3 = this.rocketDetailsRange.AddRow();
		breakdownListRow3.ShowData(UI.STARMAP.ROCKETSTATS.TOTAL_RANGE, GameUtil.GetFormattedDistance(this.currentCommandModule.rocketStats.GetRocketMaxDistance() * 1000f));
		breakdownListRow3.SetImportant(true);
	}

	private void UpdateMassDisplay()
	{
		this.rocketDetailsMass.AddRow().ShowData(UI.STARMAP.ROCKETSTATS.DRY_MASS, GameUtil.GetFormattedMass(this.currentCommandModule.rocketStats.GetDryMass(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		this.rocketDetailsMass.AddRow().ShowData(UI.STARMAP.ROCKETSTATS.WET_MASS, GameUtil.GetFormattedMass(this.currentCommandModule.rocketStats.GetWetMass(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		BreakdownListRow breakdownListRow = this.rocketDetailsMass.AddRow();
		breakdownListRow.ShowData(UI.STARMAP.ROCKETSTATUS.TOTAL, GameUtil.GetFormattedMass(this.currentCommandModule.rocketStats.GetTotalMass(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		breakdownListRow.SetImportant(true);
	}

	private void UpdateFuelDisplay()
	{
		Tag engineFuelTag = this.currentCommandModule.rocketStats.GetEngineFuelTag();
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.currentCommandModule.GetComponent<AttachableBuilding>()))
		{
			IFuelTank component = gameObject.GetComponent<IFuelTank>();
			if (!component.IsNullOrDestroyed())
			{
				BreakdownListRow breakdownListRow = this.rocketDetailsFuel.AddRow();
				if (engineFuelTag.IsValid)
				{
					Element element = ElementLoader.GetElement(engineFuelTag);
					global::Debug.Assert(element != null, "fuel_element");
					breakdownListRow.ShowData(gameObject.gameObject.GetProperName() + " (" + element.name + ")", GameUtil.GetFormattedMass(component.Storage.GetAmountAvailable(engineFuelTag), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
				}
				else
				{
					breakdownListRow.ShowData(gameObject.gameObject.GetProperName(), UI.STARMAP.ROCKETSTATS.NO_ENGINE);
					breakdownListRow.SetStatusColor(BreakdownListRow.Status.Red);
				}
			}
			SolidBooster component2 = gameObject.GetComponent<SolidBooster>();
			if (component2 != null)
			{
				BreakdownListRow breakdownListRow2 = this.rocketDetailsFuel.AddRow();
				Element element2 = ElementLoader.GetElement(component2.fuelTag);
				global::Debug.Assert(element2 != null, "fuel_element");
				breakdownListRow2.ShowData(gameObject.gameObject.GetProperName() + " (" + element2.name + ")", GameUtil.GetFormattedMass(component2.fuelStorage.GetMassAvailable(component2.fuelTag), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
			}
		}
		BreakdownListRow breakdownListRow3 = this.rocketDetailsFuel.AddRow();
		breakdownListRow3.ShowData(UI.STARMAP.ROCKETSTATS.TOTAL_FUEL, GameUtil.GetFormattedMass(this.currentCommandModule.rocketStats.GetTotalFuel(true), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		breakdownListRow3.SetImportant(true);
	}

	private void UpdateOxidizerDisplay()
	{
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.currentCommandModule.GetComponent<AttachableBuilding>()))
		{
			OxidizerTank component = gameObject.GetComponent<OxidizerTank>();
			if (component != null)
			{
				foreach (KeyValuePair<Tag, float> keyValuePair in component.GetOxidizersAvailable())
				{
					if (keyValuePair.Value != 0f)
					{
						this.rocketDetailsOxidizer.AddRow().ShowData(gameObject.gameObject.GetProperName() + " (" + keyValuePair.Key.ProperName() + ")", GameUtil.GetFormattedMass(keyValuePair.Value, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
					}
				}
			}
			SolidBooster component2 = gameObject.GetComponent<SolidBooster>();
			if (component2 != null)
			{
				this.rocketDetailsOxidizer.AddRow().ShowData(gameObject.gameObject.GetProperName() + " (" + ElementLoader.FindElementByHash(SimHashes.OxyRock).name + ")", GameUtil.GetFormattedMass(component2.fuelStorage.GetMassAvailable(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
			}
		}
		BreakdownListRow breakdownListRow = this.rocketDetailsOxidizer.AddRow();
		breakdownListRow.ShowData(UI.STARMAP.ROCKETSTATS.TOTAL_OXIDIZER, GameUtil.GetFormattedMass(this.currentCommandModule.rocketStats.GetTotalOxidizer(true), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		breakdownListRow.SetImportant(true);
	}

	private void UpdateStorageDisplay()
	{
		float num = ((this.selectedDestination != null) ? this.selectedDestination.AvailableMass : 0f);
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.currentCommandModule.GetComponent<AttachableBuilding>()))
		{
			CargoBay component = gameObject.GetComponent<CargoBay>();
			if (component != null)
			{
				BreakdownListRow breakdownListRow = this.rocketDetailsStorage.AddRow();
				if (this.selectedDestination != null)
				{
					float availableResourcesPercentage = this.selectedDestination.GetAvailableResourcesPercentage(component.storageType);
					float num2 = Mathf.Min(component.storage.Capacity(), availableResourcesPercentage * num);
					num -= num2;
					breakdownListRow.ShowData(gameObject.gameObject.GetProperName(), string.Format(UI.STARMAP.STORAGESTATS.STORAGECAPACITY, GameUtil.GetFormattedMass(Mathf.Min(num2, component.storage.Capacity()), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"), GameUtil.GetFormattedMass(component.storage.Capacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}")));
				}
				else
				{
					breakdownListRow.ShowData(gameObject.gameObject.GetProperName(), string.Format(UI.STARMAP.STORAGESTATS.STORAGECAPACITY, GameUtil.GetFormattedMass(0f, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"), GameUtil.GetFormattedMass(component.storage.Capacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}")));
				}
			}
		}
	}

	private void ClearDestinationPanel()
	{
		this.destinationDetailsContainer.gameObject.SetActive(false);
		this.destinationStatusLabel.text = UI.STARMAP.ROCKETSTATUS.NONE;
	}

	private void ShowDestinationPanel()
	{
		if (this.selectedDestination == null)
		{
			return;
		}
		this.destinationStatusLabel.text = UI.STARMAP.ROCKETSTATUS.SELECTED;
		if (this.currentLaunchConditionManager != null && SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this.currentLaunchConditionManager).state != Spacecraft.MissionState.Grounded)
		{
			this.destinationStatusLabel.text = UI.STARMAP.ROCKETSTATUS.LOCKEDIN;
		}
		SpaceDestinationType destinationType = this.selectedDestination.GetDestinationType();
		this.destinationNameLabel.text = ((SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete) ? destinationType.Name : UI.STARMAP.UNKNOWN_DESTINATION.text);
		this.destinationTypeValueLabel.text = ((SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete) ? destinationType.typeName : UI.STARMAP.UNKNOWN_TYPE.text);
		this.destinationDistanceValueLabel.text = this.DisplayDistance((float)this.selectedDestination.OneBasedDistance * 10000f);
		this.destinationDescriptionLabel.text = destinationType.description;
		this.destinationDetailsComposition.ClearRows();
		this.destinationDetailsResearch.ClearRows();
		this.destinationDetailsMass.ClearRows();
		this.destinationDetailsResources.ClearRows();
		this.destinationDetailsArtifacts.ClearRows();
		if (destinationType.visitable)
		{
			float num = 0f;
			if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
			{
				num = this.selectedDestination.GetTotalMass();
			}
			if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
			{
				foreach (SpaceDestination.ResearchOpportunity researchOpportunity in this.selectedDestination.researchOpportunities)
				{
					BreakdownListRow breakdownListRow = this.destinationDetailsResearch.AddRow();
					string text = ((researchOpportunity.discoveredRareResource != SimHashes.Void) ? string.Format("(!!) {0}", researchOpportunity.description) : researchOpportunity.description);
					breakdownListRow.ShowCheckmarkData(text, researchOpportunity.dataValue.ToString(), researchOpportunity.completed ? BreakdownListRow.Status.Green : BreakdownListRow.Status.Default);
				}
			}
			this.destinationAnalysisProgressBar.SetFillPercentage(SpacecraftManager.instance.GetDestinationAnalysisScore(this.selectedDestination.id) / (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE);
			float num2 = ConditionHasMinimumMass.CargoCapacity(this.selectedDestination, this.currentCommandModule);
			if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
			{
				string formattedMass = GameUtil.GetFormattedMass(this.selectedDestination.CurrentMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}");
				string formattedMass2 = GameUtil.GetFormattedMass((float)destinationType.minimumMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}");
				BreakdownListRow breakdownListRow2 = this.destinationDetailsMass.AddRow();
				breakdownListRow2.ShowData(UI.STARMAP.CURRENT_MASS, formattedMass);
				if (this.selectedDestination.AvailableMass < num2)
				{
					breakdownListRow2.SetStatusColor(BreakdownListRow.Status.Yellow);
					breakdownListRow2.AddTooltip(string.Format(UI.STARMAP.CURRENT_MASS_TOOLTIP, GameUtil.GetFormattedMass(this.selectedDestination.AvailableMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"), GameUtil.GetFormattedMass(num2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}")));
				}
				this.destinationDetailsMass.AddRow().ShowData(UI.STARMAP.MAXIMUM_MASS, GameUtil.GetFormattedMass((float)destinationType.maxiumMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
				BreakdownListRow breakdownListRow3 = this.destinationDetailsMass.AddRow();
				breakdownListRow3.ShowData(UI.STARMAP.MINIMUM_MASS, formattedMass2);
				breakdownListRow3.AddTooltip(UI.STARMAP.MINIMUM_MASS_TOOLTIP);
				BreakdownListRow breakdownListRow4 = this.destinationDetailsMass.AddRow();
				breakdownListRow4.ShowData(UI.STARMAP.REPLENISH_RATE, GameUtil.GetFormattedMass(destinationType.replishmentPerCycle, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"));
				breakdownListRow4.AddTooltip(UI.STARMAP.REPLENISH_RATE_TOOLTIP);
			}
			if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
			{
				foreach (KeyValuePair<SimHashes, float> keyValuePair in this.selectedDestination.recoverableElements)
				{
					BreakdownListRow breakdownListRow5 = this.destinationDetailsComposition.AddRow();
					float num3 = this.selectedDestination.GetResourceValue(keyValuePair.Key, keyValuePair.Value) / num * 100f;
					Element element = ElementLoader.FindElementByHash(keyValuePair.Key);
					global::Tuple<Sprite, Color> uisprite = Def.GetUISprite(element, "ui", false);
					if (num3 <= 1f)
					{
						breakdownListRow5.ShowIconData(element.name, UI.STARMAP.COMPOSITION_SMALL_AMOUNT, uisprite.first, uisprite.second);
					}
					else
					{
						breakdownListRow5.ShowIconData(element.name, GameUtil.GetFormattedPercent(num3, GameUtil.TimeSlice.None), uisprite.first, uisprite.second);
					}
					if (element.IsGas)
					{
						string properName = Assets.GetPrefab("GasCargoBay".ToTag()).GetProperName();
						if (this.currentRocketHasGasContainer)
						{
							breakdownListRow5.SetHighlighted(true);
							breakdownListRow5.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, element.name, properName));
						}
						else
						{
							breakdownListRow5.SetDisabled(true);
							breakdownListRow5.AddTooltip(string.Format(UI.STARMAP.CONTAINER_REQUIRED, properName));
						}
					}
					if (element.IsLiquid)
					{
						string properName2 = Assets.GetPrefab("LiquidCargoBay".ToTag()).GetProperName();
						if (this.currentRocketHasLiquidContainer)
						{
							breakdownListRow5.SetHighlighted(true);
							breakdownListRow5.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, element.name, properName2));
						}
						else
						{
							breakdownListRow5.SetDisabled(true);
							breakdownListRow5.AddTooltip(string.Format(UI.STARMAP.CONTAINER_REQUIRED, properName2));
						}
					}
					if (element.IsSolid)
					{
						string properName3 = Assets.GetPrefab("CargoBay".ToTag()).GetProperName();
						if (this.currentRocketHasSolidContainer)
						{
							breakdownListRow5.SetHighlighted(true);
							breakdownListRow5.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, element.name, properName3));
						}
						else
						{
							breakdownListRow5.SetDisabled(true);
							breakdownListRow5.AddTooltip(string.Format(UI.STARMAP.CONTAINER_REQUIRED, properName3));
						}
					}
				}
				foreach (SpaceDestination.ResearchOpportunity researchOpportunity2 in this.selectedDestination.researchOpportunities)
				{
					if (!researchOpportunity2.completed && researchOpportunity2.discoveredRareResource != SimHashes.Void)
					{
						BreakdownListRow breakdownListRow6 = this.destinationDetailsComposition.AddRow();
						breakdownListRow6.ShowData(UI.STARMAP.COMPOSITION_UNDISCOVERED, UI.STARMAP.COMPOSITION_UNDISCOVERED_AMOUNT);
						breakdownListRow6.SetDisabled(true);
						breakdownListRow6.AddTooltip(UI.STARMAP.COMPOSITION_UNDISCOVERED_TOOLTIP);
					}
				}
			}
			if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
			{
				foreach (KeyValuePair<Tag, int> keyValuePair2 in this.selectedDestination.GetRecoverableEntities())
				{
					BreakdownListRow breakdownListRow7 = this.destinationDetailsResources.AddRow();
					GameObject prefab = Assets.GetPrefab(keyValuePair2.Key);
					global::Tuple<Sprite, Color> uisprite2 = Def.GetUISprite(prefab, "ui", false);
					breakdownListRow7.ShowIconData(prefab.GetProperName(), "", uisprite2.first, uisprite2.second);
					string properName4 = Assets.GetPrefab("SpecialCargoBay".ToTag()).GetProperName();
					if (this.currentRocketHasEntitiesContainer)
					{
						breakdownListRow7.SetHighlighted(true);
						breakdownListRow7.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, prefab.GetProperName(), properName4));
					}
					else
					{
						breakdownListRow7.SetDisabled(true);
						breakdownListRow7.AddTooltip(string.Format(UI.STARMAP.CANT_CARRY_ELEMENT, properName4, prefab.GetProperName()));
					}
				}
			}
			if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
			{
				ArtifactDropRate artifactDropTable = this.selectedDestination.GetDestinationType().artifactDropTable;
				foreach (global::Tuple<ArtifactTier, float> tuple in artifactDropTable.rates)
				{
					this.destinationDetailsArtifacts.AddRow().ShowData(Strings.Get(tuple.first.name_key), GameUtil.GetFormattedPercent(tuple.second / artifactDropTable.totalWeight * 100f, GameUtil.TimeSlice.None));
				}
			}
			this.destinationDetailsContainer.gameObject.SetActive(true);
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(this.destinationDetailsContainer);
	}

	private void ValidateTravelAbility()
	{
		if (this.selectedDestination != null && SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete && this.currentCommandModule != null && this.currentLaunchConditionManager != null)
		{
			this.launchButton.ChangeState(this.currentLaunchConditionManager.CheckReadyToLaunch() ? 0 : 1);
		}
	}

	private void UpdateDistanceOverlay(LaunchConditionManager lcmToVisualize = null)
	{
		if (lcmToVisualize == null)
		{
			lcmToVisualize = this.currentLaunchConditionManager;
		}
		Spacecraft spacecraft = null;
		if (lcmToVisualize != null)
		{
			spacecraft = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(lcmToVisualize);
		}
		if (lcmToVisualize != null && spacecraft != null && spacecraft.state == Spacecraft.MissionState.Grounded)
		{
			this.distanceOverlay.gameObject.SetActive(true);
			float num = lcmToVisualize.GetComponent<CommandModule>().rocketStats.GetRocketMaxDistance();
			num = (float)((int)(num / 10000f)) * 10000f;
			Vector2 sizeDelta = this.distanceOverlay.rectTransform.sizeDelta;
			sizeDelta.x = this.rowsContiner.rect.width;
			sizeDelta.y = (1f - num / this.planetsMaxDistance) * this.rowsContiner.rect.height + (float)this.distanceOverlayYOffset + (float)this.distanceOverlayVerticalOffset;
			this.distanceOverlay.rectTransform.sizeDelta = sizeDelta;
			this.distanceOverlay.rectTransform.anchoredPosition = new Vector3(0f, (float)this.distanceOverlayVerticalOffset, 0f);
			return;
		}
		this.distanceOverlay.gameObject.SetActive(false);
	}

	private void UpdateMissionOverlay(LaunchConditionManager lcmToVisualize = null)
	{
		if (lcmToVisualize == null)
		{
			lcmToVisualize = this.currentLaunchConditionManager;
		}
		Spacecraft spacecraft = null;
		if (lcmToVisualize != null)
		{
			spacecraft = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(lcmToVisualize);
		}
		if (lcmToVisualize != null && spacecraft != null)
		{
			SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(lcmToVisualize);
			if (spacecraftDestination == null)
			{
				global::Debug.Log("destination is null");
				return;
			}
			StarmapPlanet starmapPlanet = this.planetWidgets[spacecraftDestination];
			if (spacecraft == null)
			{
				global::Debug.Log("craft is null");
				return;
			}
			if (starmapPlanet == null)
			{
				global::Debug.Log("planet is null");
				return;
			}
			this.UnselectAllPlanets();
			this.SelectPlanet(starmapPlanet);
			starmapPlanet.ShowAsCurrentRocketDestination(spacecraftDestination.GetDestinationType().visitable);
			if (spacecraft.state != Spacecraft.MissionState.Grounded && spacecraftDestination.GetDestinationType().visitable)
			{
				this.visualizeRocketImage.gameObject.SetActive(true);
				this.visualizeRocketTrajectory.gameObject.SetActive(true);
				this.visualizeRocketLabel.gameObject.SetActive(true);
				this.visualizeRocketProgress.gameObject.SetActive(true);
				float duration = spacecraft.GetDuration();
				float timeLeft = spacecraft.GetTimeLeft();
				float num = ((duration == 0f) ? 0f : (1f - timeLeft / duration));
				bool flag = num > 0.5f;
				Vector2 vector = new Vector2(0f, -this.rowsContiner.rect.size.y);
				Vector3 vector2 = starmapPlanet.rectTransform().localPosition + new Vector3(starmapPlanet.rectTransform().sizeDelta.x * 0.5f, 0f, 0f);
				vector2 = starmapPlanet.transform.parent.rectTransform().localPosition + vector2;
				Vector2 vector3 = new Vector2(vector2.x, vector2.y);
				float num2 = Vector2.Distance(vector, vector3);
				Vector2 vector4 = vector3 - vector;
				float num3 = Mathf.Atan2(vector4.y, vector4.x) * 57.29578f;
				Vector2 vector5;
				if (flag)
				{
					vector5 = new Vector2(Mathf.Lerp(vector.x, vector3.x, 1f - num * 2f + 1f), Mathf.Lerp(vector.y, vector3.y, 1f - num * 2f + 1f));
				}
				else
				{
					vector5 = new Vector2(Mathf.Lerp(vector.x, vector3.x, num * 2f), Mathf.Lerp(vector.y, vector3.y, num * 2f));
				}
				this.visualizeRocketLabel.text = StarmapScreen.GetTextForState(spacecraft.state).first;
				this.visualizeRocketProgress.text = GameUtil.GetFormattedPercent(num * 100f, GameUtil.TimeSlice.None);
				this.visualizeRocketTrajectory.transform.SetLocalPosition(vector);
				this.visualizeRocketTrajectory.rectTransform.sizeDelta = new Vector2(num2, this.visualizeRocketTrajectory.rectTransform.sizeDelta.y);
				this.visualizeRocketTrajectory.rectTransform.localRotation = Quaternion.Euler(0f, 0f, num3);
				this.visualizeRocketImage.transform.SetLocalPosition(vector5);
				return;
			}
		}
		else
		{
			if (this.selectedDestination != null && this.planetWidgets.ContainsKey(this.selectedDestination))
			{
				this.UnselectAllPlanets();
				StarmapPlanet starmapPlanet2 = this.planetWidgets[this.selectedDestination];
				this.SelectPlanet(starmapPlanet2);
			}
			else
			{
				this.UnselectAllPlanets();
			}
			this.visualizeRocketImage.gameObject.SetActive(false);
			this.visualizeRocketTrajectory.gameObject.SetActive(false);
			this.visualizeRocketLabel.gameObject.SetActive(false);
			this.visualizeRocketProgress.gameObject.SetActive(false);
		}
	}

	public GameObject listPanel;

	public GameObject rocketPanel;

	public LocText listHeaderLabel;

	public LocText listHeaderStatusLabel;

	public HierarchyReferences listRocketTemplate;

	public LocText listNoRocketText;

	public RectTransform rocketListContainer;

	private Dictionary<Spacecraft, HierarchyReferences> listRocketRows = new Dictionary<Spacecraft, HierarchyReferences>();

	[Header("Shared References")]
	public BreakdownList breakdownListPrefab;

	public GameObject progressBarPrefab;

	[Header("Selected Rocket References")]
	public LocText rocketHeaderLabel;

	public LocText rocketHeaderStatusLabel;

	private BreakdownList rocketDetailsStatus;

	public Sprite rocketDetailsStatusIcon;

	private BreakdownList rocketDetailsChecklist;

	public Sprite rocketDetailsChecklistIcon;

	private BreakdownList rocketDetailsMass;

	public Sprite rocketDetailsMassIcon;

	private BreakdownList rocketDetailsRange;

	public Sprite rocketDetailsRangeIcon;

	public RocketThrustWidget rocketThrustWidget;

	private BreakdownList rocketDetailsStorage;

	public Sprite rocketDetailsStorageIcon;

	private BreakdownList rocketDetailsDupes;

	public Sprite rocketDetailsDupesIcon;

	private BreakdownList rocketDetailsFuel;

	public Sprite rocketDetailsFuelIcon;

	private BreakdownList rocketDetailsOxidizer;

	public Sprite rocketDetailsOxidizerIcon;

	public RectTransform rocketDetailsContainer;

	[Header("Selected Destination References")]
	public LocText destinationHeaderLabel;

	public LocText destinationStatusLabel;

	public LocText destinationNameLabel;

	public LocText destinationTypeNameLabel;

	public LocText destinationTypeValueLabel;

	public LocText destinationDistanceNameLabel;

	public LocText destinationDistanceValueLabel;

	public LocText destinationDescriptionLabel;

	private BreakdownList destinationDetailsAnalysis;

	private GenericUIProgressBar destinationAnalysisProgressBar;

	public Sprite destinationDetailsAnalysisIcon;

	private BreakdownList destinationDetailsResearch;

	public Sprite destinationDetailsResearchIcon;

	private BreakdownList destinationDetailsMass;

	public Sprite destinationDetailsMassIcon;

	private BreakdownList destinationDetailsComposition;

	public Sprite destinationDetailsCompositionIcon;

	private BreakdownList destinationDetailsResources;

	public Sprite destinationDetailsResourcesIcon;

	private BreakdownList destinationDetailsArtifacts;

	public Sprite destinationDetailsArtifactsIcon;

	public RectTransform destinationDetailsContainer;

	public MultiToggle showRocketsButton;

	public MultiToggle launchButton;

	public MultiToggle analyzeButton;

	private int rocketConditionEventHandler = -1;

	[Header("Map References")]
	public RectTransform Map;

	public RectTransform rowsContiner;

	public GameObject rowPrefab;

	public StarmapPlanet planetPrefab;

	public GameObject rocketIconPrefab;

	private List<GameObject> planetRows = new List<GameObject>();

	private Dictionary<SpaceDestination, StarmapPlanet> planetWidgets = new Dictionary<SpaceDestination, StarmapPlanet>();

	private float planetsMaxDistance = 1f;

	public Image distanceOverlay;

	private int distanceOverlayVerticalOffset = 500;

	private int distanceOverlayYOffset = 24;

	public Image visualizeRocketImage;

	public Image visualizeRocketTrajectory;

	public LocText visualizeRocketLabel;

	public LocText visualizeRocketProgress;

	public Color[] distanceColors;

	public LocText titleBarLabel;

	public KButton button;

	private const int DESTINATION_ICON_SCALE = 2;

	public static StarmapScreen Instance;

	private int selectionUpdateHandle = -1;

	private SpaceDestination selectedDestination;

	private KSelectable currentSelectable;

	private CommandModule currentCommandModule;

	private LaunchConditionManager currentLaunchConditionManager;

	private bool currentRocketHasGasContainer;

	private bool currentRocketHasLiquidContainer;

	private bool currentRocketHasSolidContainer;

	private bool currentRocketHasEntitiesContainer;

	private bool forceScrollDown = true;

	private Coroutine animateAnalysisRoutine;

	private Coroutine animateSelectedPlanetRoutine;

	private BreakdownListRow rangeRowTotal;
}
