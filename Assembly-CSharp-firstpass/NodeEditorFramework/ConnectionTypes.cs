using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NodeEditorFramework
{
	public static class ConnectionTypes
	{
		private static Type NullType
		{
			get
			{
				return typeof(ConnectionTypes);
			}
		}

		public static Type GetType(string typeName)
		{
			return ConnectionTypes.GetTypeData(typeName).Type ?? ConnectionTypes.NullType;
		}

		public static TypeData GetTypeData(string typeName)
		{
			if (ConnectionTypes.types == null || ConnectionTypes.types.Count == 0)
			{
				ConnectionTypes.FetchTypes();
			}
			TypeData typeData;
			if (!ConnectionTypes.types.TryGetValue(typeName, out typeData))
			{
				Type type = Type.GetType(typeName);
				if (type == null)
				{
					typeData = ConnectionTypes.types.First<KeyValuePair<string, TypeData>>().Value;
					global::Debug.LogError("No TypeData defined for: " + typeName + " and type could not be found either");
				}
				else
				{
					typeData = ((ConnectionTypes.types.Values.Count > 0) ? ConnectionTypes.types.Values.First<TypeData>((TypeData data) => data.isValid() && data.Type == type) : null);
					if (typeData == null)
					{
						ConnectionTypes.types.Add(typeName, typeData = new TypeData(type));
					}
				}
			}
			return typeData;
		}

		public static TypeData GetTypeData(Type type)
		{
			if (ConnectionTypes.types == null || ConnectionTypes.types.Count == 0)
			{
				ConnectionTypes.FetchTypes();
			}
			TypeData typeData = ((ConnectionTypes.types.Values.Count > 0) ? ConnectionTypes.types.Values.First<TypeData>((TypeData data) => data.isValid() && data.Type == type) : null);
			if (typeData == null)
			{
				ConnectionTypes.types.Add(type.Name, typeData = new TypeData(type));
			}
			return typeData;
		}

		internal static void FetchTypes()
		{
			ConnectionTypes.types = new Dictionary<string, TypeData> { 
			{
				"None",
				new TypeData(typeof(object))
			} };
			IEnumerable<Assembly> enumerable = from assembly in AppDomain.CurrentDomain.GetAssemblies()
				where assembly.FullName.Contains("Assembly")
				select assembly;
			foreach (Assembly assembly2 in enumerable)
			{
				foreach (Type type in from T in assembly2.GetTypes()
					where T.IsClass && !T.IsAbstract && T.GetInterfaces().Contains(typeof(IConnectionTypeDeclaration))
					select T)
				{
					IConnectionTypeDeclaration connectionTypeDeclaration = assembly2.CreateInstance(type.FullName) as IConnectionTypeDeclaration;
					if (connectionTypeDeclaration == null)
					{
						throw new UnityException("Error with Type Declaration " + type.FullName);
					}
					ConnectionTypes.types.Add(connectionTypeDeclaration.Identifier, new TypeData(connectionTypeDeclaration));
				}
			}
		}

		private static Dictionary<string, TypeData> types;
	}
}
