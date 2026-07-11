using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizableDialogScreen : KModalScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.gameObject.SetActive(false);
		this.buttons = new List<CustomizableDialogScreen.Button>();
	}

	public override bool IsModal()
	{
		return true;
	}

	public void AddOption(string text, global::System.Action action)
	{
		GameObject gameObject = Util.KInstantiateUI(this.buttonPrefab, this.buttonPanel, true);
		this.buttons.Add(new CustomizableDialogScreen.Button
		{
			label = text,
			action = action,
			gameObject = gameObject
		});
	}

	public void PopupConfirmDialog(string text, string title_text = null, Sprite image_sprite = null)
	{
		foreach (CustomizableDialogScreen.Button button in this.buttons)
		{
			button.gameObject.GetComponentInChildren<LocText>().text = button.label;
			button.gameObject.GetComponent<KButton>().onClick += button.action;
		}
		if (image_sprite != null)
		{
			this.image.sprite = image_sprite;
			this.image.gameObject.SetActive(true);
		}
		if (title_text != null)
		{
			this.titleText.text = title_text;
		}
		this.popupMessage.text = text;
	}

	protected override void OnDeactivate()
	{
		if (this.onDeactivateCB != null)
		{
			this.onDeactivateCB();
		}
		base.OnDeactivate();
	}

	public global::System.Action onDeactivateCB;

	[SerializeField]
	private GameObject buttonPrefab;

	[SerializeField]
	private GameObject buttonPanel;

	[SerializeField]
	private LocText titleText;

	[SerializeField]
	private LocText popupMessage;

	[SerializeField]
	private Image image;

	private List<CustomizableDialogScreen.Button> buttons;

	private struct Button
	{
		public global::System.Action action;

		public GameObject gameObject;

		public string label;
	}
}
