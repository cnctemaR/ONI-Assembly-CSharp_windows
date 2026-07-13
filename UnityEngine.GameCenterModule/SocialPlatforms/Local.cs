using System;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;

namespace UnityEngine.SocialPlatforms
{
	[Obsolete("Local is deprecated and will be removed in a future release.", false)]
	public class Local : ISocialPlatform
	{
		public ILocalUser localUser
		{
			get
			{
				bool flag = Local.m_LocalUser == null;
				if (flag)
				{
					Local.m_LocalUser = new LocalUser();
				}
				return Local.m_LocalUser;
			}
		}

		void ISocialPlatform.Authenticate(ILocalUser user, Action<bool> callback)
		{
			LocalUser localUser = (LocalUser)user;
			this.m_DefaultTexture = this.CreateDummyTexture(32, 32);
			this.PopulateStaticData();
			localUser.SetAuthenticated(true);
			localUser.SetUnderage(false);
			localUser.SetUserID("1000");
			localUser.SetUserName("Lerpz");
			localUser.SetImage(this.m_DefaultTexture);
			bool flag = callback != null;
			if (flag)
			{
				callback(true);
			}
		}

		void ISocialPlatform.Authenticate(ILocalUser user, Action<bool, string> callback)
		{
			((ISocialPlatform)this).Authenticate(user, delegate(bool success)
			{
				callback(success, null);
			});
		}

		void ISocialPlatform.LoadFriends(ILocalUser user, Action<bool> callback)
		{
			bool flag = !this.VerifyUser();
			if (!flag)
			{
				LocalUser localUser = (LocalUser)user;
				IUserProfile[] array = this.m_Friends.ToArray();
				localUser.SetFriends(array);
				bool flag2 = callback != null;
				if (flag2)
				{
					callback(true);
				}
			}
		}

		public void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
		{
			List<UserProfile> list = new List<UserProfile>();
			bool flag = !this.VerifyUser();
			if (!flag)
			{
				foreach (string text in userIDs)
				{
					foreach (UserProfile userProfile in this.m_Users)
					{
						bool flag2 = userProfile.id == text;
						if (flag2)
						{
							list.Add(userProfile);
						}
					}
					foreach (UserProfile userProfile2 in this.m_Friends)
					{
						bool flag3 = userProfile2.id == text;
						if (flag3)
						{
							list.Add(userProfile2);
						}
					}
				}
				IUserProfile[] array = list.ToArray();
				callback(array);
			}
		}

		public void ReportProgress(string id, double progress, Action<bool> callback)
		{
			bool flag = !this.VerifyUser();
			if (!flag)
			{
				foreach (Achievement achievement in this.m_Achievements)
				{
					bool flag2 = achievement.id == id && achievement.percentCompleted <= progress;
					if (flag2)
					{
						bool flag3 = progress >= 100.0;
						if (flag3)
						{
							achievement.SetCompleted(true);
						}
						achievement.SetHidden(false);
						achievement.SetLastReportedDate(DateTime.Now);
						achievement.percentCompleted = progress;
						bool flag4 = callback != null;
						if (flag4)
						{
							callback(true);
						}
						return;
					}
				}
				foreach (AchievementDescription achievementDescription in this.m_AchievementDescriptions)
				{
					bool flag5 = achievementDescription.id == id;
					if (flag5)
					{
						bool flag6 = progress >= 100.0;
						Achievement achievement2 = new Achievement(id, progress, flag6, false, DateTime.Now);
						this.m_Achievements.Add(achievement2);
						bool flag7 = callback != null;
						if (flag7)
						{
							callback(true);
						}
						return;
					}
				}
				Debug.LogError("Achievement ID not found");
				bool flag8 = callback != null;
				if (flag8)
				{
					callback(false);
				}
			}
		}

		public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
		{
			bool flag = !this.VerifyUser();
			if (!flag)
			{
				bool flag2 = callback != null;
				if (flag2)
				{
					IAchievementDescription[] array = this.m_AchievementDescriptions.ToArray();
					callback(array);
				}
			}
		}

		public void LoadAchievements(Action<IAchievement[]> callback)
		{
			bool flag = !this.VerifyUser();
			if (!flag)
			{
				bool flag2 = callback != null;
				if (flag2)
				{
					IAchievement[] array = this.m_Achievements.ToArray();
					callback(array);
				}
			}
		}

		public void ReportScore(long score, string board, Action<bool> callback)
		{
			bool flag = !this.VerifyUser();
			if (!flag)
			{
				foreach (Leaderboard leaderboard in this.m_Leaderboards)
				{
					bool flag2 = leaderboard.id == board;
					if (flag2)
					{
						List<Score> list = new List<Score>((Score[])leaderboard.scores);
						list.Add(new Score(board, score, this.localUser.id, DateTime.Now, score.ToString() + " points", 0));
						Leaderboard leaderboard2 = leaderboard;
						IScore[] array = list.ToArray();
						leaderboard2.SetScores(array);
						bool flag3 = callback != null;
						if (flag3)
						{
							callback(true);
						}
						return;
					}
				}
				Debug.LogError("Leaderboard not found");
				bool flag4 = callback != null;
				if (flag4)
				{
					callback(false);
				}
			}
		}

