using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	[Serializable]
	internal class UxmlObjectAsset : UxmlAsset
	{
		public override bool HasParent()
		{
			return this.m_ParentId != 0;
		}

		public int parentId
		{
			get
			{
				return this.m_ParentId;
			}
			set
			{
				this.m_ParentId = value;
			}
		}

		public int orderInDocument
		{
			get
			{
				return this.m_OrderInDocument;
			}
			set
			{
				this.m_OrderInDocument = value;
			}
		}

		public bool isField
		{
			get
			{
				return this.m_IsField;
			}
		}

		public UxmlObjectAsset(string fullTypeNameOrFieldName, bool isField, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition))
			: base(fullTypeNameOrFieldName, xmlNamespace)
		{
			this.m_IsField = isField;
		}

		public override void GetExportTypename(out string typename, out UxmlNamespaceDefinition uxmlNamespaceDefinition)
		{
			bool isField = this.isField;
			if (isField)
			{
				typename = base.fullTypeName;
				uxmlNamespaceDefinition = UxmlNamespaceDefinition.Empty;
			}
			else
			{
				base.GetExportTypename(out typename, out uxmlNamespaceDefinition);
			}
		}

		internal override bool Accepts(UxmlAsset asset, out string errorMessage)
		{
			bool flag = asset is UxmlObjectAsset;
			errorMessage = ((!flag) ? string.Concat(new string[] { "[UI Toolkit] Cannot add a UXML asset of type '", asset.fullTypeName, "' to a UXML asset of type '", base.fullTypeName, "': UXML objects can only contain other UXML objects." }) : null);
			return flag;
		}

		public override string ToString()
		{
			return this.isField ? string.Format("Reference: {0} (id:{1} parent:{2})", base.fullTypeName, base.id, this.parentId) : base.ToString();
		}

		[SerializeField]
		private int m_ParentId;

		[SerializeField]
		private int m_OrderInDocument;

		[SerializeField]
		private bool m_IsField;
	}
}
