using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal class AttributeHelperEngine
	{
		[RequiredByNativeCode]
		private static Type GetParentTypeDisallowingMultipleInclusion(Type type)
		{
			Type type2 = null;
			while (type != null && type != typeof(MonoBehaviour))
			{
				if (Attribute.IsDefined(type, typeof(DisallowMultipleComponent)))
				{
					type2 = type;
				}
				type = type.BaseType;
			}
			return type2;
		}

		[RequiredByNativeCode]
		private static Type[] GetRequiredComponents(Type klass)
		{
			List<Type> list = null;
			while (klass != null && klass != typeof(MonoBehaviour))
			{
				RequireComponent[] array = (RequireComponent[])klass.GetCustomAttributes(typeof(RequireComponent), false);
				Type baseType = klass.BaseType;
				foreach (RequireComponent requireComponent in array)
				{
					if (list == null && array.Length == 1 && baseType == typeof(MonoBehaviour))
					{
						return new Type[] { requireComponent.m_Type0, requireComponent.m_Type1, requireComponent.m_Type2 };
					}
					if (list == null)
					{
						list = new List<Type>();
					}
					if (requireComponent.m_Type0 != null)
					{
						list.Add(requireComponent.m_Type0);
					}
					if (requireComponent.m_Type1 != null)
					{
						list.Add(requireComponent.m_Type1);
					}
					if (requireComponent.m_Type2 != null)
					{
						list.Add(requireComponent.m_Type2);
					}
				}
				klass = baseType;
			}
			if (list == null)
			{
				return null;
			}
			return list.ToArray();
		}

		private static int GetExecuteMode(Type klass)
		{
			object[] customAttributes = klass.GetCustomAttributes(typeof(ExecuteAlways), false);
			int num;
			if (customAttributes.Length != 0)
			{
				num = 2;
			}
			else
			{
				object[] customAttributes2 = klass.GetCustomAttributes(typeof(ExecuteInEditMode), false);
				if (customAttributes2.Length != 0)
				{
					num = 1;
				}
				else
				{
					num = 0;
				}
			}
			return num;
		}

		[RequiredByNativeCode]
		private static int CheckIsEditorScript(Type klass)
		{
			while (klass != null && klass != typeof(MonoBehaviour))
			{
				int executeMode = AttributeHelperEngine.GetExecuteMode(klass);
				if (executeMode > 0)
				{
					return executeMode;
				}
				klass = klass.BaseType;
			}
			return 0;
		}

		[RequiredByNativeCode]
		private static int GetDefaultExecutionOrderFor(Type klass)
		{
			DefaultExecutionOrder customAttributeOfType = AttributeHelperEngine.GetCustomAttributeOfType<DefaultExecutionOrder>(klass);
			int num;
			if (customAttributeOfType == null)
			{
				num = 0;
			}
			else
			{
				num = customAttributeOfType.order;
			}
			return num;
		}

		private static T GetCustomAttributeOfType<T>(Type klass) where T : Attribute
		{
			Type typeFromHandle = typeof(T);
			object[] customAttributes = klass.GetCustomAttributes(typeFromHandle, true);
			T t;
			if (customAttributes != null && customAttributes.Length != 0)
			{
				t = (T)((object)customAttributes[0]);
			}
			else
			{
				t = (T)((object)null);
			}
			return t;
		}
	}
}
