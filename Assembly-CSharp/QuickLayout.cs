using System;
using UnityEngine;

public class QuickLayout : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Run(false);
	}

	private void Update()
	{
		this.Run(false);
	}

	public void ForceUpdate()
	{
		this.Run(true);
	}

	private void Run(bool forceUpdate = false)
	{
		forceUpdate = forceUpdate || this._elementSize != this.elementSize;
		forceUpdate = forceUpdate || this._spacing != this.spacing;
		forceUpdate = forceUpdate || this._layoutDirection != this.layoutDirection;
		forceUpdate = forceUpdate || this._offset != this.offset;
		if (forceUpdate)
		{
			this._elementSize = this.elementSize;
			this._spacing = this.spacing;
			this._layoutDirection = this.layoutDirection;
			this._offset = this.offset;
		}
		int num = 0;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			if (base.transform.GetChild(i).gameObject.activeInHierarchy)
			{
				num++;
			}
		}
		if (num != this.oldActiveChildCount || forceUpdate)
		{
			this.Layout();
			this.oldActiveChildCount = num;
		}
	}

	public void Layout()
	{
		Vector3 vector = this._offset;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			if (base.transform.GetChild(i).gameObject.activeInHierarchy)
			{
				base.transform.GetChild(i).rectTransform().anchoredPosition = vector;
				vector += (float)(this._elementSize + this._spacing) * this.GetDirectionVector();
			}
		}
	}

	private Vector2 GetDirectionVector()
	{
		Vector2 vector = Vector3.zero;
		switch (this._layoutDirection)
		{
		case QuickLayout.LayoutDirection.TopToBottom:
			vector = Vector2.down;
			break;
		case QuickLayout.LayoutDirection.BottomToTop:
			vector = Vector2.up;
			break;
		case QuickLayout.LayoutDirection.LeftToRight:
			vector = Vector2.right;
			break;
		case QuickLayout.LayoutDirection.RightToLeft:
			vector = Vector2.left;
			break;
		}
		return vector;
	}

	[Header("Configuration")]
	[SerializeField]
	private int elementSize;

	[SerializeField]
	private int spacing;

	[SerializeField]
	private QuickLayout.LayoutDirection layoutDirection;

	[SerializeField]
	private Vector2 offset;

	private int _elementSize;

	private int _spacing;

	private QuickLayout.LayoutDirection _layoutDirection;

	private Vector2 _offset;

	private int oldActiveChildCount;

	private enum LayoutDirection
	{
		TopToBottom,
		BottomToTop,
		LeftToRight,
		RightToLeft
	}
}
