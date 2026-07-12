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

		public static ATTRIBUTES_3D To3DAttributes(this GameObject go)
		{
			return go.transform.To3DAttributes();
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

		public static ATTRIBUTES_3D To3DAttributes(GameObject go, Rigidbody rigidbody)
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

		public static THREAD_TYPE ToFMODThreadType(ThreadType threadType)
		{
			switch (threadType)
			{
			case ThreadType.Mixer:
				return THREAD_TYPE.MIXER;
			case ThreadType.Feeder:
				return THREAD_TYPE.FEEDER;
			case ThreadType.Stream:
				return THREAD_TYPE.STREAM;
			case ThreadType.File:
				return THREAD_TYPE.FILE;
			case ThreadType.Nonblocking:
				return THREAD_TYPE.NONBLOCKING;
			case ThreadType.Record:
				return THREAD_TYPE.RECORD;
			case ThreadType.Geometry:
				return THREAD_TYPE.GEOMETRY;
			case ThreadType.Profiler:
				return THREAD_TYPE.PROFILER;
			case ThreadType.Studio_Update:
				return THREAD_TYPE.STUDIO_UPDATE;
			case ThreadType.Studio_Load_Bank:
				return THREAD_TYPE.STUDIO_LOAD_BANK;
			case ThreadType.Studio_Load_Sample:
				return THREAD_TYPE.STUDIO_LOAD_SAMPLE;
			case ThreadType.Convolution_1:
				return THREAD_TYPE.CONVOLUTION1;
			case ThreadType.Convolution_2:
				return THREAD_TYPE.CONVOLUTION2;
			default:
				throw new ArgumentException("Unrecognised thread type '" + threadType.ToString() + "'");
			}
		}

		public static string DisplayName(this ThreadType thread)
		{
			return thread.ToString().Replace('_', ' ');
		}

		public static THREAD_AFFINITY ToFMODThreadAffinity(ThreadAffinity affinity)
		{
			THREAD_AFFINITY thread_AFFINITY = THREAD_AFFINITY.CORE_ALL;
			RuntimeUtils.SetFMODAffinityBit(affinity, ThreadAffinity.Core0, THREAD_AFFINITY.CORE_0, ref thread_AFFINITY);
			RuntimeUtils.SetFMODAffinityBit(affinity, ThreadAffinity.Core1, THREAD_AFFINITY.CORE_1, ref thread_AFFINITY);
			RuntimeUtils.SetFMODAffinityBit(affinity, ThreadAffinity.Core2, THREAD_AFFINITY.CORE_2, ref thread_AFFINITY);
			RuntimeUtils.SetFMODAffinityBit(affinity, ThreadAffinity.Core3, THREAD_AFFINITY.CORE_3, ref thread_AFFINITY);
			RuntimeUtils.SetFMODAffinityBit(affinity, ThreadAffinity.Core4, THREAD_AFFINITY.CORE_4, ref thread_AFFINITY);
			RuntimeUtils.SetFMODAffinityBit(affinity, ThreadAffinity.Core5, THREAD_AFFINITY.CORE_5, ref thread_AFFINITY);
			RuntimeUtils.SetFMODAffinityBit(affinity, ThreadAffinity.Core6, THREAD_AFFINITY.CORE_6, ref thread_AFFINITY);
			RuntimeUtils.SetFMODAffinityBit(affinity, ThreadAffinity.Core7, THREAD_AFFINITY.CORE_7, ref thread_AFFINITY);
			RuntimeUtils.SetFMODAffinityBit(affinity, ThreadAffinity.Core8, THREAD_AFFINITY.CORE_8, ref thread_AFFINITY);
			RuntimeUtils.SetFMODAffinityBit(affinity, ThreadAffinity.Core9, THREAD_AFFINITY.CORE_9, ref thread_AFFINITY);
			RuntimeUtils.SetFMODAffinityBit(affinity, ThreadAffinity.Core10, THREAD_AFFINITY.CORE_10, ref thread_AFFINITY);
			RuntimeUtils.SetFMODAffinityBit(affinity, ThreadAffinity.Core11, THREAD_AFFINITY.CORE_11, ref thread_AFFINITY);
			RuntimeUtils.SetFMODAffinityBit(affinity, ThreadAffinity.Core12, THREAD_AFFINITY.CORE_12, ref thread_AFFINITY);
			RuntimeUtils.SetFMODAffinityBit(affinity, ThreadAffinity.Core13, THREAD_AFFINITY.CORE_13, ref thread_AFFINITY);
			RuntimeUtils.SetFMODAffinityBit(affinity, ThreadAffinity.Core14, THREAD_AFFINITY.CORE_14, ref thread_AFFINITY);
			RuntimeUtils.SetFMODAffinityBit(affinity, ThreadAffinity.Core15, THREAD_AFFINITY.CORE_15, ref thread_AFFINITY);
			return thread_AFFINITY;
		}

		private static void SetFMODAffinityBit(ThreadAffinity affinity, ThreadAffinity mask, THREAD_AFFINITY fmodMask, ref THREAD_AFFINITY fmodAffinity)
		{
			if ((affinity & mask) != ThreadAffinity.Any)
			{
				fmodAffinity |= fmodMask;
			}
		}

		public static void EnforceLibraryOrder()
		{
			int num;
			int num2;
			Memory.GetStats(out num, out num2, true);
			Guid guid;
			Util.parseID("", out guid);
		}
	}
}
