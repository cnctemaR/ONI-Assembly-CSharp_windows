using System;

namespace UnityEngine.UIElements
{
	internal class TextEditorEngine : TextEditor
	{
		public TextEditorEngine(TextEditorEngine.OnDetectFocusChangeFunction detectFocusChange, TextEditorEngine.OnIndexChangeFunction indexChangeFunction)
		{
			this.m_DetectFocusChangeFunction = detectFocusChange;
			this.m_IndexChangeFunction = indexChangeFunction;
		}

		internal override Rect localPosition
		{
			get
			{
				return new Rect(0f, 0f, base.position.width, base.position.height);
			}
		}

		internal override void OnDetectFocusChange()
		{
			this.m_DetectFocusChangeFunction();
		}

		internal override void OnCursorIndexChange()
		{
			this.m_IndexChangeFunction();
		}

		internal override void OnSelectIndexChange()
		{
			this.m_IndexChangeFunction();
		}

		private TextEditorEngine.OnDetectFocusChangeFunction m_DetectFocusChangeFunction;

		private TextEditorEngine.OnIndexChangeFunction m_IndexChangeFunction;

		internal delegate void OnDetectFocusChangeFunction();

		internal delegate void OnIndexChangeFunction();
	}
}
