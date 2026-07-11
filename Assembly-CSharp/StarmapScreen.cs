using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StarmapScreen : KModalScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.ConsumeMouseScroll = true;
		this.rocketDetailsStatus = global::UnityEngine.Object.Instantiate<BreakdownList>(this.rocketDetailsStatus, this.rocketDetailsContainer);
		this.rocketDetailsStatus.SetTitle(UI.STARMAP.LISTTITLES.MISSIONSTATUS);
		this.rocketDetailsStatus.SetIcon(this.rocketDetailsStatusIcon);
		this.rocketDetailsStatus.gameObject.name = "rocketDetailsStatus";
		this.rocketDetailsChecklist = global::UnityEngine.Object.Instantiate<BreakdownList>(this.rocketDetailsChecklist, this.rocketDetailsContainer);
		this.rocketDetailsChecklist.SetTitle(UI.STARMAP.LISTTITLES.LAUNCHCHECKLIST);
		this.rocketDetailsChecklist.SetIcon(this.rocketDetailsChecklistIcon);
		this.rocketDetailsChecklist.gameObject.name = "rocketDetailsChecklist";
		this.rocketDetailsRange = global::UnityEngine.Object.Instantiate<BreakdownList>(this.rocketDetailsRange, this.rocketDetailsContainer);
		this.rocketDetailsRange.SetTitle(UI.STARMAP.LISTTITLES.MAXRANGE);
		this.rocketDetailsRange.SetIcon(this.rocketDetailsRangeIcon);
		this.rocketDetailsRange.gameObject.name = "rocketDetailsRange";
		this.rocketDetailsMass = global::UnityEngine.Object.Instantiate<BreakdownList>(this.rocketDetailsMass, this.rocketDetailsContainer);
		this.rocketDetailsMass.SetTitle(UI.STARMAP.LISTTITLES.MASS);
		this.rocketDetailsMass.SetIcon(this.rocketDetailsMassIcon);
		this.rocketDetailsMass.gameObject.name = "rocketDetailsMass";
		this.rocketThrustWidget = global::UnityEngine.Object.Instantiate<RocketThrustWidget>(this.rocketThrustWidget, this.rocketDetailsContainer);
		this.rocketDetailsStorage = global::UnityEngine.Object.Instantiate<BreakdownList>(this.rocketDetailsStorage, this.rocketDetailsContainer);
		this.rocketDetailsStorage.SetTitle(UI.STARMAP.LISTTITLES.STORAGE);
		this.rocketDetailsStorage.SetIcon(this.rocketDetailsStorageIcon);
		this.rocketDetailsStorage.gameObject.name = "rocketDetailsStorage";
		this.rocketDetailsFuel = global::UnityEngine.Object.Instantiate<BreakdownList>(this.rocketDetailsFuel, this.rocketDetailsContainer);
		this.rocketDetailsFuel.SetTitle(UI.STARMAP.LISTTITLES.FUEL);
		this.rocketDetailsFuel.SetIcon(this.rocketDetailsFuelIcon);
		this.rocketDetailsFuel.gameObject.name = "rocketDetailsFuel";
		this.rocketDetailsOxidizer = global::UnityEngine.Object.Instantiate<BreakdownList>(this.rocketDetailsOxidizer, this.rocketDetailsContainer);
		this.rocketDetailsOxidizer.SetTitle(UI.STARMAP.LISTTITLES.OXIDIZER);
		this.rocketDetailsOxidizer.SetIcon(this.rocketDetailsOxidizerIcon);
		this.rocketDetailsOxidizer.gameObject.name = "rocketDetailsOxidizer";
		this.rocketDetailsDupes = global::UnityEngine.Object.Instantiate<BreakdownList>(this.rocketDetailsDupes, this.rocketDetailsContainer);
		this.rocketDetailsDupes.SetTitle(UI.STARMAP.LISTTITLES.PASSENGERS);
		this.rocketDetailsDupes.SetIcon(this.rocketDetailsDupesIcon);
		this.rocketDetailsDupes.gameObject.name = "rocketDetailsDupes";
		this.destinationDetailsResearch = global::UnityEngine.Object.Instantiate<BreakdownList>(this.destinationDetailsResearch, this.destinationDetailsContainer);
		this.destinationDetailsResearch.SetTitle(UI.STARMAP.LISTTITLES.RESEARCH);
		this.destinationDetailsResearch.SetIcon(this.destinationDetailsResearchIcon);
		this.destinationDetailsResearch.gameObject.name = "destinationDetailsResearch";
		this.destinationDetailsResearch.SetDescription(string.Format(UI.STARMAP.RESEARCH_DESCRIPTION, 0));
		this.destinationDetailsComposition = global::UnityEngine.Object.Instantiate<BreakdownList>(this.destinationDetailsComposition, this.destinationDetailsContainer);
		this.destinationDetailsComposition.SetTitle(UI.STARMAP.LISTTITLES.WORLDCOMPOSITION);
		this.destinationDetailsComposition.SetIcon(this.destinationDetailsCompositionIcon);
		this.destinationDetailsComposition.gameObject.name = "destinationDetailsComposition";
		this.destinationDetailsResources = global::UnityEngine.Object.Instantiate<BreakdownList>(this.destinationDetailsResources, this.destinationDetailsContainer);
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
		}
		else
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUStarmapSnapshot, STOP_MODE.ALLOWFADEOUT);
			MusicManager.instance.StopSong("Music_Starmap", true, STOP_MODE.ALLOWFADEOUT);
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
		using (List<SpaceDestination>.Enumerator enumerator = Game.Instance.spacecraftManager.destinations.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				StarmapScreen.<LoadPlanets>c__AnonStorey2 <LoadPlanets>c__AnonStorey = new StarmapScreen.<LoadPlanets>c__AnonStorey2();
				<LoadPlanets>c__AnonStorey.destination = enumerator.Current;
				<LoadPlanets>c__AnonStorey.$this = this;
				while (<LoadPlanets>c__AnonStorey.destination.distance - 1 >= this.planetRows.Count)
				{
					GameObject gameObject = global::Util.KInstantiateUI(this.rowPrefab, this.rowsContiner.gameObject, true);
					gameObject.rectTransform().SetAsFirstSibling();
					this.planetRows.Add(gameObject);
					gameObject.GetComponentInChildren<Image>().color = this.distanceColors[this.planetRows.Count % this.distanceColors.Length];
					gameObject.GetComponentInChildren<LocText>().text = this.DisplayDistance((float)<LoadPlanets>c__AnonStorey.destination.distance * 10000f);
					if ((float)<LoadPlanets>c__AnonStorey.destination.distance * 10000f > this.planetsMaxDistance)
					{
						this.planetsMaxDistance = (float)<LoadPlanets>c__AnonStorey.destination.distance * 10000f;
					}
				}
				GameObject newPlanet = global::Util.KInstantiateUI(this.planetPrefab, this.planetRows[<LoadPlanets>c__AnonStorey.destination.distance - 1], true);
				HierarchyReferences component = newPlanet.GetComponent<HierarchyReferences>();
				MultiToggle component2 = newPlanet.GetComponent<MultiToggle>();
				component2.onClick = delegate
				{
					<LoadPlanets>c__AnonStorey.$this.UnselectAllPlanets();
					<LoadPlanets>c__AnonStorey.$this.SelectPlanet(newPlanet);
					<LoadPlanets>c__AnonStorey.$this.SelectDestination(<LoadPlanets>c__AnonStorey.destination);
				};
				LocText planetLabel = component.GetReference<RectTransform>("PlanetLabel").GetComponent<LocText>();
				planetLabel.text = <LoadPlanets>c__AnonStorey.destination.name;
				planetLabel.gameObject.SetActive(false);
				LocText doppelgangerLabel = component.GetReference<RectTransform>("DoppelgangerLabel").GetComponent<LocText>();
				doppelgangerLabel.text = <LoadPlanets>c__AnonStorey.destination.name;
				doppelgangerLabel.gameObject.SetActive(false);
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
				Image component3 = component.GetReference<RectTransform>("Planet").GetComponent<Image>();
				component3.sprite = Assets.GetSprite(<LoadPlanets>c__AnonStorey.destination.spriteName);
				Image component4 = component.GetReference<RectTransform>("Doppelganger").GetComponent<Image>();
				component4.sprite = Assets.GetSprite(<LoadPlanets>c__AnonStorey.destination.spriteName);
				this.planetWidgets.Add(<LoadPlanets>c__AnonStorey.destination, newPlanet);
			}
		}
	}

	private void UnselectAllPlanets()
	{
		base.StopAllCoroutines();
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
		base.StartCoroutine(this.AnimatePlanetSelection(reference, reference2));
	}

	private IEnumerator AnimatePlanetSelection(RectTransform planetSelection, RectTransform doppelgangerSelection)
	{
		bool scalingUp = false;
		float currentScale = 1f;
		for (;;)
		{
			if (currentScale <= 0.7f)
			{
				scalingUp = true;
			}
			if (currentScale >= 1f)
			{
				scalingUp = false;
			}
			currentScale += ((!scalingUp) ? (-0.5f * Time.unscaledDeltaTime) : (2f * Time.unscaledDeltaTime));
			planetSelection.offsetMax = new Vector2(10f * currentScale, 10f * currentScale);
			planetSelection.offsetMin = new Vector2(-10f * currentScale, -10f * currentScale);
			doppelgangerSelection.offsetMax = new Vector2(10f * currentScale, 10f * currentScale);
			doppelgangerSelection.offsetMin = new Vector2(-10f * currentScale, -10f * currentScale);
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	private void Update()
	{
		this.PositionPlanetWidgets();
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
		foreach (KeyValuePair<SpaceDestination, GameObject> keyValuePair in this.planetWidgets)
		{
			keyValuePair.Value.rectTransform().anchoredPosition = new Vector2(keyValuePair.Key.GetCurrentOrbitPercentage() * this.planetRows[keyValuePair.Key.distance - 1].rectTransform().sizeDelta.x, 0f);
			keyValuePair.Value.GetComponent<HierarchyReferences>().GetReference("Doppelganger").rectTransform()
				.anchoredPosition = Vector2.left * this.planetRows[keyValuePair.Key.distance - 1].rectTransform().sizeDelta.x;
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
		this.showingRocketDetails = false;
		this.listPanel.SetActive(true);
		this.rocketPanel.SetActive(false);
		this.launchButton.ChangeState(1);
		this.distanceOverlayEnabled = false;
		this.UpdateDistanceOverlay(null);
		this.UpdateMissionOverlay(null);
	}

	private void ShowRocketDetailsPanel()
	{
		this.showingRocketDetails = true;
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
				Image selectionOutline = hierarchyReferences.GetReference<RectTransform>("SelectionOutline").GetComponent<Image>();
				MultiToggle component4 = hierarchyReferences.GetReference<RectTransform>("LaunchRocketButton").GetComponent<MultiToggle>();
				HierarchyReferences component5 = hierarchyReferences.GetReference<RectTransform>("ProgressBar").GetComponent<HierarchyReferences>();
				LaunchConditionManager launchConditionManager = <FillRocketListPanel>c__AnonStorey.rocket.launchConditions;
				CommandModule component6 = launchConditionManager.GetComponent<CommandModule>();
				MinionStorage component7 = launchConditionManager.GetComponent<MinionStorage>();
				component3.SetTitle(<FillRocketListPanel>c__AnonStorey.rocket.rocketName);
				component3.OnNameChanged += delegate(string newName)
				{
					<FillRocketListPanel>c__AnonStorey.rocket.SetRocketName(newName);
				};
				MultiToggle multiToggle = component2;
				multiToggle.onEnter = (global::System.Action)Delegate.Combine(multiToggle.onEnter, new global::System.Action(delegate
				{
					selectionOutline.SetAlpha(1f);
					if (<FillRocketListPanel>c__AnonStorey.rocket.state != Spacecraft.MissionState.Grounded)
					{
						<FillRocketListPanel>c__AnonStorey.$this.rocketInMissionHovered = true;
					}
					LaunchConditionManager launchConditions = <FillRocketListPanel>c__AnonStorey.rocket.launchConditions;
					CommandModule component9 = launchConditionManager.GetComponent<CommandModule>();
					<FillRocketListPanel>c__AnonStorey.$this.UpdateDistanceOverlay(component9);
					<FillRocketListPanel>c__AnonStorey.$this.UpdateMissionOverlay(component9);
				}));
				MultiToggle multiToggle2 = component2;
				multiToggle2.onExit = (global::System.Action)Delegate.Combine(multiToggle2.onExit, new global::System.Action(delegate
				{
					selectionOutline.SetAlpha(0f);
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
					CommandModule component10 = launchConditionManager.GetComponent<CommandModule>();
					<FillRocketListPanel>c__AnonStorey.$this.currentCommandModule = component10;
					<FillRocketListPanel>c__AnonStorey.$this.currentLaunchConditionManager = launchConditions2;
					if (!component10.GetComponent<RocketModule>().IsSuspended())
					{
						Vector3 position = component10.transform.position;
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
				component4.play_sound_on_click = false;
				MultiToggle multiToggle4 = component4;
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
				if (component7 != null)
				{
					List<MinionStorage.Info> storedMinionInfo = component7.GetStoredMinionInfo();
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
					breakdownListRow4.ShowData(UI.STARMAP.LISTTITLES.MAXRANGE, this.DisplayDistance(component6.GetRocketMaxDistance()));
					component4.GetComponent<RectTransform>().SetAsLastSibling();
					component4.gameObject.SetActive(true);
					component5.gameObject.SetActive(false);
				}
				else
				{
					float duration = <FillRocketListPanel>c__AnonStorey.rocket.GetDuration();
					float timeLeft = <FillRocketListPanel>c__AnonStorey.rocket.GetTimeLeft();
					float num = ((duration != 0f) ? (1f - timeLeft / duration) : 0f);
					BreakdownListRow breakdownListRow5 = component.AddRow();
					breakdownListRow5.ShowData(UI.STARMAP.ROCKETSTATUS.TIMEREMAINING, global::Util.FormatOneDecimalPlace(timeLeft / 600f) + " / " + GameUtil.GetFormattedCycles(duration, "F1"));
					component5.gameObject.SetActive(true);
					RectTransform reference = component5.GetReference<RectTransform>("ProgressImage");
					LocText component8 = component5.GetReference<RectTransform>("ProgressText").GetComponent<LocText>();
					reference.transform.localScale = new Vector3(num, 1f, 1f);
					component8.text = GameUtil.GetFormattedPercent(num * 100f, GameUtil.TimeSlice.None);
					component5.GetComponent<RectTransform>().SetAsLastSibling();
					component4.gameObject.SetActive(false);
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
		foreach (RocketLaunchCondition rocketLaunchCondition in launchConditionManager.conditions)
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

	private void Refresh(object data = null)
	{
		this.FillRocketListPanel();
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
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.currentCommandModule.GetComponent<AttachableBuilding>()))
		{
			gameObject.GetComponent<RocketModule>();
			RocketEngine component = gameObject.GetComponent<RocketEngine>();
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			CargoBay component3 = gameObject.GetComponent<CargoBay>();
			FuelTank component4 = gameObject.GetComponent<FuelTank>();
			float mass = component2.Mass;
			if (mass > 0f)
			{
				BreakdownListRow breakdownListRow = this.rocketDetailsMass.AddRow();
				breakdownListRow.ShowData(gameObject.GetProperName(), GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
				num += mass;
			}
			if (component != null)
			{
				BreakdownListRow breakdownListRow2 = this.rocketDetailsRange.AddRow();
				breakdownListRow2.ShowStatusData(gameObject.GetProperName(), string.Empty, Color.green);
				breakdownListRow2.SetHighlighted(true);
				List<Tuple<RocketModule, float>> moduleThrustContributions = this.currentCommandModule.GetModuleThrustContributions(this.currentCommandModule.GetComponent<LaunchableRocket>());
				foreach (Tuple<RocketModule, float> tuple in moduleThrustContributions)
				{
					BreakdownListRow breakdownListRow3 = this.rocketDetailsRange.AddRow();
					breakdownListRow3.ShowData(string.Format(UI.STARMAP.SUBROW, tuple.first.GetProperName()), this.DisplayDistance(tuple.second));
					breakdownListRow3.HideIcon();
				}
			}
			if (component3 != null)
			{
				BreakdownListRow breakdownListRow4 = this.rocketDetailsStorage.AddRow();
				float num6 = component3.storage.MassStored();
				num2 += num6;
				float num7 = component3.storage.Capacity();
				num3 += num7;
				string text = string.Format(UI.STARMAP.MODULE_STORAGE, GameUtil.GetFormattedMass(num6, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, false, "{0:0.#}"), GameUtil.GetFormattedMass(num7, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
				breakdownListRow4.ShowStatusData(gameObject.GetProperName(), text, (num6 <= 0f) ? Color.green : Color.red);
				this.currentRocketHasLiquidContainer = component3.storageType == CargoBay.CargoType.liquids;
				this.currentRocketHasGasContainer = component3.storageType == CargoBay.CargoType.gasses;
				this.currentRocketHasSolidContainer = component3.storageType == CargoBay.CargoType.solids;
				this.currentRocketHasEntitiesContainer = component3.storageType == CargoBay.CargoType.entities;
			}
			if (component4 != null)
			{
				BreakdownListRow breakdownListRow5 = this.rocketDetailsFuel.AddRow();
				float num8 = component4.MassStored();
				num4 += num8;
				float num9 = component4.Capacity();
				num5 += num9;
				string text2 = string.Format(UI.STARMAP.MODULE_STORAGE, GameUtil.GetFormattedMass(num8, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, false, "{0:0.#}"), GameUtil.GetFormattedMass(num9, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
				breakdownListRow5.ShowStatusData(gameObject.GetProperName(), text2, (num8 != num5) ? Color.red : Color.green);
			}
		}
		Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this.currentLaunchConditionManager);
		if (spacecraftFromLaunchConditionManager != null)
		{
			if (spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.Grounded)
			{
				this.rocketDetailsChecklist.gameObject.SetActive(true);
				this.rocketDetailsStatus.gameObject.SetActive(false);
				this.rocketInMissionSelected = false;
			}
			else
			{
				this.rocketDetailsChecklist.gameObject.SetActive(false);
				this.rocketDetailsStatus.gameObject.SetActive(true);
				this.rocketInMissionSelected = true;
				float duration = spacecraftFromLaunchConditionManager.GetDuration();
				float timeLeft = spacecraftFromLaunchConditionManager.GetTimeLeft();
				BreakdownListRow breakdownListRow6 = this.rocketDetailsStatus.AddRow();
				breakdownListRow6.ShowData(UI.STARMAP.ROCKETSTATUS.TIMEREMAINING, global::Util.FormatOneDecimalPlace(timeLeft / 600f) + " / " + GameUtil.GetFormattedCycles(duration, "F1"));
			}
		}
		BreakdownListRow breakdownListRow7 = this.rocketDetailsMass.AddRow();
		breakdownListRow7.ShowData(UI.STARMAP.ROCKETSTATUS.TOTAL, GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		breakdownListRow7.SetImportant(true);
		this.rocketThrustWidget.gameObject.SetActive(true);
		float num10 = -(this.currentCommandModule.GetTotalThrust() - this.currentCommandModule.GetRocketMaxDistance());
		BreakdownListRow breakdownListRow8 = this.rocketDetailsRange.AddRow();
		breakdownListRow8.ShowStatusData(UI.STARMAP.ROCKETSTATUS.WEIGHTPENALTY, this.DisplayDistance(num10), Color.red);
		breakdownListRow8.SetHighlighted(true);
		this.rocketDetailsRange.AddCustomRow(this.rocketThrustWidget.gameObject);
		this.rocketThrustWidget.Draw(this.currentCommandModule);
		this.rangeRowTotal = this.rocketDetailsRange.AddRow();
		this.rangeRowTotal.ShowData(UI.STARMAP.ROCKETSTATUS.TOTAL, this.DisplayDistance(this.currentCommandModule.GetRocketMaxDistance()));
		this.rangeRowTotal.SetImportant(true);
		if (this.currentCommandModule != null && this.selectedDestination != null)
		{
			this.rangeRowTotal.SetStatusColor((!this.currentCommandModule.reachable.CanReachDestination(this.selectedDestination)) ? Color.red : Color.green);
		}
		BreakdownListRow breakdownListRow9 = this.rocketDetailsStorage.AddRow();
		string text3 = string.Format(UI.STARMAP.MODULE_STORAGE, GameUtil.GetFormattedMass(num2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, false, "{0:0.#}"), GameUtil.GetFormattedMass(num3, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		breakdownListRow9.ShowStatusData(UI.STARMAP.ROCKETSTATUS.TOTAL, text3, (num2 <= 0f) ? Color.green : Color.red);
		breakdownListRow9.SetImportant(true);
		BreakdownListRow breakdownListRow10 = this.rocketDetailsFuel.AddRow();
		string text4 = string.Format(UI.STARMAP.MODULE_STORAGE, GameUtil.GetFormattedMass(num4, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, false, "{0:0.#}"), GameUtil.GetFormattedMass(num5, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Tonne, true, "{0:0.#}"));
		breakdownListRow10.ShowData(UI.STARMAP.ROCKETSTATUS.TOTAL, text4);
		breakdownListRow10.ShowStatusData(UI.STARMAP.ROCKETSTATUS.TOTAL, text4, (num4 != num5) ? Color.red : Color.green);
		breakdownListRow10.SetImportant(true);
		MinionStorage component5 = this.currentCommandModule.GetComponent<MinionStorage>();
		if (component5 != null)
		{
			List<MinionStorage.Info> storedMinionInfo = component5.GetStoredMinionInfo();
			foreach (MinionStorage.Info info in storedMinionInfo)
			{
				BreakdownListRow breakdownListRow11 = this.rocketDetailsDupes.AddRow();
				breakdownListRow11.ShowData(info.name, string.Empty);
			}
			if (storedMinionInfo.Count == 0)
			{
				BreakdownListRow breakdownListRow12 = this.rocketDetailsDupes.AddRow();
				breakdownListRow12.ShowStatusData(UI.STARMAP.ROCKETSTATUS.NOPASSENGERS, string.Empty, Color.red);
			}
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(this.rocketDetailsContainer);
	}

	private void ClearDestinationPanel()
	{
		this.destinationDetailsContainer.gameObject.SetActive(false);
	}

	private void ShowDestinationPanel()
	{
		this.destinationStatusLabel.text = UI.STARMAP.ROCKETSTATUS.SELECTED;
		this.destinationNameLabel.text = this.selectedDestination.name;
		this.destinationTypeValueLabel.text = this.selectedDestination.typeName;
		this.destinationDistanceValueLabel.text = this.DisplayDistance((float)this.selectedDestination.distance * 10000f);
		this.destinationDescriptionLabel.text = this.selectedDestination.description;
		this.destinationDetailsComposition.ClearRows();
		float num = 0f;
		foreach (KeyValuePair<SimHashes, float> keyValuePair in this.selectedDestination.recoverableElements)
		{
			num += keyValuePair.Value;
		}
		this.destinationDetailsResearch.gameObject.SetActive(false);
		foreach (KeyValuePair<SimHashes, float> keyValuePair2 in this.selectedDestination.recoverableElements)
		{
			BreakdownListRow breakdownListRow = this.destinationDetailsComposition.AddRow();
			float num2 = keyValuePair2.Value / num * 100f;
			Element element = ElementLoader.FindElementByHash(keyValuePair2.Key);
			Tuple<Sprite, Color> uisprite = Def.GetUISprite(element, "ui", false);
			breakdownListRow.ShowIconData(element.name, GameUtil.GetFormattedPercent(num2, GameUtil.TimeSlice.None), uisprite.first, uisprite.second);
			if (element.IsGas)
			{
				string properName = Assets.GetPrefab("GasCargoBay".ToTag()).GetProperName();
				if (this.currentRocketHasGasContainer)
				{
					breakdownListRow.SetHighlighted(true);
					breakdownListRow.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, element.name, properName));
				}
				else
				{
					breakdownListRow.SetDisabled(true);
					breakdownListRow.AddTooltip(string.Format(UI.STARMAP.CONTAINER_REQUIRED, properName));
				}
			}
			if (element.IsLiquid)
			{
				string properName2 = Assets.GetPrefab("LiquidCargoBay".ToTag()).GetProperName();
				if (this.currentRocketHasLiquidContainer)
				{
					breakdownListRow.SetHighlighted(true);
					breakdownListRow.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, element.name, properName2));
				}
				else
				{
					breakdownListRow.SetDisabled(true);
					breakdownListRow.AddTooltip(string.Format(UI.STARMAP.CONTAINER_REQUIRED, properName2));
				}
			}
			if (element.IsSolid)
			{
				string properName3 = Assets.GetPrefab("CargoBay".ToTag()).GetProperName();
				if (this.currentRocketHasSolidContainer)
				{
					breakdownListRow.SetHighlighted(true);
					breakdownListRow.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, element.name, properName3));
				}
				else
				{
					breakdownListRow.SetDisabled(true);
					breakdownListRow.AddTooltip(string.Format(UI.STARMAP.CONTAINER_REQUIRED, properName3));
				}
			}
		}
		this.destinationDetailsResources.ClearRows();
		foreach (KeyValuePair<string, int> keyValuePair3 in this.selectedDestination.recoverableEntities)
		{
			BreakdownListRow breakdownListRow2 = this.destinationDetailsResources.AddRow();
			GameObject prefab = Assets.GetPrefab(keyValuePair3.Key);
			Tuple<Sprite, Color> uisprite2 = Def.GetUISprite(prefab, "ui", false);
			breakdownListRow2.ShowIconData(prefab.GetProperName(), string.Empty, uisprite2.first, uisprite2.second);
			string properName4 = Assets.GetPrefab("SpecialCargoBay".ToTag()).GetProperName();
			if (this.currentRocketHasEntitiesContainer)
			{
				breakdownListRow2.SetHighlighted(true);
				breakdownListRow2.AddTooltip(string.Format(UI.STARMAP.CAN_CARRY_ELEMENT, prefab.GetProperName(), properName4));
			}
			else
			{
				breakdownListRow2.SetDisabled(true);
				breakdownListRow2.AddTooltip(string.Format(UI.STARMAP.CANT_CARRY_ELEMENT, properName4, prefab.GetProperName()));
			}
		}
		this.destinationDetailsContainer.gameObject.SetActive(true);
		LayoutRebuilder.ForceRebuildLayoutImmediate(this.destinationDetailsContainer);
	}

	private void ValidateTravelAbility()
	{
		if (this.selectedDestination != null && this.currentCommandModule != null && this.currentLaunchConditionManager != null)
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
			float num = previewCommandModule.GetRocketMaxDistance();
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

	public LocText rocketHeaderLabel;

	public LocText rocketHeaderStatusLabel;

	public BreakdownList rocketDetailsStatus;

	public Sprite rocketDetailsStatusIcon;

	public BreakdownList rocketDetailsChecklist;

	public Sprite rocketDetailsChecklistIcon;

	public BreakdownList rocketDetailsMass;

	public Sprite rocketDetailsMassIcon;

	public BreakdownList rocketDetailsRange;

	public Sprite rocketDetailsRangeIcon;

	public RocketThrustWidget rocketThrustWidget;

	public BreakdownList rocketDetailsStorage;

	public Sprite rocketDetailsStorageIcon;

	public BreakdownList rocketDetailsDupes;

	public Sprite rocketDetailsDupesIcon;

	public BreakdownList rocketDetailsFuel;

	public Sprite rocketDetailsFuelIcon;

	public BreakdownList rocketDetailsOxidizer;

	public Sprite rocketDetailsOxidizerIcon;

	public RectTransform rocketDetailsContainer;

	public LocText destinationHeaderLabel;

	public LocText destinationStatusLabel;

	public LocText destinationNameLabel;

	public LocText destinationTypeNameLabel;

	public LocText destinationTypeValueLabel;

	public LocText destinationDistanceNameLabel;

	public LocText destinationDistanceValueLabel;

	public LocText destinationDescriptionLabel;

	public BreakdownList destinationDetailsResearch;

	public Sprite destinationDetailsResearchIcon;

	public BreakdownList destinationDetailsComposition;

	public Sprite destinationDetailsCompositionIcon;

	public BreakdownList destinationDetailsResources;

	public Sprite destinationDetailsResourcesIcon;

	public RectTransform destinationDetailsContainer;

	public MultiToggle showRocketsButton;

	public MultiToggle launchButton;

	private int rocketConditionEventHandler = -1;

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

	private bool showingRocketDetails;

	private BreakdownListRow rangeRowTotal;
}
