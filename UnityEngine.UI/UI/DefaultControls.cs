using System;

namespace UnityEngine.UI
{
	public static class DefaultControls
	{
		public static DefaultControls.IFactoryControls factory
		{
			get
			{
				return DefaultControls.m_CurrentFactory;
			}
		}

		private static GameObject CreateUIElementRoot(string name, Vector2 size, params Type[] components)
		{
			GameObject gameObject = DefaultControls.factory.CreateGameObject(name, components);
			gameObject.GetComponent<RectTransform>().sizeDelta = size;
			return gameObject;
		}

		private static GameObject CreateUIObject(string name, GameObject parent, params Type[] components)
		{
			GameObject gameObject = DefaultControls.factory.CreateGameObject(name, components);
			DefaultControls.SetParentAndAlign(gameObject, parent);
			return gameObject;
		}

		private static void SetDefaultTextValues(Text lbl)
		{
			lbl.color = DefaultControls.s_TextColor;
			lbl.AssignDefaultFont();
		}

		private static void SetDefaultColorTransitionValues(Selectable slider)
		{
			ColorBlock colors = slider.colors;
			colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
			colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
			colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
		}

		private static void SetParentAndAlign(GameObject child, GameObject parent)
		{
			if (parent == null)
			{
				return;
			}
			child.transform.SetParent(parent.transform, false);
			DefaultControls.SetLayerRecursively(child, parent.layer);
		}

		private static void SetLayerRecursively(GameObject go, int layer)
		{
			go.layer = layer;
			Transform transform = go.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				DefaultControls.SetLayerRecursively(transform.GetChild(i).gameObject, layer);
			}
		}

		public static GameObject CreatePanel(DefaultControls.Resources resources)
		{
			GameObject gameObject = DefaultControls.CreateUIElementRoot("Panel", DefaultControls.s_ThickElementSize, new Type[] { typeof(Image) });
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.one;
			component.anchoredPosition = Vector2.zero;
			component.sizeDelta = Vector2.zero;
			Image component2 = gameObject.GetComponent<Image>();
			component2.sprite = resources.background;
			component2.type = Image.Type.Sliced;
			component2.color = DefaultControls.s_PanelColor;
			return gameObject;
		}

		public static GameObject CreateButton(DefaultControls.Resources resources)
		{
			GameObject gameObject = DefaultControls.CreateUIElementRoot("Button", DefaultControls.s_ThickElementSize, new Type[]
			{
				typeof(Image),
				typeof(Button)
			});
			GameObject gameObject2 = DefaultControls.CreateUIObject("Text", gameObject, new Type[] { typeof(Text) });
			Image component = gameObject.GetComponent<Image>();
			component.sprite = resources.standard;
			component.type = Image.Type.Sliced;
			component.color = DefaultControls.s_DefaultSelectableColor;
			DefaultControls.SetDefaultColorTransitionValues(gameObject.GetComponent<Button>());
			Text component2 = gameObject2.GetComponent<Text>();
			component2.text = "Button";
			component2.alignment = TextAnchor.MiddleCenter;
			DefaultControls.SetDefaultTextValues(component2);
			RectTransform component3 = gameObject2.GetComponent<RectTransform>();
			component3.anchorMin = Vector2.zero;
			component3.anchorMax = Vector2.one;
			component3.sizeDelta = Vector2.zero;
			return gameObject;
		}

		public static GameObject CreateText(DefaultControls.Resources resources)
		{
			GameObject gameObject = DefaultControls.CreateUIElementRoot("Text", DefaultControls.s_ThickElementSize, new Type[] { typeof(Text) });
			Text component = gameObject.GetComponent<Text>();
			component.text = "New Text";
			DefaultControls.SetDefaultTextValues(component);
			return gameObject;
		}

		public static GameObject CreateImage(DefaultControls.Resources resources)
		{
			return DefaultControls.CreateUIElementRoot("Image", DefaultControls.s_ImageElementSize, new Type[] { typeof(Image) });
		}

		public static GameObject CreateRawImage(DefaultControls.Resources resources)
		{
			return DefaultControls.CreateUIElementRoot("RawImage", DefaultControls.s_ImageElementSize, new Type[] { typeof(RawImage) });
		}

