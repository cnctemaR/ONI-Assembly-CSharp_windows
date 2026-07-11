using System;
using System.Collections;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/SplashMessageScreen")]
public class SplashMessageScreen : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.confirmButton.onClick += delegate
		{
			base.gameObject.SetActive(false);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot, STOP_MODE.ALLOWFADEOUT);
		};
	}

	private void OnEnable()
	{
		LayoutElement component = this.confirmButton.GetComponent<LayoutElement>();
		LocText componentInChildren = this.confirmButton.GetComponentInChildren<LocText>();
		if (Screen.width > 2560)
		{
			component.minWidth = 720f;
			component.minHeight = 128f;
			this.bodyText.minWidth = 840f;
			componentInChildren.fontSizeMax = 24f;
			return;
		}
		if (Screen.width > 1920)
		{
			component.minWidth = 720f;
			component.minHeight = 128f;
			this.bodyText.minWidth = 700f;
			componentInChildren.fontSizeMax = 24f;
			return;
		}
		if (Screen.width > 1280)
		{
			component.minWidth = 440f;
			component.minHeight = 64f;
			this.bodyText.minWidth = 480f;
			componentInChildren.fontSizeMax = 18f;
			return;
		}
		component.minWidth = 300f;
		component.minHeight = 48f;
		this.bodyText.minWidth = 300f;
		componentInChildren.fontSizeMax = 16f;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot);
		base.StartCoroutine(this.ShowMessage());
	}

	private IEnumerator ShowMessage()
	{
		yield return null;
		base.GetComponentInChildren<KScreen>(true).Show(true);
		yield break;
	}

	public KButton confirmButton;

	public LayoutElement bodyText;

	public bool previewInEditor;
}
