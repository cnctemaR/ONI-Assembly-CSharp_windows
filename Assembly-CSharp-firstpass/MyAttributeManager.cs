using System;
using System.Collections.Generic;
using System.Reflection;

public abstract class MyAttributeManager<T> : IAttributeManager where T : class
{
	public MyAttributeManager(Dictionary<Type, MethodInfo> attributeMap, Action<T> spawnFunc = null)
	{
		this.m_methodInfosByAttribute = attributeMap;
		this.m_spawnFunc = spawnFunc;
	}

	private void GetFieldDatas(List<MyAttributeManager<T>.FieldData> field_data_list, Type type)
	{
		foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
		{
			object[] customAttributes = fieldInfo.GetCustomAttributes(false);
			for (int j = 0; j < customAttributes.Length; j++)
			{
				Type type2 = customAttributes[j].GetType();
				if (this.IsFunctionAttribute(type2))
				{
					bool flag = true;
					using (List<MyAttributeManager<T>.FieldData>.Enumerator enumerator = field_data_list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.fieldInfo.Name == fieldInfo.Name)
							{
								flag = false;
								break;
							}
						}
					}
					if (flag)
					{
						field_data_list.Add(new MyAttributeManager<T>.FieldData
						{
							myAttributeType = type2,
							attrFns = this.GetAttrFns(fieldInfo.FieldType),
							fieldInfo = fieldInfo
						});
					}
				}
			}
		}
		Type baseType = type.BaseType;
		if (baseType != typeof(KMonoBehaviour) && baseType != typeof(object) && baseType != null)
		{
			this.GetFieldDatas(field_data_list, baseType);
		}
	}

	private MyAttributeManager<T>.FieldData[] GetFields(Type type)
	{
		if (this.m_typeFieldInfos == null)
		{
			this.m_typeFieldInfos = new Dictionary<Type, MyAttributeManager<T>.FieldData[]>();
		}
		MyAttributeManager<T>.FieldData[] array = null;
		if (!this.m_typeFieldInfos.TryGetValue(type, out array))
		{
			List<MyAttributeManager<T>.FieldData> list = new List<MyAttributeManager<T>.FieldData>();
			this.GetFieldDatas(list, type);
			array = list.ToArray();
			this.m_typeFieldInfos[type] = array;
		}
		return array;
	}

	public void OnAwake(object obj, KMonoBehaviour cmp)
	{
		Type type = obj.GetType();
		foreach (MyAttributeManager<T>.FieldData fieldData in this.GetFields(type))
		{
			MyAttributeManager<T>.AttrFns attrFns = fieldData.attrFns;
			FieldInfo fieldInfo = fieldData.fieldInfo;
			if ((T)((object)fieldInfo.GetValue(obj)) == null)
			{
				T t = attrFns.GetFunction(fieldData.myAttributeType)(cmp, false);
				fieldInfo.SetValue(obj, t);
			}
		}
	}

	public void OnStart(object obj, KMonoBehaviour cmp)
	{
		Type type = obj.GetType();
		foreach (MyAttributeManager<T>.FieldData fieldData in this.GetFields(type))
		{
			MyAttributeManager<T>.AttrFns attrFns = fieldData.attrFns;
			FieldInfo fieldInfo = fieldData.fieldInfo;
			T t = fieldInfo.GetValue(obj) as T;
			if (t != null)
			{
				if (this.m_spawnFunc != null)
				{
					this.m_spawnFunc(t);
				}
			}
			else
			{
				t = attrFns.GetFunction(fieldData.myAttributeType)(cmp, true);
				if (t != null && this.m_spawnFunc != null)
				{
					this.m_spawnFunc(t);
				}
				fieldInfo.SetValue(obj, t);
			}
		}
	}

	private bool IsFunctionAttribute(Type attribute)
	{
		foreach (KeyValuePair<Type, MethodInfo> keyValuePair in this.m_methodInfosByAttribute)
		{
			if (attribute == keyValuePair.Key)
			{
				return true;
			}
		}
		return false;
	}

	private MyAttributeManager<T>.AttrFns GetAttrFns(Type type)
	{
		MyAttributeManager<T>.AttrFns attrFns = null;
		if (!this.m_attrFns.TryGetValue(type, out attrFns))
		{
			attrFns = new MyAttributeManager<T>.AttrFns(type, this.m_methodInfosByAttribute);
			this.m_attrFns[type] = attrFns;
		}
		return attrFns;
	}

	private Dictionary<Type, MyAttributeManager<T>.FieldData[]> m_typeFieldInfos;

	private Action<T> m_spawnFunc;

	private Dictionary<Type, MethodInfo> m_methodInfosByAttribute = new Dictionary<Type, MethodInfo>();

	private Dictionary<Type, MyAttributeManager<T>.AttrFns> m_attrFns = new Dictionary<Type, MyAttributeManager<T>.AttrFns>();

	private class FieldData
	{
		public Type myAttributeType;

		public MyAttributeManager<T>.AttrFns attrFns;

		public FieldInfo fieldInfo;
	}

	private class AttrFns
	{
		public AttrFns(Type type, Dictionary<Type, MethodInfo> methodInfosByAttribute)
		{
			foreach (KeyValuePair<Type, MethodInfo> keyValuePair in methodInfosByAttribute)
			{
				MethodInfo methodInfo = null;
				try
				{
					methodInfo = keyValuePair.Value.MakeGenericMethod(new Type[] { type });
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Format("Exception for type {0}: {1}", type, ex));
				}
				Func<KMonoBehaviour, bool, T> func = (Func<KMonoBehaviour, bool, T>)Delegate.CreateDelegate(typeof(Func<KMonoBehaviour, bool, T>), methodInfo);
				this.m_fnsByAttribute[keyValuePair.Key] = func;
			}
		}

		public Func<KMonoBehaviour, bool, T> GetFunction(Type attribute)
		{
			return this.m_fnsByAttribute[attribute];
		}

		private Dictionary<Type, Func<KMonoBehaviour, bool, T>> m_fnsByAttribute = new Dictionary<Type, Func<KMonoBehaviour, bool, T>>();
	}
}
