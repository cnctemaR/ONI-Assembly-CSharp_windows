using System;
using UnityEngine;

public class URLOpenFunction : MonoBehaviour
{
	private void Start()
	{
		if (this.triggerButton != null)
		{
			this.triggerButton.ClearOnClick();
			this.triggerButton.onClick += delegate
			{
				this.OpenUrl(this.fixedURL);
			};
		}
	}

	public void OpenUrl(string url)
	{
		App.OpenWebURL(url);
	}

	[SerializeField]
	private KButton triggerButton;

	[SerializeField]
	private string fixedURL;
}
