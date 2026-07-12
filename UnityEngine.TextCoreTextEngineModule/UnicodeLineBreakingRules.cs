using System;
using System.Collections.Generic;

namespace UnityEngine.TextCore.Text
{
	[Serializable]
	public class UnicodeLineBreakingRules
	{
		public TextAsset lineBreakingRules
		{
			get
			{
				return this.m_UnicodeLineBreakingRules;
			}
		}

		public TextAsset leadingCharacters
		{
			get
			{
				return this.m_LeadingCharacters;
			}
		}

		public TextAsset followingCharacters
		{
			get
			{
				return this.m_FollowingCharacters;
			}
		}

		internal HashSet<uint> leadingCharactersLookup
		{
			get
			{
				bool flag = this.m_LeadingCharactersLookup == null;
				if (flag)
				{
					this.LoadLineBreakingRules(this.leadingCharacters, this.followingCharacters);
				}
				return this.m_LeadingCharactersLookup;
			}
			set
			{
				this.m_LeadingCharactersLookup = value;
			}
		}

		internal HashSet<uint> followingCharactersLookup
		{
			get
			{
				bool flag = this.m_LeadingCharactersLookup == null;
				if (flag)
				{
					this.LoadLineBreakingRules(this.leadingCharacters, this.followingCharacters);
				}
				return this.m_FollowingCharactersLookup;
			}
			set
			{
				this.m_FollowingCharactersLookup = value;
			}
		}

		public bool useModernHangulLineBreakingRules
		{
			get
			{
				return this.m_UseModernHangulLineBreakingRules;
			}
			set
			{
				this.m_UseModernHangulLineBreakingRules = value;
			}
		}

		internal void LoadLineBreakingRules()
		{
			bool flag = this.m_LeadingCharactersLookup == null;
			if (flag)
			{
				bool flag2 = this.m_LeadingCharacters == null;
				if (flag2)
				{
					this.m_LeadingCharacters = Resources.Load<TextAsset>("LineBreaking Leading Characters");
				}
				this.m_LeadingCharactersLookup = ((this.m_LeadingCharacters != null) ? UnicodeLineBreakingRules.GetCharacters(this.m_LeadingCharacters) : new HashSet<uint>());
				bool flag3 = this.m_FollowingCharacters == null;
				if (flag3)
				{
					this.m_FollowingCharacters = Resources.Load<TextAsset>("LineBreaking Following Characters");
				}
				this.m_FollowingCharactersLookup = ((this.m_FollowingCharacters != null) ? UnicodeLineBreakingRules.GetCharacters(this.m_FollowingCharacters) : new HashSet<uint>());
			}
		}

		internal void LoadLineBreakingRules(TextAsset leadingRules, TextAsset followingRules)
		{
			bool flag = this.m_LeadingCharactersLookup == null;
			if (flag)
			{
				bool flag2 = leadingRules == null;
				if (flag2)
				{
					leadingRules = Resources.Load<TextAsset>("LineBreaking Leading Characters");
				}
				this.m_LeadingCharactersLookup = ((leadingRules != null) ? UnicodeLineBreakingRules.GetCharacters(leadingRules) : new HashSet<uint>());
				bool flag3 = followingRules == null;
				if (flag3)
				{
					followingRules = Resources.Load<TextAsset>("LineBreaking Following Characters");
				}
				this.m_FollowingCharactersLookup = ((followingRules != null) ? UnicodeLineBreakingRules.GetCharacters(followingRules) : new HashSet<uint>());
			}
		}

		private static HashSet<uint> GetCharacters(TextAsset file)
		{
			HashSet<uint> hashSet = new HashSet<uint>();
			string text = file.text;
			for (int i = 0; i < text.Length; i++)
			{
				hashSet.Add((uint)text[i]);
			}
			return hashSet;
		}

		[SerializeField]
		private TextAsset m_UnicodeLineBreakingRules;

		[SerializeField]
		private TextAsset m_LeadingCharacters;

		[SerializeField]
		private TextAsset m_FollowingCharacters;

		[SerializeField]
		private bool m_UseModernHangulLineBreakingRules;

		private HashSet<uint> m_LeadingCharactersLookup;

		private HashSet<uint> m_FollowingCharactersLookup;
	}
}
