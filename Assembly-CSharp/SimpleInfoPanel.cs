using System;
using UnityEngine;

public class SimpleInfoPanel
{
	public SimpleInfoPanel(SimpleInfoScreen simpleInfoRoot)
	{
		this.simpleInfoRoot = simpleInfoRoot;
	}

	public virtual void Refresh(CollapsibleDetailContentPanel panel, GameObject selectedTarget)
	{
	}

	protected SimpleInfoScreen simpleInfoRoot;
}
