using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[StructLayout(LayoutKind.Sequential)]
	internal class FieldOnTypeBuilderInst : FieldInfo
	{
		public FieldOnTypeBuilderInst(TypeBuilderInstantiation instantiation, FieldInfo fb)
		{
			this.instantiation = instantiation;
			this.fb = fb;
		}

		public override Type DeclaringType
		{
			get
			{
				return this.instantiation;
			}
		}

		public override string Name
		{
			get
			{
				return this.fb.Name;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return this.instantiation;
			}
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		public override string ToString()
		{
			return this.fb.FieldType.ToString() + " " + this.Name;
		}

		public override FieldAttributes Attributes
		{
			get
			{
				return this.fb.Attributes;
			}
		}

		public override RuntimeFieldHandle FieldHandle
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override int MetadataToken
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		public override Type FieldType
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override object GetValue(object obj)
		{
			throw new NotSupportedException();
		}

		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		internal FieldInfo RuntimeResolve()
		{
			return this.instantiation.RuntimeResolve().GetField(this.fb);
		}

		internal TypeBuilderInstantiation instantiation;

		internal FieldInfo fb;
	}
}
