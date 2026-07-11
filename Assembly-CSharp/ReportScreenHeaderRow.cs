using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ReportScreenHeaderRow")]
public class ReportScreenHeaderRow : KMonoBehaviour
{
	public void SetLine(ReportManager.ReportGroup reportGroup)
	{
		LayoutElement component = this.name.GetComponent<LayoutElement>();
		component.minWidth = (component.preferredWidth = this.nameWidth);
		this.spacer.minWidth = this.groupSpacerWidth;
		this.name.text = reportGroup.stringKey;
	}

	[SerializeField]
	public new LocText name;

	[SerializeField]
	private LayoutElement spacer;

	[SerializeField]
	private Image bgImage;

	public float groupSpacerWidth;

	private float nameWidth = 164f;

	[SerializeField]
	private Color oddRowColor;
}
