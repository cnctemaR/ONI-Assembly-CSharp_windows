using System;
using KSerialization;
using UnityEngine;

public class Prioritizable : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		Prioritizable component = gameObject.GetComponent<Prioritizable>();
		if (component != null)
		{
			this.SetMasterPriority(component.GetMasterPriority());
		}
	}

	protected override void OnSpawn()
	{
		if (this.onPriorityChanged != null)
		{
			this.onPriorityChanged(this.masterPriority);
		}
		Game.Instance.Subscribe(1248612973, new Action<object>(this.OnEnableOverlay));
		Game.Instance.Subscribe(1798162660, new Action<object>(this.OnEnableOverlay));
		Game.Instance.Subscribe(2015652040, new Action<object>(this.OnDisableOverlay));
		this.OnEnableOverlay(SimDebugView.Instance.GetMode());
	}

	public int GetMasterPriority()
	{
		return this.masterPriority;
	}

	public void SetMasterPriority(int priority)
	{
		if (priority != this.masterPriority)
		{
			this.masterPriority = priority;
			if (this.onPriorityChanged != null)
			{
				this.onPriorityChanged(this.masterPriority);
			}
			this.RefreshOverlayIcon();
		}
	}

	private void DestroyOverlayIcon()
	{
		if (this.priorityOverlayIcon != null)
		{
			global::UnityEngine.Object.Destroy(this.priorityOverlayIcon.gameObject);
			this.priorityOverlayIcon = null;
		}
	}

	private void CreateOverlayIcon()
	{
		if (this.priorityOverlayIcon != null)
		{
			return;
		}
		if (base.GetComponent<Clearable>() == null && base.GetComponent<MinionIdentity>() == null && (base.GetComponent<Uprootable>() == null || !base.GetComponent<Uprootable>().IsInPlanterBox()) && base.GetComponent<AttackableBase>() == null)
		{
			this.priorityOverlayIcon = Util.KInstantiate(Assets.UIPrefabs.PriorityOverlayIcon, GameScreenManager.Instance.worldSpaceCanvas, null).GetComponent<RectTransform>();
			KAnimControllerBase component = base.GetComponent<KAnimControllerBase>();
			if (component != null)
			{
				this.priorityOverlayIcon.transform.position = component.GetWorldPivot();
			}
			else
			{
				this.priorityOverlayIcon.transform.position = this.transform.position;
			}
			this.RefreshOverlayIcon();
		}
	}

	private void RefreshOverlayIcon()
	{
		if (this.priorityOverlayIcon != null)
		{
			OverlayLegend.OverlayInfo overlayInfo = OverlayLegend.Instance.GetOverlayInfo(SimViewMode.Priorities);
			LocText componentInChildren = this.priorityOverlayIcon.GetComponentInChildren<LocText>();
			componentInChildren.text = this.masterPriority.ToString();
			componentInChildren.color = overlayInfo.infoUnits[this.masterPriority - 1].color;
			componentInChildren.outlineColor = Color.white;
			if (Localization.isLocalized)
			{
				componentInChildren.outlineWidth = 0.08f;
			}
			else
			{
				componentInChildren.outlineWidth = 0.3f;
			}
		}
	}

	private void OnEnableOverlay(object data)
	{
		if ((int)data == 1529952898)
		{
			this.CreateOverlayIcon();
		}
		else
		{
			this.DestroyOverlayIcon();
		}
	}

	private void OnDisableOverlay(object data)
	{
		this.DestroyOverlayIcon();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.DestroyOverlayIcon();
		Game.Instance.Unsubscribe(1248612973, new Action<object>(this.OnEnableOverlay));
		Game.Instance.Unsubscribe(2015652040, new Action<object>(this.OnDisableOverlay));
		Game.Instance.Unsubscribe(1798162660, new Action<object>(this.OnEnableOverlay));
	}

	[SerializeField]
	[Serialize]
	private int masterPriority = 5;

	public Action<int> onPriorityChanged;

	public RectTransform priorityOverlayIcon;
}
