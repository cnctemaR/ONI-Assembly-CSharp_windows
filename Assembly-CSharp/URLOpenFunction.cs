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
		if (url == "blueprints")
		{
			if (LockerMenuScreen.Instance != null)
			{
				LockerMenuScreen.Instance.ShowInventoryScreen();
				return;
			}
		}
		else
		{
			App.OpenWebURL(url);
		}
	}

	public void SetURL(string url)
	{
		this.fixedURL = url;
	}

	[SerializeField]
	private KButton triggerButton;

	[SerializeField]
	private string fixedURL;
}
