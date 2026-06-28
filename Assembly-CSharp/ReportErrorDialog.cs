using System;
using System.Collections;
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
		ReportErrorDialog.hasCrash = true;
	}

	private void Update()
	{
		Debug.developerConsoleVisible = false;
	}

	private void OnDestroy()
	{
		if (KCrashReporter.terminateOnError)
		{
			Application.Quit();
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

	public void PopupConfirmDialog(string text, global::System.Action onConfirm, global::System.Action onQuit, global::System.Action onContinue, string third_text = null, global::System.Action onThird = null, bool is_vcruntime_error = false)
	{
		this.confirmAction = onConfirm;
		this.quitAction = onQuit;
		this.thirdAction = onThird;
		this.continueAction = onContinue;
		if (is_vcruntime_error)
		{
			this.CrashLabel.gameObject.SetActive(false);
			this.InfoBox.gameObject.SetActive(false);
			this.VCLinkButton.onClick.AddListener(delegate
			{
				Application.OpenURL("http://support.kleientertainment.com/customer/portal/articles/2747147");
			});
		}
		else
		{
			this.VCCrashLabel.gameObject.SetActive(false);
			this.VCLinkButton.gameObject.SetActive(false);
		}
		int num = 0;
		if (this.confirmAction != null)
		{
			num++;
		}
		if (this.quitAction != null)
		{
			num++;
		}
		if (this.thirdAction != null)
		{
			num++;
		}
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
		if (this.confirmAction != null)
		{
			this.confirmAction();
		}
		this.OpenRefMessage();
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

	public void OnSelect_third()
	{
		if (this.thirdAction != null)
		{
			this.thirdAction();
		}
		global::UnityEngine.Object.Destroy(base.gameObject);
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

	private global::System.Action confirmAction;

	private global::System.Action quitAction;

	private global::System.Action thirdAction;

	private global::System.Action continueAction;

	public Text popupMessage;

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

	public static bool hasCrash;
}
