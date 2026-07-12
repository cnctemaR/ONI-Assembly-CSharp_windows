using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.Events;

public class OptionsMenuScreen : KModalButtonMenu
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.keepMenuOpen = true;
		this.buttons = new List<KButtonMenu.ButtonInfo>
		{
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.GRAPHICS, global::Action.NumActions, new UnityAction(this.OnGraphicsOptions), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.AUDIO, global::Action.NumActions, new UnityAction(this.OnAudioOptions), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.GAME, global::Action.NumActions, new UnityAction(this.OnGameOptions), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.METRICS, global::Action.NumActions, new UnityAction(this.OnMetrics), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.FEEDBACK, global::Action.NumActions, new UnityAction(this.OnFeedback), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.CREDITS, global::Action.NumActions, new UnityAction(this.OnCredits), null, null)
		};
		this.closeButton.onClick += this.Deactivate;
		this.backButton.onClick += this.Deactivate;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.title.SetText(UI.FRONTEND.OPTIONS_SCREEN.TITLE);
		this.backButton.transform.SetAsLastSibling();
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		foreach (GameObject gameObject in this.buttonObjects)
		{
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
		{
			this.Deactivate();
			return;
		}
		base.OnKeyDown(e);
	}

	private void OnGraphicsOptions()
	{
		base.ActivateChildScreen(this.graphicsOptionsScreenPrefab.gameObject);
	}

	private void OnAudioOptions()
	{
		base.ActivateChildScreen(this.audioOptionsScreenPrefab.gameObject);
	}

	private void OnGameOptions()
	{
		base.ActivateChildScreen(this.gameOptionsScreenPrefab.gameObject);
	}

	private void OnMetrics()
	{
		base.ActivateChildScreen(this.metricsScreenPrefab.gameObject);
	}

	public void ShowMetricsScreen()
	{
		base.ActivateChildScreen(this.metricsScreenPrefab.gameObject);
	}

	private void OnFeedback()
	{
		base.ActivateChildScreen(this.feedbackScreenPrefab.gameObject);
	}

	private void OnCredits()
	{
		base.ActivateChildScreen(this.creditsScreenPrefab.gameObject);
	}

	private void Update()
	{
		global::Debug.developerConsoleVisible = false;
	}

	[SerializeField]
	private GameOptionsScreen gameOptionsScreenPrefab;

	[SerializeField]
	private AudioOptionsScreen audioOptionsScreenPrefab;

	[SerializeField]
	private GraphicsOptionsScreen graphicsOptionsScreenPrefab;

	[SerializeField]
	private CreditsScreen creditsScreenPrefab;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private MetricsOptionsScreen metricsScreenPrefab;

	[SerializeField]
	private FeedbackScreen feedbackScreenPrefab;

	[SerializeField]
	private LocText title;

	[SerializeField]
	private KButton backButton;
}
