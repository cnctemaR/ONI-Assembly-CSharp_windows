using System;
using System.Collections.Generic;
using UnityEngine;

public class KPopupMenu : KScreen
{
	public void SetOptions(IList<string> options)
	{
		List<KButtonMenu.ButtonInfo> list = new List<KButtonMenu.ButtonInfo>();
		for (int i = 0; i < options.Count; i++)
		{
			int index = i;
			string option = options[i];
			list.Add(new KButtonMenu.ButtonInfo(option, global::Action.NumActions, delegate
			{
				this.SelectOption(option, index);
			}, null, null));
		}
		this.Buttons = list.ToArray();
	}

	public void OnClick()
	{
		if (this.Buttons != null)
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				this.buttonMenu.SetButtons(this.Buttons);
				this.buttonMenu.RefreshButtons();
				base.gameObject.SetActive(true);
			}
		}
	}

	public void SelectOption(string option, int index)
	{
		if (this.OnSelect != null)
		{
			this.OnSelect(option, index);
		}
		base.gameObject.SetActive(false);
	}

	[SerializeField]
	private KButtonMenu buttonMenu;

	private KButtonMenu.ButtonInfo[] Buttons;

	public Action<string, int> OnSelect;
}
