using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMPro
{
	[AddComponentMenu("UI (Canvas)/Dropdown - TextMeshPro", 35)]
	[RequireComponent(typeof(RectTransform))]
	public class TMP_Dropdown : Selectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler, ICancelHandler
	{
		public RectTransform template
		{
			get
			{
				return this.m_Template;
			}
			set
			{
				this.m_Template = value;
				this.RefreshShownValue();
			}
		}

		public TMP_Text captionText
		{
			get
			{
				return this.m_CaptionText;
			}
			set
			{
				this.m_CaptionText = value;
				this.RefreshShownValue();
			}
		}

		public Image captionImage
		{
			get
			{
				return this.m_CaptionImage;
			}
			set
			{
				this.m_CaptionImage = value;
				this.RefreshShownValue();
			}
		}

		public Graphic placeholder
		{
			get
			{
				return this.m_Placeholder;
			}
			set
			{
				this.m_Placeholder = value;
				this.RefreshShownValue();
			}
		}

		public TMP_Text itemText
		{
			get
			{
				return this.m_ItemText;
			}
			set
			{
				this.m_ItemText = value;
				this.RefreshShownValue();
			}
		}

		public Image itemImage
		{
			get
			{
				return this.m_ItemImage;
			}
			set
			{
				this.m_ItemImage = value;
				this.RefreshShownValue();
			}
		}

		public List<TMP_Dropdown.OptionData> options
		{
			get
			{
				return this.m_Options.options;
			}
			set
			{
				this.m_Options.options = value;
				this.RefreshShownValue();
			}
		}

		public TMP_Dropdown.DropdownEvent onValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				this.m_OnValueChanged = value;
			}
		}

		public float alphaFadeSpeed
		{
			get
			{
				return this.m_AlphaFadeSpeed;
			}
			set
			{
				this.m_AlphaFadeSpeed = value;
			}
		}

		public int value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.SetValue(value, true);
			}
		}

		public void SetValueWithoutNotify(int input)
		{
			this.SetValue(input, false);
		}

		private void SetValue(int value, bool sendCallback = true)
		{
			if (Application.isPlaying && (value == this.m_Value || this.options.Count == 0))
			{
				return;
			}
			if (this.m_MultiSelect)
			{
				this.m_Value = value;
			}
			else
			{
				this.m_Value = Mathf.Clamp(value, this.m_Placeholder ? (-1) : 0, this.options.Count - 1);
			}
			this.RefreshShownValue();
			if (sendCallback)
			{
				UISystemProfilerApi.AddMarker("Dropdown.value", this);
				this.m_OnValueChanged.Invoke(this.m_Value);
			}
		}

		public bool IsExpanded
		{
			get
			{
				return this.m_Dropdown != null;
			}
		}

		public bool MultiSelect
		{
			get
			{
				return this.m_MultiSelect;
			}
			set
			{
				this.m_MultiSelect = value;
			}
		}

		protected TMP_Dropdown()
		{
		}

		protected override void Awake()
		{
			if (this.m_CaptionImage)
			{
				this.m_CaptionImage.enabled = this.m_CaptionImage.sprite != null && this.m_CaptionImage.color.a > 0f;
			}
			if (this.m_Template)
			{
				this.m_Template.gameObject.SetActive(false);
			}
		}

		protected override void Start()
		{
			this.m_AlphaTweenRunner = new TweenRunner<FloatTween>();
			this.m_AlphaTweenRunner.Init(this);
			base.Start();
			this.RefreshShownValue();
		}

		protected override void OnDisable()
		{
			this.ImmediateDestroyDropdownList();
			if (this.m_Blocker != null)
			{
				this.DestroyBlocker(this.m_Blocker);
			}
			this.m_Blocker = null;
			base.OnDisable();
		}

		public void RefreshShownValue()
		{
			TMP_Dropdown.OptionData optionData = TMP_Dropdown.s_NoOptionData;
			if (this.options.Count > 0)
			{
				if (this.m_MultiSelect)
				{
					int num = TMP_Dropdown.FirstActiveFlagIndex(this.m_Value);
					if (this.m_Value == 0 || num >= this.options.Count)
					{
						optionData = TMP_Dropdown.k_NothingOption;
					}
					else if (TMP_Dropdown.IsEverythingValue(this.options.Count, this.m_Value))
					{
						optionData = TMP_Dropdown.k_EverythingOption;
					}
					else if (Mathf.IsPowerOfTwo(this.m_Value) && this.m_Value > 0)
					{
						optionData = this.options[num];
					}
					else
					{
						optionData = TMP_Dropdown.k_MixedOption;
					}
				}
				else if (this.m_Value >= 0)
				{
					optionData = this.options[Mathf.Clamp(this.m_Value, 0, this.options.Count - 1)];
				}
			}
			if (this.m_CaptionText)
			{
				if (optionData != null && optionData.text != null)
				{
					this.m_CaptionText.text = optionData.text;
				}
				else
				{
					this.m_CaptionText.text = "";
				}
			}
			if (this.m_CaptionImage)
			{
				this.m_CaptionImage.sprite = optionData.image;
				this.m_CaptionImage.color = optionData.color;
				this.m_CaptionImage.enabled = this.m_CaptionImage.sprite != null && this.m_CaptionImage.color.a > 0f;
			}
			if (this.m_Placeholder)
			{
				this.m_Placeholder.enabled = this.options.Count == 0 || this.m_Value == -1;
			}
		}

		public void AddOptions(List<TMP_Dropdown.OptionData> options)
		{
			this.options.AddRange(options);
			this.RefreshShownValue();
		}

		public void AddOptions(List<string> options)
		{
			for (int i = 0; i < options.Count; i++)
			{
				this.options.Add(new TMP_Dropdown.OptionData(options[i]));
			}
			this.RefreshShownValue();
		}

		public void AddOptions(List<Sprite> options)
		{
			for (int i = 0; i < options.Count; i++)
			{
				this.options.Add(new TMP_Dropdown.OptionData(options[i]));
			}
			this.RefreshShownValue();
		}

		public void ClearOptions()
		{
			this.options.Clear();
			this.m_Value = (this.m_Placeholder ? (-1) : 0);
			this.RefreshShownValue();
		}

		private void SetupTemplate()
		{
			this.validTemplate = false;
			if (!this.m_Template)
			{
				Debug.LogError("The dropdown template is not assigned. The template needs to be assigned and must have a child GameObject with a Toggle component serving as the item.", this);
				return;
			}
			GameObject gameObject = this.m_Template.gameObject;
			gameObject.SetActive(true);
			Toggle componentInChildren = this.m_Template.GetComponentInChildren<Toggle>();
			this.validTemplate = true;
			if (!componentInChildren || componentInChildren.transform == this.template)
			{
				this.validTemplate = false;
				Debug.LogError("The dropdown template is not valid. The template must have a child GameObject with a Toggle component serving as the item.", this.template);
			}
			else if (!(componentInChildren.transform.parent is RectTransform))
			{
				this.validTemplate = false;
				Debug.LogError("The dropdown template is not valid. The child GameObject with a Toggle component (the item) must have a RectTransform on its parent.", this.template);
			}
			else if (this.itemText != null && !this.itemText.transform.IsChildOf(componentInChildren.transform))
			{
				this.validTemplate = false;
				Debug.LogError("The dropdown template is not valid. The Item Text must be on the item GameObject or children of it.", this.template);
			}
			else if (this.itemImage != null && !this.itemImage.transform.IsChildOf(componentInChildren.transform))
			{
				this.validTemplate = false;
				Debug.LogError("The dropdown template is not valid. The Item Image must be on the item GameObject or children of it.", this.template);
			}
			if (!this.validTemplate)
			{
				gameObject.SetActive(false);
				return;
			}
			TMP_Dropdown.DropdownItem dropdownItem = componentInChildren.gameObject.AddComponent<TMP_Dropdown.DropdownItem>();
			dropdownItem.text = this.m_ItemText;
			dropdownItem.image = this.m_ItemImage;
			dropdownItem.toggle = componentInChildren;
			dropdownItem.rectTransform = (RectTransform)componentInChildren.transform;
			Canvas canvas = null;
			Transform transform = this.m_Template.parent;
			while (transform != null)
			{
				canvas = transform.GetComponent<Canvas>();
				if (canvas != null)
				{
					break;
				}
				transform = transform.parent;
			}
			Canvas orAddComponent = TMP_Dropdown.GetOrAddComponent<Canvas>(gameObject);
			orAddComponent.overrideSorting = true;
			orAddComponent.sortingOrder = 30000;
			if (canvas != null)
			{
				Component[] components = canvas.GetComponents<BaseRaycaster>();
				Component[] array = components;
				for (int i = 0; i < array.Length; i++)
				{
					Type type = array[i].GetType();
					if (gameObject.GetComponent(type) == null)
					{
						gameObject.AddComponent(type);
					}
				}
			}
			else
			{
				TMP_Dropdown.GetOrAddComponent<GraphicRaycaster>(gameObject);
			}
			TMP_Dropdown.GetOrAddComponent<CanvasGroup>(gameObject);
			gameObject.SetActive(false);
			this.validTemplate = true;
		}

		private static T GetOrAddComponent<T>(GameObject go) where T : Component
		{
			T t = go.GetComponent<T>();
			if (!t)
			{
				t = go.AddComponent<T>();
			}
			return t;
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			this.Show();
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			this.Show();
		}

		public virtual void OnCancel(BaseEventData eventData)
		{
			this.Hide();
		}

		public void Show()
		{
			if (this.m_Coroutine != null)
			{
				base.StopCoroutine(this.m_Coroutine);
				this.ImmediateDestroyDropdownList();
			}
			if (!this.IsActive() || !this.IsInteractable() || this.m_Dropdown != null)
			{
				return;
			}
			List<Canvas> list = TMP_ListPool<Canvas>.Get();
			base.gameObject.GetComponentsInParent<Canvas>(false, list);
			if (list.Count == 0)
			{
				return;
			}
			Canvas canvas = list[list.Count - 1];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].isRootCanvas)
				{
					canvas = list[i];
					break;
				}
			}
			TMP_ListPool<Canvas>.Release(list);
			if (!this.validTemplate)
			{
				this.SetupTemplate();
				if (!this.validTemplate)
				{
					return;
				}
			}
			this.m_Template.gameObject.SetActive(true);
			this.m_Template.GetComponent<Canvas>().sortingLayerID = canvas.sortingLayerID;
			this.m_Dropdown = this.CreateDropdownList(this.m_Template.gameObject);
			this.m_Dropdown.name = "Dropdown List";
			this.m_Dropdown.SetActive(true);
			RectTransform rectTransform = this.m_Dropdown.transform as RectTransform;
			rectTransform.SetParent(this.m_Template.transform.parent, false);
			TMP_Dropdown.DropdownItem componentInChildren = this.m_Dropdown.GetComponentInChildren<TMP_Dropdown.DropdownItem>();
			RectTransform rectTransform2 = componentInChildren.rectTransform.parent.gameObject.transform as RectTransform;
			componentInChildren.rectTransform.gameObject.SetActive(true);
			Rect rect = rectTransform2.rect;
			Rect rect2 = componentInChildren.rectTransform.rect;
			Vector2 vector = rect2.min - rect.min + componentInChildren.rectTransform.localPosition;
			Vector2 vector2 = rect2.max - rect.max + componentInChildren.rectTransform.localPosition;
			Vector2 size = rect2.size;
			this.m_Items.Clear();
			Toggle toggle = null;
			if (this.m_MultiSelect && this.options.Count > 0)
			{
				TMP_Dropdown.DropdownItem dropdownItem = this.AddItem(TMP_Dropdown.k_NothingOption, this.value == 0, componentInChildren, this.m_Items);
				if (dropdownItem.image != null)
				{
					dropdownItem.image.gameObject.SetActive(false);
				}
				Toggle nothingToggle = dropdownItem.toggle;
				nothingToggle.isOn = this.value == 0;
				nothingToggle.onValueChanged.AddListener(delegate(bool x)
				{
					this.OnSelectItem(nothingToggle);
				});
				toggle = nothingToggle;
				bool flag = TMP_Dropdown.IsEverythingValue(this.options.Count, this.value);
				dropdownItem = this.AddItem(TMP_Dropdown.k_EverythingOption, flag, componentInChildren, this.m_Items);
				if (dropdownItem.image != null)
				{
					dropdownItem.image.gameObject.SetActive(false);
				}
				Toggle everythingToggle = dropdownItem.toggle;
				everythingToggle.isOn = flag;
				everythingToggle.onValueChanged.AddListener(delegate(bool x)
				{
					this.OnSelectItem(everythingToggle);
				});
				if (toggle != null)
				{
					Navigation navigation = toggle.navigation;
					Navigation navigation2 = dropdownItem.toggle.navigation;
					navigation.mode = Navigation.Mode.Explicit;
					navigation2.mode = Navigation.Mode.Explicit;
					navigation.selectOnDown = dropdownItem.toggle;
					navigation.selectOnRight = dropdownItem.toggle;
					navigation2.selectOnLeft = toggle;
					navigation2.selectOnUp = toggle;
					toggle.navigation = navigation;
					dropdownItem.toggle.navigation = navigation2;
				}
			}
			for (int j = 0; j < this.options.Count; j++)
			{
				TMP_Dropdown.OptionData optionData = this.options[j];
				TMP_Dropdown.DropdownItem item = this.AddItem(optionData, this.value == j, componentInChildren, this.m_Items);
				if (!(item == null))
				{
					if (this.m_MultiSelect)
					{
						item.toggle.isOn = (this.value & (1 << j)) != 0;
					}
					else
					{
						item.toggle.isOn = this.value == j;
					}
					item.toggle.onValueChanged.AddListener(delegate(bool x)
					{
						this.OnSelectItem(item.toggle);
					});
					if (item.toggle.isOn)
					{
						item.toggle.Select();
					}
					if (toggle != null)
					{
						Navigation navigation3 = toggle.navigation;
						Navigation navigation4 = item.toggle.navigation;
						navigation3.mode = Navigation.Mode.Explicit;
						navigation4.mode = Navigation.Mode.Explicit;
						navigation3.selectOnDown = item.toggle;
						navigation3.selectOnRight = item.toggle;
						navigation4.selectOnLeft = toggle;
						navigation4.selectOnUp = toggle;
						toggle.navigation = navigation3;
						item.toggle.navigation = navigation4;
					}
					toggle = item.toggle;
				}
			}
			Vector2 sizeDelta = rectTransform2.sizeDelta;
			sizeDelta.y = size.y * (float)this.m_Items.Count + vector.y - vector2.y;
			rectTransform2.sizeDelta = sizeDelta;
			float num = rectTransform.rect.height - rectTransform2.rect.height;
			if (num > 0f)
			{
				rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y - num);
			}
			Vector3[] array = new Vector3[4];
			rectTransform.GetWorldCorners(array);
			RectTransform rectTransform3 = canvas.transform as RectTransform;
			Rect rect3 = rectTransform3.rect;
			for (int k = 0; k < 2; k++)
			{
				bool flag2 = false;
				for (int l = 0; l < 4; l++)
				{
					Vector3 vector3 = rectTransform3.InverseTransformPoint(array[l]);
					if ((vector3[k] < rect3.min[k] && !Mathf.Approximately(vector3[k], rect3.min[k])) || (vector3[k] > rect3.max[k] && !Mathf.Approximately(vector3[k], rect3.max[k])))
					{
						flag2 = true;
						break;
					}
				}
				if (flag2)
				{
					RectTransformUtility.FlipLayoutOnAxis(rectTransform, k, false, false);
				}
			}
			for (int m = 0; m < this.m_Items.Count; m++)
			{
				RectTransform rectTransform4 = this.m_Items[m].rectTransform;
				rectTransform4.anchorMin = new Vector2(rectTransform4.anchorMin.x, 0f);
				rectTransform4.anchorMax = new Vector2(rectTransform4.anchorMax.x, 0f);
				rectTransform4.anchoredPosition = new Vector2(rectTransform4.anchoredPosition.x, vector.y + size.y * (float)(this.m_Items.Count - 1 - m) + size.y * rectTransform4.pivot.y);
				rectTransform4.sizeDelta = new Vector2(rectTransform4.sizeDelta.x, size.y);
			}
			this.AlphaFadeList(this.m_AlphaFadeSpeed, 0f, 1f);
			this.m_Template.gameObject.SetActive(false);
			componentInChildren.gameObject.SetActive(false);
			this.m_Blocker = this.CreateBlocker(canvas);
		}

		private static bool IsEverythingValue(int count, int value)
		{
			bool flag = true;
			for (int i = 0; i < count; i++)
			{
				if ((value & (1 << i)) == 0)
				{
					flag = false;
				}
			}
			return flag;
		}

		private static int EverythingValue(int count)
		{
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				num |= 1 << i;
			}
			return num;
		}

		protected virtual GameObject CreateBlocker(Canvas rootCanvas)
		{
			GameObject gameObject = new GameObject("Blocker");
			gameObject.layer = rootCanvas.gameObject.layer;
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.SetParent(rootCanvas.transform, false);
			rectTransform.anchorMin = Vector3.zero;
			rectTransform.anchorMax = Vector3.one;
			rectTransform.sizeDelta = Vector2.zero;
			Canvas canvas = gameObject.AddComponent<Canvas>();
			canvas.overrideSorting = true;
			Canvas component = this.m_Dropdown.GetComponent<Canvas>();
			canvas.sortingLayerID = component.sortingLayerID;
			canvas.sortingOrder = component.sortingOrder - 1;
			Canvas canvas2 = null;
			Transform transform = this.m_Template.parent;
			while (transform != null)
			{
				canvas2 = transform.GetComponent<Canvas>();
				if (canvas2 != null)
				{
					break;
				}
				transform = transform.parent;
			}
			if (canvas2 != null)
			{
				Component[] components = canvas2.GetComponents<BaseRaycaster>();
				Component[] array = components;
				for (int i = 0; i < array.Length; i++)
				{
					Type type = array[i].GetType();
					if (gameObject.GetComponent(type) == null)
					{
						gameObject.AddComponent(type);
					}
				}
			}
			else
			{
				TMP_Dropdown.GetOrAddComponent<GraphicRaycaster>(gameObject);
			}
			gameObject.AddComponent<Image>().color = Color.clear;
			gameObject.AddComponent<Button>().onClick.AddListener(new UnityAction(this.Hide));
			gameObject.AddComponent<CanvasGroup>().ignoreParentGroups = true;
			return gameObject;
		}

		protected virtual void DestroyBlocker(GameObject blocker)
		{
			global::UnityEngine.Object.Destroy(blocker);
		}

		protected virtual GameObject CreateDropdownList(GameObject template)
		{
			return global::UnityEngine.Object.Instantiate<GameObject>(template);
		}

		protected virtual void DestroyDropdownList(GameObject dropdownList)
		{
			global::UnityEngine.Object.Destroy(dropdownList);
		}

		protected virtual TMP_Dropdown.DropdownItem CreateItem(TMP_Dropdown.DropdownItem itemTemplate)
		{
			return global::UnityEngine.Object.Instantiate<TMP_Dropdown.DropdownItem>(itemTemplate);
		}

		protected virtual void DestroyItem(TMP_Dropdown.DropdownItem item)
		{
		}

		private TMP_Dropdown.DropdownItem AddItem(TMP_Dropdown.OptionData data, bool selected, TMP_Dropdown.DropdownItem itemTemplate, List<TMP_Dropdown.DropdownItem> items)
		{
			TMP_Dropdown.DropdownItem dropdownItem = this.CreateItem(itemTemplate);
			dropdownItem.rectTransform.SetParent(itemTemplate.rectTransform.parent, false);
			dropdownItem.gameObject.SetActive(true);
			dropdownItem.gameObject.name = "Item " + items.Count.ToString() + ((data.text != null) ? (": " + data.text) : "");
			if (dropdownItem.toggle != null)
			{
				dropdownItem.toggle.isOn = false;
			}
			if (dropdownItem.text)
			{
				dropdownItem.text.text = data.text;
			}
			if (dropdownItem.image)
			{
				dropdownItem.image.sprite = data.image;
				dropdownItem.image.color = data.color;
				dropdownItem.image.enabled = dropdownItem.image.sprite != null && data.color.a > 0f;
			}
			items.Add(dropdownItem);
			return dropdownItem;
		}

		private void AlphaFadeList(float duration, float alpha)
		{
			CanvasGroup component = this.m_Dropdown.GetComponent<CanvasGroup>();
			this.AlphaFadeList(duration, component.alpha, alpha);
		}

		private void AlphaFadeList(float duration, float start, float end)
		{
			if (end.Equals(start))
			{
				return;
			}
			FloatTween floatTween = new FloatTween
			{
				duration = duration,
				startValue = start,
				targetValue = end
			};
			floatTween.AddOnChangedCallback(new UnityAction<float>(this.SetAlpha));
			floatTween.ignoreTimeScale = true;
			this.m_AlphaTweenRunner.StartTween(floatTween);
		}

		private void SetAlpha(float alpha)
		{
			if (!this.m_Dropdown)
			{
				return;
			}
			this.m_Dropdown.GetComponent<CanvasGroup>().alpha = alpha;
		}

		public void Hide()
		{
			if (this.m_Coroutine == null)
			{
				if (this.m_Dropdown != null)
				{
					this.AlphaFadeList(this.m_AlphaFadeSpeed, 0f);
					if (this.IsActive())
					{
						this.m_Coroutine = base.StartCoroutine(this.DelayedDestroyDropdownList(this.m_AlphaFadeSpeed));
					}
				}
				if (this.m_Blocker != null)
				{
					this.DestroyBlocker(this.m_Blocker);
				}
				this.m_Blocker = null;
				this.Select();
			}
		}

		private IEnumerator DelayedDestroyDropdownList(float delay)
		{
			yield return new WaitForSecondsRealtime(delay);
			this.ImmediateDestroyDropdownList();
			yield break;
		}

		private void ImmediateDestroyDropdownList()
		{
			for (int i = 0; i < this.m_Items.Count; i++)
			{
				if (this.m_Items[i] != null)
				{
					this.DestroyItem(this.m_Items[i]);
				}
			}
			this.m_Items.Clear();
			if (this.m_Dropdown != null)
			{
				this.DestroyDropdownList(this.m_Dropdown);
			}
			if (this.m_AlphaTweenRunner != null)
			{
				this.m_AlphaTweenRunner.StopTween();
			}
			this.m_Dropdown = null;
			this.m_Coroutine = null;
		}

		private void OnSelectItem(Toggle toggle)
		{
			int num = -1;
			Transform transform = toggle.transform;
			Transform parent = transform.parent;
			for (int i = 1; i < parent.childCount; i++)
			{
				if (parent.GetChild(i) == transform)
				{
					num = i - 1;
					break;
				}
			}
			if (num < 0)
			{
				return;
			}
			if (this.m_MultiSelect)
			{
				if (num != 0)
				{
					if (num != 1)
					{
						int num2 = 1 << num - 2;
						bool flag = (this.value & num2) != 0;
						toggle.SetIsOnWithoutNotify(!flag);
						if (flag)
						{
							this.value &= ~num2;
						}
						else
						{
							this.value |= num2;
						}
					}
					else
					{
						this.value = TMP_Dropdown.EverythingValue(this.options.Count);
						for (int j = 3; j < parent.childCount; j++)
						{
							Toggle componentInChildren = parent.GetChild(j).GetComponentInChildren<Toggle>();
							if (componentInChildren)
							{
								componentInChildren.SetIsOnWithoutNotify(j > 2);
							}
						}
					}
				}
				else
				{
					this.value = 0;
					for (int k = 3; k < parent.childCount; k++)
					{
						Toggle componentInChildren2 = parent.GetChild(k).GetComponentInChildren<Toggle>();
						if (componentInChildren2)
						{
							componentInChildren2.SetIsOnWithoutNotify(false);
						}
					}
					toggle.isOn = true;
				}
			}
			else
			{
				if (!toggle.isOn)
				{
					toggle.SetIsOnWithoutNotify(true);
				}
				this.value = num;
			}
			this.Hide();
		}

		private static int FirstActiveFlagIndex(int value)
		{
			if (value == 0)
			{
				return 0;
			}
			for (int i = 0; i < 32; i++)
			{
				if ((value & (1 << i)) != 0)
				{
					return i;
				}
			}
			return 0;
		}

		private static readonly TMP_Dropdown.OptionData k_NothingOption = new TMP_Dropdown.OptionData
		{
			text = "Nothing"
		};

		private static readonly TMP_Dropdown.OptionData k_EverythingOption = new TMP_Dropdown.OptionData
		{
			text = "Everything"
		};

		private static readonly TMP_Dropdown.OptionData k_MixedOption = new TMP_Dropdown.OptionData
		{
			text = "Mixed..."
		};

		[SerializeField]
		private RectTransform m_Template;

		[SerializeField]
		private TMP_Text m_CaptionText;

		[SerializeField]
		private Image m_CaptionImage;

		[SerializeField]
		private Graphic m_Placeholder;

		[Space]
		[SerializeField]
		private TMP_Text m_ItemText;

		[SerializeField]
		private Image m_ItemImage;

		[Space]
		[SerializeField]
		private int m_Value;

		[SerializeField]
		private bool m_MultiSelect;

		[Space]
		[SerializeField]
		private TMP_Dropdown.OptionDataList m_Options = new TMP_Dropdown.OptionDataList();

		[Space]
		[SerializeField]
		private TMP_Dropdown.DropdownEvent m_OnValueChanged = new TMP_Dropdown.DropdownEvent();

		[SerializeField]
		private float m_AlphaFadeSpeed = 0.15f;

		private GameObject m_Dropdown;

		private GameObject m_Blocker;

		private List<TMP_Dropdown.DropdownItem> m_Items = new List<TMP_Dropdown.DropdownItem>();

		private TweenRunner<FloatTween> m_AlphaTweenRunner;

		private bool validTemplate;

		private Coroutine m_Coroutine;

		private static TMP_Dropdown.OptionData s_NoOptionData = new TMP_Dropdown.OptionData();

		protected internal class DropdownItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, ICancelHandler
		{
			public TMP_Text text
			{
				get
				{
					return this.m_Text;
				}
				set
				{
					this.m_Text = value;
				}
			}

			public Image image
			{
				get
				{
					return this.m_Image;
				}
				set
				{
					this.m_Image = value;
				}
			}

			public RectTransform rectTransform
			{
				get
				{
					return this.m_RectTransform;
				}
				set
				{
					this.m_RectTransform = value;
				}
			}

			public Toggle toggle
			{
				get
				{
					return this.m_Toggle;
				}
				set
				{
					this.m_Toggle = value;
				}
			}

			public virtual void OnPointerEnter(PointerEventData eventData)
			{
				EventSystem.current.SetSelectedGameObject(base.gameObject);
			}

			public virtual void OnCancel(BaseEventData eventData)
			{
				TMP_Dropdown componentInParent = base.GetComponentInParent<TMP_Dropdown>();
				if (componentInParent)
				{
					componentInParent.Hide();
				}
			}

			[SerializeField]
			private TMP_Text m_Text;

			[SerializeField]
			private Image m_Image;

			[SerializeField]
			private RectTransform m_RectTransform;

			[SerializeField]
			private Toggle m_Toggle;
		}

		[Serializable]
		public class OptionData
		{
			public string text
			{
				get
				{
					return this.m_Text;
				}
				set
				{
					this.m_Text = value;
				}
			}

			public Sprite image
			{
				get
				{
					return this.m_Image;
				}
				set
				{
					this.m_Image = value;
				}
			}

			public Color color
			{
				get
				{
					return this.m_Color;
				}
				set
				{
					this.m_Color = value;
				}
			}

			public OptionData()
			{
			}

			public OptionData(string text)
			{
				this.text = text;
			}

			public OptionData(Sprite image)
			{
				this.image = image;
			}

			public OptionData(string text, Sprite image, Color color)
			{
				this.text = text;
				this.image = image;
				this.color = color;
			}

			[SerializeField]
			private string m_Text;

			[SerializeField]
			private Sprite m_Image;

			[SerializeField]
			private Color m_Color = Color.white;
		}

		[Serializable]
		public class OptionDataList
		{
			public List<TMP_Dropdown.OptionData> options
			{
				get
				{
					return this.m_Options;
				}
				set
				{
					this.m_Options = value;
				}
			}

			public OptionDataList()
			{
				this.options = new List<TMP_Dropdown.OptionData>();
			}

			[SerializeField]
			private List<TMP_Dropdown.OptionData> m_Options;
		}

		[Serializable]
		public class DropdownEvent : UnityEvent<int>
		{
		}
	}
}
