using System;
using UnityEngine;

public class IncubatorSideScreen : ReceptacleSideScreen
{
	protected override void SetResultDescriptions(GameObject go)
	{
		string text = string.Empty;
		InfoDescription component = go.GetComponent<InfoDescription>();
		if (component)
		{
			text += component.description;
		}
		this.descriptionLabel.SetText(text);
	}

	public DescriptorPanel RequirementsDescriptorPanel;

	public DescriptorPanel HarvestDescriptorPanel;

	public DescriptorPanel EffectsDescriptorPanel;
}
