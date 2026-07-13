using System;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.Experimental;

namespace UnityEngine.UIElements
{
	internal class TextEventHandler
	{
		private TextInfo textInfo
		{
			get
			{
				return this.m_TextElement.uitkTextHandle.textInfo;
			}
		}

		public TextEventHandler(TextElement textElement)
		{
			this.m_TextElement = textElement;
		}

		public void OnDestroy()
		{
			bool flag = this.HasAllocatedLinkCallbacks();
			if (flag)
			{
				this.m_TextElement.UnregisterCallback<PointerDownEvent>(this.m_LinkTagOnPointerDown, TrickleDown.TrickleDown);
				this.m_TextElement.UnregisterCallback<PointerUpEvent>(this.m_LinkTagOnPointerUp, TrickleDown.TrickleDown);
				this.m_TextElement.UnregisterCallback<PointerMoveEvent>(this.m_LinkTagOnPointerMove, TrickleDown.TrickleDown);
				this.m_TextElement.UnregisterCallback<PointerOutEvent>(this.m_LinkTagOnPointerOut, TrickleDown.TrickleDown);
			}
			bool flag2 = this.HasAllocatedATagCallbacks();
			if (flag2)
			{
				this.m_TextElement.UnregisterCallback<PointerOverEvent>(this.m_ATagOnPointerOver, TrickleDown.TrickleDown);
				this.m_TextElement.UnregisterCallback<PointerMoveEvent>(this.m_ATagOnPointerMove, TrickleDown.TrickleDown);
				this.m_TextElement.UnregisterCallback<PointerUpEvent>(this.m_ATagOnPointerUp, TrickleDown.TrickleDown);
				this.m_TextElement.UnregisterCallback<PointerOutEvent>(this.m_ATagOnPointerOut, TrickleDown.TrickleDown);
			}
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

		private bool HasAllocatedATagCallbacks()
		{
			return this.m_ATagOnPointerUp != null;
		}

		private void AllocateATagCallbacks()
		{
			bool flag = this.HasAllocatedATagCallbacks();
			if (!flag)
			{
				this.m_ATagOnPointerUp = new EventCallback<PointerUpEvent>(this.ATagOnPointerUp);
				this.m_ATagOnPointerMove = new EventCallback<PointerMoveEvent>(this.ATagOnPointerMove);
				this.m_ATagOnPointerOver = new EventCallback<PointerOverEvent>(this.ATagOnPointerOver);
				this.m_ATagOnPointerOut = new EventCallback<PointerOutEvent>(this.ATagOnPointerOut);
			}
		}

		private void ATagOnPointerUp(PointerUpEvent pue)
		{
			Vector3 vector = pue.localPosition - new Vector3(this.m_TextElement.contentRect.min.x, this.m_TextElement.contentRect.min.y);
			int num = this.m_TextElement.uitkTextHandle.FindIntersectingLink(vector, true);
			bool flag = num < 0;
			if (!flag)
			{
				LinkInfo linkInfo = this.textInfo.linkInfo[num];
				bool flag2 = linkInfo.hashCode != 2535353;
				if (!flag2)
				{
					bool flag3 = linkInfo.linkId == null || linkInfo.linkIdLength <= 0;
					if (!flag3)
					{
						string linkId = linkInfo.GetLinkId();
						bool flag4 = Uri.IsWellFormedUriString(linkId, UriKind.Absolute);
						if (flag4)
						{
							Application.OpenURL(linkId);
						}
					}
				}
			}
		}

		private void ATagOnPointerOver(PointerOverEvent _)
		{
			this.isOverridingCursor = false;
		}

		private void ATagOnPointerMove(PointerMoveEvent pme)
		{
			Vector3 vector = pme.localPosition - new Vector3(this.m_TextElement.contentRect.min.x, this.m_TextElement.contentRect.min.y);
			int num = this.m_TextElement.uitkTextHandle.FindIntersectingLink(vector, true);
			BaseVisualElementPanel baseVisualElementPanel = this.m_TextElement.panel as BaseVisualElementPanel;
			ICursorManager cursorManager = ((baseVisualElementPanel != null) ? baseVisualElementPanel.cursorManager : null);
			bool flag = num >= 0;
			if (flag)
			{
				LinkInfo linkInfo = this.textInfo.linkInfo[num];
				bool flag2 = linkInfo.hashCode == 2535353;
				if (flag2)
				{
					bool flag3 = !this.isOverridingCursor;
					if (flag3)
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
					return;
				}
			}
			bool flag4 = this.isOverridingCursor;
			if (flag4)
			{
				if (cursorManager != null)
				{
					cursorManager.SetCursor(this.m_TextElement.computedStyle.cursor);
				}
				this.isOverridingCursor = false;
			}
		}

		private void ATagOnPointerOut(PointerOutEvent evt)
		{
			this.isOverridingCursor = false;
		}

		private void LinkTagOnPointerDown(PointerDownEvent pde)
		{
			Vector3 vector = pde.localPosition - new Vector3(this.m_TextElement.contentRect.min.x, this.m_TextElement.contentRect.min.y);
			int num = this.m_TextElement.uitkTextHandle.FindIntersectingLink(vector, true);
			bool flag = num < 0;
			if (!flag)
			{
				LinkInfo linkInfo = this.textInfo.linkInfo[num];
				bool flag2 = linkInfo.hashCode == 2535353;
				if (!flag2)
				{
					bool flag3 = linkInfo.linkId == null || linkInfo.linkIdLength <= 0;
					if (!flag3)
					{
						using (PointerDownLinkTagEvent pooled = PointerDownLinkTagEvent.GetPooled(pde, linkInfo.GetLinkId(), linkInfo.GetLinkText(this.textInfo)))
						{
							pooled.elementTarget = this.m_TextElement;
							this.m_TextElement.SendEvent(pooled);
						}
					}
				}
			}
		}

		private void LinkTagOnPointerUp(PointerUpEvent pue)
		{
			Vector3 vector = pue.localPosition - new Vector3(this.m_TextElement.contentRect.min.x, this.m_TextElement.contentRect.min.y);
			int num = this.m_TextElement.uitkTextHandle.FindIntersectingLink(vector, true);
			bool flag = num < 0;
			if (!flag)
			{
				LinkInfo linkInfo = this.textInfo.linkInfo[num];
				bool flag2 = linkInfo.hashCode == 2535353;
				if (!flag2)
				{
					bool flag3 = linkInfo.linkId == null || linkInfo.linkIdLength <= 0;
					if (!flag3)
					{
						using (PointerUpLinkTagEvent pooled = PointerUpLinkTagEvent.GetPooled(pue, linkInfo.GetLinkId(), linkInfo.GetLinkText(this.textInfo)))
						{
							pooled.elementTarget = this.m_TextElement;
							this.m_TextElement.SendEvent(pooled);
						}
					}
				}
			}
		}

		private void LinkTagOnPointerMove(PointerMoveEvent pme)
		{
			Vector3 vector = pme.localPosition - new Vector3(this.m_TextElement.contentRect.min.x, this.m_TextElement.contentRect.min.y);
			int num = this.m_TextElement.uitkTextHandle.FindIntersectingLink(vector, true);
			bool flag = num >= 0;
			if (flag)
			{
				LinkInfo linkInfo = this.textInfo.linkInfo[num];
				bool flag2 = linkInfo.hashCode != 2535353;
				if (flag2)
				{
					bool flag3 = this.currentLinkIDHash == -1;
					if (flag3)
					{
						this.currentLinkIDHash = linkInfo.hashCode;
						using (PointerOverLinkTagEvent pooled = PointerOverLinkTagEvent.GetPooled(pme, linkInfo.GetLinkId(), linkInfo.GetLinkText(this.textInfo)))
						{
							pooled.elementTarget = this.m_TextElement;
							this.m_TextElement.SendEvent(pooled);
						}
						return;
					}
					bool flag4 = this.currentLinkIDHash == linkInfo.hashCode;
					if (flag4)
					{
						using (PointerMoveLinkTagEvent pooled2 = PointerMoveLinkTagEvent.GetPooled(pme, linkInfo.GetLinkId(), linkInfo.GetLinkText(this.textInfo)))
						{
							pooled2.elementTarget = this.m_TextElement;
							this.m_TextElement.SendEvent(pooled2);
						}
						return;
					}
				}
			}
			bool flag5 = this.currentLinkIDHash != -1;
			if (flag5)
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

		internal void HandleLinkAndATagCallbacks()
		{
			TextElement textElement = this.m_TextElement;
			bool flag = ((textElement != null) ? textElement.panel : null) == null;
			if (!flag)
			{
				bool flag2 = this.hasLinkTag;
				if (flag2)
				{
					this.AllocateLinkCallbacks();
					this.m_TextElement.RegisterCallback<PointerDownEvent>(this.m_LinkTagOnPointerDown, TrickleDown.TrickleDown);
					this.m_TextElement.RegisterCallback<PointerUpEvent>(this.m_LinkTagOnPointerUp, TrickleDown.TrickleDown);
					this.m_TextElement.RegisterCallback<PointerMoveEvent>(this.m_LinkTagOnPointerMove, TrickleDown.TrickleDown);
					this.m_TextElement.RegisterCallback<PointerOutEvent>(this.m_LinkTagOnPointerOut, TrickleDown.TrickleDown);
				}
				else
				{
					bool flag3 = this.HasAllocatedLinkCallbacks();
					if (flag3)
					{
						this.m_TextElement.UnregisterCallback<PointerDownEvent>(this.m_LinkTagOnPointerDown, TrickleDown.TrickleDown);
						this.m_TextElement.UnregisterCallback<PointerUpEvent>(this.m_LinkTagOnPointerUp, TrickleDown.TrickleDown);
						this.m_TextElement.UnregisterCallback<PointerMoveEvent>(this.m_LinkTagOnPointerMove, TrickleDown.TrickleDown);
						this.m_TextElement.UnregisterCallback<PointerOutEvent>(this.m_LinkTagOnPointerOut, TrickleDown.TrickleDown);
					}
				}
				bool flag4 = this.hasATag;
				if (flag4)
				{
					this.AllocateATagCallbacks();
					this.m_TextElement.RegisterCallback<PointerUpEvent>(this.m_ATagOnPointerUp, TrickleDown.TrickleDown);
					bool flag5 = this.m_TextElement.panel.contextType == ContextType.Editor;
					if (flag5)
					{
						this.m_TextElement.RegisterCallback<PointerMoveEvent>(this.m_ATagOnPointerMove, TrickleDown.TrickleDown);
						this.m_TextElement.RegisterCallback<PointerOverEvent>(this.m_ATagOnPointerOver, TrickleDown.TrickleDown);
						this.m_TextElement.RegisterCallback<PointerOutEvent>(this.m_ATagOnPointerOut, TrickleDown.TrickleDown);
					}
				}
				else
				{
					bool flag6 = this.HasAllocatedATagCallbacks();
					if (flag6)
					{
						this.m_TextElement.UnregisterCallback<PointerUpEvent>(this.m_ATagOnPointerUp, TrickleDown.TrickleDown);
						bool flag7 = this.m_TextElement.panel.contextType == ContextType.Editor;
						if (flag7)
						{
							this.m_TextElement.UnregisterCallback<PointerMoveEvent>(this.m_ATagOnPointerMove, TrickleDown.TrickleDown);
							this.m_TextElement.UnregisterCallback<PointerOverEvent>(this.m_ATagOnPointerOver, TrickleDown.TrickleDown);
							this.m_TextElement.UnregisterCallback<PointerOutEvent>(this.m_ATagOnPointerOut, TrickleDown.TrickleDown);
						}
					}
				}
			}
		}

		internal void HandleLinkTag()
		{
			for (int i = 0; i < this.textInfo.linkCount; i++)
			{
				LinkInfo linkInfo = this.textInfo.linkInfo[i];
				bool flag = linkInfo.hashCode != 2535353;
				if (flag)
				{
					this.hasLinkTag = true;
					this.m_TextElement.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
					return;
				}
			}
			bool flag2 = this.hasLinkTag;
			if (flag2)
			{
				this.hasLinkTag = false;
				this.m_TextElement.uitkTextHandle.RemoveFromPermanentCache();
				return;
			}
		}

		internal void HandleATag()
		{
			for (int i = 0; i < this.textInfo.linkCount; i++)
			{
				LinkInfo linkInfo = this.textInfo.linkInfo[i];
				bool flag = linkInfo.hashCode == 2535353;
				if (flag)
				{
					this.hasATag = true;
					this.m_TextElement.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
					return;
				}
			}
			bool flag2 = this.hasATag;
			if (flag2)
			{
				this.hasATag = false;
				this.m_TextElement.uitkTextHandle.RemoveFromPermanentCache();
				return;
			}
		}

		private TextElement m_TextElement;

		private EventCallback<PointerDownEvent> m_LinkTagOnPointerDown;

		private EventCallback<PointerUpEvent> m_LinkTagOnPointerUp;

		private EventCallback<PointerMoveEvent> m_LinkTagOnPointerMove;

		private EventCallback<PointerOutEvent> m_LinkTagOnPointerOut;

		private EventCallback<PointerUpEvent> m_ATagOnPointerUp;

		private EventCallback<PointerMoveEvent> m_ATagOnPointerMove;

		private EventCallback<PointerOverEvent> m_ATagOnPointerOver;

		private EventCallback<PointerOutEvent> m_ATagOnPointerOut;

		internal bool isOverridingCursor;

		internal int currentLinkIDHash = -1;

		internal bool hasLinkTag;

		internal bool hasATag;
	}
}
