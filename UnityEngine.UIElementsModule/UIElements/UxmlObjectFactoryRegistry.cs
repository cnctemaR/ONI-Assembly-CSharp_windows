using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.UIElements
{
	internal class UxmlObjectFactoryRegistry
	{
		internal static Dictionary<string, List<IBaseUxmlObjectFactory>> factories
		{
			get
			{
				bool flag = UxmlObjectFactoryRegistry.s_Factories == null;
				if (flag)
				{
					UxmlObjectFactoryRegistry.s_Factories = new Dictionary<string, List<IBaseUxmlObjectFactory>>();
					UxmlObjectFactoryRegistry.RegisterEngineFactories();
					UxmlObjectFactoryRegistry.RegisterUserFactories();
				}
				return UxmlObjectFactoryRegistry.s_Factories;
			}
		}

		protected static void RegisterFactory(IBaseUxmlObjectFactory factory)
		{
			List<IBaseUxmlObjectFactory> list;
			bool flag = UxmlObjectFactoryRegistry.factories.TryGetValue(factory.uxmlQualifiedName, out list);
			if (flag)
			{
				foreach (IBaseUxmlObjectFactory baseUxmlObjectFactory in list)
				{
					bool flag2 = baseUxmlObjectFactory.GetType() == factory.GetType();
					if (flag2)
					{
						throw new ArgumentException("A factory for the type " + factory.GetType().FullName + " was already registered");
					}
				}
				list.Add(factory);
			}
			else
			{
				list = new List<IBaseUxmlObjectFactory> { factory };
				UxmlObjectFactoryRegistry.factories.Add(factory.uxmlQualifiedName, list);
			}
		}

		internal static bool TryGetFactories(string fullTypeName, out List<IBaseUxmlObjectFactory> factoryList)
		{
			return UxmlObjectFactoryRegistry.factories.TryGetValue(fullTypeName, out factoryList);
		}

		private static void RegisterEngineFactories()
		{
			IBaseUxmlObjectFactory[] array = new IBaseUxmlObjectFactory[]
			{
				new Columns.UxmlObjectFactory<Columns>(),
				new Column.UxmlObjectFactory<Column>(),
				new SortColumnDescriptions.UxmlObjectFactory<SortColumnDescriptions>(),
				new SortColumnDescription.UxmlObjectFactory<SortColumnDescription>()
			};
			foreach (IBaseUxmlObjectFactory baseUxmlObjectFactory in array)
			{
				UxmlObjectFactoryRegistry.RegisterFactory(baseUxmlObjectFactory);
			}
		}

		private static void RegisterUserFactories()
		{
			HashSet<string> hashSet = new HashSet<string>(ScriptingRuntime.GetAllUserAssemblies());
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				bool flag = !hashSet.Contains(assembly.GetName().Name + ".dll") || assembly.GetName().Name == "UnityEngine.UIElementsModule";
				if (!flag)
				{
					Type[] types = assembly.GetTypes();
					foreach (Type type in types)
					{
						bool flag2 = !typeof(IBaseUxmlObjectFactory).IsAssignableFrom(type) || type.IsInterface || type.IsAbstract || type.IsGenericType;
						if (!flag2)
						{
							IBaseUxmlObjectFactory baseUxmlObjectFactory = (IBaseUxmlObjectFactory)Activator.CreateInstance(type);
							UxmlObjectFactoryRegistry.RegisterFactory(baseUxmlObjectFactory);
						}
					}
				}
			}
		}

		private static Dictionary<string, List<IBaseUxmlObjectFactory>> s_Factories;
	}
}
