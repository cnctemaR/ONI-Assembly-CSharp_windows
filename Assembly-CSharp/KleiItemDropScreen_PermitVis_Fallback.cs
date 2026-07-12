using System;
using Database;
using UnityEngine;
using UnityEngine.UI;

public class KleiItemDropScreen_PermitVis_Fallback : KMonoBehaviour, IKleiItemDropScreen_PermitVis_Target
{
	public void ConfigureWith(PermitResource permit, PermitPresentationInfo permitPresInfo)
	{
		this.sprite.sprite = permitPresInfo.sprite;
	}

	[SerializeField]
	private Image sprite;
}
