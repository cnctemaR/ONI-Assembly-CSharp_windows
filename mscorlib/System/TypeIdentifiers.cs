using System;

namespace System
{
	internal class TypeIdentifiers
	{
		internal static TypeIdentifier FromDisplay(string displayName)
		{
			return new TypeIdentifiers.Display(displayName);
		}

		internal static TypeIdentifier FromInternal(string internalName)
		{
			return new TypeIdentifiers.Internal(internalName);
		}

		internal static TypeIdentifier FromInternal(string internalNameSpace, TypeIdentifier typeName)
		{
			return new TypeIdentifiers.Internal(internalNameSpace, typeName);
		}

		internal static TypeIdentifier WithoutEscape(string simpleName)
		{
			return new TypeIdentifiers.NoEscape(simpleName);
		}

		private class Display : TypeNames.ATypeName, TypeIdentifier, TypeName, IEquatable<TypeName>
		{
			internal Display(string displayName)
			{
				this.displayName = displayName;
				this.internal_name = null;
			}

			public override string DisplayName
			{
				get
				{
					return this.displayName;
				}
			}

			public string InternalName
			{
				get
				{
					if (this.internal_name == null)
					{
						this.internal_name = this.GetInternalName();
					}
					return this.internal_name;
				}
			}

			private string GetInternalName()
			{
				return TypeSpec.UnescapeInternalName(this.displayName);
			}

			public override TypeName NestedName(TypeIdentifier innerName)
			{
				return TypeNames.FromDisplay(this.DisplayName + "+" + innerName.DisplayName);
			}

			private string displayName;

			private string internal_name;
		}

		private class Internal : TypeNames.ATypeName, TypeIdentifier, TypeName, IEquatable<TypeName>
		{
			internal Internal(string internalName)
			{
				this.internalName = internalName;
				this.display_name = null;
			}

			internal Internal(string nameSpaceInternal, TypeIdentifier typeName)
			{
				this.internalName = nameSpaceInternal + "." + typeName.InternalName;
				this.display_name = null;
			}

			public override string DisplayName
			{
				get
				{
					if (this.display_name == null)
					{
						this.display_name = this.GetDisplayName();
					}
					return this.display_name;
				}
			}

			public string InternalName
			{
				get
				{
					return this.internalName;
				}
			}

			private string GetDisplayName()
			{
				return TypeSpec.EscapeDisplayName(this.internalName);
			}

			public override TypeName NestedName(TypeIdentifier innerName)
			{
				return TypeNames.FromDisplay(this.DisplayName + "+" + innerName.DisplayName);
			}

			private string internalName;

			private string display_name;
		}

		private class NoEscape : TypeNames.ATypeName, TypeIdentifier, TypeName, IEquatable<TypeName>
		{
			internal NoEscape(string simpleName)
			{
				this.simpleName = simpleName;
			}

			public override string DisplayName
			{
				get
				{
					return this.simpleName;
				}
			}

			public string InternalName
			{
				get
				{
					return this.simpleName;
				}
			}

			public override TypeName NestedName(TypeIdentifier innerName)
			{
				return TypeNames.FromDisplay(this.DisplayName + "+" + innerName.DisplayName);
			}

			private string simpleName;
		}
	}
}
