using System;
using System.Reflection;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public class PropertyTabAttribute : Attribute
	{
		public PropertyTabAttribute()
		{
			this.TabScopes = Array.Empty<PropertyTabScope>();
			this._tabClassNames = Array.Empty<string>();
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
			this._tabClasses = new Type[] { tabClass };
			if (tabScope < PropertyTabScope.Document)
			{
				throw new ArgumentException(SR.Format("Scope must be PropertyTabScope.Document or PropertyTabScope.Component", Array.Empty<object>()), "tabScope");
			}
			this.TabScopes = new PropertyTabScope[] { tabScope };
		}

		public PropertyTabAttribute(string tabClassName, PropertyTabScope tabScope)
		{
			this._tabClassNames = new string[] { tabClassName };
			if (tabScope < PropertyTabScope.Document)
			{
				throw new ArgumentException(SR.Format("Scope must be PropertyTabScope.Document or PropertyTabScope.Component", Array.Empty<object>()), "tabScope");
			}
			this.TabScopes = new PropertyTabScope[] { tabScope };
		}

		public Type[] TabClasses
		{
			get
			{
				if (this._tabClasses == null && this._tabClassNames != null)
				{
					this._tabClasses = new Type[this._tabClassNames.Length];
					for (int i = 0; i < this._tabClassNames.Length; i++)
					{
						int num = this._tabClassNames[i].IndexOf(',');
						string text = null;
						string text2;
						if (num != -1)
						{
							text2 = this._tabClassNames[i].Substring(0, num).Trim();
							text = this._tabClassNames[i].Substring(num + 1).Trim();
						}
						else
						{
							text2 = this._tabClassNames[i];
						}
						this._tabClasses[i] = Type.GetType(text2, false);
						if (this._tabClasses[i] == null)
						{
							if (text == null)
							{
								throw new TypeLoadException(SR.Format("Couldn't find type {0}", text2));
							}
							Assembly assembly = Assembly.Load(text);
							if (assembly != null)
							{
								this._tabClasses[i] = assembly.GetType(text2, true);
							}
						}
					}
				}
				return this._tabClasses;
			}
		}

		protected string[] TabClassNames
		{
			get
			{
				string[] tabClassNames = this._tabClassNames;
				return (string[])((tabClassNames != null) ? tabClassNames.Clone() : null);
			}
		}

		public PropertyTabScope[] TabScopes { get; private set; }

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
					throw new ArgumentException("tabClasses must have the same number of items as tabScopes");
				}
				this._tabClasses = (Type[])tabClasses.Clone();
			}
			else if (tabClassNames != null)
			{
				if (tabScopes != null && tabClassNames.Length != tabScopes.Length)
				{
					throw new ArgumentException("tabClasses must have the same number of items as tabScopes");
				}
				this._tabClassNames = (string[])tabClassNames.Clone();
				this._tabClasses = null;
			}
			else if (this._tabClasses == null && this._tabClassNames == null)
			{
				throw new ArgumentException("An array of tab type names or tab types must be specified");
			}
			if (tabScopes != null)
			{
				for (int i = 0; i < tabScopes.Length; i++)
				{
					if (tabScopes[i] < PropertyTabScope.Document)
					{
						throw new ArgumentException("Scope must be PropertyTabScope.Document or PropertyTabScope.Component");
					}
				}
				this.TabScopes = (PropertyTabScope[])tabScopes.Clone();
				return;
			}
			this.TabScopes = new PropertyTabScope[tabClasses.Length];
			for (int j = 0; j < this.TabScopes.Length; j++)
			{
				this.TabScopes[j] = PropertyTabScope.Component;
			}
		}

		private Type[] _tabClasses;

		private string[] _tabClassNames;
	}
}
