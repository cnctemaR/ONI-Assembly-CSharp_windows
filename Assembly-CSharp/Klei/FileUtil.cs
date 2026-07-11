using System;
using System.IO;
using STRINGS;
using UnityEngine;

namespace Klei
{
	public static class FileUtil
	{
		public static FileStream Create(string filename)
		{
			FileStream fileStream = null;
			try
			{
				fileStream = File.Create(filename);
			}
			catch (Exception ex)
			{
				string text = null;
				if (ex is UnauthorizedAccessException)
				{
					text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNAUTHORIZED, filename);
				}
				else if (ex is IOException)
				{
					text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_SUFFICIENT_SPACE, filename);
				}
				if (text == null)
				{
					throw ex;
				}
				GameObject gameObject = ((!(FrontEndManager.Instance == null)) ? FrontEndManager.Instance.gameObject : GameScreenManager.Instance.ssOverlayCanvas);
				ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, gameObject, true).GetComponent<ConfirmDialogScreen>();
				component.PopupConfirmDialog(text, null, null, null, null, null, null, null, null);
				global::UnityEngine.Object.DontDestroyOnLoad(component.gameObject);
			}
			return fileStream;
		}

		public static bool CreateDirectory(string path)
		{
			bool flag = false;
			try
			{
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				flag = true;
			}
			catch (Exception ex)
			{
				string text = null;
				if (ex is UnauthorizedAccessException)
				{
					text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNAUTHORIZED, path);
				}
				else if (ex is IOException)
				{
					text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_SUFFICIENT_SPACE, path);
				}
				if (text == null)
				{
					throw ex;
				}
				GameObject gameObject = ((!(FrontEndManager.Instance == null)) ? FrontEndManager.Instance.gameObject : GameScreenManager.Instance.ssOverlayCanvas);
				ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, gameObject, true).GetComponent<ConfirmDialogScreen>();
				component.PopupConfirmDialog(text, null, null, null, null, null, null, null, null);
				global::UnityEngine.Object.DontDestroyOnLoad(component.gameObject);
			}
			return flag;
		}

		public static bool DeleteDirectory(string path)
		{
			if (!Directory.Exists(path))
			{
				return true;
			}
			bool flag;
			try
			{
				Directory.Delete(path, true);
				flag = true;
			}
			catch (Exception ex)
			{
				string text = null;
				if (ex is UnauthorizedAccessException)
				{
					text = string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNAUTHORIZED, path);
				}
				if (text == null)
				{
					throw ex;
				}
				GameObject gameObject = ((!(FrontEndManager.Instance == null)) ? FrontEndManager.Instance.gameObject : GameScreenManager.Instance.ssOverlayCanvas);
				ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, gameObject, true).GetComponent<ConfirmDialogScreen>();
				component.PopupConfirmDialog(text, null, null, null, null, null, null, null, null);
				global::UnityEngine.Object.DontDestroyOnLoad(component.gameObject);
				flag = false;
			}
			return flag;
		}
	}
}