		public static GameObject CreateSlider(DefaultControls.Resources resources)
		{
			GameObject gameObject = DefaultControls.CreateUIElementRoot("Slider", DefaultControls.s_ThinElementSize, new Type[] { typeof(Slider) });
			GameObject gameObject2 = DefaultControls.CreateUIObject("Background", gameObject, new Type[] { typeof(Image) });
			GameObject gameObject3 = DefaultControls.CreateUIObject("Fill Area", gameObject, new Type[] { typeof(RectTransform) });
			GameObject gameObject4 = DefaultControls.CreateUIObject("Fill", gameObject3, new Type[] { typeof(Image) });
			GameObject gameObject5 = DefaultControls.CreateUIObject("Handle Slide Area", gameObject, new Type[] { typeof(RectTransform) });
			GameObject gameObject6 = DefaultControls.CreateUIObject("Handle", gameObject5, new Type[] { typeof(Image) });
			Image component = gameObject2.GetComponent<Image>();
			component.sprite = resources.background;
			component.type = Image.Type.Sliced;
			component.color = DefaultControls.s_DefaultSelectableColor;
			RectTransform component2 = gameObject2.GetComponent<RectTransform>();
			component2.anchorMin = new Vector2(0f, 0.25f);
			component2.anchorMax = new Vector2(1f, 0.75f);
			component2.sizeDelta = new Vector2(0f, 0f);
			RectTransform component3 = gameObject3.GetComponent<RectTransform>();
			component3.anchorMin = new Vector2(0f, 0.25f);
			component3.anchorMax = new Vector2(1f, 0.75f);
			component3.anchoredPosition = new Vector2(-5f, 0f);
			component3.sizeDelta = new Vector2(-20f, 0f);
			Image component4 = gameObject4.GetComponent<Image>();
			component4.sprite = resources.standard;
			component4.type = Image.Type.Sliced;
			component4.color = DefaultControls.s_DefaultSelectableColor;
			gameObject4.GetComponent<RectTransform>().sizeDelta = new Vector2(10f, 0f);
			RectTransform component5 = gameObject5.GetComponent<RectTransform>();
			component5.sizeDelta = new Vector2(-20f, 0f);
			component5.anchorMin = new Vector2(0f, 0f);
			component5.anchorMax = new Vector2(1f, 1f);
			Image component6 = gameObject6.GetComponent<Image>();
			component6.sprite = resources.knob;
			component6.color = DefaultControls.s_DefaultSelectableColor;
			gameObject6.GetComponent<RectTransform>().sizeDelta = new Vector2(20f, 0f);
			Slider component7 = gameObject.GetComponent<Slider>();
			component7.fillRect = gameObject4.GetComponent<RectTransform>();
			component7.handleRect = gameObject6.GetComponent<RectTransform>();
			component7.targetGraphic = component6;
			component7.direction = Slider.Direction.LeftToRight;
			DefaultControls.SetDefaultColorTransitionValues(component7);
			return gameObject;
		}

		public static GameObject CreateScrollbar(DefaultControls.Resources resources)
		{
			GameObject gameObject = DefaultControls.CreateUIElementRoot("Scrollbar", DefaultControls.s_ThinElementSize, new Type[]
			{
				typeof(Image),
				typeof(Scrollbar)
			});
			GameObject gameObject2 = DefaultControls.CreateUIObject("Sliding Area", gameObject, new Type[] { typeof(RectTransform) });
			GameObject gameObject3 = DefaultControls.CreateUIObject("Handle", gameObject2, new Type[] { typeof(Image) });
			Image component = gameObject.GetComponent<Image>();
			component.sprite = resources.background;
			component.type = Image.Type.Sliced;
			component.color = DefaultControls.s_DefaultSelectableColor;
			Image component2 = gameObject3.GetComponent<Image>();
			component2.sprite = resources.standard;
			component2.type = Image.Type.Sliced;
			component2.color = DefaultControls.s_DefaultSelectableColor;
			RectTransform component3 = gameObject2.GetComponent<RectTransform>();
			component3.sizeDelta = new Vector2(-20f, -20f);
			component3.anchorMin = Vector2.zero;
			component3.anchorMax = Vector2.one;
			RectTransform component4 = gameObject3.GetComponent<RectTransform>();
			component4.sizeDelta = new Vector2(20f, 20f);
			Scrollbar component5 = gameObject.GetComponent<Scrollbar>();
			component5.handleRect = component4;
			component5.targetGraphic = component2;
			DefaultControls.SetDefaultColorTransitionValues(component5);
			return gameObject;
		}

