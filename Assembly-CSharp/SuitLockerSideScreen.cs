using System;
using STRINGS;
using UnityEngine;

public class SuitLockerSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<SuitLocker>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		this.suitLocker = target.GetComponent<SuitLocker>();
		this.initialConfigRequestSuitButton.GetComponentInChildren<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_REQUEST_SUIT_TOOLTIP);
		this.initialConfigNoSuitButton.GetComponentInChildren<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_NO_SUIT_TOOLTIP);
		this.initialConfigRequestSuitButton.ClearOnClick();
		this.initialConfigRequestSuitButton.onClick += delegate
		{
			this.suitLocker.ConfigRequestSuit();
		};
		this.initialConfigNoSuitButton.ClearOnClick();
		this.initialConfigNoSuitButton.onClick += delegate
		{
			this.suitLocker.ConfigNoSuit();
		};
		this.regularConfigRequestSuitButton.ClearOnClick();
		this.regularConfigRequestSuitButton.onClick += delegate
		{
			if (this.suitLocker.smi.sm.isWaitingForSuit.Get(this.suitLocker.smi))
			{
				this.suitLocker.ConfigNoSuit();
				return;
			}
			this.suitLocker.ConfigRequestSuit();
		};
		this.regularConfigDropSuitButton.ClearOnClick();
		this.regularConfigDropSuitButton.onClick += delegate
		{
			this.suitLocker.DropSuit();
		};
	}

	private void Update()
	{
		bool flag = this.suitLocker.smi.sm.isConfigured.Get(this.suitLocker.smi);
		this.initialConfigScreen.gameObject.SetActive(!flag);
		this.regularConfigScreen.gameObject.SetActive(flag);
		bool flag2 = this.suitLocker.GetStoredOutfit() != null;
		bool flag3 = this.suitLocker.smi.sm.isWaitingForSuit.Get(this.suitLocker.smi);
		this.regularConfigRequestSuitButton.isInteractable = !flag2;
		if (!flag3)
		{
			this.regularConfigRequestSuitButton.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_REQUEST_SUIT;
			this.regularConfigRequestSuitButton.GetComponentInChildren<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_REQUEST_SUIT_TOOLTIP);
		}
		else
		{
			this.regularConfigRequestSuitButton.GetComponentInChildren<LocText>().text = UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_CANCEL_REQUEST;
			this.regularConfigRequestSuitButton.GetComponentInChildren<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_CANCEL_REQUEST_TOOLTIP);
		}
		if (flag2)
		{
			this.regularConfigDropSuitButton.isInteractable = true;
			this.regularConfigDropSuitButton.GetComponentInChildren<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_DROP_SUIT_TOOLTIP);
		}
		else
		{
			this.regularConfigDropSuitButton.isInteractable = false;
			this.regularConfigDropSuitButton.GetComponentInChildren<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_DROP_SUIT_NO_SUIT_TOOLTIP);
		}
		KSelectable component = this.suitLocker.GetComponent<KSelectable>();
		if (component != null)
		{
			StatusItemGroup.Entry statusItem = component.GetStatusItem(Db.Get().StatusItemCategories.Main);
			if (statusItem.item != null)
			{
				this.regularConfigLabel.text = statusItem.item.GetName(statusItem.data);
				this.regularConfigLabel.GetComponentInChildren<ToolTip>().SetSimpleTooltip(statusItem.item.GetTooltip(statusItem.data));
			}
		}
	}

	[SerializeField]
	private GameObject initialConfigScreen;

	[SerializeField]
	private GameObject regularConfigScreen;

	[SerializeField]
	private LocText initialConfigLabel;

	[SerializeField]
	private KButton initialConfigRequestSuitButton;

	[SerializeField]
	private KButton initialConfigNoSuitButton;

	[SerializeField]
	private LocText regularConfigLabel;

	[SerializeField]
	private KButton regularConfigRequestSuitButton;

	[SerializeField]
	private KButton regularConfigDropSuitButton;

	private SuitLocker suitLocker;
}
