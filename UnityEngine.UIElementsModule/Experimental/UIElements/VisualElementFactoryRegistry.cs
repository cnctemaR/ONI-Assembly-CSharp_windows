using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	internal static class VisualElementFactoryRegistry
	{
		internal static Dictionary<string, List<IUxmlFactory>> factories { get; private set; }

		internal static void RegisterFactory(IUxmlFactory factory)
		{
			VisualElementFactoryRegistry.DiscoverFactories();
			List<IUxmlFactory> list;
			if (VisualElementFactoryRegistry.factories.TryGetValue(factory.uxmlQualifiedName, out list))
			{
				foreach (IUxmlFactory uxmlFactory in list)
				{
					if (uxmlFactory.GetType() == factory.GetType())
					{
						throw new ArgumentException("A factory of this type was already registered");
					}
				}
				list.Add(factory);
			}
			else
			{
				list = new List<IUxmlFactory>();
				list.Add(factory);
				VisualElementFactoryRegistry.factories.Add(factory.uxmlQualifiedName, list);
			}
		}

		internal static void DiscoverFactories()
		{
			if (VisualElementFactoryRegistry.factories == null)
			{
				VisualElementFactoryRegistry.factories = new Dictionary<string, List<IUxmlFactory>>();
				VisualElementFactoryRegistry.RegisterEngineFactories();
			}
		}

		internal static bool TryGetValue(string fullTypeName, out List<IUxmlFactory> factoryList)
		{
			VisualElementFactoryRegistry.DiscoverFactories();
			factoryList = null;
			return VisualElementFactoryRegistry.factories != null && VisualElementFactoryRegistry.factories.TryGetValue(fullTypeName, out factoryList);
		}

		private static void RegisterEngineFactories()
		{
			IUxmlFactory[] array = new IUxmlFactory[]
			{
				new UxmlRootElementFactory(),
				new Button.ButtonFactory(),
				new VisualElement.VisualElementFactory(),
				new IMGUIContainer.IMGUIContainerFactory(),
				new Image.ImageFactory(),
				new Label.LabelFactory(),
				new RepeatButton.RepeatButtonFactory(),
				new ScrollerButton.ScrollerButtonFactory(),
				new ScrollView.ScrollViewFactory(),
				new Scroller.ScrollerFactory(),
				new Slider.SliderFactory(),
				new TextField.TextFieldFactory(),
				new Toggle.ToggleFactory(),
				new VisualContainer.VisualContainerFactory(),
				new TemplateContainer.TemplateContainerFactory(),
				new Box.BoxFactory(),
				new PopupWindow.PopupWindowFactory(),
				new ListView.ListViewFactory()
			};
			foreach (IUxmlFactory uxmlFactory in array)
			{
				VisualElementFactoryRegistry.RegisterFactory(uxmlFactory);
			}
		}
	}
}
