using System;
using Klei.AI;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CrewPortrait : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.requiresRefresh = true;
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (global::System.Action)Delegate.Combine(instance.OnResize, new global::System.Action(this.RefreshScale));
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (global::System.Action)Delegate.Remove(instance.OnResize, new global::System.Action(this.RefreshScale));
	}

	public void SetCrewMember(MinionIdentity identity, bool jobEnabled = true)
	{
		if (identity != null)
		{
			this.crewMember = identity.gameObject;
		}
		this.SetDuplicantJobTitleActive(jobEnabled);
		this.requiresRefresh = true;
	}

	public void SetNameLabel(string newTitle)
	{
		if (this.duplicantName != null)
		{
			this.duplicantName.SetText(newTitle);
		}
	}

	public void SetDuplicantJobTitleActive(bool state)
	{
		if (this.duplicantJob != null && this.duplicantJob.gameObject.activeInHierarchy != state)
		{
			this.duplicantJob.gameObject.SetActive(state);
		}
	}

	public void SetPortraitScale(float scale)
	{
		this.targetImage.transform.localScale = Vector3.one * scale;
		this.RefreshScale();
	}

	public void ForceRefresh()
	{
		this.requiresRefresh = true;
	}

	public void Update()
	{
		if (this.requiresRefresh && this.crewMember != null)
		{
			this.requiresRefresh = false;
			this.Rebuild();
			this.RefreshScale();
		}
	}

	private void RefreshScale()
	{
		float num = 1f;
		if (GameScreenManager.Instance != null && GameScreenManager.Instance.ssOverlayCanvas != null)
		{
			num = GameScreenManager.Instance.ssOverlayCanvas.GetComponent<KCanvasScaler>().GetCanvasScale();
		}
		if (this.controller != null)
		{
			this.controller.animScale = this.animScaleBase * (1f / num);
		}
	}

	private void Rebuild()
	{
		if (this.controller == null)
		{
			this.controller = base.GetComponentInChildren<KBatchedAnimController>();
			if (this.controller == null)
			{
				if (this.targetImage != null)
				{
					this.targetImage.enabled = true;
				}
				Debug.LogWarning("Controller for [" + base.name + "] null");
				return;
			}
		}
		if (this.targetImage != null)
		{
			this.targetImage.enabled = false;
		}
		CrewPortrait.SetPortraitData(this.crewMember, this.controller, this.useDefaultExpression);
		if (this.duplicantName != null)
		{
			this.duplicantName.SetText(this.crewMember.GetProperName());
		}
		if (this.duplicantJob != null)
		{
			this.duplicantJob.SetText(this.crewMember.GetAttributes().GetProfessionString());
			this.duplicantJob.GetComponent<ToolTip>().toolTip = this.crewMember.GetAttributes().GetProfessionDescriptionString();
		}
	}

	public static void SetPortraitData(GameObject crewMember, KBatchedAnimController controller, bool useDefaultExpression = true)
	{
		controller.gameObject.SetActive(true);
		FaceGraph component = crewMember.GetComponent<FaceGraph>();
		KCompBuildInstance headComp = component.GetHeadComp();
		controller.ClearAnims();
		controller.Flip = true;
		controller.SetAnims(new KAnimFile[] { Assets.GetAnim("body_comp_default") }, false);
		controller.AddBuildOverride(headComp.GetData(), true);
		headComp.Refresh(controller);
		controller.animScale = 0.2f * (1f / global::UnityEngine.Object.FindObjectOfType<KCanvasScaler>().GetUserScale());
		string text = "ui";
		if (!useDefaultExpression)
		{
			Expression currentExpression = component.GetCurrentExpression();
			if (currentExpression != null)
			{
				text = currentExpression.face.Id;
			}
		}
		controller.Play(text, KAnim.PlayMode.Once, 1f, 0f);
	}

	public void SetAlpha(float value)
	{
		if (this.controller == null)
		{
			return;
		}
		if ((float)this.controller.TintColour.a != value)
		{
			this.controller.TintColour = new Color(1f, 1f, 1f, value);
		}
	}

	public GameObject crewMember;

	public Image targetImage;

	public bool startTransparent;

	[SerializeField]
	private KBatchedAnimController controller;

	public float animScaleBase = 0.2f;

	public LocText duplicantName;

	public LocText duplicantJob;

	public bool useDefaultExpression = true;

	private bool requiresRefresh;
}
