using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Reflection
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal class MonoProperty : RuntimePropertyInfo
	{
		private void CachePropertyInfo(PInfo flags)
		{
			if ((this.cached & flags) != flags)
			{
				MonoPropertyInfo.get_property_info(this, ref this.info, flags);
				this.cached |= flags;
			}
		}

		public override PropertyAttributes Attributes
		{
			get
			{
				this.CachePropertyInfo(PInfo.Attributes);
				return this.info.attrs;
			}
		}

		public override bool CanRead
		{
			get
			{
				this.CachePropertyInfo(PInfo.GetMethod);
				return this.info.get_method != null;
			}
		}

		public override bool CanWrite
		{
			get
			{
				this.CachePropertyInfo(PInfo.SetMethod);
				return this.info.set_method != null;
			}
		}

		public override Type PropertyType
		{
			get
			{
				this.CachePropertyInfo(PInfo.GetMethod | PInfo.SetMethod);
				if (this.info.get_method != null)
				{
					return this.info.get_method.ReturnType;
				}
				ParameterInfo[] parametersInternal = this.info.set_method.GetParametersInternal();
				return parametersInternal[parametersInternal.Length - 1].ParameterType;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				this.CachePropertyInfo(PInfo.ReflectedType);
				return this.info.parent;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				this.CachePropertyInfo(PInfo.DeclaringType);
				return this.info.declaring_type;
			}
		}

		public override string Name
		{
			get
			{
				this.CachePropertyInfo(PInfo.Name);
				return this.info.name;
			}
		}

		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			int num = 0;
			int num2 = 0;
			this.CachePropertyInfo(PInfo.GetMethod | PInfo.SetMethod);
			if (this.info.set_method != null && (nonPublic || this.info.set_method.IsPublic))
			{
				num2 = 1;
			}
			if (this.info.get_method != null && (nonPublic || this.info.get_method.IsPublic))
			{
				num = 1;
			}
			MethodInfo[] array = new MethodInfo[num + num2];
			int num3 = 0;
			if (num2 != 0)
			{
				array[num3++] = this.info.set_method;
			}
			if (num != 0)
			{
				array[num3++] = this.info.get_method;
			}
			return array;
		}

		public override MethodInfo GetGetMethod(bool nonPublic)
		{
			this.CachePropertyInfo(PInfo.GetMethod);
			if (this.info.get_method != null && (nonPublic || this.info.get_method.IsPublic))
			{
				return this.info.get_method;
			}
			return null;
		}

		public override ParameterInfo[] GetIndexParameters()
		{
			this.CachePropertyInfo(PInfo.GetMethod | PInfo.SetMethod);
			ParameterInfo[] array;
			int num;
			if (this.info.get_method != null)
			{
				array = this.info.get_method.GetParametersInternal();
				num = array.Length;
			}
			else
			{
				if (!(this.info.set_method != null))
				{
					return EmptyArray<ParameterInfo>.Value;
				}
				array = this.info.set_method.GetParametersInternal();
				num = array.Length - 1;
			}
			ParameterInfo[] array2 = new ParameterInfo[num];
			for (int i = 0; i < num; i++)
			{
				array2[i] = ParameterInfo.New(array[i], this);
			}
			return array2;
		}

		public override MethodInfo GetSetMethod(bool nonPublic)
		{
			this.CachePropertyInfo(PInfo.SetMethod);
			if (this.info.set_method != null && (nonPublic || this.info.set_method.IsPublic))
			{
				return this.info.set_method;
			}
			return null;
		}

		public override object GetConstantValue()
		{
			return MonoPropertyInfo.get_default_value(this);
		}

		public override object GetRawConstantValue()
		{
			return MonoPropertyInfo.get_default_value(this);
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.IsDefined(this, attributeType, false);
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, false);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, attributeType, false);
		}

		private static object GetterAdapterFrame<T, R>(MonoProperty.Getter<T, R> getter, object obj)
		{
			return getter((T)((object)obj));
		}

		private static object StaticGetterAdapterFrame<R>(MonoProperty.StaticGetter<R> getter, object obj)
		{
			return getter();
		}

		private static MonoProperty.GetterAdapter CreateGetterDelegate(MethodInfo method)
		{
			Type[] array;
			Type type;
			string text;
			if (method.IsStatic)
			{
				array = new Type[] { method.ReturnType };
				type = typeof(MonoProperty.StaticGetter<>);
				text = "StaticGetterAdapterFrame";
			}
			else
			{
				array = new Type[] { method.DeclaringType, method.ReturnType };
				type = typeof(MonoProperty.Getter<, >);
				text = "GetterAdapterFrame";
			}
			object obj = Delegate.CreateDelegate(type.MakeGenericType(array), method);
			MethodInfo methodInfo = typeof(MonoProperty).GetMethod(text, BindingFlags.Static | BindingFlags.NonPublic);
			methodInfo = methodInfo.MakeGenericMethod(array);
			return (MonoProperty.GetterAdapter)Delegate.CreateDelegate(typeof(MonoProperty.GetterAdapter), obj, methodInfo, true);
		}

		public override object GetValue(object obj, object[] index)
		{
			if (index != null)
			{
				int num = index.Length;
			}
			return this.GetValue(obj, BindingFlags.Default, null, index, null);
		}

		public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			object obj2 = null;
			MethodInfo getMethod = this.GetGetMethod(true);
			if (getMethod == null)
			{
				throw new ArgumentException("Get Method not found for '" + this.Name + "'");
			}
			try
			{
				if (index == null || index.Length == 0)
				{
					obj2 = getMethod.Invoke(obj, invokeAttr, binder, null, culture);
				}
				else
				{
					obj2 = getMethod.Invoke(obj, invokeAttr, binder, index, culture);
				}
			}
			catch (SecurityException ex)
			{
				throw new TargetInvocationException(ex);
			}
			return obj2;
		}

		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			MethodInfo setMethod = this.GetSetMethod(true);
			if (setMethod == null)
			{
				throw new ArgumentException("Set Method not found for '" + this.Name + "'");
			}
			object[] array;
			if (index == null || index.Length == 0)
			{
				array = new object[] { value };
			}
			else
			{
				int num = index.Length;
				array = new object[num + 1];
				index.CopyTo(array, 0);
				array[num] = value;
			}
			setMethod.Invoke(obj, invokeAttr, binder, array, culture);
		}

		public override Type[] GetOptionalCustomModifiers()
		{
			Type[] typeModifiers = MonoPropertyInfo.GetTypeModifiers(this, true);
			if (typeModifiers == null)
			{
				return Type.EmptyTypes;
			}
			return typeModifiers;
		}

		public override Type[] GetRequiredCustomModifiers()
		{
			Type[] typeModifiers = MonoPropertyInfo.GetTypeModifiers(this, false);
			if (typeModifiers == null)
			{
				return Type.EmptyTypes;
			}
			return typeModifiers;
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return CustomAttributeData.GetCustomAttributes(this);
		}

		internal IntPtr klass;

		internal IntPtr prop;

		private MonoPropertyInfo info;

		private PInfo cached;

		private MonoProperty.GetterAdapter cached_getter;

		private delegate object GetterAdapter(object _this);

		private delegate R Getter<T, R>(T _this);

		private delegate R StaticGetter<R>();
	}
}
