using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableTextStyler : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	private void Start()
	{
		this.SetState(this.state, SelectableTextStyler.HoverState.Normal);
	}

	private void SetState(SelectableTextStyler.State state, SelectableTextStyler.HoverState hover_state)
	{
		if (state == SelectableTextStyler.State.Normal)
		{
			if (hover_state != SelectableTextStyler.HoverState.Normal)
			{
				if (hover_state == SelectableTextStyler.HoverState.Hovered)
				{
					this.target.textStyleSetting = this.normalHovered;
				}
			}
			else
			{
				this.target.textStyleSetting = this.normalNormal;
			}
		}
		this.target.ApplySettings();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		this.SetState(this.state, SelectableTextStyler.HoverState.Hovered);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		this.SetState(this.state, SelectableTextStyler.HoverState.Normal);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		this.SetState(this.state, SelectableTextStyler.HoverState.Normal);
	}

	[SerializeField]
	private LocText target;

	[SerializeField]
	private SelectableTextStyler.State state;

	[SerializeField]
	private TextStyleSetting normalNormal;

	[SerializeField]
	private TextStyleSetting normalHovered;

	public enum State
	{
		Normal
	}

	public enum HoverState
	{
		Normal,
		Hovered
	}
}
