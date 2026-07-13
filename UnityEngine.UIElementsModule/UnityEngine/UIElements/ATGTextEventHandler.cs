using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine.TextCore;
using UnityEngine.UIElements.Experimental;

namespace UnityEngine.UIElements
{
	internal class ATGTextEventHandler
	{
		public ATGTextEventHandler(TextElement textElement)
		{
			Debug.Assert(textElement.uitkTextHandle.useAdvancedText);
			this.m_TextElement = textElement;
		}

		public void OnDestroy()
		{
			this.UnRegisterLinkTagCallbacks();
			this.UnRegisterHyperlinkCallbacks();
		}

		private bool HasAllocatedLinkCallbacks()
		{
			return this.m_LinkTagOnPointerDown != null;
		}

		private void AllocateLinkCallbacks()
		{
			bool flag = this.HasAllocatedLinkCallbacks();
			if (!flag)
			{
				this.m_LinkTagOnPointerDown = new EventCallback<PointerDownEvent>(this.LinkTagOnPointerDown);
				this.m_LinkTagOnPointerUp = new EventCallback<PointerUpEvent>(this.LinkTagOnPointerUp);
				this.m_LinkTagOnPointerMove = new EventCallback<PointerMoveEvent>(this.LinkTagOnPointerMove);
				this.m_LinkTagOnPointerOut = new EventCallback<PointerOutEvent>(this.LinkTagOnPointerOut);
			}
		}

		private bool HasAllocatedHyperlinkCallbacks()
		{
			return this.m_HyperlinkOnPointerUp != null;
		}

