using System;
using System.Collections.Generic;

namespace Harmony
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class HarmonyPatch : HarmonyAttribute
	{
		public HarmonyPatch()
		{
		}

		public HarmonyPatch(Type declaringType)
		{
			this.info.declaringType = declaringType;
		}

		public HarmonyPatch(Type declaringType, Type[] argumentTypes)
		{
			this.info.declaringType = declaringType;
			this.info.argumentTypes = argumentTypes;
		}

		public HarmonyPatch(Type declaringType, string methodName)
		{
			this.info.declaringType = declaringType;
			this.info.methodName = methodName;
		}

		public HarmonyPatch(Type declaringType, string methodName, params Type[] argumentTypes)
		{
			this.info.declaringType = declaringType;
			this.info.methodName = methodName;
			this.info.argumentTypes = argumentTypes;
		}

		public HarmonyPatch(Type declaringType, string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations)
		{
			this.info.declaringType = declaringType;
			this.info.methodName = methodName;
			this.ParseSpecialArguments(argumentTypes, argumentVariations);
		}

		public HarmonyPatch(Type declaringType, MethodType methodType)
		{
			this.info.declaringType = declaringType;
			this.info.methodType = new MethodType?(methodType);
		}

		public HarmonyPatch(Type declaringType, MethodType methodType, params Type[] argumentTypes)
		{
			this.info.declaringType = declaringType;
			this.info.methodType = new MethodType?(methodType);
			this.info.argumentTypes = argumentTypes;
		}

		public HarmonyPatch(Type declaringType, MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations)
		{
			this.info.declaringType = declaringType;
			this.info.methodType = new MethodType?(methodType);
			this.ParseSpecialArguments(argumentTypes, argumentVariations);
		}

		public HarmonyPatch(Type declaringType, string propertyName, MethodType methodType)
		{
			this.info.declaringType = declaringType;
			this.info.methodName = propertyName;
			this.info.methodType = new MethodType?(methodType);
		}

		public HarmonyPatch(string methodName)
		{
			this.info.methodName = methodName;
		}

		public HarmonyPatch(string methodName, params Type[] argumentTypes)
		{
			this.info.methodName = methodName;
			this.info.argumentTypes = argumentTypes;
		}

		public HarmonyPatch(string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations)
		{
			this.info.methodName = methodName;
			this.ParseSpecialArguments(argumentTypes, argumentVariations);
		}

		public HarmonyPatch(string propertyName, MethodType methodType)
		{
			this.info.methodName = propertyName;
			this.info.methodType = new MethodType?(methodType);
		}

		public HarmonyPatch(MethodType methodType)
		{
			this.info.methodType = new MethodType?(methodType);
		}

		public HarmonyPatch(MethodType methodType, params Type[] argumentTypes)
		{
			this.info.methodType = new MethodType?(methodType);
			this.info.argumentTypes = argumentTypes;
		}

		public HarmonyPatch(MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations)
		{
			this.info.methodType = new MethodType?(methodType);
			this.ParseSpecialArguments(argumentTypes, argumentVariations);
		}

		public HarmonyPatch(Type[] argumentTypes)
		{
			this.info.argumentTypes = argumentTypes;
		}

		public HarmonyPatch(Type[] argumentTypes, ArgumentType[] argumentVariations)
		{
			this.ParseSpecialArguments(argumentTypes, argumentVariations);
		}

		[Obsolete("This attribute will be removed in the next major version. Use HarmonyPatch together with MethodType.Getter or MethodType.Setter instead")]
		public HarmonyPatch(string propertyName, PropertyMethod type)
		{
			this.info.methodName = propertyName;
			this.info.methodType = new MethodType?((type == PropertyMethod.Getter) ? MethodType.Getter : MethodType.Setter);
		}

		private void ParseSpecialArguments(Type[] argumentTypes, ArgumentType[] argumentVariations)
		{
			bool flag = argumentVariations == null || argumentVariations.Length == 0;
			if (flag)
			{
				this.info.argumentTypes = argumentTypes;
			}
			else
			{
				bool flag2 = argumentTypes.Length < argumentVariations.Length;
				if (flag2)
				{
					throw new ArgumentException("argumentVariations contains more elements than argumentTypes", "argumentVariations");
				}
				List<Type> list = new List<Type>();
				for (int i = 0; i < argumentTypes.Length; i++)
				{
					Type type = argumentTypes[i];
					ArgumentType argumentType = argumentVariations[i];
					if (argumentType - ArgumentType.Ref > 1)
					{
						if (argumentType == ArgumentType.Pointer)
						{
							type = type.MakePointerType();
						}
					}
					else
					{
						type = type.MakeByRefType();
					}
					list.Add(type);
				}
				this.info.argumentTypes = list.ToArray();
			}
		}
	}
}
