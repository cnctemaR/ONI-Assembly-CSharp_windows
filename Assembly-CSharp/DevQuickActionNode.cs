using System;
using TMPro;
using UnityEngine;

public class DevQuickActionNode : MonoBehaviour
{
	public new RectTransform transform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	public void SetChildrenSeparationSpace(float space)
	{
		this.space = space;
	}

	public virtual void Recycle()
	{
		this.parentNode = null;
		this.OnNodeInteractedWith = null;
		base.gameObject.SetActive(false);
		Action<DevQuickActionNode> onRecycle = this.OnRecycle;
		if (onRecycle == null)
		{
			return;
		}
		onRecycle(this);
	}

	public TextMeshProUGUI label;

	protected DevQuickActionNode parentNode;

	public Action<DevQuickActionNode> OnRecycle;

	protected global::System.Action OnNodeInteractedWith;

	protected float space = 100f;
}
