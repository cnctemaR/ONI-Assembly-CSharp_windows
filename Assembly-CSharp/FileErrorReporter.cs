using System;
using Klei;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/FileErrorReporter")]
public class FileErrorReporter : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		this.OnFileError();
		FileUtil.onErrorMessage += this.OnFileError;
	}

	private void OnFileError()
	{
		if (FileUtil.errorType == FileUtil.ErrorType.None)
		{
			return;
		}
		string text;
		switch (FileUtil.errorType)
		{
		case FileUtil.ErrorType.UnauthorizedAccess:
			text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNAUTHORIZED, FileUtil.errorSubject);
			goto IL_0065;
		case FileUtil.ErrorType.IOError:
			text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_SUFFICIENT_SPACE, FileUtil.errorSubject);
			goto IL_0065;
		}
		text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNKNOWN, FileUtil.errorSubject);
		IL_0065:
		GameObject gameObject;
		if (FrontEndManager.Instance != null)
		{
			gameObject = FrontEndManager.Instance.gameObject;
		}
		else if (GameScreenManager.Instance != null && GameScreenManager.Instance.ssOverlayCanvas != null)
		{
			gameObject = GameScreenManager.Instance.ssOverlayCanvas;
		}
		else
		{
			gameObject = new GameObject();
			gameObject.name = "FileErrorCanvas";
			global::UnityEngine.Object.DontDestroyOnLoad(gameObject);
			Canvas canvas = gameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
			canvas.sortingOrder = 10;
			gameObject.AddComponent<GraphicRaycaster>();
		}
		if ((FileUtil.exceptionMessage != null || FileUtil.exceptionStackTrace != null) && !KCrashReporter.hasReportedError)
		{
			KCrashReporter.ReportError(FileUtil.exceptionMessage, FileUtil.exceptionStackTrace, null, null, null, "");
		}
		ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, gameObject, true).GetComponent<ConfirmDialogScreen>();
		component.PopupConfirmDialog(text, null, null, null, null, null, null, null, null, true);
		global::UnityEngine.Object.DontDestroyOnLoad(component.gameObject);
	}

	private void OpenMoreInfo()
	{
	}
}
