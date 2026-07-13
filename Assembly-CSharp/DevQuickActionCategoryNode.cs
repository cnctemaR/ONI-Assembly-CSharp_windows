using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DevQuickActionCategoryNode : DevQuickActionNode
{
	private bool IsExpanded
	{
		get
		{
			return this.toggle.isOn;
		}
	}

	protected void Awake()
	{
		this.toggle = base.GetComponent<Toggle>();
		this.originalColorBlock = this.toggle.colors;
		this.pressedColorBlock = this.toggle.colors;
		this.pressedColorBlock.normalColor = this.originalColorBlock.pressedColor;
		this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleValueChanged));
		this.RefreshVisuals();
	}

	public void Setup(string name, DevQuickActionNode parentNode)
	{
		this.label.SetText(name);
		this.parentNode = parentNode;
	}

	private void RefreshVisuals()
	{
		(this.toggle.targetGraphic as Image).sprite = (this.IsExpanded ? this.pressedSprite : this.notPressedSprite);
		this.toggle.colors = (this.IsExpanded ? this.pressedColorBlock : this.originalColorBlock);
	}

	private void OnToggleValueChanged(bool value)
	{
		this.RefreshVisuals();
		if (this.IsExpanded)
		{
			this.OnExpand();
		}
		else
		{
			this.OnCollapsed();
		}
		global::System.Action onNodeInteractedWith = this.OnNodeInteractedWith;
		if (onNodeInteractedWith == null)
		{
			return;
		}
		onNodeInteractedWith();
	}

	public virtual void Expand()
	{
		this.toggle.isOn = true;
	}

	public void Collapse()
	{
		this.toggle.isOn = false;
	}

	private void OnExpand()
	{
		Vector2 vector = Vector2.up;
		if (this.parentNode != null)
		{
			vector = base.transform.anchoredPosition - this.parentNode.transform.anchoredPosition;
		}
		int count = this.childrenNodes.Count;
		float num = 180f / (float)(count + 1);
		for (int i = 0; i < count; i++)
		{
			DevQuickActionNode devQuickActionNode = this.childrenNodes[i];
			Vector2 vector2 = this.RotateVector2Clockwise(vector, num * (float)i);
			Vector2 vector3 = base.transform.anchoredPosition + vector2.normalized * this.space;
			devQuickActionNode.transform.anchoredPosition = vector3;
			devQuickActionNode.gameObject.SetActive(true);
		}
	}

	private void OnCollapsed()
	{
		foreach (DevQuickActionNode devQuickActionNode in this.childrenNodes)
		{
			if (devQuickActionNode is DevQuickActionCategoryNode)
			{
				(devQuickActionNode as DevQuickActionCategoryNode).Collapse();
			}
			devQuickActionNode.gameObject.SetActive(false);
		}
	}

	public void AddChildren(DevQuickActionNode node)
	{
		if (!this.childrenNodes.Contains(node))
		{
			this.childrenNodes.Add(node);
		}
	}

	private Vector2 RotateVector2Clockwise(Vector2 v, float angleDegrees)
	{
		float num = angleDegrees * 0.017453292f;
		float num2 = Mathf.Cos(num);
		float num3 = Mathf.Sin(num);
		return new Vector2(v.x * num2 + v.y * num3, -v.x * num3 + v.y * num2);
	}

	public override void Recycle()
	{
		foreach (DevQuickActionNode devQuickActionNode in this.childrenNodes)
		{
			devQuickActionNode.Recycle();
		}
		this.toggle.SetIsOnWithoutNotify(false);
		this.RefreshVisuals();
		base.Recycle();
		this.childrenNodes.Clear();
	}

	public Sprite pressedSprite;

	public Sprite notPressedSprite;

	private Toggle toggle;

	protected List<DevQuickActionNode> childrenNodes = new List<DevQuickActionNode>();

	private ColorBlock originalColorBlock;

	private ColorBlock pressedColorBlock;
}