		public static GameObject CreateToggle(DefaultControls.Resources resources)
		{
			GameObject gameObject = DefaultControls.CreateUIElementRoot("Toggle", DefaultControls.s_ThinElementSize, new Type[] { typeof(Toggle) });
			GameObject gameObject2 = DefaultControls.CreateUIObject("Background", gameObject, new Type[] { typeof(Image) });
			GameObject gameObject3 = DefaultControls.CreateUIObject("Checkmark", gameObject2, new Type[] { typeof(Image) });
			GameObject gameObject4 = DefaultControls.CreateUIObject("Label", gameObject, new Type[] { typeof(Text) });
			Toggle component = gameObject.GetComponent<Toggle>();
			component.isOn = true;
			Image component2 = gameObject2.GetComponent<Image>();
			component2.sprite = resources.standard;
			component2.type = Image.Type.Sliced;
			component2.color = DefaultControls.s_DefaultSelectableColor;
			Image component3 = gameObject3.GetComponent<Image>();
			component3.sprite = resources.checkmark;
			Text component4 = gameObject4.GetComponent<Text>();
			component4.text = "Toggle";
			DefaultControls.SetDefaultTextValues(component4);
			component.graphic = component3;
			component.targetGraphic = component2;
			DefaultControls.SetDefaultColorTransitionValues(component);
			RectTransform component5 = gameObject2.GetComponent<RectTransform>();
			component5.anchorMin = new Vector2(0f, 1f);
			component5.anchorMax = new Vector2(0f, 1f);
			component5.anchoredPosition = new Vector2(10f, -10f);
			component5.sizeDelta = new Vector2(20f, 20f);
			RectTransform component6 = gameObject3.GetComponent<RectTransform>();
			component6.anchorMin = new Vector2(0.5f, 0.5f);
			component6.anchorMax = new Vector2(0.5f, 0.5f);
			component6.anchoredPosition = Vector2.zero;
			component6.sizeDelta = new Vector2(20f, 20f);
			RectTransform component7 = gameObject4.GetComponent<RectTransform>();
			component7.anchorMin = new Vector2(0f, 0f);
			component7.anchorMax = new Vector2(1f, 1f);
			component7.offsetMin = new Vector2(23f, 1f);
			component7.offsetMax = new Vector2(-5f, -2f);
			return gameObject;
		}

		public static GameObject CreateInputField(DefaultControls.Resources resources)
		{
			GameObject gameObject = DefaultControls.CreateUIElementRoot("InputField", DefaultControls.s_ThickElementSize, new Type[]
			{
				typeof(Image),
				typeof(InputField)
			});
			GameObject gameObject2 = DefaultControls.CreateUIObject("Placeholder", gameObject, new Type[] { typeof(Text) });
			GameObject gameObject3 = DefaultControls.CreateUIObject("Text", gameObject, new Type[] { typeof(Text) });
			Image component = gameObject.GetComponent<Image>();
			component.sprite = resources.inputField;
			component.type = Image.Type.Sliced;
			component.color = DefaultControls.s_DefaultSelectableColor;
			InputField component2 = gameObject.GetComponent<InputField>();
			DefaultControls.SetDefaultColorTransitionValues(component2);
			Text component3 = gameObject3.GetComponent<Text>();
			component3.text = "";
			component3.supportRichText = false;
			DefaultControls.SetDefaultTextValues(component3);
			Text component4 = gameObject2.GetComponent<Text>();
			component4.text = "Enter text...";
			component4.fontStyle = FontStyle.Italic;
			Color color = component3.color;
			color.a *= 0.5f;
			component4.color = color;
			RectTransform component5 = gameObject3.GetComponent<RectTransform>();
			component5.anchorMin = Vector2.zero;
			component5.anchorMax = Vector2.one;
			component5.sizeDelta = Vector2.zero;
			component5.offsetMin = new Vector2(10f, 6f);
			component5.offsetMax = new Vector2(-10f, -7f);
			RectTransform component6 = gameObject2.GetComponent<RectTransform>();
			component6.anchorMin = Vector2.zero;
			component6.anchorMax = Vector2.one;
			component6.sizeDelta = Vector2.zero;
			component6.offsetMin = new Vector2(10f, 6f);
			component6.offsetMax = new Vector2(-10f, -7f);
			component2.textComponent = component3;
			component2.placeholder = component4;
			return gameObject;
		}

