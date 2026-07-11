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
				new Button.UxmlFactory(),
				new VisualElement.UxmlFactory(),
				new IMGUIContainer.UxmlFactory(),
				new Image.UxmlFactory(),
				new Label.UxmlFactory(),
				new RepeatButton.UxmlFactory(),
				new ScrollerButton.UxmlFactory(),
				new ScrollView.UxmlFactory(),
				new Scroller.UxmlFactory(),
				new Slider.UxmlFactory(),
				new SliderInt.UxmlFactory(),
				new MinMaxSlider.UxmlFactory(),
				new TextField.UxmlFactory(),
				new Toggle.UxmlFactory(),
				new VisualContainer.UxmlFactory(),
				new TemplateContainer.UxmlFactory(),
				new Box.UxmlFactory(),
				new PopupWindow.UxmlFactory(),
				new ListView.UxmlFactory(),
				new Foldout.UxmlFactory(),
				new BindableElement.UxmlFactory()
			};
			foreach (IUxmlFactory uxmlFactory in array)
			{
				VisualElementFactoryRegistry.RegisterFactory(uxmlFactory);
			}
		}
	}
}
