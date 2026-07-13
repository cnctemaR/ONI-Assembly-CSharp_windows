using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
	internal class UIDocumentRootElement : TemplateContainer
	{
		internal UIRenderer uiRenderer { get; set; }

		public UIDocumentRootElement(UIDocument document, VisualTreeAsset sourceAsset)
			: base((sourceAsset != null) ? sourceAsset.name : null, sourceAsset)
		{
			this.document = document;
		}

		public readonly UIDocument document;
	}
}
