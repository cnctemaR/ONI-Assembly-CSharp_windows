using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LargeImpactorNotificationUI : KMonoBehaviour, ISim200ms
{
	protected override void OnSpawn()
	{
		GameplayEventInstance gameplayEventInstance = GameplayEventManager.Instance.GetGameplayEventInstance(Db.Get().GameplayEvents.LargeImpactor.Id, -1);
		LargeImpactorEvent.StatesInstance statesInstance = (LargeImpactorEvent.StatesInstance)gameplayEventInstance.smi;
		this.rangeVisualizer = statesInstance.impactorInstance.GetComponent<LargeImpactorVisualizer>();
		this.asteroidBackground = statesInstance.impactorInstance.GetComponent<ParallaxBackgroundObject>();
		this.statusMonitor = statesInstance.impactorInstance.GetSMI<LargeImpactorStatus.Instance>();
		LargeImpactorStatus.Instance instance = this.statusMonitor;
		instance.OnDamaged = (Action<int>)Delegate.Combine(instance.OnDamaged, new Action<int>(this.OnAsteroidDamaged));
		Game.Instance.Subscribe(445618876, new Action<object>(this.OnScreenResolutionChanged));
		Game.Instance.Subscribe(-810220474, new Action<object>(this.OnScreenResolutionChanged));
		this.cyclesLabelEffects.InitializeCycleLabelFocusMonitor();
		this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.ToggleVisibility));
		this.toggle.SetIsOnWithoutNotify(this.rangeVisualizer != null && this.rangeVisualizer.Visible);
		this.toggle.offEffectDuration = this.rangeVisualizer.FoldEffectDuration;
		LargeImpactorCrashStamp component = statesInstance.impactorInstance.GetComponent<LargeImpactorCrashStamp>();
		this.midSkyCell = Grid.FindMidSkyCellAlignedWithCellInWorld(Grid.XYToCell(component.stampLocation.x, component.stampLocation.y), gameplayEventInstance.worldId);
		this.RefreshTogglePositionInRangeVisualizer();
		this.RefreshValues();
	}

	private void OnScreenResolutionChanged(object data)
	{
		this.RefreshTogglePositionInRangeVisualizer();
	}

	private void RefreshTogglePositionInRangeVisualizer()
	{
		if (this.rangeVisualizer != null)
		{
			RectTransform rectTransform = this.toggle.rectTransform();
			Vector3 vector = rectTransform.TransformPoint(rectTransform.rect.center);
			Vector2 vector2 = RectTransformUtility.WorldToScreenPoint(null, vector);
			Vector2 vector3 = new Vector2(vector2.x / (float)Screen.width, vector2.y / (float)Screen.height);
			this.rangeVisualizer.ScreenSpaceNotificationTogglePosition = vector3;
		}
	}

	public void Sim200ms(float dt)
	{
		this.RefreshValues();
	}

	public void RefreshValues()
	{
		float num = (float)this.statusMonitor.Health / (float)this.statusMonitor.def.MAX_HEALTH;
		float num2 = this.statusMonitor.TimeRemainingBeforeCollision / LargeImpactorEvent.GetImpactTime();
		this.healthbar.fillAmount = num;
		this.clock.SetLargeImpactorTime(num2);
		string[] array = GameUtil.GetFormattedCycles(this.statusMonitor.TimeRemainingBeforeCollision, "F1", false).Split(' ', StringSplitOptions.None);
		this.numberOfCyclesLabel.SetText(array[0]);
		if (this.rangeVisualizer != null && this.toggle.isOn != this.rangeVisualizer.Visible)
		{
			this.toggle.isOn = this.rangeVisualizer.Visible;
		}
	}

	private void OnAsteroidDamaged(int newHealth)
	{
		this.hitEffects.PlayHitEffect();
		KFMOD.PlayUISound(GlobalAssets.GetSound("Notification_Imperative_hit", false));
		this.RefreshValues();
	}

	public void ToggleVisibility(bool shouldBeVisible)
	{
		if (this.rangeVisualizer != null)
		{
			KFMOD.PlayUISound(GlobalAssets.GetSound(shouldBeVisible ? "HUD_Demolior_LandingZone_toggle_on" : "HUD_Demolior_LandingZone_toggle_off", false));
			this.RefreshTogglePositionInRangeVisualizer();
			this.rangeVisualizer.SetFoldedState(!shouldBeVisible);
		}
	}

	public void OnPlayerClickedNotification()
	{
		GameUtil.FocusCamera(this.midSkyCell, true);
		KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Click_Open ", false));
		KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Demolior_Click_focus", false));
		if (this.asteroidBackground != null)
		{
			this.asteroidBackground.PlayPlayerClickFeedback();
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.cyclesLabelEffects.AbortCycleLabelFocusMonitor();
		if (this.statusMonitor != null)
		{
			LargeImpactorStatus.Instance instance = this.statusMonitor;
			instance.OnDamaged = (Action<int>)Delegate.Remove(instance.OnDamaged, new Action<int>(this.OnAsteroidDamaged));
		}
		Game.Instance.Unsubscribe(445618876, new Action<object>(this.OnScreenResolutionChanged));
		Game.Instance.Unsubscribe(-810220474, new Action<object>(this.OnScreenResolutionChanged));
	}

	protected override void OnCmpEnable()
	{
		if (base.isSpawned)
		{
			this.cyclesLabelEffects.InitializeCycleLabelFocusMonitor();
		}
	}

	protected override void OnCmpDisable()
	{
		this.cyclesLabelEffects.AbortCycleLabelFocusMonitor();
	}

	public Image healthbar;

	public LargeImpactorNotificationUI_Clock clock;

	public KToggleSlider toggle;

	public LargeImpactorUINotificationHitEffects hitEffects;

	public LargeImpactorNotificationUI_CycleLabelEffects cyclesLabelEffects;

	public LocText numberOfCyclesLabel;

	private LargeImpactorStatus.Instance statusMonitor;

	private LargeImpactorVisualizer rangeVisualizer;

	private ParallaxBackgroundObject asteroidBackground;

	private int midSkyCell = Grid.InvalidCell;

	private const string Hit_SFX = "Notification_Imperative_hit";

	private const string Click_SFX = "HUD_Click_Open ";

	private const string Focus_SFX = "HUD_Demolior_Click_focus";

	private const string ToggleOff_SFX = "HUD_Demolior_LandingZone_toggle_off";

	private const string ToggleOn_SFX = "HUD_Demolior_LandingZone_toggle_on";
}
