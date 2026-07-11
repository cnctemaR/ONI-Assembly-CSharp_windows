using System;
using System.Collections;
using System.Reflection;
using System.Security.Permissions;

namespace System.ComponentModel.Design.Serialization
{
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public sealed class InstanceDescriptor
	{
		public InstanceDescriptor(MemberInfo member, ICollection arguments)
			: this(member, arguments, true)
		{
		}

		public InstanceDescriptor(MemberInfo member, ICollection arguments, bool isComplete)
		{
			this.isComplete = isComplete;
			this.ValidateMember(member, arguments);
			this.member = member;
			this.arguments = arguments;
		}

		private void ValidateMember(MemberInfo member, ICollection arguments)
		{
			if (member == null)
			{
				return;
			}
			MemberTypes memberType = member.MemberType;
			switch (memberType)
			{
			case MemberTypes.Constructor:
			{
				ConstructorInfo constructorInfo = (ConstructorInfo)member;
				if (arguments == null && constructorInfo.GetParameters().Length != 0)
				{
					throw new ArgumentException("Invalid number of arguments for this constructor");
				}
				if (arguments.Count != constructorInfo.GetParameters().Length)
				{
					throw new ArgumentException("Invalid number of arguments for this constructor");
				}
				break;
			}
			default:
				if (memberType != MemberTypes.Method)
				{
					if (memberType == MemberTypes.Property)
					{
						PropertyInfo propertyInfo = (PropertyInfo)member;
						if (!propertyInfo.CanRead)
						{
							throw new ArgumentException("Parameter must be readable");
						}
						MethodInfo getMethod = propertyInfo.GetGetMethod();
						if (!getMethod.IsStatic)
						{
							throw new ArgumentException("Parameter must be static");
						}
					}
				}
				else
				{
					MethodInfo methodInfo = (MethodInfo)member;
					if (!methodInfo.IsStatic)
					{
						throw new ArgumentException("InstanceDescriptor only describes static (VB.Net: shared) members", "member");
					}
					if (arguments == null && methodInfo.GetParameters().Length != 0)
					{
						throw new ArgumentException("Invalid number of arguments for this method", "arguments");
					}
					if (arguments.Count != methodInfo.GetParameters().Length)
					{
						throw new ArgumentException("Invalid number of arguments for this method");
					}
				}
				break;
			case MemberTypes.Field:
			{
				FieldInfo fieldInfo = (FieldInfo)member;
				if (!fieldInfo.IsStatic)
				{
					throw new ArgumentException("Parameter must be static");
				}
				if (arguments != null && arguments.Count != 0)
				{
					throw new ArgumentException("Field members do not take any arguments");
				}
				break;
			}
			}
		}

		public ICollection Arguments
		{
			get
			{
				if (this.arguments == null)
				{
					return new object[0];
				}
				return this.arguments;
			}
		}

		public bool IsComplete
		{
			get
			{
				return this.isComplete;
			}
		}

		public MemberInfo MemberInfo
		{
			get
			{
				return this.member;
			}
		}

		public object Invoke()
		{
			if (this.member == null)
			{
				return null;
			}
			object[] array;
			if (this.arguments == null)
			{
				array = new object[0];
			}
			else
			{
				array = new object[this.arguments.Count];
				this.arguments.CopyTo(array, 0);
			}
			MemberTypes memberType = this.member.MemberType;
			switch (memberType)
			{
			case MemberTypes.Constructor:
			{
				ConstructorInfo constructorInfo = (ConstructorInfo)this.member;
				return constructorInfo.Invoke(array);
			}
			default:
			{
				if (memberType == MemberTypes.Method)
				{
					MethodInfo methodInfo = (MethodInfo)this.member;
					return methodInfo.Invoke(null, array);
				}
				if (memberType != MemberTypes.Property)
				{
					return null;
				}
				PropertyInfo propertyInfo = (PropertyInfo)this.member;
				return propertyInfo.GetValue(null, array);
			}
			case MemberTypes.Field:
			{
				FieldInfo fieldInfo = (FieldInfo)this.member;
				return fieldInfo.GetValue(null);
			}
			}
		}

		private MemberInfo member;

		private ICollection arguments;

		private bool isComplete;
	}
}
