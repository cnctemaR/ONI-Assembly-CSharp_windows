using System;

namespace UnityEngine.UIElements
{
	[Obsolete("UxmlRootElementFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public class UxmlRootElementFactory : UxmlFactory<VisualElement, UxmlRootElementTraits>
	{
		public override string uxmlName
		{
			get
			{
				return "UXML";
			}
		}

		public override string uxmlQualifiedName
		{
			get
			{
				return this.uxmlNamespace + "." + this.uxmlName;
			}
		}

		public override string substituteForTypeName
		{
			get
			{
				return string.Empty;
			}
		}

		public override string substituteForTypeNamespace
		{
			get
			{
				return string.Empty;
			}
		}

		public override string substituteForTypeQualifiedName
		{
			get
			{
				return string.Empty;
			}
		}

		public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
		{
			return null;
		}

		internal const string k_ElementName = "UXML";
	}
}
