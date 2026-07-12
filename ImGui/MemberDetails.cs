using System;
using System.Reflection;

namespace ImGuiObjectDrawer
{
	public readonly struct MemberDetails
	{
		public MemberDetails(string name, Type type, object value)
		{
			this.name = name;
			this.type = ((value != null) ? value.GetType() : null) ?? type;
			this.value = value;
		}

		public bool IsDefaultValue()
		{
			if (this.value == null)
			{
				return true;
			}
			Type type = this.value.GetType();
			if (type.IsEnum)
			{
				return false;
			}
			if (type.IsValueType)
			{
				return this.value.Equals(Activator.CreateInstance(type));
			}
			return this.value.Equals(null);
		}

		public bool CanAssignToType<T>()
		{
			return typeof(T).IsAssignableFrom(this.type);
		}

		public static void Visit(object target, in MemberDrawContext context, MemberDetails.MemberDetailsVisitor visit)
		{
			foreach (MemberInfo memberInfo in target.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				MemberDetails memberDetails;
				if (MemberDetails.TryMakeFor(target, memberInfo, out memberDetails) && (!context.hide_default_values || !memberDetails.IsDefaultValue()))
				{
					visit(in context, in memberDetails);
				}
			}
		}

		public static bool TryMakeFor(object target, MemberInfo member, out MemberDetails details)
		{
			if (member.MemberType == MemberTypes.Property)
			{
				return MemberDetails.TryMakeFor(target, (PropertyInfo)member, out details);
			}
			if (member.MemberType == MemberTypes.Field)
			{
				return MemberDetails.TryMakeFor(target, (FieldInfo)member, out details);
			}
			details = default(MemberDetails);
			return false;
		}

		private static bool TryMakeFor(object target, PropertyInfo property, out MemberDetails details)
		{
			if (property.GetIndexParameters().Length != 0)
			{
				details = default(MemberDetails);
				return false;
			}
			bool flag;
			try
			{
				details = new MemberDetails(property.Name, property.PropertyType, property.GetValue(target));
				flag = true;
			}
			catch (Exception)
			{
				details = default(MemberDetails);
				flag = false;
			}
			return flag;
		}

		private static bool TryMakeFor(object target, FieldInfo field, out MemberDetails details)
		{
			if (field.Name[0] == '<')
			{
				details = default(MemberDetails);
				return false;
			}
			bool flag;
			try
			{
				details = new MemberDetails(field.Name, field.FieldType, field.GetValue(target));
				flag = true;
			}
			catch (Exception)
			{
				details = default(MemberDetails);
				flag = false;
			}
			return flag;
		}

		public readonly string name;

		public readonly Type type;

		public readonly object value;

		public delegate void MemberDetailsVisitor(in MemberDrawContext context, in MemberDetails details);
	}
}
