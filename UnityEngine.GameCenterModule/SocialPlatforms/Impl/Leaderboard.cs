using System;

namespace UnityEngine.SocialPlatforms.Impl
{
	public class Leaderboard : ILeaderboard
	{
		public Leaderboard()
		{
			this.id = "Invalid";
			this.range = new Range(1, 10);
			this.userScope = UserScope.Global;
			this.timeScope = TimeScope.AllTime;
			this.m_Loading = false;
			this.m_LocalUserScore = new Score("Invalid", 0L);
			this.m_MaxRange = 0U;
			IScore[] array = new Score[0];
			this.m_Scores = array;
			this.m_Title = "Invalid";
			this.m_UserIDs = new string[0];
		}

		public void SetUserFilter(string[] userIDs)
		{
			this.m_UserIDs = userIDs;
		}

		public override string ToString()
		{
			string[] array = new string[20];
			array[0] = "ID: '";
			array[1] = this.id;
			array[2] = "' Title: '";
			array[3] = this.m_Title;
			array[4] = "' Loading: '";
			array[5] = this.m_Loading.ToString();
			array[6] = "' Range: [";
			int num = 7;
			Range range = this.range;
			array[num] = range.from.ToString();
			array[8] = ",";
			int num2 = 9;
			range = this.range;
			array[num2] = range.count.ToString();
			array[10] = "] MaxRange: '";
			array[11] = this.m_MaxRange.ToString();
			array[12] = "' Scores: '";
			array[13] = this.m_Scores.Length.ToString();
			array[14] = "' UserScope: '";
			array[15] = this.userScope.ToString();
			array[16] = "' TimeScope: '";
			array[17] = this.timeScope.ToString();
			array[18] = "' UserFilter: '";
			array[19] = this.m_UserIDs.Length.ToString();
			return string.Concat(array);
		}

		public void LoadScores(Action<bool> callback)
		{
			ActivePlatform.Instance.LoadScores(this, callback);
		}

		public bool loading
		{
			get
			{
				return ActivePlatform.Instance.GetLoading(this);
			}
		}

		public void SetLocalUserScore(IScore score)
		{
			this.m_LocalUserScore = score;
		}

		public void SetMaxRange(uint maxRange)
		{
			this.m_MaxRange = maxRange;
		}

		public void SetScores(IScore[] scores)
		{
			this.m_Scores = scores;
		}

		public void SetTitle(string title)
		{
			this.m_Title = title;
		}

		public string[] GetUserFilter()
		{
			return this.m_UserIDs;
		}

		public string id { get; set; }

		public UserScope userScope { get; set; }

		public Range range { get; set; }

		public TimeScope timeScope { get; set; }

		public IScore localUserScore
		{
			get
			{
				return this.m_LocalUserScore;
			}
		}

		public uint maxRange
		{
			get
			{
				return this.m_MaxRange;
			}
		}

		public IScore[] scores
		{
			get
			{
				return this.m_Scores;
			}
		}

		public string title
		{
			get
			{
				return this.m_Title;
			}
		}

		private bool m_Loading;

		private IScore m_LocalUserScore;

		private uint m_MaxRange;

		private IScore[] m_Scores;

		private string m_Title;

		private string[] m_UserIDs;
	}
}
