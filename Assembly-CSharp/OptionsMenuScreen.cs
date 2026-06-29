using System;
using System.Collections.Generic;
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
		this.buttons = new List<KButtonMenu.ButtonInfo>
		{
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.GRAPHICS, global::Action.NumActions, new UnityAction(this.OnGraphicsOptions), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.AUDIO, global::Action.NumActions, new UnityAction(this.OnAudioOptions), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.CONTROLS, global::Action.NumActions, new UnityAction(this.OnKeyBindings), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.UNITS, global::Action.NumActions, new UnityAction(this.OnUnits), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.METRICS, global::Action.NumActions, new UnityAction(this.OnMetrics), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.CREDITS, global::Action.NumActions, new UnityAction(this.OnCredits), null, null),
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.RESET_TUTORIAL, global::Action.NumActions, new UnityAction(this.OnTutorialReset), null, null)
		};
		if (SaveGame.Instance != null && !SaveGame.Instance.sandboxEnabled)
		{
			this.buttons.Add(new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.UNLOCK_SANDBOX, global::Action.NumActions, new UnityAction(this.OnUnlockSandboxMode), null, null));
		}
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

	private void OnUnlockSandboxMode()
	{
		ConfirmDialogScreen component = base.ActivateChildScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject).GetComponent<ConfirmDialogScreen>();
		ConfirmDialogScreen confirmDialogScreen = component;
		string text = UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.UNLOCK_SANDBOX_WARNING;
		global::System.Action action = delegate
		{
			SaveGame.Instance.sandboxEnabled = true;
			TopLeftControlScreen.Instance.UpdateSandboxToggleState();
			this.Deactivate();
		};
		global::System.Action action2 = delegate
		{
			string text4 = SaveLoader.GetSavePrefixAndCreateFolder();
			string text5 = text4;
			text4 = string.Concat(new string[]
			{
				text5,
				"\\",
				SaveGame.Instance.BaseName,
				UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.BACKUP_SAVE_GAME_APPEND,
				".sav"
			});
			SaveLoader.Instance.Save(text4, false, false);
			SaveGame.Instance.sandboxEnabled = true;
			TopLeftControlScreen.Instance.UpdateSandboxToggleState();
			this.Deactivate();
		};
		string text2 = UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.CANCEL;
		global::System.Action action3 = delegate
		{
		};
		string text3 = UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.CONFIRM_SAVE_BACKUP;
		confirmDialogScreen.PopupConfirmDialog(text, action, action2, text2, action3, null, UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.CONFIRM, text3);
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
