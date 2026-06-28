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

	public Image bar;

	private Func<float> updatePercentFull;
}
