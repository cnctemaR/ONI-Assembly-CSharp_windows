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
		Components.DiseaseSourceVisualizers.Add(this);
	}

	protected override void OnCleanUp()
	{
		OverlayScreen instance = OverlayScreen.Instance;
		instance.OnOverlayChanged = (Action<HashedString>)Delegate.Remove(instance.OnOverlayChanged, new Action<HashedString>(this.OnViewModeChanged));
		base.OnCleanUp();
		Components.DiseaseSourceVisualizers.Remove(this);
		if (this.visualizer != null)
		{
			global::UnityEngine.Object.Destroy(this.visualizer);
			this.visualizer = null;
		}
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
	}

	public void UpdateVisibility()
	{
		this.CreateVisualizer();
		if (string.IsNullOrEmpty(this.alwaysShowDisease))
		{
			this.visible = false;
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
		Image component = this.visualizer.transform.GetChild(0).GetComponent<Image>();
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
		this.visualizer.transform.SetPosition(base.transform.GetPosition() + this.offset);
	}

	private void OnViewModeChanged(HashedString mode)
	{
		this.Show(mode);
	}

	public void Show(HashedString mode)
	{
		base.enabled = this.visible && mode == OverlayModes.Disease.ID;
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
