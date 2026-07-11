using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public class TemplateContainer : BindableElement
	{
		public TemplateContainer()
			: this(null)
		{
		}

		public TemplateContainer(string templateId)
		{
			this.templateId = templateId;
			this.m_ContentContainer = this;
		}

		public string templateId { get; private set; }

		public override VisualElement contentContainer
		{
			get
			{
				return this.m_ContentContainer;
			}
		}

		internal void SetContentContainer(VisualElement content)
		{
			this.m_ContentContainer = content;
		}

		private VisualElement m_ContentContainer;

		public new class UxmlFactory : UxmlFactory<TemplateContainer, TemplateContainer.UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				TemplateContainer templateContainer = (TemplateContainer)ve;
				templateContainer.templateId = this.m_Template.GetValueFromBag(bag, cc);
				VisualTreeAsset visualTreeAsset = cc.visualTreeAsset.ResolveTemplate(templateContainer.templateId);
				if (visualTreeAsset == null)
				{
					templateContainer.Add(new Label(string.Format("Unknown Element: '{0}'", templateContainer.templateId)));
				}
				else
				{
					visualTreeAsset.CloneTree(templateContainer, cc.slotInsertionPoints);
				}
				if (visualTreeAsset == null)
				{
					Debug.LogErrorFormat("Could not resolve template with name '{0}'", new object[] { templateContainer.templateId });
				}
			}

			private UxmlStringAttributeDescription m_Template = new UxmlStringAttributeDescription
			{
				name = "template",
				use = UxmlAttributeDescription.Use.Required
			};
		}
	}
}
