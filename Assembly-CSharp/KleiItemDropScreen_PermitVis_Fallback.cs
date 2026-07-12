using System;
using UnityEngine;
using UnityEngine.UI;

public class KleiItemDropScreen_PermitVis_Fallback : KMonoBehaviour
{
	public void ConfigureWith(DropScreenPresentationInfo info)
	{
		this.sprite.sprite = info.Sprite;
	}

	[SerializeField]
	private Image sprite;
}
