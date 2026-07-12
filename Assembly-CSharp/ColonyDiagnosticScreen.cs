using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ColonyDiagnosticScreen : KScreen, ISim1000ms
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		ColonyDiagnosticScreen.Instance = this;
		this.RefreshSingleWorld(null);
		Game.Instance.Subscribe(1983128072, new Action<object>(this.RefreshSingleWorld));
		MultiToggle multiToggle = this.seeAllButton;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			AllDiagnosticsScreen.Instance.Show(!AllDiagnosticsScreen.Instance.gameObject.activeSelf);
		}));
	}

	private void RefreshSingleWorld(object data = null)
	{
		foreach (ColonyDiagnosticScreen.DiagnosticRow diagnosticRow in this.diagnosticRows)
		{
			diagnosticRow.OnCleanUp();
			Util.KDestroyGameObject(diagnosticRow.gameObject);
		}
		this.diagnosticRows.Clear();
		this.SpawnTrackerLines(ClusterManager.Instance.activeWorldId);
	}

	private void SpawnTrackerLines(int world)
	{
		this.AddDiagnostic<BreathabilityDiagnostic>(world, this.contentContainer, this.diagnosticRows);
		this.AddDiagnostic<FoodDiagnostic>(world, this.contentContainer, this.diagnosticRows);
		this.AddDiagnostic<StressDiagnostic>(world, this.contentContainer, this.diagnosticRows);
		this.AddDiagnostic<RadiationDiagnostic>(world, this.contentContainer, this.diagnosticRows);
		this.AddDiagnostic<ReactorDiagnostic>(world, this.contentContainer, this.diagnosticRows);
		this.AddDiagnostic<FloatingRocketDiagnostic>(world, this.contentContainer, this.diagnosticRows);
		this.AddDiagnostic<RocketFuelDiagnostic>(world, this.contentContainer, this.diagnosticRows);
		this.AddDiagnostic<RocketOxidizerDiagnostic>(world, this.contentContainer, this.diagnosticRows);
		this.AddDiagnostic<FarmDiagnostic>(world, this.contentContainer, this.diagnosticRows);
		this.AddDiagnostic<ToiletDiagnostic>(world, this.contentContainer, this.diagnosticRows);
		this.AddDiagnostic<BedDiagnostic>(world, this.contentContainer, this.diagnosticRows);
		this.AddDiagnostic<IdleDiagnostic>(world, this.contentContainer, this.diagnosticRows);
		this.AddDiagnostic<TrappedDuplicantDiagnostic>(world, this.contentContainer, this.diagnosticRows);
		this.AddDiagnostic<EntombedDiagnostic>(world, this.contentContainer, this.diagnosticRows);
		this.AddDiagnostic<PowerUseDiagnostic>(world, this.contentContainer, this.diagnosticRows);
		this.AddDiagnostic<BatteryDiagnostic>(world, this.contentContainer, this.diagnosticRows);
		this.AddDiagnostic<RocketsInOrbitDiagnostic>(world, this.contentContainer, this.diagnosticRows);
		List<ColonyDiagnosticScreen.DiagnosticRow> list = new List<ColonyDiagnosticScreen.DiagnosticRow>();
		foreach (ColonyDiagnosticScreen.DiagnosticRow diagnosticRow in this.diagnosticRows)
		{
			list.Add(diagnosticRow);
		}
		list.Sort((ColonyDiagnosticScreen.DiagnosticRow a, ColonyDiagnosticScreen.DiagnosticRow b) => a.diagnostic.name.CompareTo(b.diagnostic.name));
		foreach (ColonyDiagnosticScreen.DiagnosticRow diagnosticRow2 in list)
		{
			diagnosticRow2.gameObject.transform.SetAsLastSibling();
		}
		list.Clear();
		this.seeAllButton.transform.SetAsLastSibling();
		this.RefreshAll();
	}

	private GameObject AddDiagnostic<T>(int worldID, GameObject parent, List<ColonyDiagnosticScreen.DiagnosticRow> parentCollection) where T : ColonyDiagnostic
	{
		T diagnostic = ColonyDiagnosticUtility.Instance.GetDiagnostic<T>(worldID);
		if (diagnostic == null)
		{
			return null;
		}
		GameObject gameObject = Util.KInstantiateUI(this.linePrefab, parent, true);
		parentCollection.Add(new ColonyDiagnosticScreen.DiagnosticRow(worldID, gameObject, diagnostic));
		return gameObject;
	}

	public static void SetIndication(ColonyDiagnostic.DiagnosticResult.Opinion opinion, GameObject indicatorGameObject)
	{
		indicatorGameObject.GetComponentInChildren<Image>().color = ColonyDiagnosticScreen.GetDiagnosticIndicationColor(opinion);
	}

	public static Color GetDiagnosticIndicationColor(ColonyDiagnostic.DiagnosticResult.Opinion opinion)
	{
		switch (opinion)
		{
		case ColonyDiagnostic.DiagnosticResult.Opinion.DuplicantThreatening:
		case ColonyDiagnostic.DiagnosticResult.Opinion.Bad:
		case ColonyDiagnostic.DiagnosticResult.Opinion.Warning:
			return Constants.NEGATIVE_COLOR;
		case ColonyDiagnostic.DiagnosticResult.Opinion.Concern:
			return Constants.WARNING_COLOR;
		}
		return Color.white;
	}

	public void Sim1000ms(float dt)
	{
		this.RefreshAll();
	}

	public void RefreshAll()
	{
		string text = "";
		foreach (ColonyDiagnosticScreen.DiagnosticRow diagnosticRow in this.diagnosticRows)
		{
			if (diagnosticRow.worldID == ClusterManager.Instance.activeWorldId)
			{
				this.UpdateDiagnosticRow(diagnosticRow, text);
			}
		}
		ColonyDiagnosticScreen.SetIndication(ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResult(ClusterManager.Instance.activeWorldId), this.rootIndicator);
		this.header.GetComponent<ToolTip>().enabled = !string.IsNullOrEmpty(text);
		this.header.GetComponent<ToolTip>().SetSimpleTooltip(text);
		this.seeAllButton.GetComponentInChildren<LocText>().SetText(string.Format(UI.DIAGNOSTICS_SCREEN.SEE_ALL, AllDiagnosticsScreen.Instance.GetRowCount()));
	}

	private ColonyDiagnostic.DiagnosticResult.Opinion UpdateDiagnosticRow(ColonyDiagnosticScreen.DiagnosticRow row, string tooltipString)
	{
		ColonyDiagnostic.DiagnosticResult.Opinion currentDisplayedResult = row.currentDisplayedResult;
		bool activeInHierarchy = row.gameObject.activeInHierarchy;
		if (row.diagnostic.LatestResult.opinion < ColonyDiagnostic.DiagnosticResult.Opinion.Normal)
		{
			if (!string.IsNullOrEmpty(tooltipString))
			{
				tooltipString += "\n";
			}
			tooltipString += row.diagnostic.LatestResult.Message;
		}
		if (ColonyDiagnosticUtility.Instance.IsDiagnosticTutorialDisabled(row.diagnostic.id))
		{
			this.SetRowActive(row, false);
		}
		else
		{
			switch (ColonyDiagnosticUtility.Instance.diagnosticDisplaySettings[row.worldID][row.diagnostic.id])
			{
			case ColonyDiagnosticUtility.DisplaySetting.Always:
				this.SetRowActive(row, true);
				break;
			case ColonyDiagnosticUtility.DisplaySetting.AlertOnly:
				this.SetRowActive(row, row.diagnostic.LatestResult.opinion < ColonyDiagnostic.DiagnosticResult.Opinion.Normal);
				break;
			case ColonyDiagnosticUtility.DisplaySetting.Never:
				this.SetRowActive(row, false);
				break;
			}
			if (row.gameObject.activeInHierarchy && (row.currentDisplayedResult < currentDisplayedResult || (row.currentDisplayedResult < ColonyDiagnostic.DiagnosticResult.Opinion.Normal && !activeInHierarchy)) && row.CheckAllowVisualNotification())
			{
				row.TriggerVisualNotification();
			}
		}
		return row.diagnostic.LatestResult.opinion;
	}

	private void SetRowActive(ColonyDiagnosticScreen.DiagnosticRow row, bool active)
	{
		if (row.gameObject.activeSelf != active)
		{
			row.gameObject.SetActive(active);
			row.ResolveNotificationRoutine();
		}
	}

	public GameObject linePrefab;

	public static ColonyDiagnosticScreen Instance;

	private List<ColonyDiagnosticScreen.DiagnosticRow> diagnosticRows = new List<ColonyDiagnosticScreen.DiagnosticRow>();

	public GameObject header;

	public GameObject contentContainer;

	public GameObject rootIndicator;

	public MultiToggle seeAllButton;

	public static Dictionary<ColonyDiagnostic.DiagnosticResult.Opinion, string> notificationSoundsActive = new Dictionary<ColonyDiagnostic.DiagnosticResult.Opinion, string>
	{
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.DuplicantThreatening,
			"Diagnostic_Active_DuplicantThreatening"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Bad,
			"Diagnostic_Active_Bad"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Warning,
			"Diagnostic_Active_Warning"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Concern,
			"Diagnostic_Active_Concern"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Suggestion,
			"Diagnostic_Active_Suggestion"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Tutorial,
			"Diagnostic_Active_Tutorial"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Normal,
			""
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Good,
			""
		}
	};

	public static Dictionary<ColonyDiagnostic.DiagnosticResult.Opinion, string> notificationSoundsInactive = new Dictionary<ColonyDiagnostic.DiagnosticResult.Opinion, string>
	{
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.DuplicantThreatening,
			"Diagnostic_Inactive_DuplicantThreatening"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Bad,
			"Diagnostic_Inactive_Bad"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Warning,
			"Diagnostic_Inactive_Warning"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Concern,
			"Diagnostic_Inactive_Concern"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Suggestion,
			"Diagnostic_Inactive_Suggestion"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Tutorial,
			"Diagnostic_Inactive_Tutorial"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Normal,
			""
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Good,
			""
		}
	};

	private class DiagnosticRow : ISim4000ms
	{
		public DiagnosticRow(int worldID, GameObject gameObject, ColonyDiagnostic diagnostic)
		{
			global::Debug.Assert(diagnostic != null);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			this.worldID = worldID;
			this.sparkLayer = component.GetReference<SparkLayer>("SparkLayer");
			this.diagnostic = diagnostic;
			this.titleLabel = component.GetReference<LocText>("TitleLabel");
			this.valueLabel = component.GetReference<LocText>("ValueLabel");
			this.indicator = component.GetReference<Image>("Indicator");
			this.image = component.GetReference<Image>("Image");
			this.tooltip = gameObject.GetComponent<ToolTip>();
			this.gameObject = gameObject;
			this.titleLabel.SetText(diagnostic.name);
			this.sparkLayer.colorRules.setOwnColor = false;
			if (diagnostic.tracker == null)
			{
				this.sparkLayer.transform.parent.gameObject.SetActive(false);
			}
			else
			{
				this.sparkLayer.ClearLines();
				global::Tuple<float, float>[] array = diagnostic.tracker.ChartableData(600f);
				this.sparkLayer.NewLine(array, diagnostic.name);
			}
			this.button = gameObject.GetComponent<MultiToggle>();
			MultiToggle multiToggle = this.button;
			multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
			{
				if (diagnostic.LatestResult.clickThroughTarget == null)
				{
					CameraController.Instance.ActiveWorldStarWipe(diagnostic.worldID, null);
					return;
				}
				SelectTool.Instance.SelectAndFocus(diagnostic.LatestResult.clickThroughTarget.first, (diagnostic.LatestResult.clickThroughTarget.second == null) ? null : diagnostic.LatestResult.clickThroughTarget.second.GetComponent<KSelectable>());
			}));
			this.defaultIndicatorSizeDelta = Vector2.zero;
			this.Update();
			SimAndRenderScheduler.instance.Add(this, true);
		}

		public void OnCleanUp()
		{
			SimAndRenderScheduler.instance.Remove(this);
		}

		public void Sim4000ms(float dt)
		{
			this.Update();
		}

		public GameObject gameObject { get; private set; }

		public void Update()
		{
			Color color = Color.white;
			global::Debug.Assert(this.diagnostic.LatestResult.opinion > ColonyDiagnostic.DiagnosticResult.Opinion.Unset, string.Format("{0} criteria returned no opinion. Make sure the DiagnosticResult parameters are used or an opinion result is otherwise set in all of its criteria", this.diagnostic));
			this.currentDisplayedResult = this.diagnostic.LatestResult.opinion;
			color = this.diagnostic.colors[this.diagnostic.LatestResult.opinion];
			if (this.diagnostic.tracker != null)
			{
				global::Tuple<float, float>[] array = this.diagnostic.tracker.ChartableData(600f);
				this.sparkLayer.RefreshLine(array, this.diagnostic.name);
				this.sparkLayer.SetColor(color);
			}
			this.indicator.color = this.diagnostic.colors[this.diagnostic.LatestResult.opinion];
			this.tooltip.SetSimpleTooltip((this.diagnostic.LatestResult.Message.IsNullOrWhiteSpace() ? UI.COLONY_DIAGNOSTICS.GENERIC_STATUS_NORMAL.text : this.diagnostic.LatestResult.Message) + "\n\n" + UI.COLONY_DIAGNOSTICS.MUTE_TUTORIAL.text);
			ColonyDiagnostic.PresentationSetting presentationSetting = this.diagnostic.presentationSetting;
			if (presentationSetting == ColonyDiagnostic.PresentationSetting.AverageValue || presentationSetting != ColonyDiagnostic.PresentationSetting.CurrentValue)
			{
				this.valueLabel.SetText(this.diagnostic.GetAverageValueString());
			}
			else
			{
				this.valueLabel.SetText(this.diagnostic.GetCurrentValueString());
			}
			if (!string.IsNullOrEmpty(this.diagnostic.icon))
			{
				this.image.sprite = Assets.GetSprite(this.diagnostic.icon);
			}
			if (color == Constants.NEUTRAL_COLOR)
			{
				color = Color.white;
			}
			this.titleLabel.color = color;
		}

		public bool CheckAllowVisualNotification()
		{
			return this.timeOfLastNotification == 0f || GameClock.Instance.GetTime() >= this.timeOfLastNotification + 300f;
		}

		public void TriggerVisualNotification()
		{
			if (DebugHandler.NotificationsDisabled)
			{
				return;
			}
			if (this.activeRoutine == null)
			{
				this.timeOfLastNotification = GameClock.Instance.GetTime();
				KFMOD.PlayUISound(GlobalAssets.GetSound(ColonyDiagnosticScreen.notificationSoundsActive[this.currentDisplayedResult], false));
				this.activeRoutine = this.gameObject.GetComponent<KMonoBehaviour>().StartCoroutine(this.VisualNotificationRoutine());
			}
		}

		private IEnumerator VisualNotificationRoutine()
		{
			this.gameObject.GetComponentInChildren<NotificationAnimator>().Begin(false);
			RectTransform indicator = this.gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("Indicator").rectTransform;
			this.defaultIndicatorSizeDelta = Vector2.zero;
			indicator.sizeDelta = this.defaultIndicatorSizeDelta;
			float bounceDuration = 3f;
			for (float i = 0f; i < bounceDuration; i += Time.unscaledDeltaTime)
			{
				indicator.sizeDelta = this.defaultIndicatorSizeDelta + Vector2.one * (float)Mathf.RoundToInt(Mathf.Sin(6f * (3.1415927f * (i / bounceDuration))));
				yield return 0;
			}
			for (float i = 0f; i < bounceDuration; i += Time.unscaledDeltaTime)
			{
				indicator.sizeDelta = this.defaultIndicatorSizeDelta + Vector2.one * (float)Mathf.RoundToInt(Mathf.Sin(6f * (3.1415927f * (i / bounceDuration))));
				yield return 0;
			}
			for (float i = 0f; i < bounceDuration; i += Time.unscaledDeltaTime)
			{
				indicator.sizeDelta = this.defaultIndicatorSizeDelta + Vector2.one * (float)Mathf.RoundToInt(Mathf.Sin(6f * (3.1415927f * (i / bounceDuration))));
				yield return 0;
			}
			this.ResolveNotificationRoutine();
			yield break;
		}

		public void ResolveNotificationRoutine()
		{
			this.gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("Indicator").rectTransform.sizeDelta = Vector2.zero;
			this.gameObject.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Content").localPosition = Vector2.zero;
			this.activeRoutine = null;
		}

		private const float displayHistoryPeriod = 600f;

		public ColonyDiagnostic diagnostic;

		public SparkLayer sparkLayer;

		public int worldID;

		private LocText titleLabel;

		private LocText valueLabel;

		private Image indicator;

		private ToolTip tooltip;

		private MultiToggle button;

		private Image image;

		public ColonyDiagnostic.DiagnosticResult.Opinion currentDisplayedResult;

		private Vector2 defaultIndicatorSizeDelta;

		private float timeOfLastNotification;

		private const float MIN_TIME_BETWEEN_NOTIFICATIONS = 300f;

		private Coroutine activeRoutine;
	}
}
