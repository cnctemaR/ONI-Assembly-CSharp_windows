using System;
using FMOD.Studio;
using UnityEngine;

public class DLCBetaMessageScreen : KModalScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.confirmButton.onClick += delegate
		{
			base.gameObject.SetActive(false);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot, STOP_MODE.ALLOWFADEOUT);
		};
		this.quitButton.onClick += delegate
		{
			App.Quit();
		};
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (!this.betaIsLive || (Application.isEditor && this.skipInEditor) || !DlcManager.GetActiveDLCIds().Contains("DLC2_ID"))
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot);
	}

	private void Update()
	{
		this.logo.rectTransform().localPosition = new Vector3(0f, Mathf.Sin(Time.realtimeSinceStartup) * 7.5f);
	}

	public RectTransform logo;

	public KButton confirmButton;

	public KButton quitButton;

	public LocText bodyText;

	public RectTransform messageContainer;

	private bool betaIsLive;

	private bool skipInEditor;
}
