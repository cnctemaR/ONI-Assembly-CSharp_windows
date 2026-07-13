using System;
using System.Collections;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class LargeImpactorSequenceUIReticle : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.transform = base.transform as RectTransform;
		this.bgOriginalColor = this.bg.color;
		this.calculatingImpactLabelOriginalColor = this.calculateImpactLabel.color;
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		this.ResetGraphics();
	}

	public void Run(global::System.Action onPhase1Completed = null, global::System.Action onComplete = null)
	{
		this.SetVisibility(true);
		this.AbortCoroutine();
		this.ResetGraphics();
		this.onPhase1Completed = onPhase1Completed;
		this.onComplete = onComplete;
		this.InitializeAndRunCoroutine();
	}

	public void Hide()
	{
		this.AbortCoroutine();
		this.ResetGraphics();
		this.SetVisibility(false);
	}

	private void SetVisibility(bool visible)
	{
		this.isVisible = visible;
		this.label.enabled = visible;
		this.bg.enabled = visible;
		this.border.enabled = visible;
	}

	private void InitializeAndRunCoroutine()
	{
		this.coroutine = base.StartCoroutine(this.EnterSequence());
	}

	private void AbortCoroutine()
	{
		base.StopAllCoroutines();
		this.coroutine = null;
	}

	public void SetTarget(LargeImpactorStatus.Instance largeImpactorStatus)
	{
		this.largeImpactorStatus = largeImpactorStatus;
		this.loopingSounds = largeImpactorStatus.GetComponent<LoopingSounds>();
	}

	private void ResetGraphics()
	{
		this.label.SetText("");
		this.border.Opacity(0f);
		this.bg.color = this.bgOriginalColor;
		this.bg.Opacity(0f);
		this.sidePanelIcon.Opacity(0f);
		this.sidePanelTitleLabel.SetText("");
		this.calculateImpactLabel.SetText("");
		this.sidePanelDescriptionLabel.SetText("");
		this.calculateImpactLabel.color = this.calculatingImpactLabelOriginalColor;
	}

	private void PlayLoopingSound(string soundName)
	{
		string sound = GlobalAssets.GetSound(soundName, false);
		this.loopingSounds.StartSound(sound, false, false, false);
	}

	private IEnumerator EnterSequence()
	{
		KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Imperative_analysis_start", false));
		yield return this.Interpolate(delegate(float n)
		{
			this.transform.sizeDelta = Vector2.Lerp(this.initialSize * 2f, this.initialSize, n);
			this.border.Opacity(n);
		}, 0.4f, null);
		if (this.bg.color != this.border.color)
		{
			this.bg.color = this.border.color;
		}
		yield return this.Interpolate(delegate(float n)
		{
			this.bg.Opacity(Mathf.Abs(Mathf.Sin(n * 3.1415927f * 3f)));
		}, 0.4f, delegate
		{
			this.bg.color = this.bgOriginalColor;
		});
		KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Imperative_bracket_open_first", false));
		yield return this.Interpolate(delegate(float n)
		{
			this.bg.Opacity(n * this.bgOriginalColor.a);
			this.transform.sizeDelta = Vector2.Lerp(this.initialSize, this.zoomedOutSize, n);
		}, 0.8f, null);
		this.PlayLoopingSound("HUD_Imperative_Text_typing_header");
		string titleText = MISC.NOTIFICATIONS.LARGEIMPACTORREVEALSEQUENCE.RETICLE.LARGE_IMPACTOR_NAME;
		yield return this.Interpolate(delegate(float n)
		{
			SequenceTools.TextWriter(this.label, titleText, n);
		}, 1f, null);
		this.loopingSounds.StopSound(GlobalAssets.GetSound("HUD_Imperative_Text_typing_header", false));
		yield return null;
		this.PlayLoopingSound("HUD_Imperative_Text_typing_header");
		this.sidePanelIcon.color = Color.white;
		string sidePanelTitleText = MISC.NOTIFICATIONS.LARGEIMPACTORREVEALSEQUENCE.RETICLE.SIDE_PANEL_TITLE;
		yield return this.Interpolate(delegate(float n)
		{
			this.sidePanelIcon.Opacity(n);
			this.sidePanelIcon.transform.localRotation = Quaternion.Euler(0f, Mathf.Lerp(90f, 0f, n), 0f);
			SequenceTools.TextWriter(this.sidePanelTitleLabel, sidePanelTitleText, n);
		}, 0.5f, null);
		this.loopingSounds.StopSound(GlobalAssets.GetSound("HUD_Imperative_Text_typing_header", false));
		this.PlayLoopingSound("HUD_Imperative_Text_typing_body");
		string sidePanelDescriptionText = GameUtil.SafeStringFormat(MISC.NOTIFICATIONS.LARGEIMPACTORREVEALSEQUENCE.RETICLE.SIDE_PANEL_DESCRIPTION, new object[] { GameUtil.GetFormattedCycles(this.largeImpactorStatus.TimeRemainingBeforeCollision, "F1", false).Split(' ', StringSplitOptions.None)[0] });
		yield return this.Interpolate(delegate(float n)
		{
			SequenceTools.TextWriter(this.sidePanelDescriptionLabel, sidePanelDescriptionText, n);
		}, 1.5f, null);
		this.loopingSounds.StopSound(GlobalAssets.GetSound("HUD_Imperative_Text_typing_body", false));
		yield return new WaitForSecondsRealtime(2f);
		if (this.onPhase1Completed != null)
		{
			this.onPhase1Completed();
			this.onPhase1Completed = null;
		}
		yield return this.Interpolate(delegate(float n)
		{
			SequenceTools.TextEraser(this.label, titleText, n);
			SequenceTools.TextEraser(this.sidePanelTitleLabel, sidePanelTitleText, n);
			SequenceTools.TextEraser(this.sidePanelDescriptionLabel, sidePanelDescriptionText, n);
			this.sidePanelIcon.color = Color.Lerp(Color.white, Color.red, n);
			this.sidePanelIcon.Opacity(1f - n);
		}, 0.5f, null);
		Color bgColor = this.bg.color;
		Color targetBgColor = Color.Lerp(this.bgOriginalColor, Color.black, 0.5f);
		targetBgColor.a = this.bgOriginalColor.a * 0.8f;
		KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Imperative_bracket_open_second", false));
		yield return this.Interpolate(delegate(float n)
		{
			this.bg.color = Color.Lerp(bgColor, targetBgColor, n);
			this.transform.sizeDelta = Vector2.Lerp(this.zoomedOutSize, this.calculatingImpactSize, n);
		}, 0.8f, null);
		KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Imperative_calculating_beep", false));
		Coroutine flashLabelCoroutine = null;
		this.Interpolate(delegate(float n)
		{
			this.calculateImpactLabel.color = Color.Lerp(Color.white, Color.red, Mathf.Abs(Mathf.Sin(n * 3.1415927f * 999f)));
		}, 999f, out flashLabelCoroutine, null);
		yield return this.Interpolate(delegate(float n)
		{
			SequenceTools.TextWriter(this.calculateImpactLabel, MISC.NOTIFICATIONS.LARGEIMPACTORREVEALSEQUENCE.RETICLE.CALCULATING_IMPACT_ZONE_TEXT, n);
		}, 0.5f, null);
		yield return new WaitForSecondsRealtime(3.5f);
		base.StopCoroutine(flashLabelCoroutine);
		flashLabelCoroutine = null;
		Color impactLabelColor = this.calculateImpactLabel.color;
		yield return this.Interpolate(delegate(float n)
		{
			float num = Mathf.Sqrt(1f - n);
			float num2 = Mathf.Sqrt(n);
			this.bg.Opacity(this.bgOriginalColor.a * num);
			this.border.Opacity(num);
			this.calculateImpactLabel.Opacity(impactLabelColor.a * num);
			this.transform.sizeDelta = Vector2.Lerp(this.calculatingImpactSize, this.calculatingImpactSize * 1.3f, num2);
		}, 0.8f, null);
		if (this.onComplete != null)
		{
			this.onComplete();
			this.onComplete = null;
		}
		yield break;
	}

	protected override void OnCmpDisable()
	{
		this.AbortCoroutine();
	}

	protected override void OnCmpEnable()
	{
		if (this.isVisible)
		{
			this.AbortCoroutine();
			this.ResetGraphics();
			this.InitializeAndRunCoroutine();
		}
	}

	private const float reticleEnterDuration = 0.4f;

	private const float flashDuration = 0.4f;

	private const int flashTimes = 3;

	private const float reticleZoomOutDuration = 0.8f;

	private const float labelRevealDuration = 1f;

	private const float sidePanel_TitleRevealDuration = 0.5f;

	private const float sidePanel_DescriptionRevealDuration = 1.5f;

	private const float exitToCalculationDuration = 0.5f;

	private const float expandReticleHorizontallyDuration = 0.8f;

	private const float calculateImpactZoneTextRevealDuration = 0.5f;

	private const float exitDuration = 0.8f;

	public const float RevealPOI_LandingZone_Duration = 3.5f;

	private const string Sound_LockTarget = "HUD_Imperative_analysis_start";

	private const string Sound_BracketSquareExpand = "HUD_Imperative_bracket_open_first";

	private const string Sound_BracketExpandsForCalculatingLandingZone = "HUD_Imperative_bracket_open_second";

	private const string Sound_CalculatingLandingZoneTextAppears = "HUD_Imperative_calculating_beep";

	private const string Sound_TypeHeader = "HUD_Imperative_Text_typing_header";

	private const string Sound_TypeBody = "HUD_Imperative_Text_typing_body";

	public Vector2 initialSize = new Vector2(100f, 100f);

	public Vector2 zoomedOutSize = new Vector2(180f, 180f);

	public Vector2 calculatingImpactSize = new Vector2(500f, 120f);

	[Space]
	public LocText label;

	public LocText sidePanelTitleLabel;

	public LocText sidePanelDescriptionLabel;

	public LocText calculateImpactLabel;

	public Image bg;

	public Image border;

	public Image sidePanelIcon;

	private new RectTransform transform;

	private LargeImpactorStatus.Instance largeImpactorStatus;

	private LoopingSounds loopingSounds;

	private bool isVisible;

	private Color bgOriginalColor;

	private Color calculatingImpactLabelOriginalColor;

	private global::System.Action onPhase1Completed;

	private global::System.Action onComplete;

	private Coroutine coroutine;
}
