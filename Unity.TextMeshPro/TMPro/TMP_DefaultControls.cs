using System;
using UnityEngine;
using UnityEngine.UI;

namespace TMPro
{
	public static class TMP_DefaultControls
	{
		private static GameObject CreateUIElementRoot(string name, Vector2 size)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.AddComponent<RectTransform>().sizeDelta = size;
			return gameObject;
		}

		private static GameObject CreateUIObject(string name, GameObject parent)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.AddComponent<RectTransform>();
			TMP_DefaultControls.SetParentAndAlign(gameObject, parent);
			return gameObject;
		}

		private static void SetDefaultTextValues(TMP_Text lbl)
		{
			lbl.color = TMP_DefaultControls.s_TextColor;
			lbl.fontSize = 14f;
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
			TMP_DefaultControls.SetLayerRecursively(child, parent.layer);
		}

		private static void SetLayerRecursively(GameObject go, int layer)
		{
			go.layer = layer;
			Transform transform = go.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				TMP_DefaultControls.SetLayerRecursively(transform.GetChild(i).gameObject, layer);
			}
		}

		public static GameObject CreateScrollbar(TMP_DefaultControls.Resources resources)
		{
			GameObject gameObject = TMP_DefaultControls.CreateUIElementRoot("Scrollbar", TMP_DefaultControls.s_ThinElementSize);
			GameObject gameObject2 = TMP_DefaultControls.CreateUIObject("Sliding Area", gameObject);
			GameObject gameObject3 = TMP_DefaultControls.CreateUIObject("Handle", gameObject2);
			Image image = TMP_DefaultControls.AddComponent<Image>(gameObject);
			image.sprite = resources.background;
			image.type = Image.Type.Sliced;
			image.color = TMP_DefaultControls.s_DefaultSelectableColor;
			Image image2 = TMP_DefaultControls.AddComponent<Image>(gameObject3);
			image2.sprite = resources.standard;
			image2.type = Image.Type.Sliced;
			image2.color = TMP_DefaultControls.s_DefaultSelectableColor;
			RectTransform component = gameObject2.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(-20f, -20f);
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.one;
			RectTransform component2 = gameObject3.GetComponent<RectTransform>();
			component2.sizeDelta = new Vector2(20f, 20f);
			Scrollbar scrollbar = TMP_DefaultControls.AddComponent<Scrollbar>(gameObject);
			scrollbar.handleRect = component2;
			scrollbar.targetGraphic = image2;
			TMP_DefaultControls.SetDefaultColorTransitionValues(scrollbar);
			return gameObject;
		}

		public static GameObject CreateButton(TMP_DefaultControls.Resources resources)
		{
			GameObject gameObject = TMP_DefaultControls.CreateUIElementRoot("Button", TMP_DefaultControls.s_ThickElementSize);
			GameObject gameObject2 = new GameObject("Text (TMP)");
			gameObject2.AddComponent<RectTransform>();
			TMP_DefaultControls.SetParentAndAlign(gameObject2, gameObject);
			Image image = TMP_DefaultControls.AddComponent<Image>(gameObject);
			image.sprite = resources.standard;
			image.type = Image.Type.Sliced;
			image.color = TMP_DefaultControls.s_DefaultSelectableColor;
			TMP_DefaultControls.SetDefaultColorTransitionValues(TMP_DefaultControls.AddComponent<Button>(gameObject));
			TextMeshProUGUI textMeshProUGUI = TMP_DefaultControls.AddComponent<TextMeshProUGUI>(gameObject2);
			textMeshProUGUI.text = "Button";
			textMeshProUGUI.alignment = TextAlignmentOptions.Center;
			TMP_DefaultControls.SetDefaultTextValues(textMeshProUGUI);
			RectTransform component = gameObject2.GetComponent<RectTransform>();
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.one;
			component.sizeDelta = Vector2.zero;
			return gameObject;
		}

		public static GameObject CreateText(TMP_DefaultControls.Resources resources)
		{
			GameObject gameObject = TMP_DefaultControls.CreateUIElementRoot("Text (TMP)", TMP_DefaultControls.s_TextElementSize);
			gameObject.AddComponent<TextMeshProUGUI>();
			return gameObject;
		}

		public static GameObject CreateInputField(TMP_DefaultControls.Resources resources)
		{
			GameObject gameObject = TMP_DefaultControls.CreateUIElementRoot("InputField (TMP)", TMP_DefaultControls.s_ThickElementSize);
			GameObject gameObject2 = TMP_DefaultControls.CreateUIObject("Text Area", gameObject);
			GameObject gameObject3 = TMP_DefaultControls.CreateUIObject("Placeholder", gameObject2);
			GameObject gameObject4 = TMP_DefaultControls.CreateUIObject("Text", gameObject2);
			Image image = TMP_DefaultControls.AddComponent<Image>(gameObject);
			image.sprite = resources.inputField;
			image.type = Image.Type.Sliced;
			image.color = TMP_DefaultControls.s_DefaultSelectableColor;
			TMP_InputField tmp_InputField = TMP_DefaultControls.AddComponent<TMP_InputField>(gameObject);
			TMP_DefaultControls.SetDefaultColorTransitionValues(tmp_InputField);
			TMP_DefaultControls.AddComponent<RectMask2D>(gameObject2).padding = new Vector4(-8f, -5f, -8f, -5f);
			RectTransform component = gameObject2.GetComponent<RectTransform>();
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.one;
			component.sizeDelta = Vector2.zero;
			component.offsetMin = new Vector2(10f, 6f);
			component.offsetMax = new Vector2(-10f, -7f);
			TextMeshProUGUI textMeshProUGUI = TMP_DefaultControls.AddComponent<TextMeshProUGUI>(gameObject4);
			textMeshProUGUI.text = "";
			textMeshProUGUI.textWrappingMode = TextWrappingModes.NoWrap;
			textMeshProUGUI.extraPadding = true;
			textMeshProUGUI.richText = true;
			TMP_DefaultControls.SetDefaultTextValues(textMeshProUGUI);
			TextMeshProUGUI textMeshProUGUI2 = TMP_DefaultControls.AddComponent<TextMeshProUGUI>(gameObject3);
			textMeshProUGUI2.text = "Enter text...";
			textMeshProUGUI2.fontSize = 14f;
			textMeshProUGUI2.fontStyle = FontStyles.Italic;
			textMeshProUGUI2.textWrappingMode = TextWrappingModes.NoWrap;
			textMeshProUGUI2.extraPadding = true;
			Color color = textMeshProUGUI.color;
			color.a *= 0.5f;
			textMeshProUGUI2.color = color;
			TMP_DefaultControls.AddComponent<LayoutElement>(textMeshProUGUI2.gameObject).ignoreLayout = true;
			RectTransform component2 = gameObject4.GetComponent<RectTransform>();
			component2.anchorMin = Vector2.zero;
			component2.anchorMax = Vector2.one;
			component2.sizeDelta = Vector2.zero;
			component2.offsetMin = new Vector2(0f, 0f);
			component2.offsetMax = new Vector2(0f, 0f);
			RectTransform component3 = gameObject3.GetComponent<RectTransform>();
			component3.anchorMin = Vector2.zero;
			component3.anchorMax = Vector2.one;
			component3.sizeDelta = Vector2.zero;
			component3.offsetMin = new Vector2(0f, 0f);
			component3.offsetMax = new Vector2(0f, 0f);
			tmp_InputField.textViewport = component;
			tmp_InputField.textComponent = textMeshProUGUI;
			tmp_InputField.placeholder = textMeshProUGUI2;
			tmp_InputField.fontAsset = textMeshProUGUI.font;
			return gameObject;
		}

		public static GameObject CreateDropdown(TMP_DefaultControls.Resources resources)
		{
			GameObject gameObject = TMP_DefaultControls.CreateUIElementRoot("Dropdown", TMP_DefaultControls.s_ThickElementSize);
			GameObject gameObject2 = TMP_DefaultControls.CreateUIObject("Label", gameObject);
			GameObject gameObject3 = TMP_DefaultControls.CreateUIObject("Arrow", gameObject);
			GameObject gameObject4 = TMP_DefaultControls.CreateUIObject("Template", gameObject);
			GameObject gameObject5 = TMP_DefaultControls.CreateUIObject("Viewport", gameObject4);
			GameObject gameObject6 = TMP_DefaultControls.CreateUIObject("Content", gameObject5);
			GameObject gameObject7 = TMP_DefaultControls.CreateUIObject("Item", gameObject6);
			GameObject gameObject8 = TMP_DefaultControls.CreateUIObject("Item Background", gameObject7);
			GameObject gameObject9 = TMP_DefaultControls.CreateUIObject("Item Checkmark", gameObject7);
			GameObject gameObject10 = TMP_DefaultControls.CreateUIObject("Item Label", gameObject7);
			GameObject gameObject11 = TMP_DefaultControls.CreateScrollbar(resources);
			gameObject11.name = "Scrollbar";
			TMP_DefaultControls.SetParentAndAlign(gameObject11, gameObject4);
			Scrollbar component = gameObject11.GetComponent<Scrollbar>();
			component.SetDirection(Scrollbar.Direction.BottomToTop, true);
			RectTransform component2 = gameObject11.GetComponent<RectTransform>();
			component2.anchorMin = Vector2.right;
			component2.anchorMax = Vector2.one;
			component2.pivot = Vector2.one;
			component2.sizeDelta = new Vector2(component2.sizeDelta.x, 0f);
			TextMeshProUGUI textMeshProUGUI = TMP_DefaultControls.AddComponent<TextMeshProUGUI>(gameObject10);
			TMP_DefaultControls.SetDefaultTextValues(textMeshProUGUI);
			textMeshProUGUI.alignment = TextAlignmentOptions.Left;
			Image image = TMP_DefaultControls.AddComponent<Image>(gameObject8);
			image.color = new Color32(245, 245, 245, byte.MaxValue);
			Image image2 = TMP_DefaultControls.AddComponent<Image>(gameObject9);
			image2.sprite = resources.checkmark;
			Toggle toggle = TMP_DefaultControls.AddComponent<Toggle>(gameObject7);
			toggle.targetGraphic = image;
			toggle.graphic = image2;
			toggle.isOn = true;
			Image image3 = TMP_DefaultControls.AddComponent<Image>(gameObject4);
			image3.sprite = resources.standard;
			image3.type = Image.Type.Sliced;
			ScrollRect scrollRect = TMP_DefaultControls.AddComponent<ScrollRect>(gameObject4);
			scrollRect.content = (RectTransform)gameObject6.transform;
			scrollRect.viewport = (RectTransform)gameObject5.transform;
			scrollRect.horizontal = false;
			scrollRect.movementType = ScrollRect.MovementType.Clamped;
			scrollRect.verticalScrollbar = component;
			scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
			scrollRect.verticalScrollbarSpacing = -3f;
			TMP_DefaultControls.AddComponent<Mask>(gameObject5).showMaskGraphic = false;
			Image image4 = TMP_DefaultControls.AddComponent<Image>(gameObject5);
			image4.sprite = resources.mask;
			image4.type = Image.Type.Sliced;
			TextMeshProUGUI textMeshProUGUI2 = TMP_DefaultControls.AddComponent<TextMeshProUGUI>(gameObject2);
			TMP_DefaultControls.SetDefaultTextValues(textMeshProUGUI2);
			textMeshProUGUI2.alignment = TextAlignmentOptions.Left;
			TMP_DefaultControls.AddComponent<Image>(gameObject3).sprite = resources.dropdown;
			Image image5 = TMP_DefaultControls.AddComponent<Image>(gameObject);
			image5.sprite = resources.standard;
			image5.color = TMP_DefaultControls.s_DefaultSelectableColor;
			image5.type = Image.Type.Sliced;
			TMP_Dropdown tmp_Dropdown = TMP_DefaultControls.AddComponent<TMP_Dropdown>(gameObject);
			tmp_Dropdown.targetGraphic = image5;
			TMP_DefaultControls.SetDefaultColorTransitionValues(tmp_Dropdown);
			tmp_Dropdown.template = gameObject4.GetComponent<RectTransform>();
			tmp_Dropdown.captionText = textMeshProUGUI2;
			tmp_Dropdown.itemText = textMeshProUGUI;
			textMeshProUGUI.text = "Option A";
			tmp_Dropdown.options.Add(new TMP_Dropdown.OptionData
			{
				text = "Option A"
			});
			tmp_Dropdown.options.Add(new TMP_Dropdown.OptionData
			{
				text = "Option B"
			});
			tmp_Dropdown.options.Add(new TMP_Dropdown.OptionData
			{
				text = "Option C"
			});
			tmp_Dropdown.RefreshShownValue();
			RectTransform component3 = gameObject2.GetComponent<RectTransform>();
			component3.anchorMin = Vector2.zero;
			component3.anchorMax = Vector2.one;
			component3.offsetMin = new Vector2(10f, 6f);
			component3.offsetMax = new Vector2(-25f, -7f);
			RectTransform component4 = gameObject3.GetComponent<RectTransform>();
			component4.anchorMin = new Vector2(1f, 0.5f);
			component4.anchorMax = new Vector2(1f, 0.5f);
			component4.sizeDelta = new Vector2(20f, 20f);
			component4.anchoredPosition = new Vector2(-15f, 0f);
			RectTransform component5 = gameObject4.GetComponent<RectTransform>();
			component5.anchorMin = new Vector2(0f, 0f);
			component5.anchorMax = new Vector2(1f, 0f);
			component5.pivot = new Vector2(0.5f, 1f);
			component5.anchoredPosition = new Vector2(0f, 2f);
			component5.sizeDelta = new Vector2(0f, 150f);
			RectTransform component6 = gameObject5.GetComponent<RectTransform>();
			component6.anchorMin = new Vector2(0f, 0f);
			component6.anchorMax = new Vector2(1f, 1f);
			component6.sizeDelta = new Vector2(-18f, 0f);
			component6.pivot = new Vector2(0f, 1f);
			RectTransform component7 = gameObject6.GetComponent<RectTransform>();
			component7.anchorMin = new Vector2(0f, 1f);
			component7.anchorMax = new Vector2(1f, 1f);
			component7.pivot = new Vector2(0.5f, 1f);
			component7.anchoredPosition = new Vector2(0f, 0f);
			component7.sizeDelta = new Vector2(0f, 28f);
			RectTransform component8 = gameObject7.GetComponent<RectTransform>();
			component8.anchorMin = new Vector2(0f, 0.5f);
			component8.anchorMax = new Vector2(1f, 0.5f);
			component8.sizeDelta = new Vector2(0f, 20f);
			RectTransform component9 = gameObject8.GetComponent<RectTransform>();
			component9.anchorMin = Vector2.zero;
			component9.anchorMax = Vector2.one;
			component9.sizeDelta = Vector2.zero;
			RectTransform component10 = gameObject9.GetComponent<RectTransform>();
			component10.anchorMin = new Vector2(0f, 0.5f);
			component10.anchorMax = new Vector2(0f, 0.5f);
			component10.sizeDelta = new Vector2(20f, 20f);
			component10.anchoredPosition = new Vector2(10f, 0f);
			RectTransform component11 = gameObject10.GetComponent<RectTransform>();
			component11.anchorMin = Vector2.zero;
			component11.anchorMax = Vector2.one;
			component11.offsetMin = new Vector2(20f, 1f);
			component11.offsetMax = new Vector2(-10f, -2f);
			gameObject4.SetActive(false);
			return gameObject;
		}

		private static T AddComponent<T>(GameObject go) where T : Component
		{
			return go.AddComponent<T>();
		}

		private const float kWidth = 160f;

		private const float kThickHeight = 30f;

		private const float kThinHeight = 20f;

		private static Vector2 s_TextElementSize = new Vector2(100f, 100f);

		private static Vector2 s_ThickElementSize = new Vector2(160f, 30f);

		private static Vector2 s_ThinElementSize = new Vector2(160f, 20f);

		private static Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);

		private static Color s_TextColor = new Color(0.19607843f, 0.19607843f, 0.19607843f, 1f);

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
