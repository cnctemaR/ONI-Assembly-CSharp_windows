using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	public sealed class EditorAttribute : Attribute
	{
		public EditorAttribute()
		{
			this.typeName = string.Empty;
			this.baseTypeName = string.Empty;
		}

		public EditorAttribute(string typeName, string baseTypeName)
		{
			typeName.ToUpperInvariant();
			this.typeName = typeName;
			this.baseTypeName = baseTypeName;
		}

		public EditorAttribute(string typeName, Type baseType)
		{
			typeName.ToUpperInvariant();
			this.typeName = typeName;
			this.baseTypeName = baseType.AssemblyQualifiedName;
		}

		public EditorAttribute(Type type, Type baseType)
		{
			this.typeName = type.AssemblyQualifiedName;
			this.baseTypeName = baseType.AssemblyQualifiedName;
		}

		public string EditorBaseTypeName
		{
			get
			{
				return this.baseTypeName;
			}
		}

		public string EditorTypeName
		{
			get
			{
				return this.typeName;
			}
		}

		public override object TypeId
		{
			get
			{
				if (this.typeId == null)
				{
					string text = this.baseTypeName;
					int num = text.IndexOf(',');
					if (num != -1)
					{
						text = text.Substring(0, num);
					}
					this.typeId = base.GetType().FullName + text;
				}
				return this.typeId;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			EditorAttribute editorAttribute = obj as EditorAttribute;
			return editorAttribute != null && editorAttribute.typeName == this.typeName && editorAttribute.baseTypeName == this.baseTypeName;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private string baseTypeName;

		private string typeName;

		private string typeId;
	}
}
