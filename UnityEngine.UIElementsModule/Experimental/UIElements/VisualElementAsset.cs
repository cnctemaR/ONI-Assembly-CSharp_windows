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
					if (uxmlFactory2.AcceptsAttributeBag(this, ctx))
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
			if (!this.m_Properties.Contains("picking-mode") && !this.m_Properties.Contains("pickingMode"))
			{
				this.AddProperty("picking-mode", this.m_PickingMode.ToString());
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

		public bool TryGetAttributeValue(string propertyName, out string value)
		{
			bool flag;
			if (this.m_Properties == null)
			{
				value = null;
				flag = false;
			}
			else
			{
				for (int i = 0; i < this.m_Properties.Count - 1; i += 2)
				{
					if (this.m_Properties[i] == propertyName)
					{
						value = this.m_Properties[i + 1];
						return true;
					}
				}
				value = null;
				flag = false;
			}
			return flag;
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
