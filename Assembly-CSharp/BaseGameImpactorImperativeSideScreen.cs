using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class BaseGameImpactorImperativeSideScreen : SideScreenContent
{
	public override bool IsValidForTarget(GameObject target)
	{
		if (DlcManager.IsExpansion1Active())
		{
			return false;
		}
		MissileLauncher.Instance smi = target.GetSMI<MissileLauncher.Instance>();
		return smi != null && this.StatusMonitor != null && smi.AmmunitionIsAllowed("MissileLongRange");
	}

	private LargeImpactorStatus.Instance StatusMonitor
	{
		get
		{
			if (this.statusMonitor == null)
			{
				GameplayEventInstance gameplayEventInstance = GameplayEventManager.Instance.GetGameplayEventInstance(Db.Get().GameplayEvents.LargeImpactor.Id, -1);
				if (gameplayEventInstance != null)
				{
					LargeImpactorEvent.StatesInstance statesInstance = (LargeImpactorEvent.StatesInstance)gameplayEventInstance.smi;
					this.statusMonitor = statesInstance.impactorInstance.GetSMI<LargeImpactorStatus.Instance>();
				}
			}
			return this.statusMonitor;
		}
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		this.targetMissileLauncher = target.GetSMI<MissileLauncher.Instance>();
		this.Build();
	}

	private void Build()
	{
		if (this.StatusMonitor != null)
		{
			this.healthBarFill.fillAmount = Mathf.Max((float)this.StatusMonitor.Health / (float)this.StatusMonitor.def.MAX_HEALTH, 0f);
			this.healthBarTooltip.toolTip = GameUtil.SafeStringFormat(UI.UISIDESCREENS.MISSILESELECTIONSIDESCREEN.VANILLALARGEIMPACTOR.HEALTH_BAR_TOOLTIP, new object[]
			{
				this.StatusMonitor.Health,
				this.StatusMonitor.def.MAX_HEALTH
			});
			this.timeBarFill.fillAmount = this.StatusMonitor.TimeRemainingBeforeCollision / LargeImpactorEvent.GetImpactTime();
			this.timeBarTooltip.toolTip = GameUtil.SafeStringFormat(UI.UISIDESCREENS.MISSILESELECTIONSIDESCREEN.VANILLALARGEIMPACTOR.TIME_UNTIL_COLLISION_TOOLTIP, new object[] { GameUtil.GetFormattedCycles(this.StatusMonitor.TimeRemainingBeforeCollision, "F1", false).Split(' ', StringSplitOptions.None)[0] });
		}
	}

	private MissileLauncher.Instance targetMissileLauncher;

	[SerializeField]
	private Image healthBarFill;

	[SerializeField]
	private Image timeBarFill;

	[SerializeField]
	private LocText healthBarLabel;

	[SerializeField]
	private LocText timeBarLabel;

	[SerializeField]
	private ToolTip healthBarTooltip;

	[SerializeField]
	private ToolTip timeBarTooltip;

	private LargeImpactorStatus.Instance statusMonitor;
}
