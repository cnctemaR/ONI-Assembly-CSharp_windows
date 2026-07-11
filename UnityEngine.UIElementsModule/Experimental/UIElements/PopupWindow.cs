using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Styled visual element that matches the EditorGUILayout.Popup IMGUI element.</para>
	/// </summary>
	public class PopupWindow : BaseTextElement
	{
		public PopupWindow()
		{
			this.m_ContentContainer = new VisualElement
			{
				name = "ContentContainer"
			};
			base.shadow.Add(this.m_ContentContainer);
		}

		public override VisualElement contentContainer
		{
			get
			{
				return this.m_ContentContainer;
			}
		}

		private VisualElement m_ContentContainer;

		/// <summary>
		///   <para>Instantiates a PopupWindow using the data read from a UXML file.</para>
		/// </summary>
		public class PopupWindowFactory : UxmlFactory<PopupWindow, PopupWindow.PopupWindowUxmlTraits>
		{
		}

		/// <summary>
		///   <para>UxmlTraits for the PopupWindow.</para>
		/// </summary>
		public class PopupWindowUxmlTraits : BaseTextElement.BaseTextElementUxmlTraits
		{
			/// <summary>
			///   <para>Returns an empty enumerable, as popup windows generally do not have children.</para>
			/// </summary>
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield return new UxmlChildElementDescription(typeof(VisualElement));
					yield break;
				}
			}
		}
	}
}
