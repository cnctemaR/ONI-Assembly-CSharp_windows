using System;
using System.IO;
using STRINGS;
using UnityEngine;

namespace Klei
{
	public static class FileUtil
	{
		public static void ErrorDialog(string msg)
		{
			global::Debug.Log(msg);
			GameObject gameObject = ((!(FrontEndManager.Instance == null)) ? FrontEndManager.Instance.gameObject : GameScreenManager.Instance.ssOverlayCanvas);
			ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, gameObject, true).GetComponent<ConfirmDialogScreen>();
			component.PopupConfirmDialog(msg, null, null, null, null, null, null, null, null);
			global::UnityEngine.Object.DontDestroyOnLoad(component.gameObject);
		}

		public static FileStream Create(string filename)
		{
			try
			{
				return File.Create(filename);
			}
			catch (UnauthorizedAccessException)
			{
				FileUtil.ErrorDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNAUTHORIZED, filename));
			}
			catch (IOException)
			{
				FileUtil.ErrorDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_SUFFICIENT_SPACE, filename));
			}
			return null;
		}

		public static bool CreateDirectory(string path)
		{
			try
			{
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				return true;
			}
			catch (UnauthorizedAccessException)
			{
				FileUtil.ErrorDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNAUTHORIZED, path));
			}
			catch (IOException)
			{
				FileUtil.ErrorDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_SUFFICIENT_SPACE, path));
			}
			return false;
		}

		public static bool DeleteDirectory(string path)
		{
			if (!Directory.Exists(path))
			{
				return true;
			}
			try
			{
				Directory.Delete(path, true);
				return true;
			}
			catch (UnauthorizedAccessException)
			{
				FileUtil.ErrorDialog(string.Format(UI.FRONTEND.SUPPORTWARNINGS.IO_UNAUTHORIZED, path));
			}
			return false;
		}
	}
}
