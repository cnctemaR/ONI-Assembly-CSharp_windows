using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using ProcGen;
using STRINGS;
using UnityEngine;

public class PasteBaseTemplateScreen : KScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		PasteBaseTemplateScreen.Instance = this;
		TemplateCache.Init();
		this.button_directory_up.onClick += this.UpDirectory;
		base.ConsumeMouseScroll = true;
		this.RefreshStampButtons();
	}

	[ContextMenu("Refresh")]
	public void RefreshStampButtons()
	{
		this.directory_path_text.text = this.m_CurrentDirectory;
		this.button_directory_up.isInteractable = this.m_CurrentDirectory != PasteBaseTemplateScreen.NO_DIRECTORY;
		foreach (GameObject gameObject in this.m_template_buttons)
		{
			global::UnityEngine.Object.Destroy(gameObject);
		}
		this.m_template_buttons.Clear();
		global::Debug.Log("Changing directory to " + this.m_CurrentDirectory);
		if (this.m_CurrentDirectory == PasteBaseTemplateScreen.NO_DIRECTORY)
		{
			this.directory_path_text.text = "";
			using (List<string>.Enumerator enumerator2 = DlcManager.RELEASE_ORDER.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					string dlcId = enumerator2.Current;
					if (DlcManager.IsContentActive(dlcId))
					{
						GameObject gameObject2 = global::Util.KInstantiateUI(this.prefab_directory_button, this.button_list_container, true);
						gameObject2.GetComponent<KButton>().onClick += delegate
						{
							this.UpdateDirectory(SettingsCache.GetScope(dlcId));
						};
						gameObject2.GetComponentInChildren<LocText>().text = ((dlcId == "") ? UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.BASE_GAME_FOLDER_NAME.text : SettingsCache.GetScope(dlcId));
						this.m_template_buttons.Add(gameObject2);
					}
				}
			}
			return;
		}
		string[] directories = Directory.GetDirectories(TemplateCache.RewriteTemplatePath(this.m_CurrentDirectory));
		for (int i = 0; i < directories.Length; i++)
		{
			string text = directories[i];
			string directory_name = global::System.IO.Path.GetFileNameWithoutExtension(text);
			GameObject gameObject3 = global::Util.KInstantiateUI(this.prefab_directory_button, this.button_list_container, true);
			gameObject3.GetComponent<KButton>().onClick += delegate
			{
				this.UpdateDirectory(directory_name);
			};
			gameObject3.GetComponentInChildren<LocText>().text = directory_name;
			this.m_template_buttons.Add(gameObject3);
		}
		ListPool<FileHandle, PasteBaseTemplateScreen>.PooledList pooledList = ListPool<FileHandle, PasteBaseTemplateScreen>.Allocate();
		FileSystem.GetFiles(TemplateCache.RewriteTemplatePath(this.m_CurrentDirectory), "*.yaml", pooledList);
		foreach (FileHandle fileHandle in pooledList)
		{
			string file_path_no_extension = global::System.IO.Path.GetFileNameWithoutExtension(fileHandle.full_path);
			GameObject gameObject4 = global::Util.KInstantiateUI(this.prefab_paste_button, this.button_list_container, true);
			gameObject4.GetComponent<KButton>().onClick += delegate
			{
				this.OnClickPasteButton(file_path_no_extension);
			};
			gameObject4.GetComponentInChildren<LocText>().text = file_path_no_extension;
			this.m_template_buttons.Add(gameObject4);
		}
	}

	private void UpdateDirectory(string relativePath)
	{
		if (this.m_CurrentDirectory == PasteBaseTemplateScreen.NO_DIRECTORY)
		{
			this.m_CurrentDirectory = "";
		}
		this.m_CurrentDirectory = FileSystem.CombineAndNormalize(new string[] { this.m_CurrentDirectory, relativePath });
		this.RefreshStampButtons();
	}

	private void UpDirectory()
	{
		int num = this.m_CurrentDirectory.LastIndexOf("/");
		if (num > 0)
		{
			this.m_CurrentDirectory = this.m_CurrentDirectory.Substring(0, num);
		}
		else
		{
			string text;
			string text2;
			SettingsCache.GetDlcIdAndPath(this.m_CurrentDirectory, out text, out text2);
			if (text2.IsNullOrWhiteSpace())
			{
				this.m_CurrentDirectory = PasteBaseTemplateScreen.NO_DIRECTORY;
			}
			else
			{
				this.m_CurrentDirectory = SettingsCache.GetScope(text);
			}
		}
		this.RefreshStampButtons();
	}

	private void OnClickPasteButton(string template_name)
	{
		if (template_name == null)
		{
			return;
		}
		string text = FileSystem.CombineAndNormalize(new string[] { this.m_CurrentDirectory, template_name });
		DebugTool.Instance.DeactivateTool(null);
		DebugBaseTemplateButton.Instance.ClearSelection();
		DebugBaseTemplateButton.Instance.nameField.text = text;
		TemplateContainer template = TemplateCache.GetTemplate(text);
		StampTool.Instance.Activate(template, true, false);
	}

	public static PasteBaseTemplateScreen Instance;

	public GameObject button_list_container;

	public GameObject prefab_paste_button;

	public GameObject prefab_directory_button;

	public KButton button_directory_up;

	public LocText directory_path_text;

	private List<GameObject> m_template_buttons = new List<GameObject>();

	private static readonly string NO_DIRECTORY = "NONE";

	private string m_CurrentDirectory = PasteBaseTemplateScreen.NO_DIRECTORY;
}
