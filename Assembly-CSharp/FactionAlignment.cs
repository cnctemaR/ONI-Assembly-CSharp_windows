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
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		this.Subscribe(2127324410, delegate(object d)
		{
			this.SetPlayerTargeted(false);
		});
		if (this.alignmentActive)
		{
			FactionManager.Instance.GetFaction(this.Alignment).Members.Add(this);
		}
		this.Subscribe(1623392196, new Action<object>(this.OnDeath));
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
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.PendingHarvest, this);
		}
		else
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingHarvest, false);
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
			buttonInfo = new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.ATTACK.NAME, delegate
			{
				this.SetPlayerTargeted(true);
			}, global::Action.NumActions, null, null, null, string.Empty, true);
		}
		else
		{
			buttonInfo = new KIconButtonMenu.ButtonInfo("action_deconstruct", UI.USERMENUACTIONS.CANCELATTACK.NAME, delegate
			{
				this.SetPlayerTargeted(false);
			}, global::Action.NumActions, null, null, null, string.Empty, true);
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