		public void LoadScores(string leaderboardID, Action<IScore[]> callback)
		{
			bool flag = !this.VerifyUser();
			if (!flag)
			{
				foreach (Leaderboard leaderboard in this.m_Leaderboards)
				{
					bool flag2 = leaderboard.id == leaderboardID;
					if (flag2)
					{
						this.SortScores(leaderboard);
						bool flag3 = callback != null;
						if (flag3)
						{
							callback(leaderboard.scores);
						}
						return;
					}
				}
				Debug.LogError("Leaderboard not found");
				bool flag4 = callback != null;
				if (flag4)
				{
					IScore[] array = new Score[0];
					callback(array);
				}
			}
		}

		void ISocialPlatform.LoadScores(ILeaderboard board, Action<bool> callback)
		{
			bool flag = !this.VerifyUser();
			if (!flag)
			{
				Leaderboard leaderboard = (Leaderboard)board;
				foreach (Leaderboard leaderboard2 in this.m_Leaderboards)
				{
					bool flag2 = leaderboard2.id == leaderboard.id;
					if (flag2)
					{
						leaderboard.SetTitle(leaderboard2.title);
						leaderboard.SetScores(leaderboard2.scores);
						leaderboard.SetMaxRange((uint)leaderboard2.scores.Length);
					}
				}
				this.SortScores(leaderboard);
				this.SetLocalPlayerScore(leaderboard);
				bool flag3 = callback != null;
				if (flag3)
				{
					callback(true);
				}
			}
		}

		bool ISocialPlatform.GetLoading(ILeaderboard board)
		{
			bool flag = !this.VerifyUser();
			return !flag && ((Leaderboard)board).loading;
		}

		private void SortScores(Leaderboard board)
		{
			List<Score> list = new List<Score>((Score[])board.scores);
			list.Sort((Score s1, Score s2) => s2.value.CompareTo(s1.value));
			for (int i = 0; i < list.Count; i++)
			{
				list[i].SetRank(i + 1);
			}
		}

		private void SetLocalPlayerScore(Leaderboard board)
		{
			foreach (Score score in board.scores)
			{
				bool flag = score.userID == this.localUser.id;
				if (flag)
				{
					board.SetLocalUserScore(score);
					break;
				}
			}
		}

		public void ShowAchievementsUI()
		{
			Debug.Log("ShowAchievementsUI not implemented");
		}

		public void ShowLeaderboardUI()
		{
			Debug.Log("ShowLeaderboardUI not implemented");
		}

		public ILeaderboard CreateLeaderboard()
		{
			return new Leaderboard();
		}

		public IAchievement CreateAchievement()
		{
			return new Achievement();
		}

		private bool VerifyUser()
		{
			bool flag = !this.localUser.authenticated;
			bool flag2;
			if (flag)
			{
				Debug.LogError("Must authenticate first");
				flag2 = false;
			}
			else
			{
				flag2 = true;
			}
			return flag2;
		}

		private void PopulateStaticData()
		{
			this.m_Friends.Add(new UserProfile("Fred", "1001", true, UserState.Online, this.m_DefaultTexture));
			this.m_Friends.Add(new UserProfile("Julia", "1002", true, UserState.Online, this.m_DefaultTexture));
			this.m_Friends.Add(new UserProfile("Jeff", "1003", true, UserState.Online, this.m_DefaultTexture));
			this.m_Users.Add(new UserProfile("Sam", "1004", false, UserState.Offline, this.m_DefaultTexture));
			this.m_Users.Add(new UserProfile("Max", "1005", false, UserState.Offline, this.m_DefaultTexture));
			this.m_AchievementDescriptions.Add(new AchievementDescription("Achievement01", "First achievement", this.m_DefaultTexture, "Get first achievement", "Received first achievement", false, 10));
			this.m_AchievementDescriptions.Add(new AchievementDescription("Achievement02", "Second achievement", this.m_DefaultTexture, "Get second achievement", "Received second achievement", false, 20));
			this.m_AchievementDescriptions.Add(new AchievementDescription("Achievement03", "Third achievement", this.m_DefaultTexture, "Get third achievement", "Received third achievement", false, 15));
			Leaderboard leaderboard = new Leaderboard();
			leaderboard.SetTitle("High Scores");
			leaderboard.id = "Leaderboard01";
			List<Score> list = new List<Score>();
			list.Add(new Score("Leaderboard01", 300L, "1001", DateTime.Now.AddDays(-1.0), "300 points", 1));
			list.Add(new Score("Leaderboard01", 255L, "1002", DateTime.Now.AddDays(-1.0), "255 points", 2));
			list.Add(new Score("Leaderboard01", 55L, "1003", DateTime.Now.AddDays(-1.0), "55 points", 3));
			list.Add(new Score("Leaderboard01", 10L, "1004", DateTime.Now.AddDays(-1.0), "10 points", 4));
			Leaderboard leaderboard2 = leaderboard;
			IScore[] array = list.ToArray();
			leaderboard2.SetScores(array);
			this.m_Leaderboards.Add(leaderboard);
		}

		private Texture2D CreateDummyTexture(int width, int height)
		{
			Texture2D texture2D = new Texture2D(width, height);
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					Color color = (((j & i) > 0) ? Color.white : Color.gray);
					texture2D.SetPixel(j, i, color);
				}
			}
			texture2D.Apply();
			return texture2D;
		}

		private static LocalUser m_LocalUser;

		private List<UserProfile> m_Friends = new List<UserProfile>();

		private List<UserProfile> m_Users = new List<UserProfile>();

		private List<AchievementDescription> m_AchievementDescriptions = new List<AchievementDescription>();

		private List<Achievement> m_Achievements = new List<Achievement>();

		private List<Leaderboard> m_Leaderboards = new List<Leaderboard>();

		private Texture2D m_DefaultTexture;
	}
}
