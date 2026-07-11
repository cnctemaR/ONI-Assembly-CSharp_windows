using System;
using System.Collections;
using System.IO;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportErrorDialog : MonoBehaviour
{
	private void Start()
	{
		ThreadedHttps<KleiMetrics>.Instance.EndSession(true);
		if (SpeedControlScreen.Instance)
		{
			SpeedControlScreen.Instance.Pause(false);
		}
		if (KScreenManager.Instance)
		{
			KScreenManager.Instance.DisableInput(true);
		}
		this.continueGameButton.onClick += this.OnSelect_CONTINUE;
		this.continueGameButton.gameObject.SetActive(!KCrashReporter.terminateOnError);
		this.submitButton.onClick += this.OnSelect_SUBMIT;
		this.quitButton.onClick += this.OnSelect_QUIT;
		this.uploadSaveButton.onClick += this.OnSelect_UPLOADSAVE;
		this.skipUploadSaveButton.onClick += this.OnSelect_SKIPUPLOADSAVE;
		this.messageInputField.text = UI.CRASHSCREEN.BODY;
		ReportErrorDialog.hasCrash = true;
	}

	private void Update()
	{
		global::Debug.developerConsoleVisible = false;
	}

	private void OnDestroy()
	{
		if (KCrashReporter.terminateOnError)
		{
			App.Quit();
		}
		if (KScreenManager.Instance)
		{
			KScreenManager.Instance.DisableInput(false);
		}
	}

	public void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape))
		{
			this.OnSelect_QUIT();
		}
	}

	public void PopupConfirmDialog(global::System.Action onConfirm, global::System.Action onQuit, global::System.Action onContinue)
	{
		this.confirmAction = onConfirm;
		this.quitAction = onQuit;
		this.continueAction = onContinue;
		this.continueGameButton.gameObject.SetActive(this.continueAction != null);
		this.VCCrashLabel.gameObject.SetActive(false);
		this.VCLinkButton.gameObject.SetActive(false);
		this.quitButton.gameObject.SetActive(onQuit != null);
	}

	public void OnSelect_SUBMIT()
	{
		this.submitButton.GetComponentInChildren<LocText>().text = UI.CRASHSCREEN.REPORTING;
		this.submitButton.GetComponent<KButton>().isInteractable = false;
		base.StartCoroutine(this.WaitForUIUpdateBeforeReporting());
	}

	private IEnumerator WaitForUIUpdateBeforeReporting()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		bool flag = false;
		if (ReportErrorDialog.MOST_RECENT_SAVEFILE != null && File.Exists(ReportErrorDialog.MOST_RECENT_SAVEFILE))
		{
			flag = true;
			long length = new FileInfo(ReportErrorDialog.MOST_RECENT_SAVEFILE).Length;
			this.saveFileInfoLabel.text = Path.GetFileName(ReportErrorDialog.MOST_RECENT_SAVEFILE) + " " + length.ToString() + " bytes";
			this.uploadSaveDialog.SetActive(true);
		}
		if (!flag)
		{
			this.Submit();
		}
		yield break;
	}

	public void OnSelect_QUIT()
	{
		if (this.quitAction != null)
		{
			this.quitAction();
		}
	}

	public void OnSelect_CONTINUE()
	{
		ReportErrorDialog.hasCrash = false;
		if (this.continueAction != null)
		{
			this.continueAction();
		}
	}

	public void OpenRefMessage()
	{
		this.submitButton.gameObject.SetActive(false);
		this.referenceMessage.SetActive(true);
	}

	public string UserMessage()
	{
		return this.messageInputField.text;
	}

	private void OnSelect_UPLOADSAVE()
	{
		this.uploadSaveDialog.SetActive(false);
		KCrashReporter.MOST_RECENT_SAVEFILE = ReportErrorDialog.MOST_RECENT_SAVEFILE;
		this.Submit();
	}

	private void OnSelect_SKIPUPLOADSAVE()
	{
		this.uploadSaveDialog.SetActive(false);
		KCrashReporter.MOST_RECENT_SAVEFILE = null;
		this.Submit();
	}

	private void Submit()
	{
		this.confirmAction();
		this.OpenRefMessage();
	}

	public static string MOST_RECENT_SAVEFILE;

	private global::System.Action confirmAction;

	private global::System.Action quitAction;

	private global::System.Action continueAction;

	public TMP_InputField messageInputField;

	public GameObject referenceMessage;

	[SerializeField]
	private KButton submitButton;

	[SerializeField]
	private KButton quitButton;

	[SerializeField]
	private KButton continueGameButton;

	[SerializeField]
	private LocText CrashLabel;

	[SerializeField]
	private LocText VCCrashLabel;

	[SerializeField]
	private Button VCLinkButton;

	[SerializeField]
	private GameObject InfoBox;

	[SerializeField]
	private GameObject uploadSaveDialog;

	[SerializeField]
	private KButton uploadSaveButton;

	[SerializeField]
	private KButton skipUploadSaveButton;

	[SerializeField]
	private LocText saveFileInfoLabel;

	public static bool hasCrash;
}
