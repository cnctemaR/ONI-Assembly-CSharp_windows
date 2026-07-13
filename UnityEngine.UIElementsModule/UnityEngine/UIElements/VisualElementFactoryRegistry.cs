using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class VisualElementFactoryRegistry
	{
		internal static string GetMovedUIControlTypeName(Type type, MovedFromAttribute attr)
		{
			bool flag = type == null;
			string text;
			if (flag)
			{
				text = string.Empty;
			}
			else
			{
				MovedFromAttributeData data = attr.data;
				string text2 = (data.nameSpaceHasChanged ? data.nameSpace : type.Namespace);
				string text3 = (data.classHasChanged ? data.className : type.Name);
				string text4 = text2 + "." + text3;
				text = text4;
			}
			return text;
		}

		internal static Dictionary<string, List<IUxmlFactory>> factories
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				bool flag = VisualElementFactoryRegistry.s_Factories == null;
				if (flag)
				{
					VisualElementFactoryRegistry.s_Factories = new Dictionary<string, List<IUxmlFactory>>();
					VisualElementFactoryRegistry.s_MovedTypesFactories = new Dictionary<string, List<IUxmlFactory>>(50);
					VisualElementFactoryRegistry.RegisterEngineFactories();
					VisualElementFactoryRegistry.RegisterUserFactories();
				}
				return VisualElementFactoryRegistry.s_Factories;
			}
		}

		protected static void RegisterFactory(IUxmlFactory factory)
		{
			List<IUxmlFactory> list;
			bool flag = VisualElementFactoryRegistry.factories.TryGetValue(factory.uxmlQualifiedName, out list);
			if (flag)
			{
				foreach (IUxmlFactory uxmlFactory in list)
				{
					bool flag2 = uxmlFactory.GetType() == factory.GetType();
					if (flag2)
					{
						throw new ArgumentException("A factory for the type " + factory.GetType().FullName + " was already registered");
					}
				}
				list.Add(factory);
			}
			else
			{
				list = new List<IUxmlFactory>();
				list.Add(factory);
				VisualElementFactoryRegistry.s_Factories.Add(factory.uxmlQualifiedName, list);
				Type uxmlType = factory.uxmlType;
				MovedFromAttribute movedFromAttribute = ((uxmlType != null) ? uxmlType.GetCustomAttribute<MovedFromAttribute>(false) : null);
				bool flag3 = movedFromAttribute != null && typeof(VisualElement).IsAssignableFrom(uxmlType);
				if (flag3)
				{
					string movedUIControlTypeName = VisualElementFactoryRegistry.GetMovedUIControlTypeName(uxmlType, movedFromAttribute);
					bool flag4 = !string.IsNullOrEmpty(movedUIControlTypeName);
					if (flag4)
					{
						VisualElementFactoryRegistry.s_MovedTypesFactories.Add(movedUIControlTypeName, list);
					}
				}
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static bool TryGetValue(string fullTypeName, out List<IUxmlFactory> factoryList)
		{
			bool flag = VisualElementFactoryRegistry.factories.TryGetValue(fullTypeName, out factoryList);
			bool flag2 = !flag;
			if (flag2)
			{
				flag = VisualElementFactoryRegistry.s_MovedTypesFactories.TryGetValue(fullTypeName, out factoryList);
			}
			return flag;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static bool TryGetValue(Type type, out List<IUxmlFactory> factoryList)
		{
			foreach (List<IUxmlFactory> list in VisualElementFactoryRegistry.factories.Values)
			{
				bool flag = list[0].uxmlType == type;
				if (flag)
				{
					factoryList = list;
					return true;
				}
			}
			factoryList = null;
			return false;
		}

		private static void RegisterEngineFactories()
		{
			IUxmlFactory[] array = new IUxmlFactory[]
			{
				new UxmlRootElementFactory(),
				new UxmlTemplateFactory(),
				new UxmlStyleFactory(),
				new UxmlAttributeOverridesFactory(),
				new Button.UxmlFactory(),
				new ToggleButtonGroup.UxmlFactory(),
				new VisualElement.UxmlFactory(),
				new IMGUIContainer.UxmlFactory(),
				new Image.UxmlFactory(),
				new Label.UxmlFactory(),
				new RepeatButton.UxmlFactory(),
				new ScrollView.UxmlFactory(),
				new Scroller.UxmlFactory(),
				new Slider.UxmlFactory(),
				new SliderInt.UxmlFactory(),
				new MinMaxSlider.UxmlFactory(),
				new GroupBox.UxmlFactory(),
				new RadioButton.UxmlFactory(),
				new RadioButtonGroup.UxmlFactory(),
				new Toggle.UxmlFactory(),
				new TextField.UxmlFactory(),
				new TemplateContainer.UxmlFactory(),
				new Box.UxmlFactory(),
				new EnumField.UxmlFactory(),
				new DropdownField.UxmlFactory(),
				new HelpBox.UxmlFactory(),
				new PopupWindow.UxmlFactory(),
				new ProgressBar.UxmlFactory(),
				new ListView.UxmlFactory(),
				new TwoPaneSplitView.UxmlFactory(),
				new TreeView.UxmlFactory(),
				new Foldout.UxmlFactory(),
				new MultiColumnListView.UxmlFactory(),
				new MultiColumnTreeView.UxmlFactory(),
				new BindableElement.UxmlFactory(),
				new TextElement.UxmlFactory(),
				new ButtonStripField.UxmlFactory(),
				new FloatField.UxmlFactory(),
				new DoubleField.UxmlFactory(),
				new Hash128Field.UxmlFactory(),
				new IntegerField.UxmlFactory(),
				new LongField.UxmlFactory(),
				new UnsignedIntegerField.UxmlFactory(),
				new UnsignedLongField.UxmlFactory(),
				new RectField.UxmlFactory(),
				new Vector2Field.UxmlFactory(),
				new RectIntField.UxmlFactory(),
				new Vector3Field.UxmlFactory(),
				new Vector4Field.UxmlFactory(),
				new Vector2IntField.UxmlFactory(),
				new Vector3IntField.UxmlFactory(),
				new BoundsField.UxmlFactory(),
				new BoundsIntField.UxmlFactory(),
				new Tab.UxmlFactory(),
				new TabView.UxmlFactory()
			};
			foreach (IUxmlFactory uxmlFactory in array)
			{
				VisualElementFactoryRegistry.RegisterFactory(uxmlFactory);
			}
		}

		internal static void RegisterUserFactories()
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
						bool flag2 = !typeof(IUxmlFactory).IsAssignableFrom(type) || type.IsInterface || type.IsAbstract || type.IsGenericType;
						if (!flag2)
						{
							IUxmlFactory uxmlFactory = (IUxmlFactory)Activator.CreateInstance(type);
							VisualElementFactoryRegistry.RegisterFactory(uxmlFactory);
						}
					}
				}
			}
		}

		private static Dictionary<string, List<IUxmlFactory>> s_Factories;

		private static Dictionary<string, List<IUxmlFactory>> s_MovedTypesFactories;
	}
}
