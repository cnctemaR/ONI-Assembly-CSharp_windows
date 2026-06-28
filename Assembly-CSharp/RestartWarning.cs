using System;
using UnityEngine;
using UnityEngine.UI;

public class RestartWarning : MonoBehaviour
{
	private void Update()
	{
		if (RestartWarning.ShouldWarn)
		{
			this.text.enabled = true;
			this.image.enabled = true;
		}
	}

	public static bool ShouldWarn;

	public LocText text;

	public Image image;
}
