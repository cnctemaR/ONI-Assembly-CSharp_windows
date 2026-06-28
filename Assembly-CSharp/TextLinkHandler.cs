using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextLinkHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		if (!this.text.AllowLinks)
		{
			return;
		}
		int num = TMP_TextUtilities.FindIntersectingLink(this.text, Input.mousePosition, null);
		if (num != -1)
		{
			string text = CodexCache.FormatLinkID(this.text.textInfo.linkInfo[num].GetLinkID());
			if (!CodexCache.entries.ContainsKey(text) || CodexCache.entries[text].disabled)
			{
				text = "PAGENOTFOUND";
			}
			if (!ManagementMenu.Instance.codexScreen.gameObject.activeInHierarchy)
			{
				ManagementMenu.Instance.ToggleCodex();
			}
			(ManagementMenu.Instance.codexScreen as CodexScreen).ChangeArticle(text, true);
		}
	}

	private void Update()
	{
		this.CheckMouseOver();
		if (TextLinkHandler.hoveredText == this)
		{
			PlayerController.Instance.ActiveTool.SetLinkCursor(this.hoverLink);
		}
	}

	private void OnEnable()
	{
		this.CheckMouseOver();
	}

	private void OnDisable()
	{
		this.ClearState();
	}

	private void Awake()
	{
		this.text = base.GetComponent<LocText>();
		if (!this.text.AllowLinks)
		{
			this.text.raycastTarget = false;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		this.SetMouseOver();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		this.ClearState();
	}

	private void ClearState()
	{
		if (TextLinkHandler.hoveredText == this)
		{
			if (this.hoverLink)
			{
				PlayerController.Instance.ActiveTool.SetLinkCursor(false);
			}
			TextLinkHandler.hoveredText = null;
			this.hoverLink = false;
		}
	}

	public void CheckMouseOver()
	{
		if (this.text == null)
		{
			return;
		}
		if (TMP_TextUtilities.FindIntersectingLink(this.text, Input.mousePosition, null) != -1)
		{
			this.SetMouseOver();
			this.hoverLink = true;
		}
	}

	private void SetMouseOver()
	{
		if (TextLinkHandler.hoveredText != null && TextLinkHandler.hoveredText != this)
		{
			TextLinkHandler.hoveredText.hoverLink = false;
		}
		TextLinkHandler.hoveredText = this;
	}

	private static TextLinkHandler hoveredText;

	[MyCmpGet]
	private LocText text;

	private bool hoverLink;
}
