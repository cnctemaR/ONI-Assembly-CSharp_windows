using System;
using KSerialization;
using STRINGS;

public class DirectionControl : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.allowedDirection = WorkableReactable.AllowedDirection.Any;
		this.directionInfos = new DirectionControl.DirectionInfo[]
		{
			new DirectionControl.DirectionInfo
			{
				allowLeft = true,
				allowRight = true,
				iconName = "action_direction_both",
				name = UI.USERMENUACTIONS.WORKABLE_DIRECTION_BOTH.NAME,
				tooltip = UI.USERMENUACTIONS.WORKABLE_DIRECTION_BOTH.TOOLTIP
			},
			new DirectionControl.DirectionInfo
			{
				allowLeft = true,
				allowRight = false,
				iconName = "action_direction_left",
				name = UI.USERMENUACTIONS.WORKABLE_DIRECTION_LEFT.NAME,
				tooltip = UI.USERMENUACTIONS.WORKABLE_DIRECTION_LEFT.TOOLTIP
			},
			new DirectionControl.DirectionInfo
			{
				allowLeft = false,
				allowRight = true,
				iconName = "action_direction_right",
				name = UI.USERMENUACTIONS.WORKABLE_DIRECTION_RIGHT.NAME,
				tooltip = UI.USERMENUACTIONS.WORKABLE_DIRECTION_RIGHT.TOOLTIP
			}
		};
		base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.DirectionControl, this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.SetAllowedDirection(this.allowedDirection);
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
	}

	private void SetAllowedDirection(WorkableReactable.AllowedDirection new_direction)
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		DirectionControl.DirectionInfo directionInfo = this.directionInfos[(int)new_direction];
		bool flag = directionInfo.allowLeft && directionInfo.allowRight;
		bool flag2 = !flag && directionInfo.allowLeft;
		bool flag3 = !flag && directionInfo.allowRight;
		if (flag)
		{
			component.StopHidingSymbol(new KAnimHashedString("arrow2"), true);
		}
		else
		{
			component.HideSymbol(new KAnimHashedString("arrow2"), true);
		}
		if (flag2)
		{
			component.StopHidingSymbol(new KAnimHashedString("arrow_left"), true);
		}
		else
		{
			component.HideSymbol(new KAnimHashedString("arrow_left"), true);
		}
		if (flag3)
		{
			component.StopHidingSymbol(new KAnimHashedString("arrow_right"), true);
		}
		else
		{
			component.HideSymbol(new KAnimHashedString("arrow_right"), true);
		}
		if (new_direction != this.allowedDirection)
		{
			this.allowedDirection = new_direction;
			if (this.onDirectionChanged != null)
			{
				this.onDirectionChanged(this.allowedDirection);
			}
		}
	}

	private void OnChangeWorkableDirection()
	{
		this.SetAllowedDirection((WorkableReactable.AllowedDirection.Left + (int)this.allowedDirection) % (WorkableReactable.AllowedDirection)this.directionInfos.Length);
	}

	private void OnRefreshUserMenu(object data)
	{
		int num = (int)((WorkableReactable.AllowedDirection.Left + (int)this.allowedDirection) % (WorkableReactable.AllowedDirection)this.directionInfos.Length);
		DirectionControl.DirectionInfo directionInfo = this.directionInfos[num];
		UserMenu userMenu = this.userMenu;
		string tooltip = directionInfo.tooltip;
		userMenu.AddButton(new KIconButtonMenu.ButtonInfo(directionInfo.iconName, directionInfo.name, new global::System.Action(this.OnChangeWorkableDirection), global::Action.NumActions, null, null, null, tooltip, true), 1f);
	}

	[Serialize]
	public WorkableReactable.AllowedDirection allowedDirection;

	private DirectionControl.DirectionInfo[] directionInfos;

	[MyCmpAdd]
	private UserMenu userMenu;

	public Action<WorkableReactable.AllowedDirection> onDirectionChanged;

	private struct DirectionInfo
	{
		public bool allowLeft;

		public bool allowRight;

		public string iconName;

		public string name;

		public string tooltip;
	}
}
