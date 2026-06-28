using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class KPopupMenu : KScreen
{
	protected override void OnPrefabInit()
	{
		Button componentInChildren = base.GetComponentInChildren<Button>();
		if (componentInChildren != null)
		{
			Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
			buttonClickedEvent.AddListener(new UnityAction(this.OnClick));
			componentInChildren.onClick = buttonClickedEvent;
		}
	}

	public void SetOptions(string[] options)
	{
		List<KButtonMenu.ButtonInfo> list = new List<KButtonMenu.ButtonInfo>();
		for (int i = 0; i < options.Length; i++)
		{
			string text = options[i];
			string option = text;
			list.Add(new KButtonMenu.ButtonInfo(text, global::Action.NumActions, delegate
			{
				this.SelectOption(option);
			}, null, null));
		}
		this.Buttons = list.ToArray();
		if (options != null && options.Length > 0)
		{
			LocText componentInChildren = base.GetComponentInChildren<LocText>();
			if (componentInChildren != null)
			{
				componentInChildren.text = options[0];
			}
		}
	}

	public void OnClick()
	{
		if (this.Buttons != null)
		{
			if (this.popupMenu.gameObject.activeSelf)
			{
				this.popupMenu.gameObject.SetActive(false);
			}
			else
			{
				this.popupMenu.SetButtons(this.Buttons);
				this.popupMenu.RefreshButtons();
				this.popupMenu.gameObject.SetActive(true);
			}
		}
	}

	public void SelectOption(string option)
	{
		if (this.OnSelect != null)
		{
			this.OnSelect(option);
		}
		LocText componentInChildren = base.GetComponentInChildren<LocText>();
		if (componentInChildren != null)
		{
			componentInChildren.text = option;
		}
		if (this.popupMenu.activateOnSpawn)
		{
			this.popupMenu.gameObject.SetActive(false);
		}
	}

	[SerializeField]
	private GameObject ButtonPrefab;

	[SerializeField]
	private KButtonMenu popupMenu;

	private KButtonMenu.ButtonInfo[] Buttons;

	public Action<string> OnSelect;
}
