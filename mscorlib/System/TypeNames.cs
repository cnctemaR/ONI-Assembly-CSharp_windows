using System;

namespace System
{
	internal class TypeNames
	{
		internal static TypeName FromDisplay(string displayName)
		{
			return new TypeNames.Display(displayName);
		}

		internal abstract class ATypeName : TypeName, IEquatable<TypeName>
		{
			public abstract string DisplayName { get; }

			public abstract TypeName NestedName(TypeIdentifier innerName);

			public bool Equals(TypeName other)
			{
				return other != null && this.DisplayName == other.DisplayName;
			}

			public override int GetHashCode()
			{
				return this.DisplayName.GetHashCode();
			}

			public override bool Equals(object other)
			{
				return this.Equals(other as TypeName);
			}
		}

		private class Display : TypeNames.ATypeName
		{
			internal Display(string displayName)
			{
				this.displayName = displayName;
			}

			public override string DisplayName
			{
				get
				{
					return this.displayName;
				}
			}

			public override TypeName NestedName(TypeIdentifier innerName)
			{
				return new TypeNames.Display(this.DisplayName + "+" + innerName.DisplayName);
			}

			private string displayName;
		}
	}
}
