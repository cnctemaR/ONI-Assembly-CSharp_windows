using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MyCmp
{
	private static void GetFieldDatas(List<MyCmp.FieldData> field_data_list, Type type)
	{
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo fieldInfo in fields)
		{
			foreach (object obj in fieldInfo.GetCustomAttributes(false))
			{
				bool flag = obj.GetType() == typeof(MyCmpAdd);
				bool flag2 = obj.GetType() == typeof(MyCmpReq);
				bool flag3 = obj.GetType() == typeof(MyCmpGet);
				if (flag || flag2 || flag3)
				{
					bool flag4 = true;
					foreach (MyCmp.FieldData fieldData in field_data_list)
					{
						if (fieldData.fieldInfo.Name == fieldInfo.Name)
						{
							flag4 = false;
							break;
						}
					}
					if (flag4)
					{
						MyCmp.FieldData fieldData2 = new MyCmp.FieldData();
						if (flag)
						{
							fieldData2.myCmpType = MyCmp.MyCmpType.Add;
						}
						else if (flag2)
						{
							fieldData2.myCmpType = MyCmp.MyCmpType.Req;
						}
						else if (flag3)
						{
							fieldData2.myCmpType = MyCmp.MyCmpType.Get;
						}
						fieldData2.cmpFns = CmpUtil.GetCmpFns(fieldInfo.FieldType);
						fieldData2.fieldInfo = fieldInfo;
						field_data_list.Add(fieldData2);
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
		Type type = c.GetType();
		MyCmp.FieldData[] fields = MyCmp.GetFields(type);
		foreach (MyCmp.FieldData fieldData in fields)
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
		MyCmp.FieldData[] fields = MyCmp.GetFields(type);
		foreach (MyCmp.FieldData fieldData in fields)
		{
			CmpFns cmpFns = fieldData.cmpFns;
			FieldInfo fieldInfo = fieldData.fieldInfo;
			if ((Component)fieldInfo.GetValue(c) != null)
			{
				Component component = fieldInfo.GetValue(c) as Component;
				Util.SpawnComponent(component);
			}
			else if (fieldData.myCmpType == MyCmp.MyCmpType.Add)
			{
				Component component2 = cmpFns.mFindOrAddFn(c);
				Util.SpawnComponent(component2);
			}
			else if (fieldData.myCmpType == MyCmp.MyCmpType.Req)
			{
				Component component3 = cmpFns.mRequireFn(c);
				DebugUtil.Assert(component3 != null, "The behaviour " + type.ToString() + " required but couldn't find a " + fieldInfo.FieldType.Name);
				Util.SpawnComponent(component3);
				fieldInfo.SetValue(c, component3);
			}
			else if (fieldData.myCmpType == MyCmp.MyCmpType.Get)
			{
				Component component4 = cmpFns.mFindFn(c);
				Util.SpawnComponent(component4);
				fieldInfo.SetValue(c, component4);
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
