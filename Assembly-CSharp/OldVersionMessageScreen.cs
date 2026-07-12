using System;
using FMOD.Studio;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SplashMessageScreen")]
public class OldVersionMessageScreen : KModalScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.forumButton.onClick += delegate
		{
			App.OpenWebURL("https://forums.kleientertainment.com/forums/topic/140474-previous-update-steam-branch-access/");
		};
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
		this.messageContainer.sizeDelta = new Vector2(Mathf.Max(384f, (float)Screen.width * 0.25f), this.messageContainer.sizeDelta.y);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot);
	}

	public KButton forumButton;

	public KButton confirmButton;

	public KButton quitButton;

	public LocText bodyText;

	public bool previewInEditor;

	public RectTransform messageContainer;
}
