using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Director
{
	[UsedByNativeCode]
	public struct ScriptPlayableOutput
	{
		public static ScriptPlayableOutput Null
		{
			get
			{
				return new ScriptPlayableOutput
				{
					m_Output = new PlayableOutput
					{
						m_Version = 69
					}
				};
			}
		}

		internal Object referenceObject
		{
			get
			{
				return PlayableOutput.GetInternalReferenceObject(ref this.m_Output);
			}
			set
			{
				PlayableOutput.SetInternalReferenceObject(ref this.m_Output, value);
			}
		}

		public Object userData
		{
			get
			{
				return PlayableOutput.GetInternalUserData(ref this.m_Output);
			}
			set
			{
				PlayableOutput.SetInternalUserData(ref this.m_Output, value);
			}
		}

		public bool IsValid()
		{
			return PlayableOutput.IsValidInternal(ref this.m_Output);
		}

		public PlayableHandle sourcePlayable
		{
			get
			{
				return PlayableOutput.InternalGetSourcePlayable(ref this.m_Output);
			}
			set
			{
				PlayableOutput.InternalSetSourcePlayable(ref this.m_Output, ref value);
			}
		}

		internal PlayableOutput m_Output;
	}
}
