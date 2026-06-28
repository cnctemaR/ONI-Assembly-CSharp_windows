using System;
using Klei.AI;
using UnityEngine;
using UnityEngine.UI;

public class DiseaseSourceVisualizer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.UpdateVisibility();
		OverlayScreen instance = OverlayScreen.Instance;
		instance.OnOverlayChanged = (Action<SimViewMode>)Delegate.Combine(instance.OnOverlayChanged, new Action<SimViewMode>(this.OnViewModeChanged));
	}

	protected override void OnCleanUp()
	{
		OverlayScreen instance = OverlayScreen.Instance;
		instance.OnOverlayChanged = (Action<SimViewMode>)Delegate.Remove(instance.OnOverlayChanged, new Action<SimViewMode>(this.OnViewModeChanged));
		base.OnCleanUp();
	}

	private void CreateVisualizer()
	{
		if (this.visualizer != null)
		{
			return;
		}
		if (GameScreenManager.Instance.worldSpaceCanvas == null)
		{
			return;
		}
		this.visualizer = Util.KInstantiate(Assets.UIPrefabs.ResourceVisualizer, GameScreenManager.Instance.worldSpaceCanvas, null);
		this.visualizer.SetActive(false);
	}

	public void UpdateVisibility()
	{
		this.CreateVisualizer();
		if (string.IsNullOrEmpty(this.alwaysShowDisease))
		{
			this.visible = false;
			MinionModifiers component = base.gameObject.GetComponent<MinionModifiers>();
			Diseases diseases = component.diseases;
			if (diseases.Count > 0)
			{
				DiseaseInstance diseaseInstance = diseases[0];
				Disease modifier = diseaseInstance.modifier;
				this.SetVisibleDisease(modifier);
			}
		}
		else
		{
			Disease disease = Db.Get().Diseases.Get(this.alwaysShowDisease);
			if (disease != null)
			{
				this.SetVisibleDisease(disease);
			}
		}
		if (OverlayScreen.Instance != null)
		{
			this.Show(OverlayScreen.Instance.GetMode());
		}
	}

	private void SetVisibleDisease(Disease disease)
	{
		Sprite overlaySprite = Assets.instance.DiseaseVisualization.overlaySprite;
		Color32 overlayColour = disease.overlayColour;
		Transform child = this.visualizer.transform.GetChild(0);
		Image component = child.GetComponent<Image>();
		component.sprite = overlaySprite;
		component.color = overlayColour;
		this.visible = true;
	}

	private void Update()
	{
		if (this.visualizer == null)
		{
			return;
		}
		this.visualizer.transform.position = base.transform.position + this.offset;
	}

	private void OnViewModeChanged(SimViewMode mode)
	{
		this.Show(mode);
	}

	private void Show(SimViewMode mode)
	{
		base.enabled = this.visible && mode == SimViewMode.Disease;
		if (this.visualizer != null)
		{
			this.visualizer.SetActive(base.enabled);
		}
	}

	[SerializeField]
	private Vector3 offset;

	private GameObject visualizer;

	private bool visible;

	public string alwaysShowDisease;
}
