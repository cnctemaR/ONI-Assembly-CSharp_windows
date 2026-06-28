using System;
using System.Reflection;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public class PropertyTabAttribute : Attribute
	{
		public PropertyTabAttribute()
		{
			this.tabs = Type.EmptyTypes;
			this.scopes = new PropertyTabScope[0];
		}

		public PropertyTabAttribute(string tabClassName)
			: this(tabClassName, PropertyTabScope.Component)
		{
		}

		public PropertyTabAttribute(Type tabClass)
			: this(tabClass, PropertyTabScope.Component)
		{
		}

		public PropertyTabAttribute(string tabClassName, PropertyTabScope tabScope)
		{
			if (tabClassName == null)
			{
				throw new ArgumentNullException("tabClassName");
			}
			this.InitializeArrays(new string[] { tabClassName }, new PropertyTabScope[] { tabScope });
		}

		public PropertyTabAttribute(Type tabClass, PropertyTabScope tabScope)
		{
			if (tabClass == null)
			{
				throw new ArgumentNullException("tabClass");
			}
			this.InitializeArrays(new Type[] { tabClass }, new PropertyTabScope[] { tabScope });
		}

		public Type[] TabClasses
		{
			get
			{
				return this.tabs;
			}
		}

		public PropertyTabScope[] TabScopes
		{
			get
			{
				return this.scopes;
			}
		}

		protected string[] TabClassNames
		{
			get
			{
				string[] array = new string[this.tabs.Length];
				for (int i = 0; i < this.tabs.Length; i++)
				{
					array[i] = this.tabs[i].Name;
				}
				return array;
			}
		}

		public override bool Equals(object other)
		{
			return other is PropertyTabAttribute && this.Equals((PropertyTabAttribute)other);
		}

		public bool Equals(PropertyTabAttribute other)
		{
			if (other != this)
			{
				if (other.TabClasses.Length != this.tabs.Length)
				{
					return false;
				}
				for (int i = 0; i < this.tabs.Length; i++)
				{
					if (this.tabs[i] != other.TabClasses[i])
					{
						return false;
					}
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		protected void InitializeArrays(string[] tabClassNames, PropertyTabScope[] tabScopes)
		{
			if (tabScopes == null)
			{
				throw new ArgumentNullException("tabScopes");
			}
			if (tabClassNames == null)
			{
				throw new ArgumentNullException("tabClassNames");
			}
			this.scopes = tabScopes;
			this.tabs = new Type[tabClassNames.Length];
			for (int i = 0; i < tabClassNames.Length; i++)
			{
				this.tabs[i] = this.GetTypeFromName(tabClassNames[i]);
			}
		}

		protected void InitializeArrays(Type[] tabClasses, PropertyTabScope[] tabScopes)
		{
			if (tabScopes == null)
			{
				throw new ArgumentNullException("tabScopes");
			}
			if (tabClasses == null)
			{
				throw new ArgumentNullException("tabClasses");
			}
			if (tabClasses.Length != tabScopes.Length)
			{
				throw new ArgumentException("tabClasses.Length != tabScopes.Length");
			}
			this.tabs = tabClasses;
			this.scopes = tabScopes;
		}

		private Type GetTypeFromName(string typeName)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			int num = typeName.IndexOf(",");
			if (num != -1)
			{
				string text = typeName.Substring(0, num);
				string text2 = typeName.Substring(num + 1);
				Assembly assembly = Assembly.Load(text2);
				if (assembly != null)
				{
					return assembly.GetType(text, true);
				}
			}
			return Type.GetType(typeName, true);
		}

		private Type[] tabs;

		private PropertyTabScope[] scopes;
	}
}
