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
		base.enabled = this.updatePercentFull != null;
	}

	public void SetUpdateFunc(Func<float> func)
	{
		this.updatePercentFull = func;
		base.enabled = this.updatePercentFull != null;
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
		base.enabled = this.updatePercentFull != null;
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

	private void OnBecameInvisible()
	{
		base.enabled = false;
	}

	private void OnBecameVisible()
	{
		base.enabled = true;
	}

	public static ProgressBar CreateProgressBar(KMonoBehaviour entity, Func<float> updateFunc)
	{
		ProgressBar progressBar = Util.KInstantiateUI<ProgressBar>(ProgressBarsConfig.Instance.progressBarPrefab, null, false);
		progressBar.SetUpdateFunc(updateFunc);
		progressBar.transform.SetParent(GameScreenManager.Instance.worldSpaceCanvas.transform);
		progressBar.name = ((!(entity != null)) ? string.Empty : (entity.name + "_")) + " ProgressBar";
		progressBar.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("ProgressBar");
		progressBar.Update();
		Vector3 vector = entity.transform.GetPosition() + Vector3.down * 0.5f;
		if (entity is Building)
		{
			vector = vector - Vector3.right * 0.5f * (float)((entity as Building).Def.WidthInCells % 2) + (entity as Building).Def.placementPivot;
		}
		else
		{
			vector -= Vector3.right * 0.5f;
		}
		progressBar.transform.SetPosition(vector);
		return progressBar;
	}

	public Image bar;

	private Func<float> updatePercentFull;

	private int overlayUpdateHandle = -1;
}
