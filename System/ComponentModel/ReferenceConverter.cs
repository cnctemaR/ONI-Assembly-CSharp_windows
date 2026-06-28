using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Globalization;

namespace System.ComponentModel
{
	public class ReferenceConverter : TypeConverter
	{
		public ReferenceConverter(Type type)
		{
			this.reference_type = type;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return (context != null && sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (!(value is string))
			{
				return base.ConvertFrom(context, culture, value);
			}
			if (context != null)
			{
				object obj = null;
				global::System.ComponentModel.Design.IReferenceService referenceService = context.GetService(typeof(global::System.ComponentModel.Design.IReferenceService)) as global::System.ComponentModel.Design.IReferenceService;
				if (referenceService != null)
				{
					obj = referenceService.GetReference((string)value);
				}
				if (obj == null && context.Container != null && context.Container.Components != null)
				{
					obj = context.Container.Components[(string)value];
				}
				return obj;
			}
			return null;
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType != typeof(string))
			{
				return base.ConvertTo(context, culture, value, destinationType);
			}
			if (value == null)
			{
				return "(none)";
			}
			string text = string.Empty;
			if (context != null)
			{
				global::System.ComponentModel.Design.IReferenceService referenceService = context.GetService(typeof(global::System.ComponentModel.Design.IReferenceService)) as global::System.ComponentModel.Design.IReferenceService;
				if (referenceService != null)
				{
					text = referenceService.GetName(value);
				}
				if ((text == null || text.Length == 0) && value is IComponent)
				{
					IComponent component = (IComponent)value;
					if (component.Site != null && component.Site.Name != null)
					{
						text = component.Site.Name;
					}
				}
			}
			return text;
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ArrayList arrayList = new ArrayList();
			if (context != null)
			{
				global::System.ComponentModel.Design.IReferenceService referenceService = context.GetService(typeof(global::System.ComponentModel.Design.IReferenceService)) as global::System.ComponentModel.Design.IReferenceService;
				if (referenceService != null)
				{
					foreach (object obj in referenceService.GetReferences(this.reference_type))
					{
						if (this.IsValueAllowed(context, obj))
						{
							arrayList.Add(obj);
						}
					}
				}
				else if (context.Container != null && context.Container.Components != null)
				{
					foreach (object obj2 in context.Container.Components)
					{
						if (obj2 != null && this.IsValueAllowed(context, obj2) && this.reference_type.IsInstanceOfType(obj2))
						{
							arrayList.Add(obj2);
						}
					}
				}
				arrayList.Add(null);
			}
			return new TypeConverter.StandardValuesCollection(arrayList);
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		protected virtual bool IsValueAllowed(ITypeDescriptorContext context, object value)
		{
			return true;
		}

		private Type reference_type;
	}
}
