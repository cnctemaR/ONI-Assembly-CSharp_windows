using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextLinkHandler : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left)
		{
			return;
		}
		if (!this.text.AllowLinks)
		{
			return;
		}
		int num = TMP_TextUtilities.FindIntersectingLink(this.text, KInputManager.GetMousePos(), null);
		if (num != -1)
		{
			string text = CodexCache.FormatLinkID(this.text.textInfo.linkInfo[num].GetLinkID());
			if (this.overrideLinkAction == null || this.overrideLinkAction(text))
			{
				if (!CodexCache.entries.ContainsKey(text))
				{
					SubEntry subEntry = CodexCache.FindSubEntry(text);
					if (subEntry == null || subEntry.disabled)
					{
						text = "PAGENOTFOUND";
					}
				}
				else if (CodexCache.entries[text].disabled)
				{
					text = "PAGENOTFOUND";
				}
				if (!ManagementMenu.Instance.codexScreen.gameObject.activeInHierarchy)
				{
					ManagementMenu.Instance.ToggleCodex();
				}
				ManagementMenu.Instance.codexScreen.ChangeArticle(text, true, default(Vector3), CodexScreen.HistoryDirection.NewArticle);
			}
		}
	}

	private void Update()
	{
		this.CheckMouseOver();
		if (TextLinkHandler.hoveredText == this && this.text.AllowLinks)
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
		if (this.text.AllowLinks && !this.text.raycastTarget)
		{
			this.text.raycastTarget = true;
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
		if (this == null || this.Equals(null))
		{
			return;
		}
		if (TextLinkHandler.hoveredText == this)
		{
			if (this.hoverLink && PlayerController.Instance != null && PlayerController.Instance.ActiveTool != null)
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
		if (TMP_TextUtilities.FindIntersectingLink(this.text, KInputManager.GetMousePos(), null) != -1)
		{
			this.SetMouseOver();
			this.hoverLink = true;
			return;
		}
		if (TextLinkHandler.hoveredText == this)
		{
			this.hoverLink = false;
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

	public Func<string, bool> overrideLinkAction;
}
