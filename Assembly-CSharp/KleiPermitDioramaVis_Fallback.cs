using System;
using Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KleiPermitDioramaVis_Fallback : KMonoBehaviour, IKleiPermitDioramaVisTarget
{
	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	public void ConfigureSetup()
	{
	}

	public void ConfigureWith(PermitResource permit)
	{
		this.sprite.sprite = PermitPresentationInfo.GetUnknownSprite();
		this.editorOnlyErrorMessageParent.gameObject.SetActive(false);
	}

	public KleiPermitDioramaVis_Fallback WithError(string error)
	{
		this.error = error;
		global::Debug.Log("[KleiInventoryScreen Error] Had to use fallback vis. " + error);
		return this;
	}

	[SerializeField]
	private Image sprite;

	[SerializeField]
	private RectTransform editorOnlyErrorMessageParent;

	[SerializeField]
	private TextMeshProUGUI editorOnlyErrorMessageText;

	private Option<string> error;
}
