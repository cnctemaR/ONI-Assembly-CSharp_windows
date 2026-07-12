using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/CrewPortrait")]
[Serializable]
public class CrewPortrait : KMonoBehaviour
{
	public IAssignableIdentity identityObject { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (this.startTransparent)
		{
			base.StartCoroutine(this.AlphaIn());
		}
		this.requiresRefresh = true;
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
		if (this.controller == null)
		{
			return;
		}
		CrewPortrait.RefreshHat(this.identityObject, this.controller);
	}

	private void RegisterEvents()
	{
		if (this.areEventsRegistered)
		{
			return;
		}
		KMonoBehaviour kmonoBehaviour = this.identityObject as KMonoBehaviour;
		if (kmonoBehaviour == null)
		{
			return;
		}
		kmonoBehaviour.Subscribe(540773776, new Action<object>(this.OnRoleChanged));
		this.areEventsRegistered = true;
	}

	private void UnregisterEvents()
	{
		if (!this.areEventsRegistered)
		{
			return;
		}
		this.areEventsRegistered = false;
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
		if (this.useLabels && (identity is MinionIdentity || identity is MinionAssignablesProxy))
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
				return;
			}
			this.subTitle.gameObject.SetActive(true);
			this.subTitle.SetText(newTitle);
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
				global::Debug.LogWarning("Controller for [" + base.name + "] null");
				return;
			}
		}
		CrewPortrait.SetPortraitData(this.identityObject, this.controller, this.useDefaultExpression);
		if (this.useLabels && this.duplicantName != null)
		{
			this.duplicantName.SetText((!this.identityObject.IsNullOrDestroyed()) ? this.identityObject.GetProperName() : "");
			if (this.identityObject is MinionIdentity && this.duplicantJob != null)
			{
				this.duplicantJob.SetText((this.identityObject != null) ? (this.identityObject as MinionIdentity).GetComponent<MinionResume>().GetSkillsSubtitle() : "");
				this.duplicantJob.GetComponent<ToolTip>().toolTip = (this.identityObject as MinionIdentity).GetComponent<MinionResume>().GetSkillsSubtitle();
			}
		}
	}

	private static void RefreshHat(IAssignableIdentity identityObject, KBatchedAnimController controller)
	{
		string text = "";
		MinionIdentity minionIdentity = identityObject as MinionIdentity;
		if (minionIdentity != null)
		{
			text = minionIdentity.GetComponent<MinionResume>().CurrentHat;
		}
		else if (identityObject as StoredMinionIdentity != null)
		{
			text = (identityObject as StoredMinionIdentity).currentHat;
		}
		MinionResume.ApplyHat(text, controller);
	}

	public static void SetPortraitData(IAssignableIdentity identityObject, KBatchedAnimController controller, bool useDefaultExpression = true)
	{
		if (identityObject == null)
		{
			controller.gameObject.SetActive(false);
			return;
		}
		MinionIdentity minionIdentity = identityObject as MinionIdentity;
		if (minionIdentity == null)
		{
			MinionAssignablesProxy minionAssignablesProxy = identityObject as MinionAssignablesProxy;
			if (minionAssignablesProxy != null && minionAssignablesProxy.target != null)
			{
				minionIdentity = minionAssignablesProxy.target as MinionIdentity;
			}
		}
		controller.gameObject.SetActive(true);
		controller.Play("ui_idle", KAnim.PlayMode.Once, 1f, 0f);
		SymbolOverrideController component = controller.GetComponent<SymbolOverrideController>();
		component.RemoveAllSymbolOverrides(0);
		if (minionIdentity != null)
		{
			Accessorizer component2 = minionIdentity.GetComponent<Accessorizer>();
			foreach (AccessorySlot accessorySlot in Db.Get().AccessorySlots.resources)
			{
				Accessory accessory = component2.GetAccessory(accessorySlot);
				if (accessory != null)
				{
					component.AddSymbolOverride(accessorySlot.targetSymbolId, accessory.symbol, 0);
					controller.SetSymbolVisiblity(accessorySlot.targetSymbolId, true);
				}
				else
				{
					controller.SetSymbolVisiblity(accessorySlot.targetSymbolId, false);
				}
			}
			component.AddSymbolOverride(Db.Get().AccessorySlots.HatHair.targetSymbolId, Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(component2.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol, 1);
			CrewPortrait.RefreshHat(minionIdentity, controller);
		}
		else
		{
			StoredMinionIdentity storedMinionIdentity = identityObject as StoredMinionIdentity;
			if (storedMinionIdentity == null)
			{
				MinionAssignablesProxy minionAssignablesProxy2 = identityObject as MinionAssignablesProxy;
				if (minionAssignablesProxy2 != null && minionAssignablesProxy2.target != null)
				{
					storedMinionIdentity = minionAssignablesProxy2.target as StoredMinionIdentity;
				}
			}
			if (!(storedMinionIdentity != null))
			{
				controller.gameObject.SetActive(false);
				return;
			}
			foreach (AccessorySlot accessorySlot2 in Db.Get().AccessorySlots.resources)
			{
				Accessory accessory2 = storedMinionIdentity.GetAccessory(accessorySlot2);
				if (accessory2 != null)
				{
					component.AddSymbolOverride(accessorySlot2.targetSymbolId, accessory2.symbol, 0);
					controller.SetSymbolVisiblity(accessorySlot2.targetSymbolId, true);
				}
				else
				{
					controller.SetSymbolVisiblity(accessorySlot2.targetSymbolId, false);
				}
			}
			component.AddSymbolOverride(Db.Get().AccessorySlots.HatHair.targetSymbolId, Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(storedMinionIdentity.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol, 1);
			CrewPortrait.RefreshHat(storedMinionIdentity, controller);
		}
		float num = 0.25f;
		controller.animScale = num;
		string text = "ui_idle";
		controller.Play(text, KAnim.PlayMode.Loop, 1f, 0f);
		controller.SetSymbolVisiblity("snapTo_neck", false);
		controller.SetSymbolVisiblity("snapTo_goggles", false);
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

	private bool areEventsRegistered;
}
