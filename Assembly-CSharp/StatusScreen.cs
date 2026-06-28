using System;
using UnityEngine;
using UnityEngine.UI;

public class StatusScreen : KScreen
{
	protected override void OnPrefabInit()
	{
		this.portrait = Util.KInstantiate(this.portraitPrefab, SceneOrganizer.Instance.GetFolder(Folder.Cameras), null).GetComponent<Portrait>();
		this.portrait.maintainHighlight = true;
		RawImage rawImage = this.icon as RawImage;
		rawImage.texture = this.portrait.CreateTexture((int)this.icon.rectTransform.rect.width, (int)this.icon.rectTransform.rect.height);
		this.ClearSelection();
	}

	private void Update()
	{
		if (this.selected == null)
		{
			this.ClearSelection();
		}
	}

	public void Select(GameObject go)
	{
		this.ToggleWidget(true);
		this.selected = go;
		KSelectable component = this.selected.GetComponent<KSelectable>();
		this.portrait.SetTarget(go);
		this.nameLabel.text = component.GetName();
	}

	private void ClearSelection()
	{
		if (this.selected != null)
		{
			this.portrait.SetTarget(null);
			this.selected = null;
		}
		this.ToggleWidget(false);
	}

	protected override void OnDeactivate()
	{
		if (this == null)
		{
			return;
		}
		if (this.portrait != null && this.portrait.gameObject != null)
		{
			global::UnityEngine.Object.Destroy(this.portrait.gameObject);
		}
	}

	private void ToggleWidget(bool on)
	{
		this.nameLabel.gameObject.SetActive(on);
		this.icon.gameObject.SetActive(on);
	}

	private GameObject selected;

	public Text nameLabel;

	public MaskableGraphic icon;

	public Portrait portraitPrefab;

	private Portrait portrait;
}
