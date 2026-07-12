using System;
using FMOD.Studio;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/SplashMessageScreen")]
public class SplashMessageScreen : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.forumButton.onClick += delegate
		{
			App.OpenWebURL("https://forums.kleientertainment.com/forums/forum/118-oxygen-not-included/");
		};
		this.confirmButton.onClick += delegate
		{
			base.gameObject.SetActive(false);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot, STOP_MODE.ALLOWFADEOUT);
		};
		this.bodyText.text = UI.DEVELOPMENTBUILDS.ALPHA.LOADING.BODY;
	}

	private void OnEnable()
	{
		this.confirmButton.GetComponent<LayoutElement>();
		this.confirmButton.GetComponentInChildren<LocText>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (!DlcManager.IsExpansion1Active())
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot);
	}

	public KButton forumButton;

	public KButton confirmButton;

	public LocText bodyText;

	public bool previewInEditor;
}
