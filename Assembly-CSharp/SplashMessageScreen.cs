using System;
using System.Collections;

public class SplashMessageScreen : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.confirmButton.onClick += delegate
		{
			base.gameObject.SetActive(false);
		};
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
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
