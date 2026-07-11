using System;
using FMOD;
using FMOD.Studio;
using UnityEngine;

namespace FMODUnity
{
	public static class RuntimeUtils
	{
		public static string GetCommonPlatformPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return path;
			}
			return path.Replace('\\', '/');
		}

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

		public static void EnforceLibraryOrder()
		{
			int num;
			int num2;
			Memory.GetStats(out num, out num2, true);
			Guid guid;
			Util.parseID("", out guid);
		}

		public static void SetThreadAffinity(Action<RESULT, string> reportResult)
		{
			RESULT result = Thread.SetAttributes(THREAD_TYPE.MIXER, THREAD_AFFINITY.CORE_2, THREAD_PRIORITY.DEFAULT, THREAD_STACK_SIZE.DEFAULT);
			reportResult(result, "FMOD.Thread.SetAttributes(Mixer)");
			result = Thread.SetAttributes(THREAD_TYPE.STUDIO_UPDATE, THREAD_AFFINITY.CORE_4, THREAD_PRIORITY.DEFAULT, THREAD_STACK_SIZE.DEFAULT);
			reportResult(result, "FMOD.Thread.SetAttributes(Update)");
			result = Thread.SetAttributes(THREAD_TYPE.STUDIO_LOAD_BANK, THREAD_AFFINITY.CORE_4, THREAD_PRIORITY.DEFAULT, THREAD_STACK_SIZE.DEFAULT);
			reportResult(result, "FMOD.Thread.SetAttributes(Load_Bank)");
			result = Thread.SetAttributes(THREAD_TYPE.STUDIO_LOAD_SAMPLE, THREAD_AFFINITY.CORE_4, THREAD_PRIORITY.DEFAULT, THREAD_STACK_SIZE.DEFAULT);
			reportResult(result, "FMOD.Thread.SetAttributes(Load_Sample)");
		}
	}
}
