using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : KMonoBehaviour
{
	public Color barColor
	{
		get
		{
			return this.bar.color;
		}
		set
		{
			this.bar.color = value;
		}
	}

	public float PercentFull
	{
		get
		{
			return this.bar.fillAmount;
		}
		set
		{
			this.bar.fillAmount = value;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.overlayUpdateHandle = Game.Instance.Subscribe(1798162660, new Action<object>(this.OnOverlayChanged));
		if (OverlayScreen.Instance != null && OverlayScreen.Instance.GetMode() != SimViewMode.None)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void SetUpdateFunc(Func<float> func)
	{
		this.updatePercentFull = func;
	}

	public virtual void Update()
	{
		if (this.updatePercentFull != null)
		{
			this.PercentFull = this.updatePercentFull();
		}
	}

	public void ClearPercentFunction()
	{
		this.updatePercentFull = null;
	}

	public virtual void OnOverlayChanged(object data = null)
	{
		if ((SimViewMode)data == SimViewMode.None)
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
		}
		else if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(false);
		}
	}

	protected override void OnCleanUp()
	{
		if (this.overlayUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(this.overlayUpdateHandle);
		}
		base.OnCleanUp();
	}

	public Image bar;

	private Func<float> updatePercentFull;

	private int overlayUpdateHandle = -1;
}
