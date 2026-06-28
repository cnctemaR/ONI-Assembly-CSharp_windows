using System;
using System.IO;
using FMOD;
using FMOD.Studio;
using UnityEngine;

namespace FMODUnity
{
	public static class RuntimeUtils
	{
		public static VECTOR ToFMODVector(this Vector3 vec)
		{
			VECTOR vector;
			vector.x = vec.x;
			vector.y = vec.y;
			vector.z = vec.z;
			return vector;
		}

		public static ATTRIBUTES_3D To3DAttributes(this Vector3 pos)
		{
			return new ATTRIBUTES_3D
			{
				forward = Vector3.forward.ToFMODVector(),
				up = Vector3.up.ToFMODVector(),
				position = pos.ToFMODVector()
			};
		}

		public static ATTRIBUTES_3D To3DAttributes(this Transform transform)
		{
			return new ATTRIBUTES_3D
			{
				forward = transform.forward.ToFMODVector(),
				up = transform.up.ToFMODVector(),
				position = transform.position.ToFMODVector()
			};
		}

		public static ATTRIBUTES_3D To3DAttributes(Transform transform, Rigidbody rigidbody = null)
		{
			ATTRIBUTES_3D attributes_3D = transform.To3DAttributes();
			if (rigidbody)
			{
				attributes_3D.velocity = rigidbody.velocity.ToFMODVector();
			}
			return attributes_3D;
		}

		public static ATTRIBUTES_3D To3DAttributes(GameObject go, Rigidbody rigidbody = null)
		{
			ATTRIBUTES_3D attributes_3D = go.transform.To3DAttributes();
			if (rigidbody)
			{
				attributes_3D.velocity = rigidbody.velocity.ToFMODVector();
			}
			return attributes_3D;
		}

		public static ATTRIBUTES_3D To3DAttributes(Transform transform, Rigidbody2D rigidbody)
		{
			ATTRIBUTES_3D attributes_3D = transform.To3DAttributes();
			if (rigidbody)
			{
				VECTOR vector;
				vector.x = rigidbody.velocity.x;
				vector.y = rigidbody.velocity.y;
				vector.z = 0f;
				attributes_3D.velocity = vector;
			}
			return attributes_3D;
		}

		public static ATTRIBUTES_3D To3DAttributes(GameObject go, Rigidbody2D rigidbody)
		{
			ATTRIBUTES_3D attributes_3D = go.transform.To3DAttributes();
			if (rigidbody)
			{
				VECTOR vector;
				vector.x = rigidbody.velocity.x;
				vector.y = rigidbody.velocity.y;
				vector.z = 0f;
				attributes_3D.velocity = vector;
			}
			return attributes_3D;
		}

		internal static FMODPlatform GetCurrentPlatform()
		{
			return FMODPlatform.Windows;
		}

		internal static string GetBankPath(string bankName)
		{
			string streamingAssetsPath = Application.streamingAssetsPath;
			if (Path.GetExtension(bankName) != ".bank")
			{
				return string.Format("{0}/{1}.bank", streamingAssetsPath, bankName);
			}
			return string.Format("{0}/{1}", streamingAssetsPath, bankName);
		}

		internal static string GetPluginPath(string pluginName)
		{
			string text = pluginName + ".dll";
			string text2 = Application.dataPath + "/Plugins/";
			return text2 + text;
		}

		public static void EnforceLibraryOrder()
		{
			int num;
			int num2;
			Memory.GetStats(out num, out num2);
			Guid guid;
			global::FMOD.Studio.Util.ParseID(string.Empty, out guid);
		}

		public const string LogFileName = "fmod.log";

		private const string BankExtension = ".bank";
	}
}
