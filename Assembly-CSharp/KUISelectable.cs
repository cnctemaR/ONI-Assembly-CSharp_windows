using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/KUISelectable")]
public class KUISelectable : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	protected override void OnPrefabInit()
	{
	}

	protected override void OnSpawn()
	{
		base.GetComponent<Button>().onClick.AddListener(new UnityAction(this.OnClick));
	}

	public void SetTarget(GameObject target)
	{
		this.target = target;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (this.target != null)
		{
			SelectTool.Instance.SetHoverOverride(this.target.GetComponent<KSelectable>());
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		SelectTool.Instance.SetHoverOverride(null);
	}

	private void OnClick()
	{
		if (this.target != null)
		{
			SelectTool.Instance.Select(this.target.GetComponent<KSelectable>(), false);
		}
	}

	protected override void OnCmpDisable()
	{
		if (SelectTool.Instance != null)
		{
			SelectTool.Instance.SetHoverOverride(null);
		}
	}

	private GameObject target;
}