		public static GameObject CreateDropdown(DefaultControls.Resources resources)
		{
			GameObject gameObject = DefaultControls.CreateUIElementRoot("Dropdown", DefaultControls.s_ThickElementSize, new Type[]
			{
				typeof(Image),
				typeof(Dropdown)
			});
			GameObject gameObject2 = DefaultControls.CreateUIObject("Label", gameObject, new Type[] { typeof(Text) });
			GameObject gameObject3 = DefaultControls.CreateUIObject("Arrow", gameObject, new Type[] { typeof(Image) });
			GameObject gameObject4 = DefaultControls.CreateUIObject("Template", gameObject, new Type[]
			{
				typeof(Image),
				typeof(ScrollRect)
			});
			GameObject gameObject5 = DefaultControls.CreateUIObject("Viewport", gameObject4, new Type[]
			{
				typeof(Image),
				typeof(Mask)
			});
			GameObject gameObject6 = DefaultControls.CreateUIObject("Content", gameObject5, new Type[] { typeof(RectTransform) });
			GameObject gameObject7 = DefaultControls.CreateUIObject("Item", gameObject6, new Type[] { typeof(Toggle) });
			GameObject gameObject8 = DefaultControls.CreateUIObject("Item Background", gameObject7, new Type[] { typeof(Image) });
			GameObject gameObject9 = DefaultControls.CreateUIObject("Item Checkmark", gameObject7, new Type[] { typeof(Image) });
			GameObject gameObject10 = DefaultControls.CreateUIObject("Item Label", gameObject7, new Type[] { typeof(Text) });
			GameObject gameObject11 = DefaultControls.CreateScrollbar(resources);
			gameObject11.name = "Scrollbar";
			DefaultControls.SetParentAndAlign(gameObject11, gameObject4);
			Scrollbar component = gameObject11.GetComponent<Scrollbar>();
			component.SetDirection(Scrollbar.Direction.BottomToTop, true);
			RectTransform component2 = gameObject11.GetComponent<RectTransform>();
			component2.anchorMin = Vector2.right;
			component2.anchorMax = Vector2.one;
			component2.pivot = Vector2.one;
			component2.sizeDelta = new Vector2(component2.sizeDelta.x, 0f);
			Text component3 = gameObject10.GetComponent<Text>();
			DefaultControls.SetDefaultTextValues(component3);
			component3.alignment = TextAnchor.MiddleLeft;
			Image component4 = gameObject8.GetComponent<Image>();
			component4.color = new Color32(245, 245, 245, byte.MaxValue);
			Image component5 = gameObject9.GetComponent<Image>();
			component5.sprite = resources.checkmark;
			Toggle component6 = gameObject7.GetComponent<Toggle>();
			component6.targetGraphic = component4;
			component6.graphic = component5;
			component6.isOn = true;
			Image component7 = gameObject4.GetComponent<Image>();
			component7.sprite = resources.standard;
			component7.type = Image.Type.Sliced;
			ScrollRect component8 = gameObject4.GetComponent<ScrollRect>();
			component8.content = gameObject6.GetComponent<RectTransform>();
			component8.viewport = gameObject5.GetComponent<RectTransform>();
			component8.horizontal = false;
			component8.movementType = ScrollRect.MovementType.Clamped;
			component8.verticalScrollbar = component;
			component8.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
			component8.verticalScrollbarSpacing = -3f;
			gameObject5.GetComponent<Mask>().showMaskGraphic = false;
			Image component9 = gameObject5.GetComponent<Image>();
			component9.sprite = resources.mask;
			component9.type = Image.Type.Sliced;
			Text component10 = gameObject2.GetComponent<Text>();
			DefaultControls.SetDefaultTextValues(component10);
			component10.alignment = TextAnchor.MiddleLeft;
			gameObject3.GetComponent<Image>().sprite = resources.dropdown;
			Image component11 = gameObject.GetComponent<Image>();
			component11.sprite = resources.standard;
			component11.color = DefaultControls.s_DefaultSelectableColor;
			component11.type = Image.Type.Sliced;
			Dropdown component12 = gameObject.GetComponent<Dropdown>();
			component12.targetGraphic = component11;
			DefaultControls.SetDefaultColorTransitionValues(component12);
			component12.template = gameObject4.GetComponent<RectTransform>();
			component12.captionText = component10;
			component12.itemText = component3;
			component3.text = "Option A";
			component12.options.Add(new Dropdown.OptionData
			{
				text = "Option A"
			});
			component12.options.Add(new Dropdown.OptionData
			{
				text = "Option B"
			});
			component12.options.Add(new Dropdown.OptionData
			{
				text = "Option C"
			});
			component12.RefreshShownValue();
			RectTransform component13 = gameObject2.GetComponent<RectTransform>();
			component13.anchorMin = Vector2.zero;
			component13.anchorMax = Vector2.one;
			component13.offsetMin = new Vector2(10f, 6f);
			component13.offsetMax = new Vector2(-25f, -7f);
			RectTransform component14 = gameObject3.GetComponent<RectTransform>();
			component14.anchorMin = new Vector2(1f, 0.5f);
			component14.anchorMax = new Vector2(1f, 0.5f);
			component14.sizeDelta = new Vector2(20f, 20f);
			component14.anchoredPosition = new Vector2(-15f, 0f);
			RectTransform component15 = gameObject4.GetComponent<RectTransform>();
			component15.anchorMin = new Vector2(0f, 0f);
			component15.anchorMax = new Vector2(1f, 0f);
			component15.pivot = new Vector2(0.5f, 1f);
			component15.anchoredPosition = new Vector2(0f, 2f);
			component15.sizeDelta = new Vector2(0f, 150f);
			RectTransform component16 = gameObject5.GetComponent<RectTransform>();
			component16.anchorMin = new Vector2(0f, 0f);
			component16.anchorMax = new Vector2(1f, 1f);
			component16.sizeDelta = new Vector2(-18f, 0f);
			component16.pivot = new Vector2(0f, 1f);
			RectTransform component17 = gameObject6.GetComponent<RectTransform>();
			component17.anchorMin = new Vector2(0f, 1f);
			component17.anchorMax = new Vector2(1f, 1f);
			component17.pivot = new Vector2(0.5f, 1f);
			component17.anchoredPosition = new Vector2(0f, 0f);
			component17.sizeDelta = new Vector2(0f, 28f);
			RectTransform component18 = gameObject7.GetComponent<RectTransform>();
			component18.anchorMin = new Vector2(0f, 0.5f);
			component18.anchorMax = new Vector2(1f, 0.5f);
			component18.sizeDelta = new Vector2(0f, 20f);
			RectTransform component19 = gameObject8.GetComponent<RectTransform>();
			component19.anchorMin = Vector2.zero;
			component19.anchorMax = Vector2.one;
			component19.sizeDelta = Vector2.zero;
			RectTransform component20 = gameObject9.GetComponent<RectTransform>();
			component20.anchorMin = new Vector2(0f, 0.5f);
			component20.anchorMax = new Vector2(0f, 0.5f);
			component20.sizeDelta = new Vector2(20f, 20f);
			component20.anchoredPosition = new Vector2(10f, 0f);
			RectTransform component21 = gameObject10.GetComponent<RectTransform>();
			component21.anchorMin = Vector2.zero;
			component21.anchorMax = Vector2.one;
			component21.offsetMin = new Vector2(20f, 1f);
			component21.offsetMax = new Vector2(-10f, -2f);
			gameObject4.SetActive(false);
			return gameObject;
		}

