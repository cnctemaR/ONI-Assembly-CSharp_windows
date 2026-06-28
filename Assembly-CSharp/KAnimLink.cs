using System;
using UnityEngine;

public class KAnimLink
{
	public KAnimLink(KAnimControllerBase master, KAnimControllerBase slave)
	{
		this.slave = slave;
		this.master = master;
		this.Register();
	}

	private void Register()
	{
		this.master.onOverlayColourChanged += this.OnOverlayColourChanged;
		KAnimControllerBase kanimControllerBase = this.master;
		kanimControllerBase.OnTintChanged = (Action<Color32>)Delegate.Combine(kanimControllerBase.OnTintChanged, new Action<Color32>(this.OnTintColourChanged));
		KAnimControllerBase kanimControllerBase2 = this.master;
		kanimControllerBase2.OnHighlightChanged = (Action<Color32>)Delegate.Combine(kanimControllerBase2.OnHighlightChanged, new Action<Color32>(this.OnHighlightColourChanged));
		this.master.onLayerChanged += this.slave.SetLayer;
	}

	public void Unregister()
	{
		if (this.master != null)
		{
			this.master.onOverlayColourChanged -= this.OnOverlayColourChanged;
			KAnimControllerBase kanimControllerBase = this.master;
			kanimControllerBase.OnTintChanged = (Action<Color32>)Delegate.Remove(kanimControllerBase.OnTintChanged, new Action<Color32>(this.OnTintColourChanged));
			KAnimControllerBase kanimControllerBase2 = this.master;
			kanimControllerBase2.OnHighlightChanged = (Action<Color32>)Delegate.Remove(kanimControllerBase2.OnHighlightChanged, new Action<Color32>(this.OnHighlightColourChanged));
			if (this.slave != null)
			{
				this.master.onLayerChanged -= this.slave.SetLayer;
			}
		}
	}

	private void OnOverlayColourChanged(Color32 c)
	{
		if (this.slave != null)
		{
			this.slave.OverlayColour = c;
		}
	}

	private void OnTintColourChanged(Color32 c)
	{
		if (this.slave != null)
		{
			this.slave.TintColour = c;
		}
	}

	private void OnHighlightColourChanged(Color32 c)
	{
		if (this.slave != null)
		{
			this.slave.HighlightColour = c;
		}
	}

	private KAnimControllerBase master;

	private KAnimControllerBase slave;
}
