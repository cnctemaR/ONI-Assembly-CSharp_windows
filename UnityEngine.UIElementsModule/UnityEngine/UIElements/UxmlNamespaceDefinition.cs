using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	[Serializable]
	internal struct UxmlNamespaceDefinition : IEquatable<UxmlNamespaceDefinition>
	{
		public static UxmlNamespaceDefinition Empty { get; } = default(UxmlNamespaceDefinition);

		public string Export()
		{
			bool flag = string.IsNullOrEmpty(this.prefix);
			string text;
			if (flag)
			{
				text = "xmlns=\"" + this.resolvedNamespace + "\"";
			}
			else
			{
				text = string.Concat(new string[] { "xmlns:", this.prefix, "=\"", this.resolvedNamespace, "\"" });
			}
			return text;
		}

		public static bool operator ==(UxmlNamespaceDefinition lhs, UxmlNamespaceDefinition rhs)
		{
			bool flag = string.IsNullOrEmpty(lhs.prefix) && string.IsNullOrEmpty(rhs.prefix) && string.IsNullOrEmpty(lhs.resolvedNamespace) && string.IsNullOrEmpty(rhs.resolvedNamespace);
			return flag || (string.Compare(lhs.prefix, rhs.prefix, StringComparison.Ordinal) == 0 && string.Compare(lhs.resolvedNamespace, rhs.resolvedNamespace, StringComparison.Ordinal) == 0);
		}

		public static bool operator !=(UxmlNamespaceDefinition lhs, UxmlNamespaceDefinition rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(UxmlNamespaceDefinition other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is UxmlNamespaceDefinition)
			{
				UxmlNamespaceDefinition uxmlNamespaceDefinition = (UxmlNamespaceDefinition)obj;
				flag = this.Equals(uxmlNamespaceDefinition);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<string, string>(this.prefix, this.resolvedNamespace);
		}

		public string prefix;

		public string resolvedNamespace;
	}
}
