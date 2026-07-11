using System;
using UnityEngine;
using UnityEngine.UI;

public class ShadowRect : MonoBehaviour
{
	private void OnEnable()
	{
		if (this.RectShadow != null)
		{
			this.RectShadow.name = "Shadow_" + this.RectMain.name;
			this.MatchRect();
			return;
		}
		global::Debug.LogWarning("Shadowrect is missing rectshadow: " + base.gameObject.name);
	}

	private void Update()
	{
		this.MatchRect();
	}

	protected virtual void MatchRect()
	{
		if (this.RectShadow == null || this.RectMain == null)
		{
			return;
		}
		if (this.shadowLayoutElement == null)
		{
			this.shadowLayoutElement = this.RectShadow.GetComponent<LayoutElement>();
		}
		if (this.shadowLayoutElement != null && !this.shadowLayoutElement.ignoreLayout)
		{
			this.shadowLayoutElement.ignoreLayout = true;
		}
		if (this.RectShadow.transform.parent != this.RectMain.transform.parent)
		{
			this.RectShadow.transform.SetParent(this.RectMain.transform.parent);
		}
		if (this.RectShadow.GetSiblingIndex() >= this.RectMain.GetSiblingIndex())
		{
			this.RectShadow.SetAsFirstSibling();
		}
		this.RectShadow.transform.localScale = Vector3.one;
		if (this.RectShadow.pivot != this.RectMain.pivot)
		{
			this.RectShadow.pivot = this.RectMain.pivot;
		}
		if (this.RectShadow.anchorMax != this.RectMain.anchorMax)
		{
			this.RectShadow.anchorMax = this.RectMain.anchorMax;
		}
		if (this.RectShadow.anchorMin != this.RectMain.anchorMin)
		{
			this.RectShadow.anchorMin = this.RectMain.anchorMin;
		}
		if (this.RectShadow.sizeDelta != this.RectMain.sizeDelta)
		{
			this.RectShadow.sizeDelta = this.RectMain.sizeDelta;
		}
		if (this.RectShadow.anchoredPosition != this.RectMain.anchoredPosition + this.ShadowOffset)
		{
			this.RectShadow.anchoredPosition = this.RectMain.anchoredPosition + this.ShadowOffset;
		}
		if (this.RectMain.gameObject.activeInHierarchy != this.RectShadow.gameObject.activeInHierarchy)
		{
			this.RectShadow.gameObject.SetActive(this.RectMain.gameObject.activeInHierarchy);
		}
	}

	public RectTransform RectMain;

	public RectTransform RectShadow;

	[SerializeField]
	protected Color shadowColor = new Color(0f, 0f, 0f, 0.6f);

	[SerializeField]
	protected Vector2 ShadowOffset = new Vector2(1.5f, -1.5f);

	private LayoutElement shadowLayoutElement;
}
