using System;
using KSerialization;
using STRINGS;

public class FactionAlignment : KMonoBehaviour
{
	[MyCmpAdd]
	public Health health { get; private set; }

	public AttackableBase attackable { get; private set; }

	public bool CheckAlignmentActive
	{
		get
		{
			return FactionManager.Instance.GetFaction(this.Alignment).Members.Contains(this);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.health = base.GetComponent<Health>();
		this.attackable = base.GetComponent<AttackableBase>();
		Components.FactionAlignments.Add(this);
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		base.Subscribe(2127324410, delegate(object d)
		{
			this.SetPlayerTargeted(false);
		});
		if (this.alignmentActive)
		{
			FactionManager.Instance.GetFaction(this.Alignment).Members.Add(this);
		}
		base.Subscribe(1623392196, new Action<object>(this.OnDeath));
	}

	private void OnDeath(object data)
	{
		this.ToggleAlignmentActive(false);
	}

	public void ToggleAlignmentActive(bool active)
	{
		this.SetPlayerTargetable(active);
		this.alignmentActive = active;
		if (active)
		{
			FactionManager.Instance.GetFaction(this.Alignment).Members.Add(this);
		}
		else
		{
			FactionManager.Instance.GetFaction(this.Alignment).Members.Remove(this);
		}
	}

	public void SetPlayerTargetable(bool state)
	{
		this.targetable = state;
		if (!state)
		{
			this.SetPlayerTargeted(false);
		}
	}

	public void SetPlayerTargeted(bool state)
	{
		this.targeted = state && this.targetable;
		if (this.targeted)
		{
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.OrderAttack, this);
		}
		else
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.OrderAttack, false);
		}
	}

	public void SwitchAlignment(FactionManager.FactionID newAlignment)
	{
		this.ToggleAlignmentActive(false);
		this.Alignment = newAlignment;
		this.ToggleAlignmentActive(true);
	}

	protected override void OnCleanUp()
	{
		Components.FactionAlignments.Remove(this);
		FactionManager.Instance.GetFaction(this.Alignment).Members.Remove(this);
		base.OnCleanUp();
	}

	private void OnRefreshUserMenu(object data)
	{
		if (this.Alignment == FactionManager.FactionID.Duplicant)
		{
			return;
		}
		if (!this.CheckAlignmentActive)
		{
			return;
		}
		KIconButtonMenu.ButtonInfo buttonInfo;
		if (!this.targeted)
		{
			string text = "action_attack";
			string text2 = UI.USERMENUACTIONS.ATTACK.NAME;
			global::System.Action action = delegate
			{
				this.SetPlayerTargeted(true);
			};
			string text3 = UI.USERMENUACTIONS.ATTACK.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true);
		}
		else
		{
			string text3 = "action_attack";
			string text2 = UI.USERMENUACTIONS.CANCELATTACK.NAME;
			global::System.Action action = delegate
			{
				this.SetPlayerTargeted(false);
			};
			string text = UI.USERMENUACTIONS.CANCELATTACK.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true);
		}
		if (buttonInfo != null)
		{
			this.userMenu.AddButton(buttonInfo, 1f);
		}
	}

	[MyCmpAdd]
	private UserMenu userMenu;

	[Serialize]
	private bool alignmentActive = true;

	public FactionManager.FactionID Alignment;

	[Serialize]
	public bool targeted;

	[Serialize]
	public bool targetable = true;
}
