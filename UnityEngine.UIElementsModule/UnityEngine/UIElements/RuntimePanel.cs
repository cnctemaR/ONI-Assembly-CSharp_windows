using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class RuntimePanel : BaseRuntimePanel, IRuntimePanel, IPanel, IDisposable
	{
		public PanelSettings panelSettings
		{
			get
			{
				return this.m_PanelSettings;
			}
		}

		internal List<UIDocument> documents
		{
			get
			{
				UIDocumentList attachedUIDocumentsList = this.m_PanelSettings.m_AttachedUIDocumentsList;
				return ((attachedUIDocumentsList != null) ? attachedUIDocumentsList.m_AttachedUIDocuments : null) ?? RuntimePanel.s_EmptyDocumentList;
			}
		}

		public static RuntimePanel Create(ScriptableObject ownerObject)
		{
			return new RuntimePanel(ownerObject);
		}

		private RuntimePanel(ScriptableObject ownerObject)
			: base(ownerObject, RuntimePanel.s_EventDispatcher)
		{
			this.focusController = new FocusController(new NavigateFocusRing(this.visualTree));
			this.m_PanelSettings = ownerObject as PanelSettings;
			base.name = ((this.m_PanelSettings != null) ? this.m_PanelSettings.name : "RuntimePanel");
			this.visualTree.RegisterCallback<FocusEvent, RuntimePanel>(delegate(FocusEvent e, RuntimePanel p)
			{
				p.OnElementFocus(e);
			}, this, TrickleDown.TrickleDown);
		}

		internal override void Update()
		{
			bool flag = this.m_PanelSettings != null;
			if (flag)
			{
				this.m_PanelSettings.ApplyPanelSettings();
			}
			base.Update();
		}

		private void OnElementFocus(FocusEvent evt)
		{
			UIElementsRuntimeUtility.defaultEventSystem.OnFocusEvent(this, evt);
		}

		internal static readonly EventDispatcher s_EventDispatcher = RuntimeEventDispatcher.Create();

		private readonly PanelSettings m_PanelSettings;

		private static readonly List<UIDocument> s_EmptyDocumentList = new List<UIDocument>();
	}
}
