using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ProgressBar")]
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

	public void SetVisibility(bool visible)
	{
		this.lastVisibilityValue = visible;
		this.RefreshVisibility();
	}

	private void RefreshVisibility()
	{
		int myWorldId = base.gameObject.GetMyWorldId();
		bool flag = this.lastVisibilityValue;
		flag &= !this.hasBeenInitialize || myWorldId == ClusterManager.Instance.activeWorldId;
		flag &= !this.autoHide || SimDebugView.Instance == null || SimDebugView.Instance.GetMode() == OverlayModes.None.ID;
		base.gameObject.SetActive(flag);
		if (this.updatePercentFull == null || this.updatePercentFull.Target.IsNullOrDestroyed())
		{
			base.gameObject.SetActive(false);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.hasBeenInitialize = true;
		if (this.autoHide)
		{
			this.overlayUpdateHandle = Game.Instance.Subscribe(1798162660, new Action<object>(this.OnOverlayChanged));
			if (SimDebugView.Instance != null && SimDebugView.Instance.GetMode() != OverlayModes.None.ID)
			{
				base.gameObject.SetActive(false);
			}
		}
		this.activeWorldChangedHandlerID = Game.Instance.Subscribe(1983128072, new Action<object>(this.OnActiveWorldChanged));
		this.SetWorldActive(ClusterManager.Instance.activeWorldId);
		base.enabled = this.updatePercentFull != null;
		this.RefreshVisibility();
	}

	private void OnActiveWorldChanged(object data)
	{
		global::Tuple<int, int> tuple = (global::Tuple<int, int>)data;
		this.SetWorldActive(tuple.first);
	}

	private void SetWorldActive(int worldId)
	{
		this.RefreshVisibility();
	}

	public void SetUpdateFunc(Func<float> func)
	{
		this.updatePercentFull = func;
		base.enabled = this.updatePercentFull != null;
	}

	public virtual void Update()
	{
		if (this.updatePercentFull != null && !this.updatePercentFull.Target.IsNullOrDestroyed())
		{
			this.PercentFull = this.updatePercentFull();
		}
	}

	public virtual void OnOverlayChanged(object _ = null)
	{
		this.RefreshVisibility();
	}

	public void Retarget(GameObject entity)
	{
		Vector3 vector = entity.transform.GetPosition() + Vector3.down * 0.5f;
		Building component = entity.GetComponent<Building>();
		if (component != null)
		{
			vector -= Vector3.right * 0.5f * (float)(component.Def.WidthInCells % 2);
		}
		else
		{
			vector -= Vector3.right * 0.5f;
		}
		vector += this.offset;
		base.transform.SetPosition(vector);
	}

	protected override void OnCleanUp()
	{
		if (this.overlayUpdateHandle != -1)
		{
			Game.Instance.Unsubscribe(this.overlayUpdateHandle);
		}
		Game.Instance.Unsubscribe(ref this.activeWorldChangedHandlerID);
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

	public static ProgressBar CreateProgressBar(GameObject entity, Func<float> updateFunc)
	{
		return ProgressBar.CreateProgressBar(entity, updateFunc, Vector3.zero);
	}

	public static ProgressBar CreateProgressBar(GameObject entity, Func<float> updateFunc, Vector3 offset)
	{
		ProgressBar progressBar = Util.KInstantiateUI<ProgressBar>(ProgressBarsConfig.Instance.progressBarPrefab, null, false);
		progressBar.offset = offset;
		progressBar.SetUpdateFunc(updateFunc);
		progressBar.transform.SetParent(GameScreenManager.Instance.worldSpaceCanvas.transform);
		progressBar.name = ((entity != null) ? (entity.name + "_") : "") + " ProgressBar";
		progressBar.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("ProgressBar");
		progressBar.Update();
		progressBar.Retarget(entity);
		return progressBar;
	}

	public Image bar;

	private Func<float> updatePercentFull;

	private int overlayUpdateHandle = -1;

	public bool autoHide = true;

	private Vector3 offset = Vector3.zero;

	private bool lastVisibilityValue = true;

	private bool hasBeenInitialize;

	private int activeWorldChangedHandlerID = -1;
}
