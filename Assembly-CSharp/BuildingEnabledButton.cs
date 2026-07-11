using System;
using KSerialization;
using STRINGS;

[SerializationConfig(MemberSerialization.OptIn)]
public class BuildingEnabledButton : KMonoBehaviour, ISaveLoadable, IToggleHandler
{
	public bool IsEnabled
	{
		get
		{
			return this.Operational != null && this.Operational.GetFlag(BuildingEnabledButton.EnabledFlag);
		}
		set
		{
			this.Operational.SetFlag(BuildingEnabledButton.EnabledFlag, value);
			Game.Instance.userMenu.Refresh(base.gameObject);
			this.buildingEnabled = value;
			base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.BuildingDisabled, !this.buildingEnabled, null);
			base.Trigger(1088293757, this.buildingEnabled);
		}
	}

	public bool WaitingForDisable
	{
		get
		{
			return this.IsEnabled && this.Toggleable.IsToggleQueued(this.ToggleIdx);
		}
	}

	protected override void OnPrefabInit()
	{
		this.ToggleIdx = this.Toggleable.SetTarget(this);
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
	}

	protected override void OnSpawn()
	{
		this.IsEnabled = this.buildingEnabled;
	}

	public void HandleToggle()
	{
		Prioritizable.RemoveRef(base.gameObject);
		this.OnToggle();
	}

	public bool IsHandlerOn()
	{
		return this.IsEnabled;
	}

	private void OnToggle()
	{
		this.IsEnabled = !this.IsEnabled;
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	private void OnMenuToggle()
	{
		if (!this.Toggleable.IsToggleQueued(this.ToggleIdx))
		{
			if (this.IsEnabled)
			{
				base.Trigger(2108245096, "BuildingDisabled");
			}
			Prioritizable.AddRef(base.gameObject);
		}
		else
		{
			Prioritizable.RemoveRef(base.gameObject);
		}
		this.Toggleable.Toggle(this.ToggleIdx);
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	private void OnRefreshUserMenu(object data)
	{
		bool isEnabled = this.IsEnabled;
		bool flag = this.Toggleable.IsToggleQueued(this.ToggleIdx);
		KIconButtonMenu.ButtonInfo buttonInfo;
		if ((isEnabled && !flag) || (!isEnabled && flag))
		{
			string text = "action_building_disabled";
			string text2 = UI.USERMENUACTIONS.ENABLEBUILDING.NAME;
			global::System.Action action = new global::System.Action(this.OnMenuToggle);
			global::Action action2 = global::Action.ToggleEnabled;
			string text3 = UI.USERMENUACTIONS.ENABLEBUILDING.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text, text2, action, action2, null, null, null, text3, true);
		}
		else
		{
			string text3 = "action_building_disabled";
			string text2 = UI.USERMENUACTIONS.ENABLEBUILDING.NAME_OFF;
			global::System.Action action = new global::System.Action(this.OnMenuToggle);
			global::Action action2 = global::Action.ToggleEnabled;
			string text = UI.USERMENUACTIONS.ENABLEBUILDING.TOOLTIP_OFF;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text3, text2, action, action2, null, null, null, text, true);
		}
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 1f);
	}

	[MyCmpAdd]
	private Toggleable Toggleable;

	[MyCmpReq]
	private Operational Operational;

	private int ToggleIdx;

	[Serialize]
	private bool buildingEnabled = true;

	public static Operational.Flag EnabledFlag = new Operational.Flag("building_enabled", Operational.Flag.Type.Functional);
}
