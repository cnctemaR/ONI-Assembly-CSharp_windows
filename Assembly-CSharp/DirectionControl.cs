using System;
using KSerialization;
using STRINGS;
using UnityEngine;

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
		base.Subscribe<DirectionControl>(493375141, DirectionControl.OnRefreshUserMenuDelegate);
		base.Subscribe<DirectionControl>(-905833192, DirectionControl.OnCopySettingsDelegate);
	}

	private void SetAllowedDirection(WorkableReactable.AllowedDirection new_direction)
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		DirectionControl.DirectionInfo directionInfo = this.directionInfos[(int)new_direction];
		bool flag = directionInfo.allowLeft && directionInfo.allowRight;
		bool flag2 = !flag && directionInfo.allowLeft;
		bool flag3 = !flag && directionInfo.allowRight;
		component.SetSymbolVisiblity("arrow2", flag);
		component.SetSymbolVisiblity("arrow_left", flag2);
		component.SetSymbolVisiblity("arrow_right", flag3);
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

	private void OnCopySettings(object data)
	{
		DirectionControl component = ((GameObject)data).GetComponent<DirectionControl>();
		this.SetAllowedDirection(component.allowedDirection);
	}

	private void OnRefreshUserMenu(object data)
	{
		int num = (int)((WorkableReactable.AllowedDirection.Left + (int)this.allowedDirection) % (WorkableReactable.AllowedDirection)this.directionInfos.Length);
		DirectionControl.DirectionInfo directionInfo = this.directionInfos[num];
		Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo(directionInfo.iconName, directionInfo.name, new global::System.Action(this.OnChangeWorkableDirection), global::Action.NumActions, null, null, null, directionInfo.tooltip, true), 0.4f);
	}

	[Serialize]
	public WorkableReactable.AllowedDirection allowedDirection;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private DirectionControl.DirectionInfo[] directionInfos;

	public Action<WorkableReactable.AllowedDirection> onDirectionChanged;

	private static readonly EventSystem.IntraObjectHandler<DirectionControl> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<DirectionControl>(delegate(DirectionControl component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<DirectionControl> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<DirectionControl>(delegate(DirectionControl component, object data)
	{
		component.OnCopySettings(data);
	});

	private struct DirectionInfo
	{
		public bool allowLeft;

		public bool allowRight;

		public string iconName;

		public string name;

		public string tooltip;
	}
}
