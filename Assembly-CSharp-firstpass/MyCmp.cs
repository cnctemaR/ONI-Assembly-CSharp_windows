using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MyCmp
{
	private static void GetFieldDatas(List<MyCmp.FieldData> field_data_list, Type type)
	{
		foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
		{
			foreach (object obj in fieldInfo.GetCustomAttributes(false))
			{
				bool flag = obj.GetType() == typeof(MyCmpAdd);
				bool flag2 = obj.GetType() == typeof(MyCmpReq);
				bool flag3 = obj.GetType() == typeof(MyCmpGet);
				if (flag || flag2 || flag3)
				{
					bool flag4 = true;
					using (List<MyCmp.FieldData>.Enumerator enumerator = field_data_list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.fieldInfo.Name == fieldInfo.Name)
							{
								flag4 = false;
								break;
							}
						}
					}
					if (flag4)
					{
						MyCmp.FieldData fieldData = new MyCmp.FieldData();
						if (flag)
						{
							fieldData.myCmpType = MyCmp.MyCmpType.Add;
						}
						else if (flag2)
						{
							fieldData.myCmpType = MyCmp.MyCmpType.Req;
						}
						else if (flag3)
						{
							fieldData.myCmpType = MyCmp.MyCmpType.Get;
						}
						fieldData.cmpFns = CmpUtil.GetCmpFns(fieldInfo.FieldType);
						fieldData.fieldInfo = fieldInfo;
						field_data_list.Add(fieldData);
					}
				}
			}
		}
		Type baseType = type.BaseType;
		if (baseType != typeof(KMonoBehaviour))
		{
			MyCmp.GetFieldDatas(field_data_list, baseType);
		}
	}

	public static MyCmp.FieldData[] GetFields(Type type)
	{
		if (MyCmp.typeFieldInfos == null)
		{
			MyCmp.typeFieldInfos = new Dictionary<Type, MyCmp.FieldData[]>();
		}
		MyCmp.FieldData[] array = null;
		if (!MyCmp.typeFieldInfos.TryGetValue(type, out array))
		{
			List<MyCmp.FieldData> list = new List<MyCmp.FieldData>();
			MyCmp.GetFieldDatas(list, type);
			array = list.ToArray();
			MyCmp.typeFieldInfos[type] = array;
		}
		return array;
	}

	public static void OnAwake(KMonoBehaviour c)
	{
		foreach (MyCmp.FieldData fieldData in MyCmp.GetFields(c.GetType()))
		{
			CmpFns cmpFns = fieldData.cmpFns;
			FieldInfo fieldInfo = fieldData.fieldInfo;
			if (!((Component)fieldInfo.GetValue(c) != null))
			{
				if (fieldData.myCmpType == MyCmp.MyCmpType.Add)
				{
					Component component = cmpFns.mFindOrAddFn(c);
					fieldInfo.SetValue(c, component);
				}
				else if (fieldData.myCmpType == MyCmp.MyCmpType.Req)
				{
					Component component2 = cmpFns.mFindFn(c);
					fieldInfo.SetValue(c, component2);
				}
				else if (fieldData.myCmpType == MyCmp.MyCmpType.Get)
				{
					Component component3 = cmpFns.mFindFn(c);
					fieldInfo.SetValue(c, component3);
				}
			}
		}
	}

	public static void OnStart(KMonoBehaviour c)
	{
		Type type = c.GetType();
		foreach (MyCmp.FieldData fieldData in MyCmp.GetFields(type))
		{
			CmpFns cmpFns = fieldData.cmpFns;
			FieldInfo fieldInfo = fieldData.fieldInfo;
			if ((Component)fieldInfo.GetValue(c) != null)
			{
				Util.SpawnComponent(fieldInfo.GetValue(c) as Component);
			}
			else if (fieldData.myCmpType == MyCmp.MyCmpType.Add)
			{
				Util.SpawnComponent(cmpFns.mFindOrAddFn(c));
			}
			else if (fieldData.myCmpType == MyCmp.MyCmpType.Req)
			{
				Component component = cmpFns.mRequireFn(c);
				if (component == null)
				{
					global::Debug.LogError("The behaviour " + type.ToString() + " required but couldn't find a " + fieldInfo.FieldType.Name);
				}
				Util.SpawnComponent(component);
				fieldInfo.SetValue(c, component);
			}
			else if (fieldData.myCmpType == MyCmp.MyCmpType.Get)
			{
				Component component2 = cmpFns.mFindFn(c);
				Util.SpawnComponent(component2);
				fieldInfo.SetValue(c, component2);
			}
		}
	}

	private static Dictionary<Type, MyCmp.FieldData[]> typeFieldInfos;

	public enum MyCmpType
	{
		Req,
		Add,
		Get
	}

	public class FieldData
	{
		public MyCmp.MyCmpType myCmpType;

		public CmpFns cmpFns;

		public FieldInfo fieldInfo;
	}
}
