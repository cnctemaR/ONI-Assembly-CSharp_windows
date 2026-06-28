using System;
using UnityEngine;
using UnityEngine.UI;

public class FilterSideScreenRow : KMonoBehaviour
{
	public Element element { get; private set; }

	public bool isSelected { get; private set; }

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.regularColor = this.outline.color;
		if (this.button != null)
		{
			KButton kbutton = this.button;
			kbutton.onPointerEnter = (global::System.Action)Delegate.Combine(kbutton.onPointerEnter, new global::System.Action(delegate
			{
				if (!this.isSelected)
				{
					this.outline.color = this.outlineHighLightColor;
				}
			}));
			KButton kbutton2 = this.button;
			kbutton2.onPointerExit = (global::System.Action)Delegate.Combine(kbutton2.onPointerExit, new global::System.Action(delegate
			{
				if (!this.isSelected)
				{
					this.outline.color = this.regularColor;
				}
			}));
		}
	}

	public void SetElement(Element elem)
	{
		this.element = elem;
		this.SetText(elem.name);
	}

	private void SetText(string assignmentStr)
	{
		this.labelText.text = (string.IsNullOrEmpty(assignmentStr) ? "-" : assignmentStr);
	}

	public void SetSelected(bool selected)
	{
		this.isSelected = selected;
		this.outline.color = ((!selected) ? this.outlineDefaultColor : this.outlineHighLightColor);
		this.BG.color = ((!selected) ? Color.white : this.BGHighLightColor);
	}

	[SerializeField]
	private LocText labelText;

	[SerializeField]
	private Image BG;

	[SerializeField]
	private Image outline;

	[SerializeField]
	private Color outlineHighLightColor = new Color32(168, 74, 121, byte.MaxValue);

	[SerializeField]
	private Color BGHighLightColor = new Color32(168, 74, 121, 80);

	[SerializeField]
	private Color outlineDefaultColor = new Color32(204, 204, 204, byte.MaxValue);

	private Color regularColor = Color.white;

	[SerializeField]
	public KButton button;
}
