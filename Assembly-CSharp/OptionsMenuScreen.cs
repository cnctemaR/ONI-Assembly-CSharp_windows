using System;
using STRINGS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionsMenuScreen : KModalButtonMenu
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.keepMenuOpen = true;
		this.buttons = new KButtonMenu.ButtonInfo[]
		{
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.GRAPHICS, global::Action.NumActions, new UnityAction(this.OnGraphicsOptions), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.AUDIO, global::Action.NumActions, new UnityAction(this.OnAudioOptions), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.CONTROLS, global::Action.NumActions, new UnityAction(this.OnKeyBindings), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.UNITS, global::Action.NumActions, new UnityAction(this.OnUnits), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.METRICS, global::Action.NumActions, new UnityAction(this.OnMetrics), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.CREDITS, global::Action.NumActions, new UnityAction(this.OnCredits), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.RESET_TUTORIAL, global::Action.NumActions, new UnityAction(this.OnTutorialReset), null, null)
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
			gameObject.GetComponent<LayoutElement>().minWidth = 512f;
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
		{
			this.Deactivate();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private void OnGraphicsOptions()
	{
		base.ActivateChildScreen(this.graphicsOptionsScreenPrefab.gameObject);
	}

	private void OnAudioOptions()
	{
		base.ActivateChildScreen(this.audioOptionsScreenPrefab.gameObject);
	}

	private void OnKeyBindings()
	{
		base.ActivateChildScreen(this.inputBindingsScreenPrefab.gameObject);
	}

	private void OnUnits()
	{
		base.ActivateChildScreen(this.unitScreenPrefab.gameObject);
	}

	private void OnMetrics()
	{
		base.ActivateChildScreen(this.metricsScreenPrefab.gameObject);
	}

	private void OnCredits()
	{
		base.ActivateChildScreen(this.creditsScreenPrefab.gameObject);
	}

	private void OnTutorialReset()
	{
		ConfirmDialogScreen component = base.ActivateChildScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject).GetComponent<ConfirmDialogScreen>();
		component.PopupConfirmDialog(UI.FRONTEND.OPTIONS_SCREEN.RESET_TUTORIAL_WARNING, delegate
		{
			Tutorial.ResetHiddenTutorialMessages();
		}, delegate
		{
		}, null, null, null, null, null);
		component.Activate();
	}

	private void Update()
	{
		global::Debug.developerConsoleVisible = false;
	}

	[SerializeField]
	private UnitConfigurationScreen unitScreenPrefab;

	[SerializeField]
	private InputBindingsScreen inputBindingsScreenPrefab;

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
	private LocText title;

	[SerializeField]
	private KButton backButton;
}
