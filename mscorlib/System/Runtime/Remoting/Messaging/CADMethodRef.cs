using System;
using System.Reflection;

namespace System.Runtime.Remoting.Messaging
{
	[Serializable]
	internal class CADMethodRef
	{
		private Type[] GetTypes(string[] typeArray)
		{
			Type[] array = new Type[typeArray.Length];
			for (int i = 0; i < typeArray.Length; i++)
			{
				array[i] = Type.GetType(typeArray[i], true);
			}
			return array;
		}

		public MethodBase Resolve()
		{
			Type type = Type.GetType(this.typeName, true);
			Type[] types = this.GetTypes(this.param_names);
			MethodBase methodBase;
			if (this.ctor)
			{
				methodBase = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, types, null);
			}
			else
			{
				methodBase = type.GetMethod(this.methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, types, null);
			}
			if (methodBase != null && this.generic_arg_names != null && !methodBase.IsGenericMethodDefinition)
			{
				methodBase = null;
			}
			if (methodBase != null && this.generic_arg_names != null)
			{
				methodBase = ((MethodInfo)methodBase).MakeGenericMethod(this.GetTypes(this.generic_arg_names));
			}
			if (methodBase == null && this.generic_arg_names != null)
			{
				foreach (MethodInfo methodInfo in type.GetMethods())
				{
					if (!(methodInfo.Name != this.methodName) && methodInfo.IsGenericMethodDefinition && methodInfo.GetGenericArguments().Length == this.generic_arg_names.Length)
					{
						methodBase = methodInfo.MakeGenericMethod(this.GetTypes(this.generic_arg_names));
						ParameterInfo[] parameters = methodBase.GetParameters();
						if (this.param_names.Length == parameters.Length)
						{
							for (int j = 0; j < parameters.Length; j++)
							{
								if (parameters[j].ParameterType.AssemblyQualifiedName != this.param_names[j])
								{
									methodBase = null;
									break;
								}
							}
							if (methodBase != null)
							{
								break;
							}
						}
					}
				}
			}
			if (methodBase == null)
			{
				throw new RemotingException(string.Concat(new string[] { "Method '", this.methodName, "' not found in type '", this.typeName, "'" }));
			}
			return methodBase;
		}

		public CADMethodRef(IMethodMessage msg)
		{
			MethodBase methodBase = msg.MethodBase;
			this.typeName = methodBase.DeclaringType.AssemblyQualifiedName;
			this.ctor = methodBase.IsConstructor;
			this.methodName = methodBase.Name;
			ParameterInfo[] parameters = methodBase.GetParameters();
			this.param_names = new string[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				this.param_names[i] = parameters[i].ParameterType.AssemblyQualifiedName;
			}
			if (!this.ctor && methodBase.IsGenericMethod)
			{
				Type[] genericArguments = methodBase.GetGenericArguments();
				this.generic_arg_names = new string[genericArguments.Length];
				for (int j = 0; j < genericArguments.Length; j++)
				{
					this.generic_arg_names[j] = genericArguments[j].AssemblyQualifiedName;
				}
			}
		}

		private bool ctor;

		private string typeName;

		private string methodName;

		private string[] param_names;

		private string[] generic_arg_names;
	}
}
