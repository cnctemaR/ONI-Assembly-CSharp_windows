using System;
using System.Collections.Generic;
using Steamworks;
using STRINGS;
using UnityEngine;

public class ModErrorsScreen : KScreen
{
	public static void ShowErrors(ICollection<ModError> errors)
	{
		GameObject gameObject = GameObject.Find("Canvas");
		ModErrorsScreen modErrorsScreen = Util.KInstantiateUI<ModErrorsScreen>(Global.Instance.modErrorsPrefab, gameObject, false);
		modErrorsScreen.Initialize(errors);
		modErrorsScreen.gameObject.SetActive(true);
	}

	private void Initialize(ICollection<ModError> errors)
	{
		using (IEnumerator<ModError> enumerator = errors.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ModError error = enumerator.Current;
				HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(this.entryPrefab, this.entryParent.gameObject, true);
				LocText reference = hierarchyReferences.GetReference<LocText>("Title");
				LocText reference2 = hierarchyReferences.GetReference<LocText>("Description");
				KButton reference3 = hierarchyReferences.GetReference<KButton>("Details");
				string text;
				string text2;
				ModErrorsScreen.GetErrorText(error.errorType, out text, out text2);
				reference.text = text;
				reference.GetComponent<ToolTip>().toolTip = text2;
				ModInfo.Source source = error.modInfo.source;
				if (source != ModInfo.Source.Steam)
				{
					if (source == ModInfo.Source.Local)
					{
						reference2.text = error.modInfo.assetID;
						reference3.onClick += delegate
						{
							Application.OpenURL("file://" + error.modInfo.assetID);
						};
					}
				}
				else
				{
					ulong num = ulong.Parse(error.modInfo.assetID);
					PublishedFileId_t publishedFileId_t = new PublishedFileId_t(num);
					SteamUGCService.Subscribed subscribed = SteamUGCService.Instance.GetSubscribed(publishedFileId_t);
					reference2.text = subscribed.title;
					reference3.onClick += delegate
					{
						ModErrorsScreen.OpenDetailsPage(error.modInfo.assetID);
					};
				}
			}
		}
	}

	private static void GetErrorText(ModError.ErrorType err_type, out string title, out string title_tooltip)
	{
		if (err_type != ModError.ErrorType.LoadError)
		{
			throw new ArgumentOutOfRangeException();
		}
		title = UI.FRONTEND.MOD_ERRORS.MOD_REQUIRED;
		title_tooltip = UI.FRONTEND.MOD_ERRORS.TOOLTIPS.MOD_REQUIRED;
	}

	private static void OpenDetailsPage(string key)
	{
		Application.OpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + key);
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
