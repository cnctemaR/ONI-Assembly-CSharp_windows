using System;
using System.Collections;
using FMOD.Studio;

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
}
