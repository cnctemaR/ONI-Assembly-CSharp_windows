using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class MemorySnapshot
{
	public MemorySnapshot()
	{
		foreach (Type type in App.GetCurrentDomainTypes())
		{
			foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
			{
				if (fieldInfo.IsStatic)
				{
					this.statics.Add(fieldInfo);
				}
			}
		}
		foreach (FieldInfo fieldInfo2 in this.statics)
		{
			MemorySnapshot.CountField(fieldInfo2, null, this.types, this.walked, this.fieldCounts, this.detailTypeCount, null, null, null, null, fieldInfo2.DeclaringType);
		}
		foreach (global::UnityEngine.Object @object in Resources.FindObjectsOfTypeAll(typeof(global::UnityEngine.Object)))
		{
			MemorySnapshot.CountReference(@object.GetType(), @object, this.types, this.walked, this.fieldCounts, this.detailTypeCount, "Object." + @object.name, null, null, null, null, @object.GetType());
		}
	}

	public static MemorySnapshot.TypeData GetTypeData(Type type, Dictionary<int, MemorySnapshot.TypeData> types)
	{
		int hashCode = type.GetHashCode();
		MemorySnapshot.TypeData typeData = null;
		if (!types.TryGetValue(hashCode, out typeData))
		{
			typeData = new MemorySnapshot.TypeData(type);
			types[hashCode] = typeData;
		}
		return typeData;
	}

	public static void IncrementFieldCount(Dictionary<int, MemorySnapshot.FieldCount> field_counts, string name)
	{
		int hashCode = name.GetHashCode();
		MemorySnapshot.FieldCount fieldCount = null;
		if (!field_counts.TryGetValue(hashCode, out fieldCount))
		{
			fieldCount = new MemorySnapshot.FieldCount();
			fieldCount.name = name;
			field_counts[hashCode] = fieldCount;
		}
		fieldCount.count++;
	}

	public static void CountReference(Type reference_type, object obj, Dictionary<int, MemorySnapshot.TypeData> types, HashSet<object> walked, Dictionary<int, MemorySnapshot.FieldCount> field_counts, Dictionary<string, MemorySnapshot.DetailInfo> detailTypeCount, string field_name, Type parent_4, Type parent_3, Type parent_2, Type parent_1, Type parent_0)
	{
		if (MemorySnapshot.ShouldExclude(reference_type))
		{
			return;
		}
		if (reference_type == MemorySnapshot.detailType)
		{
			string text;
			if (obj as global::UnityEngine.Object != null)
			{
				text = "\"" + ((global::UnityEngine.Object)obj).name;
			}
			else
			{
				text = "\"" + MemorySnapshot.detailTypeStr;
			}
			if (parent_0 != null)
			{
				text += "\",\"";
				text += parent_0.ToString();
			}
			if (parent_1 != null)
			{
				text = text + "\",\"" + parent_1.ToString();
			}
			if (parent_2 != null)
			{
				text = text + "\",\"" + parent_2.ToString();
			}
			if (parent_3 != null)
			{
				text = text + "\",\"" + parent_3.ToString();
			}
			if (parent_4 != null)
			{
				text = text + "\",\"" + parent_4.ToString();
			}
			text += "\"\n";
			MemorySnapshot.DetailInfo detailInfo;
			detailTypeCount.TryGetValue(text, out detailInfo);
			detailInfo.count++;
			if (typeof(Array).IsAssignableFrom(reference_type) && obj != null)
			{
				Array array = obj as Array;
				detailInfo.numArrayEntries += array.Length;
			}
			detailTypeCount[text] = detailInfo;
		}
		if (reference_type.IsClass)
		{
			MemorySnapshot.TypeData typeData = MemorySnapshot.GetTypeData(reference_type, types);
			typeData.refCount++;
			MemorySnapshot.IncrementFieldCount(field_counts, field_name);
		}
		if (obj != null && (!obj.GetType().IsClass || walked.Add(obj)))
		{
			MemorySnapshot.TypeData typeData2 = MemorySnapshot.GetTypeData(obj.GetType(), types);
			if (typeData2.type.IsClass)
			{
				typeData2.instanceCount++;
				if (typeof(Array).IsAssignableFrom(typeData2.type))
				{
					Array array2 = obj as Array;
					typeData2.numArrayEntries += array2.Length;
				}
				MemorySnapshot.HierarchyNode hierarchyNode = new MemorySnapshot.HierarchyNode(parent_0, parent_1, parent_2, parent_3, parent_4);
				int num = 0;
				typeData2.hierarchies.TryGetValue(hierarchyNode, out num);
				typeData2.hierarchies[hierarchyNode] = num + 1;
			}
			foreach (FieldInfo fieldInfo in typeData2.fields)
			{
				MemorySnapshot.CountField(fieldInfo, obj, types, walked, field_counts, detailTypeCount, parent_3, parent_2, parent_1, parent_0, fieldInfo.DeclaringType);
			}
			ICollection collection = obj as ICollection;
			if (collection != null)
			{
				Type type = typeof(object);
				if (collection.GetType().GetElementType() != null)
				{
					type = collection.GetType().GetElementType();
				}
				else if (collection.GetType().GetGenericArguments().Length > 0)
				{
					type = collection.GetType().GetGenericArguments()[0];
				}
				IEnumerator enumerator2 = collection.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						object obj2 = enumerator2.Current;
						MemorySnapshot.CountReference(type, obj2, types, walked, field_counts, detailTypeCount, field_name + ".Item", parent_3, parent_2, parent_1, parent_0, collection.GetType());
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = enumerator2 as IDisposable) != null)
					{
						disposable.Dispose();
					}
				}
			}
		}
	}

	public static void CountField(FieldInfo field, object obj, Dictionary<int, MemorySnapshot.TypeData> types, HashSet<object> walked, Dictionary<int, MemorySnapshot.FieldCount> field_counts, Dictionary<string, MemorySnapshot.DetailInfo> detailTypeCount, Type parent_4, Type parent_3, Type parent_2, Type parent_1, Type parent_0)
	{
		if (!MemorySnapshot.ShouldExclude(field.FieldType))
		{
			object obj2 = null;
			try
			{
				if (!field.FieldType.Name.Contains("*"))
				{
					obj2 = field.GetValue(obj);
				}
				string text = field.DeclaringType.ToString() + "." + field.Name;
				MemorySnapshot.CountReference(field.FieldType, obj2, types, walked, field_counts, detailTypeCount, text, parent_3, parent_2, parent_1, parent_0, field.DeclaringType);
			}
			catch
			{
				obj2 = null;
				string text2 = field.DeclaringType.ToString() + "." + field.Name;
				MemorySnapshot.CountReference(field.FieldType, obj2, types, walked, field_counts, detailTypeCount, text2, parent_3, parent_2, parent_1, parent_0, field.DeclaringType);
			}
		}
	}

	private static bool ShouldExclude(Type type)
	{
		return type.IsPrimitive || type.IsEnum || type == typeof(MemorySnapshot);
	}

	public void WriteTypeDetails(MemorySnapshot compare)
	{
		List<KeyValuePair<string, MemorySnapshot.DetailInfo>> list = null;
		if (compare != null)
		{
			list = compare.detailTypeCount.ToList<KeyValuePair<string, MemorySnapshot.DetailInfo>>();
		}
		List<KeyValuePair<string, MemorySnapshot.DetailInfo>> list2 = this.detailTypeCount.ToList<KeyValuePair<string, MemorySnapshot.DetailInfo>>();
		list2.Sort((KeyValuePair<string, MemorySnapshot.DetailInfo> x, KeyValuePair<string, MemorySnapshot.DetailInfo> y) => y.Value.count - x.Value.count);
		using (StreamWriter streamWriter = new StreamWriter(GarbageProfiler.GetFileName("type_details_" + MemorySnapshot.detailTypeStr)))
		{
			streamWriter.WriteLine("Delta,Count,NumArrayEntries,Type");
			foreach (KeyValuePair<string, MemorySnapshot.DetailInfo> keyValuePair in list2)
			{
				int num = keyValuePair.Value.count;
				if (list != null)
				{
					foreach (KeyValuePair<string, MemorySnapshot.DetailInfo> keyValuePair2 in list)
					{
						if (keyValuePair2.Key == keyValuePair.Key)
						{
							num -= keyValuePair2.Value.count;
							break;
						}
					}
				}
				streamWriter.Write(string.Concat(new object[]
				{
					num,
					",",
					keyValuePair.Value.count,
					",",
					keyValuePair.Value.numArrayEntries,
					",",
					keyValuePair.Key
				}));
			}
		}
	}

	public Dictionary<int, MemorySnapshot.TypeData> types = new Dictionary<int, MemorySnapshot.TypeData>();

	public Dictionary<int, MemorySnapshot.FieldCount> fieldCounts = new Dictionary<int, MemorySnapshot.FieldCount>();

	public HashSet<object> walked = new HashSet<object>();

	public List<FieldInfo> statics = new List<FieldInfo>();

	public Dictionary<string, MemorySnapshot.DetailInfo> detailTypeCount = new Dictionary<string, MemorySnapshot.DetailInfo>();

	private static readonly Type detailType = typeof(byte[]);

	private static readonly string detailTypeStr = MemorySnapshot.detailType.ToString();

	public struct HierarchyNode
	{
		public HierarchyNode(Type parent_0, Type parent_1, Type parent_2, Type parent_3, Type parent_4)
		{
			this.parent0 = parent_0;
			this.parent1 = parent_1;
			this.parent2 = parent_2;
			this.parent3 = parent_3;
			this.parent4 = parent_4;
		}

		public bool Equals(MemorySnapshot.HierarchyNode a, MemorySnapshot.HierarchyNode b)
		{
			return a.parent0 == b.parent0 && a.parent1 == b.parent1 && a.parent2 == b.parent2 && a.parent3 == b.parent3 && a.parent4 == b.parent4;
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (this.parent0 != null)
			{
				num += this.parent0.GetHashCode();
			}
			if (this.parent1 != null)
			{
				num += this.parent1.GetHashCode();
			}
			if (this.parent2 != null)
			{
				num += this.parent2.GetHashCode();
			}
			if (this.parent3 != null)
			{
				num += this.parent3.GetHashCode();
			}
			if (this.parent4 != null)
			{
				num += this.parent4.GetHashCode();
			}
			return num;
		}

		public override string ToString()
		{
			if (this.parent4 != null)
			{
				return string.Concat(new string[]
				{
					this.parent4.ToString(),
					"--",
					this.parent3.ToString(),
					"--",
					this.parent2.ToString(),
					"--",
					this.parent1.ToString(),
					"--",
					this.parent0.ToString()
				});
			}
			if (this.parent3 != null)
			{
				return string.Concat(new string[]
				{
					this.parent3.ToString(),
					"--",
					this.parent2.ToString(),
					"--",
					this.parent1.ToString(),
					"--",
					this.parent0.ToString()
				});
			}
			if (this.parent2 != null)
			{
				return string.Concat(new string[]
				{
					this.parent2.ToString(),
					"--",
					this.parent1.ToString(),
					"--",
					this.parent0.ToString()
				});
			}
			if (this.parent1 != null)
			{
				return this.parent1.ToString() + "--" + this.parent0.ToString();
			}
			return this.parent0.ToString();
		}

		public Type parent0;

		public Type parent1;

		public Type parent2;

		public Type parent3;

		public Type parent4;
	}

	public class FieldCount
	{
		public string name;

		public int count;
	}

	public class TypeData
	{
		public TypeData(Type type)
		{
			this.type = type;
			this.fields = new List<FieldInfo>();
			this.instanceCount = 0;
			this.refCount = 0;
			this.numArrayEntries = 0;
			foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
			{
				if (!fieldInfo.IsStatic && !MemorySnapshot.ShouldExclude(fieldInfo.FieldType))
				{
					this.fields.Add(fieldInfo);
				}
			}
		}

		public Dictionary<MemorySnapshot.HierarchyNode, int> hierarchies = new Dictionary<MemorySnapshot.HierarchyNode, int>();

		public Type type;

		public List<FieldInfo> fields;

		public int instanceCount;

		public int refCount;

		public int numArrayEntries;
	}

	public struct DetailInfo
	{
		public int count;

		public int numArrayEntries;
	}
}
