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
			new KButtonMenu.ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.BACK, global::Action.NumActions, new UnityAction(this.Deactivate), null, null)
		};
		this.closeButton.onClick += this.Deactivate;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.title.SetText(UI.FRONTEND.OPTIONS_SCREEN.TITLE);
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
		Util.KInstantiateUI(this.graphicsOptionsScreenPrefab.gameObject, this.transform.parent.gameObject, false);
	}

	private void OnAudioOptions()
	{
		Util.KInstantiateUI(this.audioOptionsScreenPrefab.gameObject, this.transform.parent.gameObject, false);
	}

	private void OnKeyBindings()
	{
		Util.KInstantiateUI(this.inputBindingsScreenPrefab.gameObject, this.transform.parent.gameObject, false);
	}

	private void OnUnits()
	{
		Util.KInstantiateUI(this.unitScreenPrefab.gameObject, this.transform.parent.gameObject, false);
	}

	private void OnMetrics()
	{
		Util.KInstantiateUI(this.metricsScreenPrefab.gameObject, this.transform.parent.gameObject, false);
	}

	private void OnCredits()
	{
		Util.KInstantiateUI(this.creditsScreenPrefab.gameObject, this.transform.parent.gameObject, false);
	}

	private void Update()
	{
		Debug.developerConsoleVisible = false;
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
}