		private void AllocateHyperlinkCallbacks()
		{
			bool flag = this.HasAllocatedHyperlinkCallbacks();
			if (!flag)
			{
				this.m_HyperlinkOnPointerUp = new EventCallback<PointerUpEvent>(this.HyperlinkOnPointerUp);
				this.m_HyperlinkOnPointerMove = new EventCallback<PointerMoveEvent>(this.HyperlinkOnPointerMove);
				this.m_HyperlinkOnPointerOver = new EventCallback<PointerOverEvent>(this.HyperlinkOnPointerOver);
				this.m_HyperlinkOnPointerOut = new EventCallback<PointerOutEvent>(this.HyperlinkOnPointerOut);
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal static event Action<Dictionary<string, string>> onComplexHyperlinkClicked;

		private void HyperlinkOnPointerUp(PointerUpEvent pue)
		{
			Vector3 vector = pue.localPosition - new Vector3(this.m_TextElement.contentRect.min.x, this.m_TextElement.contentRect.min.y);
			ValueTuple<RichTextTagParser.TagType, string> valueTuple = this.m_TextElement.uitkTextHandle.ATGFindIntersectingLink(vector);
			RichTextTagParser.TagType item = valueTuple.Item1;
			string item2 = valueTuple.Item2;
			bool flag = item2 == null || item > RichTextTagParser.TagType.Hyperlink;
			if (!flag)
			{
				bool flag2 = Uri.IsWellFormedUriString(item2, UriKind.Absolute);
				if (flag2)
				{
					Application.OpenURL(item2);
				}
				else
				{
					Dictionary<string, string> dictionary;
					bool flag3 = ATGTextEventHandler.IsComplexHyperLink(item2, out dictionary);
					if (flag3)
					{
						Action<Dictionary<string, string>> action = ATGTextEventHandler.onComplexHyperlinkClicked;
						if (action != null)
						{
							action(dictionary);
						}
					}
				}
			}
		}

		private static bool IsComplexHyperLink(string link, out Dictionary<string, string> hyperLinkData)
		{
			hyperLinkData = new Dictionary<string, string>();
			MatchCollection matchCollection = ATGTextEventHandler.s_ATagRegex.Matches(link);
			bool flag = matchCollection.Count == 0;
			if (flag)
			{
				matchCollection = ATGTextEventHandler.s_LinkTagRegex.Matches(link);
			}
			int num = 0;
			foreach (object obj in matchCollection)
			{
				Match match = (Match)obj;
				string text = link.Substring(num, match.Index - 2 - num);
				int num2 = text.LastIndexOf(' ') + 1;
				string text2 = text.Substring(num2);
				hyperLinkData.Add(text2, match.Value);
				num = match.Index + match.Value.Length + 1;
			}
			return true;
		}

		private void HyperlinkOnPointerOver(PointerOverEvent _)
		{
			this.isOverridingCursor = false;
		}

		private void HyperlinkOnPointerMove(PointerMoveEvent pme)
		{
			Vector3 vector = pme.localPosition - new Vector3(this.m_TextElement.contentRect.min.x, this.m_TextElement.contentRect.min.y);
			ValueTuple<RichTextTagParser.TagType, string> valueTuple = this.m_TextElement.uitkTextHandle.ATGFindIntersectingLink(vector);
			RichTextTagParser.TagType item = valueTuple.Item1;
			string item2 = valueTuple.Item2;
			BaseVisualElementPanel baseVisualElementPanel = this.m_TextElement.panel as BaseVisualElementPanel;
			ICursorManager cursorManager = ((baseVisualElementPanel != null) ? baseVisualElementPanel.cursorManager : null);
			bool flag = item2 != null && item == RichTextTagParser.TagType.Hyperlink;
			if (flag)
			{
				bool flag2 = !this.isOverridingCursor;
				if (flag2)
				{
					this.isOverridingCursor = true;
					if (cursorManager != null)
					{
						cursorManager.SetCursor(new Cursor
						{
							defaultCursorId = 4
						});
					}
				}
			}
			else
			{
				bool flag3 = this.isOverridingCursor;
				if (flag3)
				{
					if (cursorManager != null)
					{
						cursorManager.SetCursor(this.m_TextElement.computedStyle.cursor);
					}
					this.isOverridingCursor = false;
				}
			}
		}

		private void HyperlinkOnPointerOut(PointerOutEvent evt)
		{
			this.isOverridingCursor = false;
		}

		private void LinkTagOnPointerDown(PointerDownEvent pde)
		{
			Vector3 vector = pde.localPosition - new Vector3(this.m_TextElement.contentRect.min.x, this.m_TextElement.contentRect.min.y);
			ValueTuple<RichTextTagParser.TagType, string> valueTuple = this.m_TextElement.uitkTextHandle.ATGFindIntersectingLink(vector);
			RichTextTagParser.TagType item = valueTuple.Item1;
			string item2 = valueTuple.Item2;
			bool flag = item2 == null || item != RichTextTagParser.TagType.Link;
			if (!flag)
			{
				using (PointerDownLinkTagEvent pooled = PointerDownLinkTagEvent.GetPooled(pde, item2, "test"))
				{
					pooled.elementTarget = this.m_TextElement;
					this.m_TextElement.SendEvent(pooled);
				}
			}
		}

		private void LinkTagOnPointerUp(PointerUpEvent pue)
		{
			Vector3 vector = pue.localPosition - new Vector3(this.m_TextElement.contentRect.min.x, this.m_TextElement.contentRect.min.y);
			ValueTuple<RichTextTagParser.TagType, string> valueTuple = this.m_TextElement.uitkTextHandle.ATGFindIntersectingLink(vector);
			RichTextTagParser.TagType item = valueTuple.Item1;
			string item2 = valueTuple.Item2;
			bool flag = item2 == null || item != RichTextTagParser.TagType.Link;
			if (!flag)
			{
				using (PointerUpLinkTagEvent pooled = PointerUpLinkTagEvent.GetPooled(pue, item2, "test"))
				{
					pooled.elementTarget = this.m_TextElement;
					this.m_TextElement.SendEvent(pooled);
				}
			}
		}

		private void LinkTagOnPointerMove(PointerMoveEvent pme)
		{
			Vector3 vector = pme.localPosition - new Vector3(this.m_TextElement.contentRect.min.x, this.m_TextElement.contentRect.min.y);
			ValueTuple<RichTextTagParser.TagType, string> valueTuple = this.m_TextElement.uitkTextHandle.ATGFindIntersectingLink(vector);
			RichTextTagParser.TagType item = valueTuple.Item1;
			string item2 = valueTuple.Item2;
			bool flag = item2 != null && item == RichTextTagParser.TagType.Link;
			if (flag)
			{
				bool flag2 = this.currentLinkIDHash == -1;
				if (flag2)
				{
					this.currentLinkIDHash = 0;
					using (PointerOverLinkTagEvent pooled = PointerOverLinkTagEvent.GetPooled(pme, item2, "test"))
					{
						pooled.elementTarget = this.m_TextElement;
						this.m_TextElement.SendEvent(pooled);
					}
					return;
				}
				bool flag3 = this.currentLinkIDHash == 0;
				if (flag3)
				{
					using (PointerMoveLinkTagEvent pooled2 = PointerMoveLinkTagEvent.GetPooled(pme, item2, "test"))
					{
						pooled2.elementTarget = this.m_TextElement;
						this.m_TextElement.SendEvent(pooled2);
					}
					return;
				}
			}
			bool flag4 = this.currentLinkIDHash != -1;
			if (flag4)
			{
				this.currentLinkIDHash = -1;
				using (PointerOutLinkTagEvent pooled3 = PointerOutLinkTagEvent.GetPooled(pme, string.Empty))
				{
					pooled3.elementTarget = this.m_TextElement;
					this.m_TextElement.SendEvent(pooled3);
				}
			}
		}

		private void LinkTagOnPointerOut(PointerOutEvent poe)
		{
			bool flag = this.currentLinkIDHash != -1;
			if (flag)
			{
				using (PointerOutLinkTagEvent pooled = PointerOutLinkTagEvent.GetPooled(poe, string.Empty))
				{
					pooled.elementTarget = this.m_TextElement;
					this.m_TextElement.SendEvent(pooled);
				}
				this.currentLinkIDHash = -1;
			}
		}

		internal void RegisterLinkTagCallbacks()
		{
			TextElement textElement = this.m_TextElement;
			bool flag = ((textElement != null) ? textElement.panel : null) == null;
			if (!flag)
			{
				this.AllocateLinkCallbacks();
				this.m_TextElement.RegisterCallback<PointerDownEvent>(this.m_LinkTagOnPointerDown, TrickleDown.TrickleDown);
				this.m_TextElement.RegisterCallback<PointerUpEvent>(this.m_LinkTagOnPointerUp, TrickleDown.TrickleDown);
				this.m_TextElement.RegisterCallback<PointerMoveEvent>(this.m_LinkTagOnPointerMove, TrickleDown.TrickleDown);
				this.m_TextElement.RegisterCallback<PointerOutEvent>(this.m_LinkTagOnPointerOut, TrickleDown.TrickleDown);
			}
		}

		internal void UnRegisterLinkTagCallbacks()
		{
			bool flag = this.HasAllocatedLinkCallbacks();
			if (flag)
			{
				this.m_TextElement.UnregisterCallback<PointerDownEvent>(this.m_LinkTagOnPointerDown, TrickleDown.TrickleDown);
				this.m_TextElement.UnregisterCallback<PointerUpEvent>(this.m_LinkTagOnPointerUp, TrickleDown.TrickleDown);
				this.m_TextElement.UnregisterCallback<PointerMoveEvent>(this.m_LinkTagOnPointerMove, TrickleDown.TrickleDown);
				this.m_TextElement.UnregisterCallback<PointerOutEvent>(this.m_LinkTagOnPointerOut, TrickleDown.TrickleDown);
			}
		}

		internal void RegisterHyperlinkCallbacks()
		{
			TextElement textElement = this.m_TextElement;
			bool flag = ((textElement != null) ? textElement.panel : null) == null;
			if (!flag)
			{
				this.AllocateHyperlinkCallbacks();
				this.m_TextElement.RegisterCallback<PointerUpEvent>(this.m_HyperlinkOnPointerUp, TrickleDown.TrickleDown);
				bool flag2 = this.m_TextElement.panel.contextType == ContextType.Editor;
				if (flag2)
				{
					this.m_TextElement.RegisterCallback<PointerMoveEvent>(this.m_HyperlinkOnPointerMove, TrickleDown.TrickleDown);
					this.m_TextElement.RegisterCallback<PointerOverEvent>(this.m_HyperlinkOnPointerOver, TrickleDown.TrickleDown);
					this.m_TextElement.RegisterCallback<PointerOutEvent>(this.m_HyperlinkOnPointerOut, TrickleDown.TrickleDown);
				}
			}
		}

		internal void UnRegisterHyperlinkCallbacks()
		{
			TextElement textElement = this.m_TextElement;
			bool flag = ((textElement != null) ? textElement.panel : null) == null;
			if (!flag)
			{
				bool flag2 = this.HasAllocatedHyperlinkCallbacks();
				if (flag2)
				{
					this.m_TextElement.UnregisterCallback<PointerUpEvent>(this.m_HyperlinkOnPointerUp, TrickleDown.TrickleDown);
					bool flag3 = this.m_TextElement.panel.contextType == ContextType.Editor;
					if (flag3)
					{
						this.m_TextElement.UnregisterCallback<PointerMoveEvent>(this.m_HyperlinkOnPointerMove, TrickleDown.TrickleDown);
						this.m_TextElement.UnregisterCallback<PointerOverEvent>(this.m_HyperlinkOnPointerOver, TrickleDown.TrickleDown);
						this.m_TextElement.UnregisterCallback<PointerOutEvent>(this.m_HyperlinkOnPointerOut, TrickleDown.TrickleDown);
					}
				}
			}
		}

		private static readonly Regex s_ATagRegex = new Regex("(?<=\\b=\")[^\"]*");

		private static readonly Regex s_LinkTagRegex = new Regex("(?<=\\b=')[^']*");

		private TextElement m_TextElement;

		private EventCallback<PointerDownEvent> m_LinkTagOnPointerDown;

		private EventCallback<PointerUpEvent> m_LinkTagOnPointerUp;

		private EventCallback<PointerMoveEvent> m_LinkTagOnPointerMove;

		private EventCallback<PointerOutEvent> m_LinkTagOnPointerOut;

		private EventCallback<PointerUpEvent> m_HyperlinkOnPointerUp;

		private EventCallback<PointerMoveEvent> m_HyperlinkOnPointerMove;

		private EventCallback<PointerOverEvent> m_HyperlinkOnPointerOver;

		private EventCallback<PointerOutEvent> m_HyperlinkOnPointerOut;

		internal bool isOverridingCursor;

		internal int currentLinkIDHash = -1;
	}
}
