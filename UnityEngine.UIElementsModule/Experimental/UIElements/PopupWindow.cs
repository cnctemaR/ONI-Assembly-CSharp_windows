using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public class PopupWindow : TextElement
	{
		public PopupWindow()
		{
			this.m_ContentContainer = new VisualElement
			{
				name = "ContentContainer"
			};
			base.shadow.Add(this.m_ContentContainer);
		}

		public override VisualElement contentContainer
		{
			get
			{
				return this.m_ContentContainer;
			}
		}

		private VisualElement m_ContentContainer;

		public new class UxmlFactory : UxmlFactory<PopupWindow, PopupWindow.UxmlTraits>
		{
		}

		public new class UxmlTraits : TextElement.UxmlTraits
		{
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield return new UxmlChildElementDescription(typeof(VisualElement));
					yield break;
				}
			}
		}
	}
}
