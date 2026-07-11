using System;
using System.Reflection;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public class PropertyTabAttribute : Attribute
	{
		public PropertyTabAttribute()
		{
			this.tabScopes = new PropertyTabScope[0];
			this.tabClassNames = new string[0];
		}

		public PropertyTabAttribute(Type tabClass)
			: this(tabClass, PropertyTabScope.Component)
		{
		}

		public PropertyTabAttribute(string tabClassName)
			: this(tabClassName, PropertyTabScope.Component)
		{
		}

		public PropertyTabAttribute(Type tabClass, PropertyTabScope tabScope)
		{
			this.tabClasses = new Type[] { tabClass };
			if (tabScope < PropertyTabScope.Document)
			{
				throw new ArgumentException(global::SR.GetString("Scope must be PropertyTabScope.Document or PropertyTabScope.Component"), "tabScope");
			}
			this.tabScopes = new PropertyTabScope[] { tabScope };
		}

		public PropertyTabAttribute(string tabClassName, PropertyTabScope tabScope)
		{
			this.tabClassNames = new string[] { tabClassName };
			if (tabScope < PropertyTabScope.Document)
			{
				throw new ArgumentException(global::SR.GetString("Scope must be PropertyTabScope.Document or PropertyTabScope.Component"), "tabScope");
			}
			this.tabScopes = new PropertyTabScope[] { tabScope };
		}

		public Type[] TabClasses
		{
			get
			{
				if (this.tabClasses == null && this.tabClassNames != null)
				{
					this.tabClasses = new Type[this.tabClassNames.Length];
					for (int i = 0; i < this.tabClassNames.Length; i++)
					{
						int num = this.tabClassNames[i].IndexOf(',');
						string text = null;
						string text2;
						if (num != -1)
						{
							text2 = this.tabClassNames[i].Substring(0, num).Trim();
							text = this.tabClassNames[i].Substring(num + 1).Trim();
						}
						else
						{
							text2 = this.tabClassNames[i];
						}
						this.tabClasses[i] = Type.GetType(text2, false);
						if (this.tabClasses[i] == null)
						{
							if (text == null)
							{
								throw new TypeLoadException(global::SR.GetString("Couldn't find type {0}", new object[] { text2 }));
							}
							Assembly assembly = Assembly.Load(text);
							if (assembly != null)
							{
								this.tabClasses[i] = assembly.GetType(text2, true);
							}
						}
					}
				}
				return this.tabClasses;
			}
		}

		protected string[] TabClassNames
		{
			get
			{
				if (this.tabClassNames != null)
				{
					return (string[])this.tabClassNames.Clone();
				}
				return null;
			}
		}

		public PropertyTabScope[] TabScopes
		{
			get
			{
				return this.tabScopes;
			}
		}

		public override bool Equals(object other)
		{
			return other is PropertyTabAttribute && this.Equals((PropertyTabAttribute)other);
		}

		public bool Equals(PropertyTabAttribute other)
		{
			if (other == this)
			{
				return true;
			}
			if (other.TabClasses.Length != this.TabClasses.Length || other.TabScopes.Length != this.TabScopes.Length)
			{
				return false;
			}
			for (int i = 0; i < this.TabClasses.Length; i++)
			{
				if (this.TabClasses[i] != other.TabClasses[i] || this.TabScopes[i] != other.TabScopes[i])
				{
					return false;
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
			this.InitializeArrays(tabClassNames, null, tabScopes);
		}

		protected void InitializeArrays(Type[] tabClasses, PropertyTabScope[] tabScopes)
		{
			this.InitializeArrays(null, tabClasses, tabScopes);
		}

		private void InitializeArrays(string[] tabClassNames, Type[] tabClasses, PropertyTabScope[] tabScopes)
		{
			if (tabClasses != null)
			{
				if (tabScopes != null && tabClasses.Length != tabScopes.Length)
				{
					throw new ArgumentException(global::SR.GetString("tabClasses must have the same number of items as tabScopes"));
				}
				this.tabClasses = (Type[])tabClasses.Clone();
			}
			else if (tabClassNames != null)
			{
				if (tabScopes != null && tabClasses.Length != tabScopes.Length)
				{
					throw new ArgumentException(global::SR.GetString("tabClasses must have the same number of items as tabScopes"));
				}
				this.tabClassNames = (string[])tabClassNames.Clone();
				this.tabClasses = null;
			}
			else if (this.tabClasses == null && this.tabClassNames == null)
			{
				throw new ArgumentException(global::SR.GetString("An array of tab type names or tab types must be specified"));
			}
			if (tabScopes != null)
			{
				for (int i = 0; i < tabScopes.Length; i++)
				{
					if (tabScopes[i] < PropertyTabScope.Document)
					{
						throw new ArgumentException(global::SR.GetString("Scope must be PropertyTabScope.Document or PropertyTabScope.Component"));
					}
				}
				this.tabScopes = (PropertyTabScope[])tabScopes.Clone();
				return;
			}
			this.tabScopes = new PropertyTabScope[tabClasses.Length];
			for (int j = 0; j < this.TabScopes.Length; j++)
			{
				this.tabScopes[j] = PropertyTabScope.Component;
			}
		}

		private PropertyTabScope[] tabScopes;

		private Type[] tabClasses;

		private string[] tabClassNames;
	}
}
