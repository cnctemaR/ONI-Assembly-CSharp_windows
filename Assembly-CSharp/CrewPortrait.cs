using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CrewPortrait : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (this.startTransparent)
		{
			base.StartCoroutine(this.AlphaIn());
		}
		this.requiresRefresh = true;
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (global::System.Action)Delegate.Combine(instance.OnResize, new global::System.Action(this.RefreshScale));
	}

	private IEnumerator AlphaIn()
	{
		this.SetAlpha(0f);
		for (float i = 0f; i < 1f; i += Time.unscaledDeltaTime * 4f)
		{
			this.SetAlpha(i);
			yield return 0;
		}
		this.SetAlpha(1f);
		yield break;
	}

	private void OnRoleChanged(object data)
	{
		CrewPortrait.RefreshHat(this.identityObject, this.controller);
	}

	private void RegisterEvents()
	{
		KMonoBehaviour kmonoBehaviour = this.identityObject as KMonoBehaviour;
		if (kmonoBehaviour == null)
		{
			return;
		}
		kmonoBehaviour.Subscribe(540773776, new Action<object>(this.OnRoleChanged));
	}

	private void UnregisterEvents()
	{
		KMonoBehaviour kmonoBehaviour = this.identityObject as KMonoBehaviour;
		if (kmonoBehaviour == null)
		{
			return;
		}
		kmonoBehaviour.Unsubscribe(540773776, new Action<object>(this.OnRoleChanged));
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		this.RegisterEvents();
		this.ForceRefresh();
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		this.UnregisterEvents();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.UnregisterEvents();
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (global::System.Action)Delegate.Remove(instance.OnResize, new global::System.Action(this.RefreshScale));
	}

	public void SetIdentityObject(IAssignableIdentity identity, bool jobEnabled = true)
	{
		this.UnregisterEvents();
		this.identityObject = identity;
		this.RegisterEvents();
		this.targetImage.enabled = true;
		if (this.identityObject != null)
		{
			this.targetImage.enabled = false;
		}
		if (this.useLabels && identity is MinionIdentity)
		{
			this.SetDuplicantJobTitleActive(jobEnabled);
		}
		this.requiresRefresh = true;
	}

	public void SetSubTitle(string newTitle)
	{
		if (this.subTitle != null)
		{
			if (string.IsNullOrEmpty(newTitle))
			{
				this.subTitle.gameObject.SetActive(false);
			}
			else
			{
				this.subTitle.gameObject.SetActive(true);
				this.subTitle.SetText(newTitle);
			}
		}
	}

	public void SetDuplicantJobTitleActive(bool state)
	{
		if (this.duplicantJob != null && this.duplicantJob.gameObject.activeInHierarchy != state)
		{
			this.duplicantJob.gameObject.SetActive(state);
		}
	}

	public void ForceRefresh()
	{
		this.requiresRefresh = true;
	}

	public void Update()
	{
		if (this.requiresRefresh && (this.controller == null || this.controller.enabled))
		{
			this.requiresRefresh = false;
			this.Rebuild();
			this.RefreshScale();
		}
	}

	private void RefreshScale()
	{
		float num = 1f;
		if (GameScreenManager.Instance != null && GameScreenManager.Instance.ssOverlayCanvas != null)
		{
			num = GameScreenManager.Instance.ssOverlayCanvas.GetComponent<KCanvasScaler>().GetCanvasScale();
		}
		if (this.controller != null)
		{
			this.controller.animScale = this.animScaleBase * (1f / num);
		}
	}

	private void Rebuild()
	{
		if (this.controller == null)
		{
			this.controller = base.GetComponentInChildren<KBatchedAnimController>();
			if (this.controller == null)
			{
				if (this.targetImage != null)
				{
					this.targetImage.enabled = true;
				}
				global::Debug.LogWarning("Controller for [" + base.name + "] null", null);
				return;
			}
		}
		CrewPortrait.SetPortraitData(this.identityObject, this.controller, this.useDefaultExpression);
		if (this.useLabels && this.duplicantName != null)
		{
			this.duplicantName.SetText(this.identityObject.GetProperName());
			if (this.identityObject is MinionIdentity && this.duplicantJob != null)
			{
				this.duplicantJob.SetText((this.identityObject == null) ? string.Empty : (this.identityObject as MinionIdentity).GetComponent<MinionResume>().GetCurrentRoleString());
				this.duplicantJob.GetComponent<ToolTip>().toolTip = (this.identityObject as MinionIdentity).GetComponent<MinionResume>().GetCurrentRoleDescription();
			}
		}
	}

	private static void RefreshHat(IAssignableIdentity identityObject, KBatchedAnimController controller)
	{
		MinionIdentity minionIdentity = identityObject as MinionIdentity;
		if (minionIdentity == null)
		{
			return;
		}
		MinionResume component = minionIdentity.GetComponent<MinionResume>();
		if (component != null)
		{
			RoleConfig roleConfig = null;
			if (!string.IsNullOrEmpty(component.CurrentRole))
			{
				roleConfig = Game.Instance.roleManager.GetRole(component.CurrentRole);
			}
			RoleManager.ApplyRoleHat(roleConfig, component.GetComponent<Accessorizer>(), controller);
		}
	}

	public static void SetPortraitData(IAssignableIdentity identityObject, KBatchedAnimController controller, bool useDefaultExpression = true)
	{
		controller.gameObject.SetActive(true);
		if (identityObject == null)
		{
			return;
		}
		MinionIdentity minionIdentity = identityObject as MinionIdentity;
		if (minionIdentity == null)
		{
			return;
		}
		SymbolOverrideController component = controller.GetComponent<SymbolOverrideController>();
		component.RemoveAllSymbolOverrides(0);
		Accessorizer component2 = minionIdentity.GetComponent<Accessorizer>();
		foreach (AccessorySlot accessorySlot in Db.Get().AccessorySlots.resources)
		{
			Accessory accessory = component2.GetAccessory(accessorySlot);
			if (accessory != null)
			{
				component.AddSymbolOverride(accessorySlot.targetSymbolId, accessory.symbol, 0);
				controller.SetSymbolVisiblity(accessorySlot.targetSymbolId, true);
			}
		}
		component.AddSymbolOverride(Db.Get().AccessorySlots.HatHair.targetSymbolId, Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(component2.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol, 1);
		CrewPortrait.RefreshHat(identityObject, controller);
		float num = 1f;
		if (GameScreenManager.Instance != null && GameScreenManager.Instance.ssOverlayCanvas != null)
		{
			num = 0.2f * (1f / GameScreenManager.Instance.ssOverlayCanvas.GetComponent<KCanvasScaler>().GetUserScale());
		}
		controller.animScale = num;
		string text = "ui";
		controller.Play(text, KAnim.PlayMode.Once, 1f, 0f);
		controller.SetSymbolVisiblity(CrewPortrait.snapTo_neck, false);
		controller.SetSymbolVisiblity(CrewPortrait.snapTo_pivot, false);
		controller.SetSymbolVisiblity(CrewPortrait.snapTo_rgthand, false);
		controller.SetSymbolVisiblity(CrewPortrait.snapTo_chest, false);
	}

	public void SetAlpha(float value)
	{
		if (this.controller == null)
		{
			return;
		}
		if ((float)this.controller.TintColour.a != value)
		{
			this.controller.TintColour = new Color(1f, 1f, 1f, value);
		}
	}

	public IAssignableIdentity identityObject;

	public Image targetImage;

	public bool startTransparent;

	public bool useLabels = true;

	[SerializeField]
	public KBatchedAnimController controller;

	public float animScaleBase = 0.2f;

	public LocText duplicantName;

	public LocText duplicantJob;

	public LocText subTitle;

	public bool useDefaultExpression = true;

	private bool requiresRefresh;

	private static readonly HashedString snapTo_neck = new HashedString("snapTo_neck");

	private static readonly HashedString snapTo_pivot = new HashedString("snapTo_pivot");

	private static readonly HashedString snapTo_rgthand = new HashedString("snapTo_rgthand");

	private static readonly HashedString snapTo_chest = new HashedString("snapTo_chest");
}
