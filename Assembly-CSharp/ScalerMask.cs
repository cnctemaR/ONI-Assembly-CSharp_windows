using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ScalerMask")]
public class ScalerMask : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private RectTransform ThisTransform
	{
		get
		{
			if (this._thisTransform == null)
			{
				this._thisTransform = base.GetComponent<RectTransform>();
			}
			return this._thisTransform;
		}
	}

	private LayoutElement ThisLayoutElement
	{
		get
		{
			if (this._thisLayoutElement == null)
			{
				this._thisLayoutElement = base.GetComponent<LayoutElement>();
			}
			return this._thisLayoutElement;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		DetailsScreen componentInParent = base.GetComponentInParent<DetailsScreen>();
		if (componentInParent)
		{
			DetailsScreen detailsScreen = componentInParent;
			detailsScreen.pointerEnterActions = (KScreen.PointerEnterActions)Delegate.Combine(detailsScreen.pointerEnterActions, new KScreen.PointerEnterActions(this.OnPointerEnterGrandparent));
			DetailsScreen detailsScreen2 = componentInParent;
			detailsScreen2.pointerExitActions = (KScreen.PointerExitActions)Delegate.Combine(detailsScreen2.pointerExitActions, new KScreen.PointerExitActions(this.OnPointerExitGrandparent));
		}
	}

	protected override void OnCleanUp()
	{
		DetailsScreen componentInParent = base.GetComponentInParent<DetailsScreen>();
		if (componentInParent)
		{
			DetailsScreen detailsScreen = componentInParent;
			detailsScreen.pointerEnterActions = (KScreen.PointerEnterActions)Delegate.Remove(detailsScreen.pointerEnterActions, new KScreen.PointerEnterActions(this.OnPointerEnterGrandparent));
			DetailsScreen detailsScreen2 = componentInParent;
			detailsScreen2.pointerExitActions = (KScreen.PointerExitActions)Delegate.Remove(detailsScreen2.pointerExitActions, new KScreen.PointerExitActions(this.OnPointerExitGrandparent));
		}
		base.OnCleanUp();
	}

	private void Update()
	{
		if (this.SourceTransform != null)
		{
			this.SourceTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.ThisTransform.rect.width);
		}
		if (this.SourceTransform != null && (!this.hoverLock || !this.grandparentIsHovered || this.isHovered || this.queuedSizeUpdate))
		{
			this.ThisLayoutElement.minHeight = this.SourceTransform.rect.height + this.topPadding + this.bottomPadding;
			this.SourceTransform.anchoredPosition = new Vector2(0f, -this.topPadding);
			this.queuedSizeUpdate = false;
		}
		if (this.hoverIndicator != null)
		{
			if (this.SourceTransform != null && this.SourceTransform.rect.height > this.ThisTransform.rect.height)
			{
				this.hoverIndicator.SetActive(true);
				return;
			}
			this.hoverIndicator.SetActive(false);
		}
	}

	public void UpdateSize()
	{
		this.queuedSizeUpdate = true;
	}

	public void OnPointerEnterGrandparent(PointerEventData eventData)
	{
		this.grandparentIsHovered = true;
	}

	public void OnPointerExitGrandparent(PointerEventData eventData)
	{
		this.grandparentIsHovered = false;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		this.isHovered = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		this.isHovered = false;
	}

	public RectTransform SourceTransform;

	private RectTransform _thisTransform;

	private LayoutElement _thisLayoutElement;

	public GameObject hoverIndicator;

	public bool hoverLock;

	private bool grandparentIsHovered;

	private bool isHovered;

	private bool queuedSizeUpdate = true;

	public float topPadding;

	public float bottomPadding;
}
