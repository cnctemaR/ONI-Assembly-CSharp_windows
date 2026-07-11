using System;
using System.Collections;
using System.Collections.Generic;
using Database;
using FMOD.Studio;
using STRINGS;
using TUNING;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StarmapScreen : KModalScreen
{
	public static void DestroyInstance()
	{
		StarmapScreen.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.ConsumeMouseScroll = true;
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
		this.destinationDetailsComposition = global::UnityEngine.Object.Instantiate<BreakdownList>(this.breakdownListPrefab, this.destinationDetailsContainer);
		this.destinationDetailsComposition.SetTitle(UI.STARMAP.LISTTITLES.WORLDCOMPOSITION);
		this.destinationDetailsComposition.SetIcon(this.destinationDetailsCompositionIcon);
		this.destinationDetailsComposition.gameObject.name = "destinationDetailsComposition";
		this.destinationDetailsResources = global::UnityEngine.Object.Instantiate<BreakdownList>(this.breakdownListPrefab, this.destinationDetailsContainer);
		this.destinationDetailsResources.SetTitle(UI.STARMAP.LISTTITLES.RESOURCES);
		this.destinationDetailsResources.SetIcon(this.destinationDetailsResourcesIcon);
		this.destinationDetailsResources.gameObject.name = "destinationDetailsResources";
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
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
				KFMOD.PlayOneShot(GlobalAssets.GetSound("HUD_Click", false));
				this.currentLaunchConditionManager.Launch(this.selectedDestination);
				this.ClearRocketListPanel();
				this.FillRocketListPanel();
				this.ShowRocketListPanel();
				this.Refresh(null);
			}
			else
			{
				KFMOD.PlayOneShot(GlobalAssets.GetSound("Negative", false));
			}
		}));
		this.launchButton.ChangeState(1);
		MultiToggle multiToggle2 = this.showRocketsButton;
		multiToggle2.onClick = (global::System.Action)Delegate.Combine(multiToggle2.onClick, new global::System.Action(delegate
		{
			this.ShowRocketListPanel();
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
		this.OnSelectableChanged((!(SelectTool.Instance.selected == null)) ? SelectTool.Instance.selected.gameObject : null);
		if (show)
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().MENUStarmapSnapshot);
			MusicManager.instance.PlaySong("Music_Starmap", false);
			this.SelectDestination(this.selectedDestination);
			this.UpdateDestinationStates();
			this.Refresh(null);
		}
		else
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUStarmapSnapshot, STOP_MODE.ALLOWFADEOUT);
			MusicManager.instance.StopSong("Music_Starmap", true, STOP_MODE.ALLOWFADEOUT);
		}
		this.forceScrollDown = true;
	}

	private void UpdateDestinationStates()
	{
		int starmapAnalysisDestinationID = SpacecraftManager.instance.GetStarmapAnalysisDestinationID();
		SpaceDestination spaceDestination = ((starmapAnalysisDestinationID != -1) ? SpacecraftManager.instance.GetDestination(SpacecraftManager.instance.GetStarmapAnalysisDestinationID()) : null);
		int num = 0;
		int num2 = 0;
		int num3 = 1;
		foreach (SpaceDestination spaceDestination2 in SpacecraftManager.instance.destinations)
		{
			num = Mathf.Max(num, spaceDestination2.OneBasedDistance);
			if (spaceDestination2.AnalysisState() == SpacecraftManager.DestinationAnalysisState.Complete)
			{
				num2 = Mathf.Max(num2, spaceDestination2.OneBasedDistance);
			}
		}
		for (int i = num2; i < num; i++)
		{
			bool flag = false;
			foreach (SpaceDestination spaceDestination3 in SpacecraftManager.instance.destinations)
			{
				if (spaceDestination3.distance == i)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
			num3++;
		}
		using (Dictionary<SpaceDestination, GameObject>.Enumerator enumerator3 = this.planetWidgets.GetEnumerator())
		{
			while (enumerator3.MoveNext())
			{
				StarmapScreen.<UpdateDestinationStates>c__AnonStorey1 <UpdateDestinationStates>c__AnonStorey = new StarmapScreen.<UpdateDestinationStates>c__AnonStorey1();
				<UpdateDestinationStates>c__AnonStorey.KVP = enumerator3.Current;
				<UpdateDestinationStates>c__AnonStorey.$this = this;
				HierarchyReferences component = <UpdateDestinationStates>c__AnonStorey.KVP.Value.GetComponent<HierarchyReferences>();
				MultiToggle component2 = <UpdateDestinationStates>c__AnonStorey.KVP.Value.GetComponent<MultiToggle>();
				Color color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
				Color color2 = new Color(0.75f, 0.75f, 0.75f, 0.75f);
				if (<UpdateDestinationStates>c__AnonStorey.KVP.Key.distance >= num2 + num3)
				{
					SpaceDestinationType spaceDestinationType = <UpdateDestinationStates>c__AnonStorey.KVP.Key.GetDestinationType();
					Image image = component.GetReference<RectTransform>("Planet").GetComponent<Image>();
					image.sprite = Assets.GetSprite("unknown");
					image.color = color;
					Image image2 = component.GetReference<RectTransform>("Doppelganger").GetComponent<Image>();
					image2.sprite = Assets.GetSprite("unknown");
					image2.color = color;
					component.GetReference<Image>("UnknownIcon").color = color;
				}
				else
				{
					component.GetReference<Image>("UnknownIcon").color = color2;
					<UpdateDestinationStates>c__AnonStorey.KVP.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("PlanetAnalysisSelection").gameObject.SetActive(SpacecraftManager.instance.GetStarmapAnalysisDestinationID() == <UpdateDestinationStates>c__AnonStorey.KVP.Key.id);
					<UpdateDestinationStates>c__AnonStorey.KVP.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("DoppelgangerPlanetAnalysisSelection").gameObject.SetActive(SpacecraftManager.instance.GetStarmapAnalysisDestinationID() == <UpdateDestinationStates>c__AnonStorey.KVP.Key.id);
					component2.onClick = delegate
					{
						<UpdateDestinationStates>c__AnonStorey.$this.UnselectAllPlanets();
						<UpdateDestinationStates>c__AnonStorey.$this.SelectPlanet(<UpdateDestinationStates>c__AnonStorey.KVP.Value);
						<UpdateDestinationStates>c__AnonStorey.$this.SelectDestination(<UpdateDestinationStates>c__AnonStorey.KVP.Key);
					};
					SpaceDestinationType spaceDestinationType = <UpdateDestinationStates>c__AnonStorey.KVP.Key.GetDestinationType();
					LocText planetLabel = component.GetReference<RectTransform>("PlanetLabel").GetComponent<LocText>();
					planetLabel.text = ((SpacecraftManager.instance.GetDestinationAnalysisState(<UpdateDestinationStates>c__AnonStorey.KVP.Key) != SpacecraftManager.DestinationAnalysisState.Complete) ? (UI.STARMAP.UNKNOWN_DESTINATION + "\n" + string.Format(UI.STARMAP.ANALYSIS_AMOUNT.text, GameUtil.GetFormattedPercent(100f * (SpacecraftManager.instance.GetDestinationAnalysisScore(<UpdateDestinationStates>c__AnonStorey.KVP.Key) / (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE), GameUtil.TimeSlice.None))) : (spaceDestinationType.Name + "\n<color=#979798> " + GameUtil.GetFormattedDistance((float)<UpdateDestinationStates>c__AnonStorey.KVP.Key.OneBasedDistance * 10000f * 1000f) + "</color>"));
					planetLabel.gameObject.SetActive(false);
					LocText doppelgangerLabel = component.GetReference<RectTransform>("DoppelgangerLabel").GetComponent<LocText>();
					doppelgangerLabel.text = ((SpacecraftManager.instance.GetDestinationAnalysisState(<UpdateDestinationStates>c__AnonStorey.KVP.Key) != SpacecraftManager.DestinationAnalysisState.Complete) ? (UI.STARMAP.UNKNOWN_DESTINATION + "\n" + string.Format(UI.STARMAP.ANALYSIS_AMOUNT.text, GameUtil.GetFormattedPercent(100f * (SpacecraftManager.instance.GetDestinationAnalysisScore(<UpdateDestinationStates>c__AnonStorey.KVP.Key) / (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE), GameUtil.TimeSlice.None))) : (spaceDestinationType.Name + "\n<color=#979798> " + GameUtil.GetFormattedDistance((float)<UpdateDestinationStates>c__AnonStorey.KVP.Key.OneBasedDistance * 10000f * 1000f) + "</color>"));
					doppelgangerLabel.gameObject.SetActive(false);
					Image image = component.GetReference<RectTransform>("Planet").GetComponent<Image>();
					image.sprite = ((SpacecraftManager.instance.GetDestinationAnalysisState(<UpdateDestinationStates>c__AnonStorey.KVP.Key) != SpacecraftManager.DestinationAnalysisState.Complete) ? Assets.GetSprite("unknown") : Assets.GetSprite(spaceDestinationType.spriteName));
					Image image2 = component.GetReference<RectTransform>("Doppelganger").GetComponent<Image>();
					image2.sprite = ((SpacecraftManager.instance.GetDestinationAnalysisState(<UpdateDestinationStates>c__AnonStorey.KVP.Key) != SpacecraftManager.DestinationAnalysisState.Complete) ? Assets.GetSprite("unknown") : Assets.GetSprite(spaceDestinationType.spriteName));
					Graphic graphic = image;
					Color color3 = ((SpacecraftManager.instance.GetDestinationAnalysisState(<UpdateDestinationStates>c__AnonStorey.KVP.Key) != SpacecraftManager.DestinationAnalysisState.Complete) ? color2 : Color.white);
					image2.color = color3;
					graphic.color = color3;
					component.GetReference<Image>("UnknownIcon").gameObject.SetActive(SpacecraftManager.instance.GetDestinationAnalysisState(<UpdateDestinationStates>c__AnonStorey.KVP.Key) != SpacecraftManager.DestinationAnalysisState.Complete);
					Image image3 = image2;
					float num4 = SpacecraftManager.instance.GetDestinationAnalysisScore(<UpdateDestinationStates>c__AnonStorey.KVP.Key) / (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE;
					image.fillAmount = num4;
					image3.fillAmount = num4;
					component2.onEnter = delegate
					{
						planetLabel.gameObject.SetActive(true);
						doppelgangerLabel.gameObject.SetActive(true);
					};
					component2.onExit = delegate
					{
						planetLabel.gameObject.SetActive(false);
						doppelgangerLabel.gameObject.SetActive(false);
					};
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
			GameObject gameObject2 = global::Util.KInstantiateUI(this.planetPrefab, this.planetRows[spaceDestination.distance], true);
			this.planetWidgets.Add(spaceDestination, gameObject2);
		}
		this.UpdateDestinationStates();
	}

	private void UnselectAllPlanets()
	{
		if (this.animateSelectedPlanetRoutine != null)
		{
			base.StopCoroutine(this.animateSelectedPlanetRoutine);
		}
		foreach (KeyValuePair<SpaceDestination, GameObject> keyValuePair in this.planetWidgets)
		{
			HierarchyReferences component = keyValuePair.Value.GetComponent<HierarchyReferences>();
			RectTransform reference = component.GetReference<RectTransform>("PlanetSelection");
			reference.gameObject.SetActive(false);
			RectTransform reference2 = component.GetReference<RectTransform>("DoppelgangerSelection");
			reference2.gameObject.SetActive(false);
		}
	}

	private void SelectPlanet(GameObject planet)
	{
		HierarchyReferences component = planet.GetComponent<HierarchyReferences>();
		RectTransform reference = component.GetReference<RectTransform>("PlanetSelection");
		reference.gameObject.SetActive(true);
		RectTransform reference2 = component.GetReference<RectTransform>("DoppelgangerSelection");
		reference2.gameObject.SetActive(true);
		if (this.animateSelectedPlanetRoutine != null)
		{
			base.StopCoroutine(this.animateSelectedPlanetRoutine);
		}
		this.animateSelectedPlanetRoutine = base.StartCoroutine(this.AnimatePlanetSelection(reference, reference2));
	}

	private IEnumerator AnimatePlanetSelection(RectTransform planetSelection, RectTransform doppelgangerSelection)
	{
		for (;;)
		{
			planetSelection.rectTransform().anchoredPosition = new Vector2(0f, 25f + Mathf.Sin(Time.unscaledTime * 4f) * 5f);
			doppelgangerSelection.rectTransform().anchoredPosition = new Vector2(0f, 25f + Mathf.Sin(Time.unscaledTime * 4f) * 5f);
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

	public override float GetSortKey()
	{
		return 100f;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed && (e.TryConsume(global::Action.MouseRight) || e.TryConsume(global::Action.Escape)))
		{
			ManagementMenu.Instance.CloseAll();
			return;
		}
		if (this.CheckBlockedInput())
		{
			if (!e.Consumed)
			{
				e.Consumed = true;
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
					HierarchyReferences value = keyValuePair.Value;
					EditableTitleBar component = value.GetReference<RectTransform>("EditableTitle").GetComponent<EditableTitleBar>();
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
		float num = this.rowPrefab.GetComponent<RectTransform>().rect.height / 2f - 120f;
		foreach (KeyValuePair<SpaceDestination, GameObject> keyValuePair in this.planetWidgets)
		{
			keyValuePair.Value.rectTransform().anchoredPosition = new Vector2(keyValuePair.Value.transform.parent.rectTransform().sizeDelta.x * keyValuePair.Key.startingOrbitPercentage, -num);
			keyValuePair.Value.GetComponent<HierarchyReferences>().GetReference("Doppelganger").rectTransform()
				.anchoredPosition = Vector2.left * keyValuePair.Value.transform.parent.rectTransform().sizeDelta.x;
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
				this.rocketConditionEventHandler = this.currentLaunchConditionManager.Subscribe(1655598572, new Action<object>(this.Refresh));
				this.ShowRocketDetailsPanel();
			}
			else
			{
				this.ShowRocketListPanel();
			}
		}
		else
		{
			this.ShowRocketListPanel();
		}
		this.Refresh(null);
	}

	private void ShowRocketListPanel()
	{
		this.rocketInMissionSelected = false;
		this.listPanel.SetActive(true);
		this.rocketPanel.SetActive(false);
		this.launchButton.ChangeState(1);
		this.distanceOverlayEnabled = false;
		this.UpdateDistanceOverlay(null);
		this.UpdateMissionOverlay(null);
	}

	private void ShowRocketDetailsPanel()
	{
		this.listPanel.SetActive(false);
		this.rocketPanel.SetActive(true);
		this.ValidateTravelAbility();
		this.distanceOverlayEnabled = true;
		this.UpdateDistanceOverlay(null);
		this.UpdateMissionOverlay(null);
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
				StarmapScreen.<FillRocketListPanel>c__AnonStorey3 <FillRocketListPanel>c__AnonStorey = new StarmapScreen.<FillRocketListPanel>c__AnonStorey3();
				<FillRocketListPanel>c__AnonStorey.rocket = enumerator.Current;
				<FillRocketListPanel>c__AnonStorey.$this = this;
				HierarchyReferences hierarchyReferences = global::Util.KInstantiateUI<HierarchyReferences>(this.listRocketTemplate.gameObject, this.rocketListContainer.gameObject, true);
				BreakdownList component = hierarchyReferences.GetComponent<BreakdownList>();
				MultiToggle component2 = hierarchyReferences.GetComponent<MultiToggle>();
				EditableTitleBar component3 = hierarchyReferences.GetReference<RectTransform>("EditableTitle").GetComponent<EditableTitleBar>();
				Image component4 = hierarchyReferences.GetReference<RectTransform>("SelectionOutline").GetComponent<Image>();
				MultiToggle component5 = hierarchyReferences.GetReference<RectTransform>("LaunchRocketButton").GetComponent<MultiToggle>();
				MultiToggle component6 = hierarchyReferences.GetReference<RectTransform>("LandRocketButton").GetComponent<MultiToggle>();
				HierarchyReferences component7 = hierarchyReferences.GetReference<RectTransform>("ProgressBar").GetComponent<HierarchyReferences>();
				LaunchConditionManager launchConditionManager = <FillRocketListPanel>c__AnonStorey.rocket.launchConditions;
				CommandModule component8 = launchConditionManager.GetComponent<CommandModule>();
				MinionStorage component9 = launchConditionManager.GetComponent<MinionStorage>();
				component3.SetTitle(<FillRocketListPanel>c__AnonStorey.rocket.rocketName);
				component3.OnNameChanged += delegate(string newName)
				{
					<FillRocketListPanel>c__AnonStorey.rocket.SetRocketName(newName);
				};
				MultiToggle multiToggle = component2;
				multiToggle.onEnter = (global::System.Action)Delegate.Combine(multiToggle.onEnter, new global::System.Action(delegate
				{
					if (<FillRocketListPanel>c__AnonStorey.rocket.state != Spacecraft.MissionState.Grounded)
					{
						<FillRocketListPanel>c__AnonStorey.$this.rocketInMissionHovered = true;
					}
					LaunchConditionManager launchConditions = <FillRocketListPanel>c__AnonStorey.rocket.launchConditions;
					CommandModule component11 = launchConditionManager.GetComponent<CommandModule>();
					<FillRocketListPanel>c__AnonStorey.$this.UpdateDistanceOverlay(component11);
					<FillRocketListPanel>c__AnonStorey.$this.UpdateMissionOverlay(component11);
				}));
				MultiToggle multiToggle2 = component2;
				multiToggle2.onExit = (global::System.Action)Delegate.Combine(multiToggle2.onExit, new global::System.Action(delegate
				{
					if (<FillRocketListPanel>c__AnonStorey.rocket.state != Spacecraft.MissionState.Grounded)
					{
						<FillRocketListPanel>c__AnonStorey.$this.rocketInMissionHovered = false;
					}
					<FillRocketListPanel>c__AnonStorey.$this.UpdateDistanceOverlay(null);
					<FillRocketListPanel>c__AnonStorey.$this.UpdateMissionOverlay(null);
				}));
				MultiToggle multiToggle3 = component2;
				multiToggle3.onClick = (global::System.Action)Delegate.Combine(multiToggle3.onClick, new global::System.Action(delegate
				{
					LaunchConditionManager launchConditions2 = <FillRocketListPanel>c__AnonStorey.rocket.launchConditions;
					CommandModule component12 = launchConditionManager.GetComponent<CommandModule>();
					<FillRocketListPanel>c__AnonStorey.$this.currentCommandModule = component12;
					<FillRocketListPanel>c__AnonStorey.$this.currentLaunchConditionManager = launchConditions2;
					if (!component12.GetComponent<RocketModule>().IsSuspended())
					{
						Vector3 position = component12.transform.position;
						position.x += 6f;
						CameraController.Instance.CameraGoTo(position, 2f, true);
					}
					if (<FillRocketListPanel>c__AnonStorey.rocket.state != Spacecraft.MissionState.Grounded)
					{
						<FillRocketListPanel>c__AnonStorey.$this.rocketInMissionSelected = true;
					}
					<FillRocketListPanel>c__AnonStorey.$this.FillRocketPanel();
					<FillRocketListPanel>c__AnonStorey.$this.ShowRocketDetailsPanel();
				}));
				component5.play_sound_on_click = false;
				MultiToggle multiToggle4 = component5;
				multiToggle4.onClick = (global::System.Action)Delegate.Combine(multiToggle4.onClick, new global::System.Action(delegate
				{
					if (launchConditionManager != null && <FillRocketListPanel>c__AnonStorey.$this.selectedDestination != null)
					{
						KFMOD.PlayOneShot(GlobalAssets.GetSound("HUD_Click", false));
						launchConditionManager.Launch(<FillRocketListPanel>c__AnonStorey.$this.selectedDestination);
						<FillRocketListPanel>c__AnonStorey.$this.ClearRocketListPanel();
						<FillRocketListPanel>c__AnonStorey.$this.FillRocketListPanel();
						<FillRocketListPanel>c__AnonStorey.$this.ShowRocketListPanel();
						<FillRocketListPanel>c__AnonStorey.$this.Refresh(null);
					}
					else
					{
						KFMOD.PlayOneShot(GlobalAssets.GetSound("Negative", false));
					}
				}));
				if ((DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive) && SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(launchConditionManager).state != Spacecraft.MissionState.Grounded)
				{
					component6.gameObject.SetActive(true);
					component6.transform.SetAsLastSibling();
					component6.play_sound_on_click = false;
					MultiToggle multiToggle5 = component6;
					multiToggle5.onClick = (global::System.Action)Delegate.Combine(multiToggle5.onClick, new global::System.Action(delegate
					{
						if (launchConditionManager != null && <FillRocketListPanel>c__AnonStorey.$this.selectedDestination != null)
						{
							KFMOD.PlayOneShot(GlobalAssets.GetSound("HUD_Click", false));
							SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(launchConditionManager).ForceComplete();
							<FillRocketListPanel>c__AnonStorey.$this.ClearRocketListPanel();
							<FillRocketListPanel>c__AnonStorey.$this.FillRocketListPanel();
							<FillRocketListPanel>c__AnonStorey.$this.ShowRocketListPanel();
							<FillRocketListPanel>c__AnonStorey.$this.Refresh(null);
						}
						else
						{
							KFMOD.PlayOneShot(GlobalAssets.GetSound("Negative", false));
						}
					}));
				}
				else
				{
					component6.gameObject.SetActive(false);
				}
				BreakdownListRow breakdownListRow = component.AddRow();
				string text = UI.STARMAP.MISSION_STATUS.GROUNDED;
				Color color = Color.green;
				switch (<FillRocketListPanel>c__AnonStorey.rocket.state)
				{
				case Spacecraft.MissionState.Grounded:
					color = Color.green;
					text = UI.STARMAP.MISSION_STATUS.GROUNDED;
					break;
				case Spacecraft.MissionState.Launching:
					text = UI.STARMAP.MISSION_STATUS.LAUNCHING;
					color = Color.yellow;
					break;
				case Spacecraft.MissionState.Underway:
					color = Color.red;
					text = UI.STARMAP.MISSION_STATUS.UNDERWAY;
					break;
				case Spacecraft.MissionState.WaitingToLand:
					color = Color.yellow;
					text = UI.STARMAP.MISSION_STATUS.WAITING_TO_LAND;
					break;
				}
				breakdownListRow.ShowStatusData(UI.STARMAP.ROCKETSTATUS.STATUS, text, color);
				breakdownListRow.SetHighlighted(true);
				if (component9 != null)
				{
					List<MinionStorage.Info> storedMinionInfo = component9.GetStoredMinionInfo();
					BreakdownListRow breakdownListRow2 = component.AddRow();
					int count = storedMinionInfo.Count;
					breakdownListRow2.ShowStatusData(UI.STARMAP.LISTTITLES.PASSENGERS, count.ToString(), (count != 0) ? Color.green : Color.red);
				}
				if (<FillRocketListPanel>c__AnonStorey.rocket.state == Spacecraft.MissionState.Grounded)
				{
					string text2 = string.Empty;
					List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(launchConditionManager.GetComponent<AttachableBuilding>());
					foreach (GameObject gameObject in attachedNetwork)
					{
						text2 = text2 + gameObject.GetProperName() + "\n";
					}
					BreakdownListRow breakdownListRow3 = component.AddRow();
					breakdownListRow3.ShowData(UI.STARMAP.LISTTITLES.MODULES, attachedNetwork.Count.ToString());
					breakdownListRow3.AddTooltip(text2);
					BreakdownListRow breakdownListRow4 = component.AddRow();
					breakdownListRow4.ShowData(UI.STARMAP.LISTTITLES.MAXRANGE, this.DisplayDistance(component8.rocketStats.GetRocketMaxDistance()));
					component5.GetComponent<RectTransform>().SetAsLastSibling();
					component5.gameObject.SetActive(true);
					component7.gameObject.SetActive(false);
				}
				else
				{
					float duration = <FillRocketListPanel>c__AnonStorey.rocket.GetDuration();
					float timeLeft = <FillRocketListPanel>c__AnonStorey.rocket.GetTimeLeft();
					float num = ((duration != 0f) ? (1f - timeLeft / duration) : 0f);
					BreakdownListRow breakdownListRow5 = component.AddRow();
					breakdownListRow5.ShowData(UI.STARMAP.ROCKETSTATUS.TIMEREMAINING, global::Util.FormatOneDecimalPlace(timeLeft / 600f) + " / " + GameUtil.GetFormattedCycles(duration, "F1"));
					component7.gameObject.SetActive(true);
					RectTransform reference = component7.GetReference<RectTransform>("ProgressImage");
					LocText component10 = component7.GetReference<RectTransform>("ProgressText").GetComponent<LocText>();
					reference.transform.localScale = new Vector3(num, 1f, 1f);
					component10.text = GameUtil.GetFormattedPercent(num * 100f, GameUtil.TimeSlice.None);
					component7.GetComponent<RectTransform>().SetAsLastSibling();
					component5.gameObject.SetActive(false);
				}
				this.listRocketRows.Add(<FillRocketListPanel>c__AnonStorey.rocket, hierarchyReferences);
			}
		}
		this.UpdateRocketRowsTravelAbility();
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
		foreach (RocketLaunchCondition rocketLaunchCondition in launchConditionManager.GetLaunchConditionList())
		{
			BreakdownListRow breakdownListRow = this.rocketDetailsChecklist.AddRow();
			string launchStatusMessage = rocketLaunchCondition.GetLaunchStatusMessage(true);
			bool flag = rocketLaunchCondition.EvaluateLaunchCondition();
			breakdownListRow.ShowCheckmarkData(launchStatusMessage, string.Empty, flag);
			if (!flag)
			{
				breakdownListRow.SetHighlighted(true);
			}
			breakdownListRow.AddTooltip(rocketLaunchCondition.GetLaunchStatusTooltip(flag));
		}
	}

	private void SelectDestination(SpaceDestination destination)
	{
		this.selectedDestination = destination;
		this.UnselectAllPlanets();
		if (this.selectedDestination != null)
		{
			this.SelectPlanet(this.planetWidgets[this.selectedDestination]);
			if (this.currentCommandModule != null)
			{
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
			this.rangeRowTotal.SetStatusColor((!this.currentCommandModule.reachable.CanReachDestination(this.selectedDestination)) ? Color.red : Color.green);
		}
		this.Refresh(null);
	}

	private void UpdateRocketRowsTravelAbility()
	{
		foreach (KeyValuePair<Spacecraft, HierarchyReferences> keyValuePair in this.listRocketRows)
		{
			Spacecraft key = keyValuePair.Key;
			LaunchConditionManager launchConditions = key.launchConditions;
			CommandModule component = launchConditions.GetComponent<CommandModule>();
			HierarchyReferences value = keyValuePair.Value;
			MultiToggle component2 = value.GetReference<RectTransform>("LaunchRocketButton").GetComponent<MultiToggle>();
			bool flag = false;
			if (this.selectedDestination != null && key.state == Spacecraft.MissionState.Grounded)
			{
				flag = true;
				key.SetMission(this.selectedDestination);
			}
			bool flag2 = this.selectedDestination != null && component.reachable.CanReachDestination(this.selectedDestination);
			bool flag3 = launchConditions.CheckReadyToLaunch();
			component2.ChangeState((!flag || !flag2 || !flag3) ? 1 : 0);
		}
	}

	private void RefreshAnalyzeButton()
	{
		if (this.selectedDestination == null)
		{
			this.analyzeButton.ChangeState(1);
			this.analyzeButton.onClick = null;
			this.analyzeButton.GetComponentInChildren<LocText>().text = UI.STARMAP.NO_ANALYZABLE_DESTINATION_SELECTED;
		}
		else if (this.selectedDestination.AnalysisState() == SpacecraftManager.DestinationAnalysisState.Complete)
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
			}
			else
			{
				this.analyzeButton.ChangeState(1);
				this.analyzeButton.onClick = null;
				this.analyzeButton.GetComponentInChildren<LocText>().text = UI.STARMAP.ANALYSIS_COMPLETE;
			}
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
			}
			else
			{
				this.analyzeButton.GetComponentInChildren<LocText>().text = UI.STARMAP.ANALYZE_DESTINATION;
				this.analyzeButton.onClick = delegate
				{
					if (DebugHandler.InstantBuildMode)
					{
						SpacecraftManager.instance.SetStarmapAnalysisDestinationID(this.selectedDestination.id);
						SpacecraftManager.instance.EarnDestinationAnalysisPoints(this.selectedDestination.id, 99999f);
						this.ShowDestinationPanel();
					}
					else
					{
						SpacecraftManager.instance.SetStarmapAnalysisDestinationID(this.selectedDestination.id);
					}
				};
			}
		}
	}

	private void Refresh(object data = null)
	{
		this.FillRocketListPanel();
		this.RefreshAnalyzeButton();
		if (this.currentCommandModule != null && this.currentLaunchConditionManager != null)
		{
			this.FillRocketPanel();
			if (this.selectedDestination != null)
			{
				this.ValidateTravelAbility();
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
		BreakdownListRow breakdownListRow = this.rocketDetailsRange.AddRow();
		breakdownListRow.ShowData(UI.STARMAP.ROCKETSTATS.TOTAL_OXIDIZABLE_FUEL, GameUtil.GetFormattedMass(this.currentCommandModule.rocketStats.GetTotalOxidizableFuel(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
		BreakdownListRow breakdownListRow2 = this.rocketDetailsRange.AddRow();
		breakdownListRow2.ShowData(UI.STARMAP.ROCKETSTATS.ENGINE_EFFICIENCY, GameUtil.GetFormattedEngineEfficiency(this.currentCommandModule.rocketStats.GetEngineEfficiency()));
		BreakdownListRow breakdownListRow3 = this.rocketDetailsRange.AddRow();
		breakdownListRow3.ShowData(UI.STARMAP.ROCKETSTATS.OXIDIZER_EFFICIENCY, GameUtil.GetFormattedPercent(this.currentCommandModule.rocketStats.GetAverageOxidizerEfficiency(), GameUtil.TimeSlice.None));
		float num = this.currentCommandModule.rocketStats.GetBoosterThrust() * 1000f;
		if (num != 0f)
		{
			BreakdownListRow breakdownListRow4 = this.rocketDetailsRange.AddRow();
			breakdownListRow4.ShowData(UI.STARMAP.ROCKETSTATS.SOLID_BOOSTER, GameUtil.GetFormattedDistance(num));
		}
		BreakdownListRow breakdownListRow5 = this.rocketDetailsRange.AddRow();
		breakdownListRow5.ShowStatusData(UI.STARMAP.ROCKETSTATS.TOTAL_THRUST, GameUtil.GetFormattedDistance(this.currentCommandModule.rocketStats.GetTotalThrust() * 1000f), Color.green);
		breakdownListRow5.SetImportant(true);
		float num2 = -(this.currentCommandModule.rocketStats.GetTotalThrust() - this.currentCommandModule.rocketStats.GetRocketMaxDistance());
		this.rocketThrustWidget.gameObject.SetActive(true);
		BreakdownListRow breakdownListRow6 = this.rocketDetailsRange.AddRow();
		breakdownListRow6.ShowStatusData(UI.STARMAP.ROCKETSTATUS.WEIGHTPENALTY, this.DisplayDistance(num2), Color.red);
		breakdownListRow6.SetHighlighted(true);
		this.rocketDetailsRange.AddCustomRow(this.rocketThrustWidget.gameObject);
		this.rocketThrustWidget.Draw(this.currentCommandModule);
		BreakdownListRow breakdownListRow7 = this.rocketDetailsRange.AddRow();
		breakdownListRow7.ShowData(UI.STARMAP.ROCKETSTATS.TOTAL_RANGE, GameUtil.GetFormattedDistance(this.currentCommandModule.rocketStats.GetRocketMaxDistance() * 1000f));
		breakdownListRow7.SetImportant(true);
	}

	private void UpdateMassDisplay()
	{
		BreakdownListRow breakdownListRow = this.rocketDetailsMass.AddRow();
		breakdownListRow.ShowData(UI.STARMAP.ROCKETSTATS.DRY_MASS, GameUtil.GetFormattedMass(this.currentCommandModule.rocketStats.GetDryMass(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		BreakdownListRow breakdownListRow2 = this.rocketDetailsMass.AddRow();
		breakdownListRow2.ShowData(UI.STARMAP.ROCKETSTATS.WET_MASS, GameUtil.GetFormattedMass(this.currentCommandModule.rocketStats.GetWetMass(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		BreakdownListRow breakdownListRow3 = this.rocketDetailsMass.AddRow();
		breakdownListRow3.ShowData(UI.STARMAP.ROCKETSTATUS.TOTAL, GameUtil.GetFormattedMass(this.currentCommandModule.rocketStats.GetTotalMass(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		breakdownListRow3.SetImportant(true);
	}

	private void UpdateFuelDisplay()
	{
		Tag engineFuelTag = this.currentCommandModule.rocketStats.GetEngineFuelTag();
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.currentCommandModule.GetComponent<AttachableBuilding>()))
		{
			FuelTank component = gameObject.GetComponent<FuelTank>();
			if (component != null)
			{
				BreakdownListRow breakdownListRow = this.rocketDetailsFuel.AddRow();
				breakdownListRow.ShowData(gameObject.gameObject.GetProperName() + " (" + ElementLoader.GetElement(engineFuelTag).name + ")", GameUtil.GetFormattedMass(component.GetAmountAvailable(engineFuelTag), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
			}
			SolidBooster component2 = gameObject.GetComponent<SolidBooster>();
			if (component2 != null)
			{
				BreakdownListRow breakdownListRow2 = this.rocketDetailsFuel.AddRow();
				breakdownListRow2.ShowData(gameObject.gameObject.GetProperName() + " (" + ElementLoader.GetElement(component2.fuelTag).name + ")", GameUtil.GetFormattedMass(component2.fuelStorage.GetMassAvailable(component2.fuelTag), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
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
				if (component.GetAmountAvailable(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag) > 0f)
				{
					BreakdownListRow breakdownListRow = this.rocketDetailsOxidizer.AddRow();
					breakdownListRow.ShowData(gameObject.gameObject.GetProperName() + " (" + ElementLoader.FindElementByHash(SimHashes.OxyRock).name + ")", GameUtil.GetFormattedMass(component.GetAmountAvailable(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
				}
				if (component.GetAmountAvailable(ElementLoader.FindElementByHash(SimHashes.LiquidOxygen).tag) > 0f)
				{
					BreakdownListRow breakdownListRow2 = this.rocketDetailsOxidizer.AddRow();
					breakdownListRow2.ShowData(gameObject.gameObject.GetProperName() + " (" + ElementLoader.FindElementByHash(SimHashes.LiquidOxygen).name + ")", GameUtil.GetFormattedMass(component.GetAmountAvailable(ElementLoader.FindElementByHash(SimHashes.LiquidOxygen).tag), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
				}
			}
			SolidBooster component2 = gameObject.GetComponent<SolidBooster>();
			if (component2 != null)
			{
				BreakdownListRow breakdownListRow3 = this.rocketDetailsOxidizer.AddRow();
				breakdownListRow3.ShowData(gameObject.gameObject.GetProperName() + " (" + ElementLoader.FindElementByHash(SimHashes.OxyRock).name + ")", GameUtil.GetFormattedMass(component2.fuelStorage.GetMassAvailable(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
			}
		}
		BreakdownListRow breakdownListRow4 = this.rocketDetailsOxidizer.AddRow();
		breakdownListRow4.ShowData(UI.STARMAP.ROCKETSTATS.TOTAL_OXIDIZER, GameUtil.GetFormattedMass(this.currentCommandModule.rocketStats.GetTotalOxidizer(true), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		breakdownListRow4.SetImportant(true);
	}

	private void UpdateStorageDisplay()
	{
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.currentCommandModule.GetComponent<AttachableBuilding>()))
		{
			CargoBay component = gameObject.GetComponent<CargoBay>();
			if (component != null)
			{
				BreakdownListRow breakdownListRow = this.rocketDetailsStorage.AddRow();
				breakdownListRow.ShowData(gameObject.gameObject.GetProperName(), GameUtil.GetFormattedMass(component.storage.Capacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
			}
		}
	}

	private void ClearDestinationPanel()
	{
		this.destinationDetailsContainer.gameObject.SetActive(false);
	}

	private void ShowDestinationPanel()
	{
		SpaceDestinationType destinationType = this.selectedDestination.GetDestinationType();
		this.destinationStatusLabel.text = UI.STARMAP.ROCKETSTATUS.SELECTED;
		this.destinationNameLabel.text = ((SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) != SpacecraftManager.DestinationAnalysisState.Complete) ? UI.STARMAP.UNKNOWN_DESTINATION.text : destinationType.Name);
		this.destinationTypeValueLabel.text = ((SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) != SpacecraftManager.DestinationAnalysisState.Complete) ? UI.STARMAP.UNKNOWN_TYPE.text : destinationType.typeName);
		this.destinationDistanceValueLabel.text = this.DisplayDistance((float)this.selectedDestination.OneBasedDistance * 10000f);
		this.destinationDescriptionLabel.text = destinationType.description;
		this.destinationDetailsComposition.ClearRows();
		float num = 0f;
		if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
		{
			foreach (KeyValuePair<SimHashes, float> keyValuePair in this.selectedDestination.recoverableElements)
			{
				num += this.selectedDestination.GetResourceValue(keyValuePair.Key, keyValuePair.Value);
			}
		}
		this.destinationDetailsResearch.ClearRows();
		if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
		{
			foreach (SpaceDestination.ResearchOpportunity researchOpportunity in this.selectedDestination.researchOpportunities)
			{
				BreakdownListRow breakdownListRow = this.destinationDetailsResearch.AddRow();
				string text = ((researchOpportunity.discoveredRareResource == SimHashes.Void) ? researchOpportunity.description : string.Format("(!!) {0}", researchOpportunity.description));
				breakdownListRow.ShowCheckmarkData(text, researchOpportunity.dataValue.ToString(), researchOpportunity.completed);
			}
		}
		this.destinationAnalysisProgressBar.SetFillPercentage(SpacecraftManager.instance.GetDestinationAnalysisScore(this.selectedDestination.id) / (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE);
		if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
		{
			foreach (KeyValuePair<SimHashes, float> keyValuePair2 in this.selectedDestination.recoverableElements)
			{
				BreakdownListRow breakdownListRow2 = this.destinationDetailsComposition.AddRow();
				float num2 = this.selectedDestination.GetResourceValue(keyValuePair2.Key, keyValuePair2.Value) / num * 100f;
				Element element = ElementLoader.FindElementByHash(keyValuePair2.Key);
				Tuple<Sprite, Color> uisprite = Def.GetUISprite(element, "ui", false);
				if (num2 <= 1f)
				{
					breakdownListRow2.ShowIconData(element.name, UI.STARMAP.COMPOSITION_SMALL_AMOUNT, uisprite.first, uisprite.second);
				}
				else
				{
					breakdownListRow2.ShowIconData(element.name, GameUtil.GetFormattedPercent(num2, GameUtil.TimeSlice.None), uisprite.first, uisprite.second);
				}
				if (element.IsGas)
				{
					string properName = Assets.GetPrefab("GasCargoBay".ToTag()).GetProperName();
					if (this.currentRocketHasGasContainer)
					{
						breakdownListRow2.SetHighlighted(true);
						breakdownListRow2.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, element.name, properName));
					}
					else
					{
						breakdownListRow2.SetDisabled(true);
						breakdownListRow2.AddTooltip(string.Format(UI.STARMAP.CONTAINER_REQUIRED, properName));
					}
				}
				if (element.IsLiquid)
				{
					string properName2 = Assets.GetPrefab("LiquidCargoBay".ToTag()).GetProperName();
					if (this.currentRocketHasLiquidContainer)
					{
						breakdownListRow2.SetHighlighted(true);
						breakdownListRow2.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, element.name, properName2));
					}
					else
					{
						breakdownListRow2.SetDisabled(true);
						breakdownListRow2.AddTooltip(string.Format(UI.STARMAP.CONTAINER_REQUIRED, properName2));
					}
				}
				if (element.IsSolid)
				{
					string properName3 = Assets.GetPrefab("CargoBay".ToTag()).GetProperName();
					if (this.currentRocketHasSolidContainer)
					{
						breakdownListRow2.SetHighlighted(true);
						breakdownListRow2.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, element.name, properName3));
					}
					else
					{
						breakdownListRow2.SetDisabled(true);
						breakdownListRow2.AddTooltip(string.Format(UI.STARMAP.CONTAINER_REQUIRED, properName3));
					}
				}
			}
			foreach (SpaceDestination.ResearchOpportunity researchOpportunity2 in this.selectedDestination.researchOpportunities)
			{
				if (!researchOpportunity2.completed && researchOpportunity2.discoveredRareResource != SimHashes.Void)
				{
					BreakdownListRow breakdownListRow3 = this.destinationDetailsComposition.AddRow();
					breakdownListRow3.ShowData(UI.STARMAP.COMPOSITION_UNDISCOVERED, UI.STARMAP.COMPOSITION_UNDISCOVERED_AMOUNT);
					breakdownListRow3.SetDisabled(true);
					breakdownListRow3.AddTooltip(UI.STARMAP.COMPOSITION_UNDISCOVERED_TOOLTIP);
				}
			}
		}
		this.destinationDetailsResources.ClearRows();
		if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
		{
			foreach (KeyValuePair<Tag, int> keyValuePair3 in this.selectedDestination.GetRecoverableEntities())
			{
				BreakdownListRow breakdownListRow4 = this.destinationDetailsResources.AddRow();
				GameObject prefab = Assets.GetPrefab(keyValuePair3.Key);
				Tuple<Sprite, Color> uisprite2 = Def.GetUISprite(prefab, "ui", false);
				breakdownListRow4.ShowIconData(prefab.GetProperName(), string.Empty, uisprite2.first, uisprite2.second);
				string properName4 = Assets.GetPrefab("SpecialCargoBay".ToTag()).GetProperName();
				if (this.currentRocketHasEntitiesContainer)
				{
					breakdownListRow4.SetHighlighted(true);
					breakdownListRow4.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, prefab.GetProperName(), properName4));
				}
				else
				{
					breakdownListRow4.SetDisabled(true);
					breakdownListRow4.AddTooltip(string.Format(UI.STARMAP.CANT_CARRY_ELEMENT, properName4, prefab.GetProperName()));
				}
			}
		}
		this.destinationDetailsContainer.gameObject.SetActive(true);
		LayoutRebuilder.ForceRebuildLayoutImmediate(this.destinationDetailsContainer);
	}

	private void ValidateTravelAbility()
	{
		if (this.selectedDestination != null && SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete && this.currentCommandModule != null && this.currentLaunchConditionManager != null)
		{
			this.launchButton.ChangeState((!this.currentLaunchConditionManager.CheckReadyToLaunch()) ? 1 : 0);
		}
	}

	private void UpdateDistanceOverlay(CommandModule previewCommandModule = null)
	{
		bool flag = previewCommandModule != null;
		if (previewCommandModule == null)
		{
			previewCommandModule = this.currentCommandModule;
		}
		if (previewCommandModule != null && (this.distanceOverlayEnabled || flag) && !this.rocketInMissionHovered)
		{
			this.distanceOverlay.gameObject.SetActive(true);
			float num = previewCommandModule.rocketStats.GetRocketMaxDistance();
			num = (float)((int)(num / 10000f)) * 10000f;
			Vector2 sizeDelta = this.distanceOverlay.rectTransform.sizeDelta;
			sizeDelta.x = this.rowsContiner.rect.width;
			sizeDelta.y = (1f - num / this.planetsMaxDistance) * this.rowsContiner.rect.height + (float)this.distanceOverlayYOffset + (float)this.distanceOverlayVerticalOffset;
			this.distanceOverlay.rectTransform.sizeDelta = sizeDelta;
			this.distanceOverlay.rectTransform.anchoredPosition = new Vector3(0f, (float)this.distanceOverlayVerticalOffset, 0f);
		}
		else
		{
			this.distanceOverlay.gameObject.SetActive(false);
		}
	}

	private void UpdateMissionOverlay(CommandModule previewCommandModule = null)
	{
		if (previewCommandModule == null)
		{
			previewCommandModule = this.currentCommandModule;
		}
		if (previewCommandModule != null && (this.rocketInMissionSelected || this.rocketInMissionHovered))
		{
			LaunchConditionManager component = previewCommandModule.GetComponent<LaunchConditionManager>();
			if (component == null)
			{
				global::Debug.Log("launchConditionManager is null", null);
				return;
			}
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(component);
			SpaceDestination destination = previewCommandModule.reachable.GetDestination();
			if (destination == null)
			{
				global::Debug.Log("destination is null", null);
				return;
			}
			GameObject gameObject = this.planetWidgets[destination];
			if (spacecraftFromLaunchConditionManager == null)
			{
				global::Debug.Log("craft is null", null);
				return;
			}
			if (gameObject == null)
			{
				global::Debug.Log("planet is null", null);
				return;
			}
			this.UnselectAllPlanets();
			this.SelectPlanet(gameObject);
			this.visualizeRocketImage.gameObject.SetActive(true);
			this.visualizeRocketTrajectory.gameObject.SetActive(true);
			this.visualizeRocketLabel.gameObject.SetActive(true);
			this.visualizeRocketProgress.gameObject.SetActive(true);
			float duration = spacecraftFromLaunchConditionManager.GetDuration();
			float timeLeft = spacecraftFromLaunchConditionManager.GetTimeLeft();
			float num = ((duration != 0f) ? (1f - timeLeft / duration) : 0f);
			bool flag = num > 0.5f;
			Vector2 vector = new Vector2(0f, -this.rowsContiner.rect.size.y);
			Vector2 vector2 = gameObject.transform.rectTransform().localPosition;
			vector2.x = vector2.x - this.rowsContiner.rect.size.x / 2f + gameObject.transform.rectTransform().sizeDelta.x / 2f;
			vector2.y = gameObject.transform.localPosition.y + gameObject.transform.parent.transform.localPosition.y;
			float num2 = Vector2.Distance(vector, vector2);
			Vector2 vector3 = vector2 - vector;
			float num3 = Mathf.Atan2(vector3.y, vector3.x);
			float num4 = num3 * 57.29578f;
			Vector2 vector4;
			if (flag)
			{
				vector4 = new Vector2(Mathf.Lerp(vector.x, vector2.x, 1f - num * 2f + 1f), Mathf.Lerp(vector.y, vector2.y, 1f - num * 2f + 1f));
			}
			else
			{
				vector4 = new Vector2(Mathf.Lerp(vector.x, vector2.x, num * 2f), Mathf.Lerp(vector.y, vector2.y, num * 2f));
			}
			this.visualizeRocketLabel.text = spacecraftFromLaunchConditionManager.state.ToString();
			this.visualizeRocketProgress.text = GameUtil.GetFormattedPercent(num * 100f, GameUtil.TimeSlice.None);
			this.visualizeRocketTrajectory.transform.SetLocalPosition(vector);
			this.visualizeRocketTrajectory.rectTransform.sizeDelta = new Vector2(num2, this.visualizeRocketTrajectory.rectTransform.sizeDelta.y);
			this.visualizeRocketTrajectory.rectTransform.localRotation = Quaternion.Euler(0f, 0f, num4);
			this.visualizeRocketImage.transform.SetLocalPosition(vector4);
		}
		else
		{
			if (this.selectedDestination != null && this.planetWidgets.ContainsKey(this.selectedDestination))
			{
				this.UnselectAllPlanets();
				GameObject gameObject2 = this.planetWidgets[this.selectedDestination];
				this.SelectPlanet(gameObject2);
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

	private BreakdownList destinationDetailsComposition;

	public Sprite destinationDetailsCompositionIcon;

	private BreakdownList destinationDetailsResources;

	public Sprite destinationDetailsResourcesIcon;

	public RectTransform destinationDetailsContainer;

	public MultiToggle showRocketsButton;

	public MultiToggle launchButton;

	public MultiToggle analyzeButton;

	private int rocketConditionEventHandler = -1;

	[Header("Map References")]
	public RectTransform Map;

	public RectTransform rowsContiner;

	public GameObject rowPrefab;

	public GameObject planetPrefab;

	private List<GameObject> planetRows = new List<GameObject>();

	private Dictionary<SpaceDestination, GameObject> planetWidgets = new Dictionary<SpaceDestination, GameObject>();

	private float planetsMaxDistance = 1f;

	public Image distanceOverlay;

	private bool distanceOverlayEnabled;

	private bool distanceOverlayPreview;

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

	private bool rocketInMissionHovered;

	private bool rocketInMissionSelected;

	private bool forceScrollDown = true;

	private Coroutine animateAnalysisRoutine;

	private Coroutine animateSelectedPlanetRoutine;

	private BreakdownListRow rangeRowTotal;
}
