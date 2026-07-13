using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.SceneManagement
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/SceneManager/UnitySceneHandle.h")]
	[NativeClass("UnitySceneHandle")]
	[Serializable]
	public struct SceneHandle : IEquatable<SceneHandle>
	{
		public static SceneHandle None
		{
			get
			{
				return default(SceneHandle);
			}
		}

		internal static SceneHandle From(EntityId entityId)
		{
			return new SceneHandle
			{
				m_Value = entityId
			};
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is SceneHandle)
			{
				SceneHandle sceneHandle = (SceneHandle)obj;
				flag = this.Equals(sceneHandle);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public bool Equals(SceneHandle other)
		{
			return this.m_Value == other.m_Value;
		}

		public static bool operator ==(SceneHandle left, SceneHandle right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(SceneHandle left, SceneHandle right)
		{
			return !left.Equals(right);
		}

		public static implicit operator int(SceneHandle handle)
		{
			return handle.m_Value;
		}

		public static implicit operator SceneHandle(int handle)
		{
			return SceneHandle.From(handle);
		}

		public static implicit operator uint(SceneHandle handle)
		{
			return (uint)handle.m_Value;
		}

		public static implicit operator SceneHandle(uint handle)
		{
			return SceneHandle.From((int)handle);
		}

		public override int GetHashCode()
		{
			return this.m_Value.GetHashCode();
		}

		public override string ToString()
		{
			return this.m_Value.ToString();
		}

		public string ToString(string format)
		{
			return this.m_Value.ToString(format);
		}

		internal EntityId ToEntityId()
		{
			return this.m_Value;
		}

		internal EntityId m_Value;
	}
}
