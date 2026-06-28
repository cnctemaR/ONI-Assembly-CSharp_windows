using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[ComVisible(true)]
	public abstract class MemberDescriptor
	{
		protected MemberDescriptor(string name, Attribute[] attrs)
		{
			this.name = name;
			this.attrs = attrs;
		}

		protected MemberDescriptor(MemberDescriptor reference, Attribute[] attrs)
		{
			this.name = reference.name;
			this.attrs = attrs;
		}

		protected MemberDescriptor(string name)
		{
			this.name = name;
		}

		protected MemberDescriptor(MemberDescriptor reference)
		{
			this.name = reference.name;
			this.attrs = reference.AttributeArray;
		}

		protected virtual Attribute[] AttributeArray
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				if (this.attrs != null)
				{
					arrayList.AddRange(this.attrs);
				}
				this.FillAttributes(arrayList);
				Hashtable hashtable = new Hashtable();
				foreach (object obj in arrayList)
				{
					Attribute attribute = (Attribute)obj;
					hashtable[attribute.TypeId] = attribute;
				}
				Attribute[] array = new Attribute[hashtable.Values.Count];
				hashtable.Values.CopyTo(array, 0);
				return array;
			}
			set
			{
				this.attrs = value;
			}
		}

		protected virtual void FillAttributes(IList attributeList)
		{
		}

		public virtual AttributeCollection Attributes
		{
			get
			{
				if (this.attrCollection == null)
				{
					this.attrCollection = this.CreateAttributeCollection();
				}
				return this.attrCollection;
			}
		}

		protected virtual AttributeCollection CreateAttributeCollection()
		{
			return new AttributeCollection(this.AttributeArray);
		}

		public virtual string Category
		{
			get
			{
				return ((CategoryAttribute)this.Attributes[typeof(CategoryAttribute)]).Category;
			}
		}

		public virtual string Description
		{
			get
			{
				foreach (Attribute attribute in this.AttributeArray)
				{
					if (attribute is DescriptionAttribute)
					{
						return ((DescriptionAttribute)attribute).Description;
					}
				}
				return string.Empty;
			}
		}

		public virtual bool DesignTimeOnly
		{
			get
			{
				foreach (Attribute attribute in this.AttributeArray)
				{
					if (attribute is DesignOnlyAttribute)
					{
						return ((DesignOnlyAttribute)attribute).IsDesignOnly;
					}
				}
				return false;
			}
		}

		public virtual string DisplayName
		{
			get
			{
				foreach (Attribute attribute in this.AttributeArray)
				{
					if (attribute is DisplayNameAttribute)
					{
						return ((DisplayNameAttribute)attribute).DisplayName;
					}
				}
				return this.name;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.name;
			}
		}

		public virtual bool IsBrowsable
		{
			get
			{
				foreach (Attribute attribute in this.AttributeArray)
				{
					if (attribute is BrowsableAttribute)
					{
						return ((BrowsableAttribute)attribute).Browsable;
					}
				}
				return true;
			}
		}

		protected virtual int NameHashCode
		{
			get
			{
				return this.name.GetHashCode();
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			MemberDescriptor memberDescriptor = obj as MemberDescriptor;
			return memberDescriptor != null && memberDescriptor.name == this.name;
		}

		protected static ISite GetSite(object component)
		{
			if (component is Component)
			{
				return ((Component)component).Site;
			}
			return null;
		}

		[Obsolete("Use GetInvocationTarget")]
		protected static object GetInvokee(Type componentClass, object component)
		{
			if (component is IComponent)
			{
				ISite site = ((IComponent)component).Site;
				if (site != null && site.DesignMode)
				{
					global::System.ComponentModel.Design.IDesignerHost designerHost = site.GetService(typeof(global::System.ComponentModel.Design.IDesignerHost)) as global::System.ComponentModel.Design.IDesignerHost;
					if (designerHost != null)
					{
						global::System.ComponentModel.Design.IDesigner designer = designerHost.GetDesigner((IComponent)component);
						if (designer != null && componentClass.IsInstanceOfType(designer))
						{
							component = designer;
						}
					}
				}
			}
			return component;
		}

		protected virtual object GetInvocationTarget(Type type, object instance)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return MemberDescriptor.GetInvokee(type, instance);
		}

		protected static MethodInfo FindMethod(Type componentClass, string name, Type[] args, Type returnType)
		{
			return MemberDescriptor.FindMethod(componentClass, name, args, returnType, true);
		}

		protected static MethodInfo FindMethod(Type componentClass, string name, Type[] args, Type returnType, bool publicOnly)
		{
			BindingFlags bindingFlags;
			if (publicOnly)
			{
				bindingFlags = BindingFlags.Public;
			}
			else
			{
				bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
			}
			return componentClass.GetMethod(name, bindingFlags, null, CallingConventions.Any, args, null);
		}

		internal static IComparer DefaultComparer
		{
			get
			{
				if (MemberDescriptor.default_comparer == null)
				{
					MemberDescriptor.default_comparer = new MemberDescriptor.MemberDescriptorComparer();
				}
				return MemberDescriptor.default_comparer;
			}
		}

		private string name;

		private Attribute[] attrs;

		private AttributeCollection attrCollection;

		private static IComparer default_comparer;

		private class MemberDescriptorComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				return string.Compare(((MemberDescriptor)x).Name, ((MemberDescriptor)y).Name, false, CultureInfo.InvariantCulture);
			}
		}
	}
}
