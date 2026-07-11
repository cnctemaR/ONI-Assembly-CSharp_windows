using System;
using System.ComponentModel.Design;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
	public sealed class DesignerAttribute : Attribute
	{
		public DesignerAttribute(string designerTypeName)
		{
			if (designerTypeName == null)
			{
				throw new NullReferenceException();
			}
			this.name = designerTypeName;
			this.basetypename = typeof(global::System.ComponentModel.Design.IDesigner).FullName;
		}

		public DesignerAttribute(Type designerType)
			: this(designerType.AssemblyQualifiedName)
		{
		}

		public DesignerAttribute(string designerTypeName, Type designerBaseType)
			: this(designerTypeName, designerBaseType.AssemblyQualifiedName)
		{
		}

		public DesignerAttribute(Type designerType, Type designerBaseType)
			: this(designerType.AssemblyQualifiedName, designerBaseType.AssemblyQualifiedName)
		{
		}

		public DesignerAttribute(string designerTypeName, string designerBaseTypeName)
		{
			if (designerTypeName == null)
			{
				throw new NullReferenceException();
			}
			this.name = designerTypeName;
			this.basetypename = designerBaseTypeName;
		}

		public string DesignerBaseTypeName
		{
			get
			{
				return this.basetypename;
			}
		}

		public string DesignerTypeName
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
				string text = this.basetypename;
				int num = text.IndexOf(',');
				if (num != -1)
				{
					text = text.Substring(0, num);
				}
				return base.GetType().ToString() + text;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is DesignerAttribute && ((DesignerAttribute)obj).DesignerBaseTypeName.Equals(this.basetypename) && ((DesignerAttribute)obj).DesignerTypeName.Equals(this.name);
		}

		public override int GetHashCode()
		{
			return (this.name + this.basetypename).GetHashCode();
		}

		private string name;

		private string basetypename;
	}
}
