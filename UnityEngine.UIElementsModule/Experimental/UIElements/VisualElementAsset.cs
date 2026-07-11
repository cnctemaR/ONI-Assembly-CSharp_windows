using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	[Serializable]
	internal class VisualElementAsset : IUxmlAttributes, ISerializationCallbackReceiver
	{
		public VisualElementAsset(string fullTypeName)
		{
			this.m_FullTypeName = fullTypeName;
			this.m_Name = string.Empty;
			this.m_Text = string.Empty;
			this.m_PickingMode = PickingMode.Position;
		}

		public string name
		{
			get
			{
				return this.GetPropertyString("name", null);
			}
			set
			{
				this.SetOrAddProperty("name", value);
			}
		}

		public int id
		{
			get
			{
				return this.m_Id;
			}
			set
			{
				this.m_Id = value;
			}
		}

		public int parentId
		{
			get
			{
				return this.m_ParentId;
			}
			set
			{
				this.m_ParentId = value;
			}
		}

		public int ruleIndex
		{
			get
			{
				return this.m_RuleIndex;
			}
			set
			{
				this.m_RuleIndex = value;
			}
		}

		public string text
		{
			get
			{
				return this.GetPropertyString("text", null);
			}
			set
			{
				this.SetOrAddProperty("text", value);
			}
		}

		public string pickingMode
		{
			get
			{
				return this.GetPropertyString("pickingMode", null);
			}
			set
			{
				this.SetOrAddProperty("pickingMode", value);
			}
		}

		public string fullTypeName
		{
			get
			{
				return this.m_FullTypeName;
			}
			set
			{
				this.m_FullTypeName = value;
			}
		}

		public string[] classes
		{
			get
			{
				return this.m_Classes;
			}
			set
			{
				this.m_Classes = value;
			}
		}

		public List<string> stylesheets
		{
			get
			{
				return (this.m_Stylesheets != null) ? this.m_Stylesheets : (this.m_Stylesheets = new List<string>());
			}
			set
			{
				this.m_Stylesheets = value;
			}
		}

		public VisualElement Create(CreationContext ctx)
		{
			List<IUxmlFactory> list;
			VisualElement visualElement;
			if (!VisualElementFactoryRegistry.TryGetValue(this.fullTypeName, out list))
			{
				Debug.LogErrorFormat("Element '{0}' has no registered factory method.", new object[] { this.fullTypeName });
				visualElement = new Label(string.Format("Unknown type: '{0}'", this.fullTypeName));
			}
			else
			{
				IUxmlFactory uxmlFactory = null;
				foreach (IUxmlFactory uxmlFactory2 in list)
				{
					if (uxmlFactory2.AcceptsAttributeBag(this))
					{
						uxmlFactory = uxmlFactory2;
						break;
					}
				}
				if (uxmlFactory == null)
				{
					Debug.LogErrorFormat("Element '{0}' has a no factory that accept the set of XML attributes specified.", new object[] { this.fullTypeName });
					visualElement = new Label(string.Format("Type with no factory: '{0}'", this.fullTypeName));
				}
				else if (uxmlFactory is UxmlRootElementFactory)
				{
					visualElement = null;
				}
				else
				{
					VisualElement visualElement2 = uxmlFactory.Create(this, ctx);
					if (visualElement2 == null)
					{
						Debug.LogErrorFormat("The factory of Visual Element Type '{0}' has returned a null object", new object[] { this.fullTypeName });
						visualElement = new Label(string.Format("The factory of Visual Element Type '{0}' has returned a null object", this.fullTypeName));
					}
					else
					{
						if (this.classes != null)
						{
							for (int i = 0; i < this.classes.Length; i++)
							{
								visualElement2.AddToClassList(this.classes[i]);
							}
						}
						if (this.stylesheets != null)
						{
							for (int j = 0; j < this.stylesheets.Count; j++)
							{
								visualElement2.AddStyleSheetPath(this.stylesheets[j]);
							}
						}
						visualElement = visualElement2;
					}
				}
			}
			return visualElement;
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (!this.m_Properties.Contains("name"))
			{
				this.AddProperty("name", this.m_Name);
			}
			if (!this.m_Properties.Contains("text"))
			{
				this.AddProperty("text", this.m_Text);
			}
			if (!this.m_Properties.Contains("pickingMode"))
			{
				this.AddProperty("pickingMode", this.m_PickingMode.ToString());
			}
		}

		public void AddProperty(string propertyName, string propertyValue)
		{
			this.SetOrAddProperty(propertyName, propertyValue);
		}

		private void SetOrAddProperty(string propertyName, string propertyValue)
		{
			if (this.m_Properties == null)
			{
				this.m_Properties = new List<string>();
			}
			for (int i = 0; i < this.m_Properties.Count - 1; i += 2)
			{
				if (this.m_Properties[i] == propertyName)
				{
					this.m_Properties[i + 1] = propertyValue;
					return;
				}
			}
			this.m_Properties.Add(propertyName);
			this.m_Properties.Add(propertyValue);
		}

		public virtual string GetPropertyString(string propertyName)
		{
			return this.GetPropertyString(propertyName, null);
		}

		public virtual string GetPropertyString(string propertyName, string defaultValue)
		{
			string text;
			if (this.m_Properties == null)
			{
				text = defaultValue;
			}
			else
			{
				for (int i = 0; i < this.m_Properties.Count - 1; i += 2)
				{
					if (this.m_Properties[i] == propertyName)
					{
						return this.m_Properties[i + 1];
					}
				}
				text = defaultValue;
			}
			return text;
		}

		public int GetPropertyInt(string propertyName, int defaultValue)
		{
			string propertyString = this.GetPropertyString(propertyName, null);
			int num;
			int num2;
			if (propertyString == null || !int.TryParse(propertyString, out num))
			{
				num2 = defaultValue;
			}
			else
			{
				num2 = num;
			}
			return num2;
		}

		public long GetPropertyLong(string propertyName, long defaultValue)
		{
			string propertyString = this.GetPropertyString(propertyName, null);
			long num;
			long num2;
			if (propertyString == null || !long.TryParse(propertyString, out num))
			{
				num2 = defaultValue;
			}
			else
			{
				num2 = num;
			}
			return num2;
		}

		public bool GetPropertyBool(string propertyName, bool defaultValue)
		{
			string propertyString = this.GetPropertyString(propertyName, null);
			bool flag;
			bool flag2;
			if (propertyString == null || !bool.TryParse(propertyString, out flag))
			{
				flag2 = defaultValue;
			}
			else
			{
				flag2 = flag;
			}
			return flag2;
		}

		public Color GetPropertyColor(string propertyName, Color defaultValue)
		{
			string propertyString = this.GetPropertyString(propertyName, null);
			Color color;
			Color color2;
			if (propertyString == null || !ColorUtility.TryParseHtmlString(propertyString, out color))
			{
				color2 = defaultValue;
			}
			else
			{
				color2 = color;
			}
			return color2;
		}

		public float GetPropertyFloat(string propertyName, float defaultValue)
		{
			string propertyString = this.GetPropertyString(propertyName, null);
			float num;
			float num2;
			if (propertyString == null || !float.TryParse(propertyString, out num))
			{
				num2 = defaultValue;
			}
			else
			{
				num2 = num;
			}
			return num2;
		}

		public double GetPropertyDouble(string propertyName, double defaultValue)
		{
			string propertyString = this.GetPropertyString(propertyName, null);
			double num;
			double num2;
			if (propertyString == null || !double.TryParse(propertyString, out num))
			{
				num2 = defaultValue;
			}
			else
			{
				num2 = num;
			}
			return num2;
		}

		public T GetPropertyEnum<T>(string propertyName, T defaultValue)
		{
			string propertyString = this.GetPropertyString(propertyName, null);
			T t;
			if (propertyString == null || !Enum.IsDefined(typeof(T), propertyString))
			{
				t = defaultValue;
			}
			else
			{
				T t2 = (T)((object)Enum.Parse(typeof(T), propertyString));
				t = t2;
			}
			return t;
		}

		[SerializeField]
		private string m_Name;

		[SerializeField]
		private int m_Id;

		[SerializeField]
		private int m_ParentId;

		[SerializeField]
		private int m_RuleIndex;

		[SerializeField]
		private string m_Text;

		[SerializeField]
		private PickingMode m_PickingMode;

		[SerializeField]
		private string m_FullTypeName;

		[SerializeField]
		private string[] m_Classes;

		[SerializeField]
		private List<string> m_Stylesheets;

		[SerializeField]
		private List<string> m_Properties;
	}
}
