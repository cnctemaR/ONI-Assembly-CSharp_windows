using System;
using KSerialization;
using UnityEngine;

public class Prioritizable : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		if (this.onPriorityChanged != null)
		{
			this.onPriorityChanged(this.masterPriority);
		}
		Game.Instance.Subscribe(1248612973, new EventSystem.EventHandler(this.OnEnableOverlay));
		Game.Instance.Subscribe(1798162660, new EventSystem.EventHandler(this.OnEnableOverlay));
		Game.Instance.Subscribe(2015652040, new EventSystem.EventHandler(this.OnDisableOverlay));
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
		if (base.GetComponent<Clearable>() == null && base.GetComponent<MinionIdentity>() == null && base.GetComponent<Harvestable>() == null && base.GetComponent<AttackableBase>() == null)
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
			componentInChildren.outlineWidth = 0.3f;
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
		Game.Instance.Unsubscribe(1248612973, new EventSystem.EventHandler(this.OnEnableOverlay));
		Game.Instance.Unsubscribe(2015652040, new EventSystem.EventHandler(this.OnDisableOverlay));
		Game.Instance.Unsubscribe(1798162660, new EventSystem.EventHandler(this.OnEnableOverlay));
	}

	[Serialize]
	[SerializeField]
	private int masterPriority = 5;

	public Action<int> onPriorityChanged;

	private RectTransform priorityOverlayIcon;
}