		public static GameObject CreateScrollView(DefaultControls.Resources resources)
		{
			GameObject gameObject = DefaultControls.CreateUIElementRoot("Scroll View", new Vector2(200f, 200f), new Type[]
			{
				typeof(Image),
				typeof(ScrollRect)
			});
			GameObject gameObject2 = DefaultControls.CreateUIObject("Viewport", gameObject, new Type[]
			{
				typeof(Image),
				typeof(Mask)
			});
			GameObject gameObject3 = DefaultControls.CreateUIObject("Content", gameObject2, new Type[] { typeof(RectTransform) });
			GameObject gameObject4 = DefaultControls.CreateScrollbar(resources);
			gameObject4.name = "Scrollbar Horizontal";
			DefaultControls.SetParentAndAlign(gameObject4, gameObject);
			RectTransform component = gameObject4.GetComponent<RectTransform>();
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.right;
			component.pivot = Vector2.zero;
			component.sizeDelta = new Vector2(0f, component.sizeDelta.y);
			GameObject gameObject5 = DefaultControls.CreateScrollbar(resources);
			gameObject5.name = "Scrollbar Vertical";
			DefaultControls.SetParentAndAlign(gameObject5, gameObject);
			gameObject5.GetComponent<Scrollbar>().SetDirection(Scrollbar.Direction.BottomToTop, true);
			RectTransform component2 = gameObject5.GetComponent<RectTransform>();
			component2.anchorMin = Vector2.right;
			component2.anchorMax = Vector2.one;
			component2.pivot = Vector2.one;
			component2.sizeDelta = new Vector2(component2.sizeDelta.x, 0f);
			RectTransform component3 = gameObject2.GetComponent<RectTransform>();
			component3.anchorMin = Vector2.zero;
			component3.anchorMax = Vector2.one;
			component3.sizeDelta = Vector2.zero;
			component3.pivot = Vector2.up;
			RectTransform component4 = gameObject3.GetComponent<RectTransform>();
			component4.anchorMin = Vector2.up;
			component4.anchorMax = Vector2.one;
			component4.sizeDelta = new Vector2(0f, 300f);
			component4.pivot = Vector2.up;
			ScrollRect component5 = gameObject.GetComponent<ScrollRect>();
			component5.content = component4;
			component5.viewport = component3;
			component5.horizontalScrollbar = gameObject4.GetComponent<Scrollbar>();
			component5.verticalScrollbar = gameObject5.GetComponent<Scrollbar>();
			component5.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
			component5.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
			component5.horizontalScrollbarSpacing = -3f;
			component5.verticalScrollbarSpacing = -3f;
			Image component6 = gameObject.GetComponent<Image>();
			component6.sprite = resources.background;
			component6.type = Image.Type.Sliced;
			component6.color = DefaultControls.s_PanelColor;
			gameObject2.GetComponent<Mask>().showMaskGraphic = false;
			Image component7 = gameObject2.GetComponent<Image>();
			component7.sprite = resources.mask;
			component7.type = Image.Type.Sliced;
			return gameObject;
		}

