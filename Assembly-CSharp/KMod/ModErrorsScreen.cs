using System;
using System.Collections.Generic;
using UnityEngine;

namespace KMod
{
	public class ModErrorsScreen : KScreen
	{
		public static bool ShowErrors(List<Event> events)
		{
			if (Global.Instance.modManager.events.Count == 0)
			{
				return false;
			}
			GameObject gameObject = GameObject.Find("Canvas");
			ModErrorsScreen modErrorsScreen = Util.KInstantiateUI<ModErrorsScreen>(Global.Instance.modErrorsPrefab, gameObject, false);
			modErrorsScreen.Initialize(events);
			modErrorsScreen.gameObject.SetActive(true);
			return true;
		}

		private void Initialize(List<Event> events)
		{
			foreach (Event @event in events)
			{
				HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(this.entryPrefab, this.entryParent.gameObject, true);
				LocText reference = hierarchyReferences.GetReference<LocText>("Title");
				LocText reference2 = hierarchyReferences.GetReference<LocText>("Description");
				KButton reference3 = hierarchyReferences.GetReference<KButton>("Details");
				string text;
				string text2;
				Event.GetUIStrings(@event.event_type, out text, out text2);
				reference.text = text;
				reference.GetComponent<ToolTip>().toolTip = text2;
				reference2.text = @event.mod.title;
				ToolTip component = reference2.GetComponent<ToolTip>();
				if (component != null)
				{
					ToolTip toolTip = component;
					Label mod = @event.mod;
					toolTip.toolTip = mod.ToString();
				}
				reference3.isInteractable = false;
				Mod mod2 = Global.Instance.modManager.FindMod(@event.mod);
				if (mod2 != null)
				{
					if (component != null && !string.IsNullOrEmpty(mod2.description))
					{
						component.toolTip = mod2.description;
					}
					if (mod2.on_managed != null)
					{
						reference3.onClick += mod2.on_managed;
						reference3.isInteractable = true;
					}
				}
			}
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			this.closeButtonTitle.onClick += this.Deactivate;
			this.closeButton.onClick += this.Deactivate;
		}

		[SerializeField]
		private KButton closeButtonTitle;

		[SerializeField]
		private KButton closeButton;

		[SerializeField]
		private GameObject entryPrefab;

		[SerializeField]
		private Transform entryParent;
	}
}
