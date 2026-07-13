using System;

namespace UnityEngine.UIElements
{
	internal class TextJobSystem
	{
		internal void GenerateText(MeshGenerationContext mgc, TextElement textElement)
		{
			bool flag = TextUtilities.IsAdvancedTextEnabledForElement(textElement);
			if (flag)
			{
				if (this.m_ATGTextJobSystem == null)
				{
					this.m_ATGTextJobSystem = new ATGTextJobSystem();
				}
				this.m_ATGTextJobSystem.GenerateText(mgc, textElement);
			}
			else
			{
				if (this.m_UITKTextJobSystem == null)
				{
					this.m_UITKTextJobSystem = new UITKTextJobSystem();
				}
				this.m_UITKTextJobSystem.GenerateText(mgc, textElement);
			}
		}

		internal void PrepareShapingBeforeLayout(BaseVisualElementPanel panel)
		{
			if (this.m_ATGTextJobSystem == null)
			{
				this.m_ATGTextJobSystem = new ATGTextJobSystem();
			}
			this.m_ATGTextJobSystem.PrepareShapingBeforeLayout(panel);
		}

		internal UITKTextJobSystem m_UITKTextJobSystem;

		private ATGTextJobSystem m_ATGTextJobSystem;
	}
}