		private static DefaultControls.IFactoryControls m_CurrentFactory = DefaultControls.DefaultRuntimeFactory.Default;

		private const float kWidth = 160f;

		private const float kThickHeight = 30f;

		private const float kThinHeight = 20f;

		private static Vector2 s_ThickElementSize = new Vector2(160f, 30f);

		private static Vector2 s_ThinElementSize = new Vector2(160f, 20f);

		private static Vector2 s_ImageElementSize = new Vector2(100f, 100f);

		private static Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);

		private static Color s_PanelColor = new Color(1f, 1f, 1f, 0.392f);

		private static Color s_TextColor = new Color(0.19607843f, 0.19607843f, 0.19607843f, 1f);

		public interface IFactoryControls
		{
			GameObject CreateGameObject(string name, params Type[] components);
		}

		private class DefaultRuntimeFactory : DefaultControls.IFactoryControls
		{
			public GameObject CreateGameObject(string name, params Type[] components)
			{
				return new GameObject(name, components);
			}

			public static DefaultControls.IFactoryControls Default = new DefaultControls.DefaultRuntimeFactory();
		}

		public struct Resources
		{
			public Sprite standard;

			public Sprite background;

			public Sprite inputField;

			public Sprite knob;

			public Sprite checkmark;

			public Sprite dropdown;

			public Sprite mask;
		}
	}
}
