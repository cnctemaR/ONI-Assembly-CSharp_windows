using System;
using UnityEngine.UI;

public class KLayoutElement : LayoutElement
{
	protected override void OnEnable()
	{
		bool flag = this.makeDirtyOnDisable;
		if (!this.hasEnabledOnce)
		{
			this.hasEnabledOnce = true;
			flag = true;
		}
		if (flag)
		{
			base.OnEnable();
		}
	}

	protected override void OnDisable()
	{
		bool flag = this.makeDirtyOnDisable;
		if (flag)
		{
			base.OnDisable();
		}
	}

	public bool makeDirtyOnDisable = true;

	private bool hasEnabledOnce;
}
