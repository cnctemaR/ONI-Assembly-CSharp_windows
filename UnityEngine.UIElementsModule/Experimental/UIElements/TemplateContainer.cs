using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public class TemplateContainer : VisualElement
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

		/// <summary>
		///   <para>Instantiates and clones a TemplateContainer using the data read from a UXML file.</para>
		/// </summary>
		public class TemplateContainerFactory : UxmlFactory<TemplateContainer, TemplateContainer.TemplateContainerUxmlTraits>
		{
		}

		/// <summary>
		///   <para>UxmlTraits for the TemplateContainer.</para>
		/// </summary>
		public class TemplateContainerUxmlTraits : VisualElement.VisualElementUxmlTraits
		{
			/// <summary>
			///   <para>Constructor.</para>
			/// </summary>
			public TemplateContainerUxmlTraits()
			{
				this.m_Template = new UxmlStringAttributeDescription
				{
					name = "template",
					use = UxmlAttributeDescription.Use.Required
				};
			}

			/// <summary>
			///   <para>Returns an enumerable containing attribute descriptions for TemplateContainer properties that should be available in UXML.</para>
			/// </summary>
			public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
			{
				get
				{
					foreach (UxmlAttributeDescription attr in this.<get_uxmlAttributesDescription>__BaseCallProxy0())
					{
						yield return attr;
					}
					yield return this.m_Template;
					yield break;
				}
			}

			/// <summary>
			///   <para>Returns an empty enumerable, as template instance do not have children.</para>
			/// </summary>
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}

			/// <summary>
			///   <para>Initialize TemplateContainer properties using values from the attribute bag.</para>
			/// </summary>
			/// <param name="ve">The object to initialize.</param>
			/// <param name="bag">The attribute bag.</param>
			/// <param name="cc">The creation context; unused.</param>
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				TemplateContainer templateContainer = (TemplateContainer)ve;
				templateContainer.templateId = this.m_Template.GetValueFromBag(bag);
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

			private UxmlStringAttributeDescription m_Template;
		}
	}
}
