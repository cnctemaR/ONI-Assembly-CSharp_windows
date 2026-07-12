using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[StructLayout(LayoutKind.Sequential)]
	internal class EventOnTypeBuilderInst : EventInfo
	{
		internal EventOnTypeBuilderInst(TypeBuilderInstantiation instantiation, EventBuilder evt)
		{
			this.instantiation = instantiation;
			this.event_builder = evt;
		}

		internal EventOnTypeBuilderInst(TypeBuilderInstantiation instantiation, EventInfo evt)
		{
			this.instantiation = instantiation;
			this.event_info = evt;
		}

		public override EventAttributes Attributes
		{
			get
			{
				if (this.event_builder == null)
				{
					return this.event_info.Attributes;
				}
				return this.event_builder.attrs;
			}
		}

		public override MethodInfo GetAddMethod(bool nonPublic)
		{
			MethodInfo methodInfo = ((this.event_builder != null) ? this.event_builder.add_method : this.event_info.GetAddMethod(nonPublic));
			if (methodInfo == null || (!nonPublic && !methodInfo.IsPublic))
			{
				return null;
			}
			return TypeBuilder.GetMethod(this.instantiation, methodInfo);
		}

		public override MethodInfo GetRaiseMethod(bool nonPublic)
		{
			MethodInfo methodInfo = ((this.event_builder != null) ? this.event_builder.raise_method : this.event_info.GetRaiseMethod(nonPublic));
			if (methodInfo == null || (!nonPublic && !methodInfo.IsPublic))
			{
				return null;
			}
			return TypeBuilder.GetMethod(this.instantiation, methodInfo);
		}

		public override MethodInfo GetRemoveMethod(bool nonPublic)
		{
			MethodInfo methodInfo = ((this.event_builder != null) ? this.event_builder.remove_method : this.event_info.GetRemoveMethod(nonPublic));
			if (methodInfo == null || (!nonPublic && !methodInfo.IsPublic))
			{
				return null;
			}
			return TypeBuilder.GetMethod(this.instantiation, methodInfo);
		}

		public override MethodInfo[] GetOtherMethods(bool nonPublic)
		{
			MethodInfo[] array;
			if (this.event_builder == null)
			{
				array = this.event_info.GetOtherMethods(nonPublic);
			}
			else
			{
				MethodInfo[] array2 = this.event_builder.other_methods;
				array = array2;
			}
			MethodInfo[] array3 = array;
			if (array3 == null)
			{
				return new MethodInfo[0];
			}
			ArrayList arrayList = new ArrayList();
			foreach (MethodInfo methodInfo in array3)
			{
				if (nonPublic || methodInfo.IsPublic)
				{
					arrayList.Add(TypeBuilder.GetMethod(this.instantiation, methodInfo));
				}
			}
			MethodInfo[] array4 = new MethodInfo[arrayList.Count];
			arrayList.CopyTo(array4, 0);
			return array4;
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
				if (this.event_builder == null)
				{
					return this.event_info.Name;
				}
				return this.event_builder.name;
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

		private TypeBuilderInstantiation instantiation;

		private EventBuilder event_builder;

		private EventInfo event_info;
	}
}
