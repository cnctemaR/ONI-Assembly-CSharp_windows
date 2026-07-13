using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/IMGUI/GUIContent.h")]
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class GUIContent
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action OnTextChanged;

		public string text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
				bool flag = this.m_Text == value;
				if (!flag)
				{
					this.m_Text = value;
					this.textWithWhitespace = value;
					Action onTextChanged = this.OnTextChanged;
					if (onTextChanged != null)
					{
						onTextChanged();
					}
				}
			}
		}

		internal string textWithWhitespace
		{
			get
			{
				return string.IsNullOrEmpty(this.m_TextWithWhitespace) ? GUIContent.k_ZeroWidthSpace : this.m_TextWithWhitespace;
			}
			set
			{
				this.m_TextWithWhitespace = value + GUIContent.k_ZeroWidthSpace;
			}
		}

		internal void SetTextWithoutNotify(string value)
		{
			this.m_Text = value;
			this.textWithWhitespace = value;
		}

		public Texture image
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

		public string tooltip
		{
			get
			{
				return this.m_Tooltip;
			}
			set
			{
				this.m_Tooltip = value;
			}
		}

		public GUIContent()
		{
		}

		public GUIContent(string text)
			: this(text, null, string.Empty)
		{
		}

		public GUIContent(Texture image)
			: this(string.Empty, image, string.Empty)
		{
		}

		public GUIContent(string text, Texture image)
			: this(text, image, string.Empty)
		{
		}

		public GUIContent(string text, string tooltip)
			: this(text, null, tooltip)
		{
		}

		public GUIContent(Texture image, string tooltip)
			: this(string.Empty, image, tooltip)
		{
		}

		public GUIContent(string text, Texture image, string tooltip)
		{
			this.text = text;
			this.image = image;
			this.tooltip = tooltip;
		}

		public GUIContent(GUIContent src)
		{
			this.text = src.m_Text;
			this.image = src.m_Image;
			this.tooltip = src.m_Tooltip;
		}

		internal int hash
		{
			get
			{
				int num = 0;
				bool flag = !string.IsNullOrEmpty(this.m_Text);
				if (flag)
				{
					num = this.m_Text.GetHashCode() * 37;
				}
				return num;
			}
		}

		internal static GUIContent Temp(string t)
		{
			GUIContent.s_Text.m_Text = t;
			GUIContent.s_Text.textWithWhitespace = t;
			GUIContent.s_Text.m_Tooltip = string.Empty;
			return GUIContent.s_Text;
		}

		internal static GUIContent Temp(string t, string tooltip)
		{
			GUIContent.s_Text.m_Text = t;
			GUIContent.s_Text.textWithWhitespace = t;
			GUIContent.s_Text.m_Tooltip = tooltip;
			return GUIContent.s_Text;
		}

		internal static GUIContent Temp(Texture i)
		{
			GUIContent.s_Image.m_Image = i;
			GUIContent.s_Image.m_Tooltip = string.Empty;
			return GUIContent.s_Image;
		}

		internal static GUIContent Temp(Texture i, string tooltip)
		{
			GUIContent.s_Image.m_Image = i;
			GUIContent.s_Image.m_Tooltip = tooltip;
			return GUIContent.s_Image;
		}

		internal static GUIContent Temp(string t, Texture i)
		{
			GUIContent.s_TextImage.m_Text = t;
			GUIContent.s_Text.textWithWhitespace = t;
			GUIContent.s_TextImage.m_Image = i;
			return GUIContent.s_TextImage;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static void ClearStaticCache()
		{
			GUIContent.s_Text.m_Text = null;
			GUIContent.s_Text.m_TextWithWhitespace = null;
			GUIContent.s_Text.m_Tooltip = string.Empty;
			GUIContent.s_Image.m_Image = null;
			GUIContent.s_Image.m_Tooltip = string.Empty;
			GUIContent.s_Image.m_TextWithWhitespace = null;
			GUIContent.s_TextImage.m_Text = null;
			GUIContent.s_TextImage.m_Image = null;
			GUIContent.s_TextImage.m_TextWithWhitespace = null;
		}

		internal static GUIContent[] Temp(string[] texts)
		{
			GUIContent[] array = new GUIContent[texts.Length];
			for (int i = 0; i < texts.Length; i++)
			{
				array[i] = new GUIContent(texts[i]);
			}
			return array;
		}

		internal static GUIContent[] Temp(Texture[] images)
		{
			GUIContent[] array = new GUIContent[images.Length];
			for (int i = 0; i < images.Length; i++)
			{
				array[i] = new GUIContent(images[i]);
			}
			return array;
		}

		public override string ToString()
		{
			string text;
			if ((text = this.text) == null)
			{
				text = this.tooltip ?? base.ToString();
			}
			return text;
		}

		[SerializeField]
		private string m_Text = string.Empty;

		[SerializeField]
		private Texture m_Image;

		[SerializeField]
		private string m_Tooltip = string.Empty;

		[SerializeField]
		private string m_TextWithWhitespace = string.Empty;

		private static readonly GUIContent s_Text = new GUIContent();

		private static readonly GUIContent s_Image = new GUIContent();

		private static readonly GUIContent s_TextImage = new GUIContent();

		internal static string k_ZeroWidthSpace = "\u200b";

		public static GUIContent none = new GUIContent("");
	}
}
