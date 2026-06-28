using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MemorySnapshot
{
	public MemorySnapshot()
	{
		foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			MemorySnapshot.LoadStatics(assembly, this.statics);
		}
		foreach (FieldInfo fieldInfo in this.statics)
		{
			MemorySnapshot.CountField(fieldInfo, null, this.types, this.walked, this.fieldCounts, null, null, fieldInfo.DeclaringType);
		}
		foreach (global::UnityEngine.Object @object in global::UnityEngine.Object.FindObjectsOfType(typeof(global::UnityEngine.Object)))
		{
			MemorySnapshot.CountReference(@object.GetType(), @object, this.types, this.walked, this.fieldCounts, "Object." + @object.name, null, null, @object.GetType());
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

	public static void CountReference(Type reference_type, object obj, Dictionary<int, MemorySnapshot.TypeData> types, HashSet<object> walked, Dictionary<int, MemorySnapshot.FieldCount> field_counts, string field_name, Type parent_2, Type parent_1, Type parent_0)
	{
		if (MemorySnapshot.ShouldExclude(reference_type))
		{
			return;
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
				MemorySnapshot.HierarchyNode hierarchyNode = new MemorySnapshot.HierarchyNode(parent_0, parent_1, parent_2);
				int num = 0;
				typeData2.hierarchies.TryGetValue(hierarchyNode, out num);
				typeData2.hierarchies[hierarchyNode] = num + 1;
			}
			foreach (FieldInfo fieldInfo in typeData2.fields)
			{
				MemorySnapshot.CountField(fieldInfo, obj, types, walked, field_counts, parent_1, parent_0, fieldInfo.DeclaringType);
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
				foreach (object obj2 in collection)
				{
					MemorySnapshot.CountReference(type, obj2, types, walked, field_counts, field_name + ".Item", parent_1, parent_0, collection.GetType());
				}
			}
		}
	}

	public static void CountField(FieldInfo field, object obj, Dictionary<int, MemorySnapshot.TypeData> types, HashSet<object> walked, Dictionary<int, MemorySnapshot.FieldCount> field_counts, Type parent_2, Type parent_1, Type parent_0)
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
				string text = field.DeclaringType.FullName + "." + field.Name;
				MemorySnapshot.CountReference(field.FieldType, obj2, types, walked, field_counts, text, parent_1, parent_0, field.DeclaringType);
			}
			catch
			{
				obj2 = null;
				string text2 = field.DeclaringType.FullName + "." + field.Name;
				MemorySnapshot.CountReference(field.FieldType, obj2, types, walked, field_counts, text2, parent_1, parent_0, field.DeclaringType);
			}
		}
	}

	public static void LoadStatics(Assembly assembly, List<FieldInfo> statics)
	{
		foreach (Type type in assembly.GetTypes())
		{
			foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
			{
				if (fieldInfo.IsStatic)
				{
					statics.Add(fieldInfo);
				}
			}
		}
	}

	private static bool ShouldExclude(Type type)
	{
		return type.IsPrimitive || type.IsEnum || type == typeof(MemorySnapshot);
	}

	public Dictionary<int, MemorySnapshot.TypeData> types = new Dictionary<int, MemorySnapshot.TypeData>();

	public Dictionary<int, MemorySnapshot.FieldCount> fieldCounts = new Dictionary<int, MemorySnapshot.FieldCount>();

	public HashSet<object> walked = new HashSet<object>();

	public List<FieldInfo> statics = new List<FieldInfo>();

	public struct HierarchyNode
	{
		public HierarchyNode(Type parent_0, Type parent_1, Type parent_2)
		{
			this.parent0 = parent_0;
			this.parent1 = parent_1;
			this.parent2 = parent_2;
		}

		public bool Equals(MemorySnapshot.HierarchyNode a, MemorySnapshot.HierarchyNode b)
		{
			return a.parent0 == b.parent0 && a.parent1 == b.parent1 && a.parent2 == b.parent2;
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
			return num;
		}

		public override string ToString()
		{
			if (this.parent2 != null)
			{
				return string.Concat(new string[]
				{
					this.parent2.FullName,
					"--",
					this.parent1.FullName,
					"--",
					this.parent0.FullName
				});
			}
			if (this.parent1 != null)
			{
				return this.parent1.FullName + "--" + this.parent0.FullName;
			}
			return this.parent0.FullName;
		}

		public Type parent0;

		public Type parent1;

		public Type parent2;
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
	}
}
