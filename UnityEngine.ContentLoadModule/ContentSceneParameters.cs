using System;
using UnityEngine.Bindings;
using UnityEngine.SceneManagement;

namespace Unity.Loading
{
	public struct ContentSceneParameters
	{
		public LoadSceneMode loadSceneMode
		{
			get
			{
				return this.m_LoadSceneMode;
			}
			set
			{
				this.m_LoadSceneMode = value;
			}
		}

		public LocalPhysicsMode localPhysicsMode
		{
			get
			{
				return this.m_LocalPhysicsMode;
			}
			set
			{
				this.m_LocalPhysicsMode = value;
			}
		}

		public bool autoIntegrate
		{
			get
			{
				return this.m_AutoIntegrate;
			}
			set
			{
				this.m_AutoIntegrate = value;
			}
		}

		[NativeName("LoadSceneMode")]
		internal LoadSceneMode m_LoadSceneMode;

		[NativeName("LocalPhysicsMode")]
		internal LocalPhysicsMode m_LocalPhysicsMode;

		[NativeName("AutoIntegrate")]
		internal bool m_AutoIntegrate;
	}
}
