using System;
using System.Collections;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	internal sealed class ExtendedPropertyDescriptor : PropertyDescriptor
	{
		public ExtendedPropertyDescriptor(ReflectPropertyDescriptor extenderInfo, Type receiverType, IExtenderProvider provider, Attribute[] attributes)
			: base(extenderInfo, attributes)
		{
			ArrayList arrayList = new ArrayList(this.AttributeArray);
			arrayList.Add(ExtenderProvidedPropertyAttribute.Create(extenderInfo, receiverType, provider));
			if (extenderInfo.IsReadOnly)
			{
				arrayList.Add(ReadOnlyAttribute.Yes);
			}
			Attribute[] array = new Attribute[arrayList.Count];
			arrayList.CopyTo(array, 0);
			this.AttributeArray = array;
			this.extenderInfo = extenderInfo;
			this.provider = provider;
		}

		public ExtendedPropertyDescriptor(PropertyDescriptor extender, Attribute[] attributes)
			: base(extender, attributes)
		{
			ExtenderProvidedPropertyAttribute extenderProvidedPropertyAttribute = extender.Attributes[typeof(ExtenderProvidedPropertyAttribute)] as ExtenderProvidedPropertyAttribute;
			ReflectPropertyDescriptor reflectPropertyDescriptor = extenderProvidedPropertyAttribute.ExtenderProperty as ReflectPropertyDescriptor;
			this.extenderInfo = reflectPropertyDescriptor;
			this.provider = extenderProvidedPropertyAttribute.Provider;
		}

		public override bool CanResetValue(object comp)
		{
			return this.extenderInfo.ExtenderCanResetValue(this.provider, comp);
		}

		public override Type ComponentType
		{
			get
			{
				return this.extenderInfo.ComponentType;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return this.Attributes[typeof(ReadOnlyAttribute)].Equals(ReadOnlyAttribute.Yes);
			}
		}

		public override Type PropertyType
		{
			get
			{
				return this.extenderInfo.ExtenderGetType(this.provider);
			}
		}

		public override string DisplayName
		{
			get
			{
				string text = base.DisplayName;
				DisplayNameAttribute displayNameAttribute = this.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;
				if (displayNameAttribute == null || displayNameAttribute.IsDefaultAttribute())
				{
					ISite site = MemberDescriptor.GetSite(this.provider);
					if (site != null)
					{
						string name = site.Name;
						if (name != null && name.Length > 0)
						{
							text = global::SR.GetString("{0} on {1}", new object[] { text, name });
						}
					}
				}
				return text;
			}
		}

		public override object GetValue(object comp)
		{
			return this.extenderInfo.ExtenderGetValue(this.provider, comp);
		}

		public override void ResetValue(object comp)
		{
			this.extenderInfo.ExtenderResetValue(this.provider, comp, this);
		}

		public override void SetValue(object component, object value)
		{
			this.extenderInfo.ExtenderSetValue(this.provider, component, value, this);
		}

		public override bool ShouldSerializeValue(object comp)
		{
			return this.extenderInfo.ExtenderShouldSerializeValue(this.provider, comp);
		}

		private readonly ReflectPropertyDescriptor extenderInfo;

		private readonly IExtenderProvider provider;
	}
}
