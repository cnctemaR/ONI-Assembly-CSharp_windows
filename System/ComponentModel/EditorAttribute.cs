using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	public sealed class EditorAttribute : Attribute
	{
		public EditorAttribute()
		{
			this.name = string.Empty;
		}

		public EditorAttribute(string typeName, string baseTypeName)
		{
			this.name = typeName;
			this.basename = baseTypeName;
		}

		public EditorAttribute(string typeName, Type baseType)
			: this(typeName, baseType.AssemblyQualifiedName)
		{
		}

		public EditorAttribute(Type type, Type baseType)
			: this(type.AssemblyQualifiedName, baseType.AssemblyQualifiedName)
		{
		}

		public string EditorBaseTypeName
		{
			get
			{
				return this.basename;
			}
		}

		public string EditorTypeName
		{
			get
			{
				return this.name;
			}
		}

		public override object TypeId
		{
			get
			{
				return base.GetType();
			}
		}

		public override bool Equals(object obj)
		{
			return obj is EditorAttribute && ((EditorAttribute)obj).EditorBaseTypeName.Equals(this.basename) && ((EditorAttribute)obj).EditorTypeName.Equals(this.name);
		}

		public override int GetHashCode()
		{
			return (this.name + this.basename).GetHashCode();
		}

		private string name;

		private string basename;
	}
}
