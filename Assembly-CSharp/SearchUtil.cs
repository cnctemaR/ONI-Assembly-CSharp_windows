using System;
using System.Collections.Generic;
using System.Text;
using Database;
using STRINGS;

public static class SearchUtil
{
	private static void CacheMeaninglessWords()
	{
		if (SearchUtil.MeaninglessWords.Count != 0)
		{
			return;
		}
		ListPool<string, SearchUtil.MatchCache>.PooledList pooledList = ListPool<string, SearchUtil.MatchCache>.Allocate();
		SearchUtil.AddCommaDelimitedSearchTerms(SEARCH_TERMS.SUPPRESSED, pooledList);
		foreach (string text in pooledList)
		{
			SearchUtil.MeaninglessWords.Add(text);
		}
		pooledList.Recycle();
	}

	public static bool IsPassingScore(int score)
	{
		return score >= 79;
	}

	public static string Canonicalize(string s)
	{
		return FuzzySearch.Canonicalize(s).ToUpper();
	}

	public static string CanonicalizePhrase(string s)
	{
		string text = FuzzySearch.Canonicalize(s).ToUpper();
		FuzzySearch.Features features = FuzzySearch.GetFeatures();
		if ((features & (FuzzySearch.Features.Suppress1And2LetterWords | FuzzySearch.Features.SuppressMeaninglessWords)) == (FuzzySearch.Features)0)
		{
			return text;
		}
		string[] array = text.Split(FuzzySearch.TOKEN_SEPARATORS);
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = (features & FuzzySearch.Features.Suppress1And2LetterWords) > (FuzzySearch.Features)0;
		bool flag2 = (features & FuzzySearch.Features.SuppressMeaninglessWords) > (FuzzySearch.Features)0;
		if (flag2)
		{
			SearchUtil.CacheMeaninglessWords();
		}
		foreach (string text2 in array)
		{
			if ((!flag || text2.Length > 2) && (!flag2 || !SearchUtil.MeaninglessWords.Contains(text2)))
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.AppendFormat(" {0}", text2);
				}
				else
				{
					stringBuilder.Append(text2);
				}
			}
		}
		return stringBuilder.ToString();
	}

	public static void AddCommaDelimitedSearchTerms(string commaDelimitedSearchTerms, List<string> searchTerms)
	{
		foreach (string text in commaDelimitedSearchTerms.ToUpper().Split(SearchUtil.COMMA_DELIMETERS, StringSplitOptions.RemoveEmptyEntries))
		{
			searchTerms.Add(text);
		}
	}

	public static Dictionary<string, SearchUtil.TechCache> CacheTechs()
	{
		Dictionary<string, SearchUtil.TechCache> dictionary = new Dictionary<string, SearchUtil.TechCache>();
		ListPool<ComplexRecipe, SearchUtil.TechCache>.PooledList pooledList = ListPool<ComplexRecipe, SearchUtil.TechCache>.Allocate();
		Techs techs = Db.Get().Techs;
		for (int num = 0; num != techs.Count; num++)
		{
			Tech tech = (Tech)techs.GetResource(num);
			Dictionary<string, SearchUtil.TechItemCache> dictionary2 = new Dictionary<string, SearchUtil.TechItemCache>();
			foreach (TechItem techItem in tech.unlockedItems)
			{
				pooledList.Clear();
				BuildingDef.CollectFabricationRecipes(techItem.Id, pooledList);
				List<SearchUtil.NameDescCache> list = new List<SearchUtil.NameDescCache>();
				foreach (ComplexRecipe complexRecipe in pooledList)
				{
					list.Add(new SearchUtil.NameDescCache
					{
						name = new SearchUtil.MatchCache
						{
							text = SearchUtil.Canonicalize(complexRecipe.GetUIName(false))
						},
						desc = new SearchUtil.MatchCache
						{
							text = SearchUtil.CanonicalizePhrase(complexRecipe.description)
						}
					});
				}
				TechItem techItem2 = Db.Get().TechItems.Get(techItem.Id);
				SearchUtil.TechItemCache techItemCache = new SearchUtil.TechItemCache
				{
					nameDescSearchTerms = new SearchUtil.NameDescSearchTermsCache
					{
						nameDesc = new SearchUtil.NameDescCache
						{
							name = new SearchUtil.MatchCache
							{
								text = SearchUtil.Canonicalize(techItem2.Name)
							},
							desc = new SearchUtil.MatchCache
							{
								text = SearchUtil.CanonicalizePhrase(techItem2.description)
							}
						},
						searchTerms = techItem2.searchTerms
					},
					recipes = list,
					tier = tech.tier
				};
				dictionary2[techItem.Id] = techItemCache;
			}
			SearchUtil.TechCache techCache = new SearchUtil.TechCache
			{
				tech = new SearchUtil.NameDescSearchTermsCache
				{
					nameDesc = new SearchUtil.NameDescCache
					{
						name = new SearchUtil.MatchCache
						{
							text = SearchUtil.Canonicalize(tech.Name)
						},
						desc = new SearchUtil.MatchCache
						{
							text = SearchUtil.CanonicalizePhrase(tech.desc)
						}
					},
					searchTerms = tech.searchTerms
				},
				techItems = dictionary2,
				tier = tech.tier
			};
			dictionary[tech.Id] = techCache;
		}
		pooledList.Recycle();
		return dictionary;
	}

	public static SearchUtil.BuildingDefCache MakeBuildingDefCache(BuildingDef def)
	{
		SearchUtil.NameDescSearchTermsCache nameDescSearchTermsCache = new SearchUtil.NameDescSearchTermsCache
		{
			nameDesc = new SearchUtil.NameDescCache
			{
				name = new SearchUtil.MatchCache
				{
					text = SearchUtil.Canonicalize(def.Name)
				},
				desc = new SearchUtil.MatchCache
				{
					text = SearchUtil.CanonicalizePhrase(def.Desc)
				}
			},
			searchTerms = def.SearchTerms
		};
		SearchUtil.MatchCache matchCache = new SearchUtil.MatchCache
		{
			text = SearchUtil.CanonicalizePhrase(def.Effect)
		};
		List<SearchUtil.NameDescCache> list = new List<SearchUtil.NameDescCache>();
		ListPool<ComplexRecipe, PlanBuildingToggle>.PooledList pooledList = ListPool<ComplexRecipe, PlanBuildingToggle>.Allocate();
		BuildingDef.CollectFabricationRecipes(def.PrefabID, pooledList);
		foreach (ComplexRecipe complexRecipe in pooledList)
		{
			list.Add(new SearchUtil.NameDescCache
			{
				name = new SearchUtil.MatchCache
				{
					text = SearchUtil.Canonicalize(complexRecipe.GetUIName(false))
				},
				desc = new SearchUtil.MatchCache
				{
					text = SearchUtil.CanonicalizePhrase(complexRecipe.description)
				}
			});
		}
		pooledList.Recycle();
		return new SearchUtil.BuildingDefCache
		{
			nameDescSearchTerms = nameDescSearchTermsCache,
			effect = matchCache,
			recipes = list,
			techTier = Db.Get().TechItems.GetTechTierForItem(def.PrefabID)
		};
	}

	public const int MATCH_SCORE_MIN = 0;

	public const int MATCH_SCORE_MAX = 100;

	public const int MATCH_SCORE_THRESHOLD = 79;

	private static readonly HashSet<string> MeaninglessWords = new HashSet<string>();

	private static readonly char[] COMMA_DELIMETERS = new char[] { ' ', ',' };

	private const int LHS_GT_RHS = -1;

	private const int RHS_GT_LHS = 1;

	private interface IScore
	{
		int Score { get; }
	}

	private struct TieBreaker
	{
		public TieBreaker(int _globalMax)
		{
			this.globalMax = _globalMax;
			this.globalMaxCmp = 0;
			this.localMaxScore = -1;
			this.localMaxCmp = 0;
		}

		public readonly bool IsTieBroken
		{
			get
			{
				return this.globalMaxCmp != 0;
			}
		}

		private int CacheLocalScore(int score, int cmp)
		{
			if (this.localMaxScore == -1 || this.localMaxScore < score)
			{
				this.localMaxScore = score;
				this.localMaxCmp = cmp;
			}
			return this.localMaxCmp;
		}

		private int CacheScore(int score, int cmp)
		{
			if (score == this.globalMax)
			{
				this.globalMaxCmp = cmp;
				return this.globalMaxCmp;
			}
			return this.CacheLocalScore(score, cmp);
		}

		public int Consider(int lhs, int rhs)
		{
			if (this.IsTieBroken)
			{
				return this.globalMaxCmp;
			}
			switch (-lhs.CompareTo(rhs))
			{
			case -1:
				return this.CacheScore(lhs, -1);
			case 0:
				if (this.localMaxScore != -1)
				{
					return this.localMaxCmp;
				}
				return 0;
			case 1:
				return this.CacheScore(rhs, 1);
			default:
				Debug.Assert(false);
				return 0;
			}
		}

		public int Consider<T>(T lhs, T rhs) where T : IComparable, SearchUtil.IScore
		{
			if (this.IsTieBroken)
			{
				return this.globalMaxCmp;
			}
			if (lhs == null)
			{
				if (rhs != null)
				{
					return this.CacheScore(rhs.Score, 1);
				}
				if (this.localMaxScore != -1)
				{
					return this.localMaxCmp;
				}
				return 0;
			}
			else
			{
				if (rhs == null)
				{
					return this.CacheScore(lhs.Score, -1);
				}
				switch (lhs.CompareTo(rhs))
				{
				case -1:
					return this.CacheScore(lhs.Score, -1);
				case 0:
					if (this.localMaxScore != -1)
					{
						return this.localMaxCmp;
					}
					return 0;
				case 1:
					return this.CacheScore(rhs.Score, 1);
				default:
					Debug.Assert(false);
					return 0;
				}
			}
		}

		private readonly int globalMax;

		private int globalMaxCmp;

		private int localMaxScore;

		private int localMaxCmp;
	}

	public class MatchCache : IComparable, SearchUtil.IScore
	{
		public int Score
		{
			get
			{
				return this.FuzzyMatch.score;
			}
		}

		public FuzzySearch.Match FuzzyMatch { get; private set; }

		public void Bind(string searchStringUpper)
		{
			try
			{
				this.FuzzyMatch = FuzzySearch.ScoreCanonicalCandidate(searchStringUpper, this.text, null);
			}
			catch (Exception ex)
			{
				throw new Exception("searchStringUpper: " + searchStringUpper + ", text: " + this.text, ex);
			}
		}

		public void Reset()
		{
			this.FuzzyMatch = FuzzySearch.Match.NONE;
		}

		public int CompareTo(object obj)
		{
			SearchUtil.MatchCache matchCache = (SearchUtil.MatchCache)obj;
			return -this.Score.CompareTo(matchCache.Score);
		}

		public string text;
	}

	public class NameDescCache : IComparable, SearchUtil.IScore
	{
		public void Bind(string searchStringUpper)
		{
			this.name.Bind(searchStringUpper);
			this.desc.Bind(searchStringUpper);
		}

		public void Reset()
		{
			this.name.Reset();
			this.desc.Reset();
		}

		public int Score
		{
			get
			{
				return Math.Max(this.name.Score, this.desc.Score);
			}
		}

		public int CompareTo(object obj)
		{
			SearchUtil.NameDescCache nameDescCache = (SearchUtil.NameDescCache)obj;
			int score = this.Score;
			int score2 = nameDescCache.Score;
			int num = -score.CompareTo(score2);
			if (num != 0)
			{
				return num;
			}
			SearchUtil.TieBreaker tieBreaker = new SearchUtil.TieBreaker(score);
			tieBreaker.Consider<SearchUtil.MatchCache>(this.name, nameDescCache.name);
			return tieBreaker.Consider<SearchUtil.MatchCache>(this.desc, nameDescCache.desc);
		}

		public SearchUtil.MatchCache name;

		public SearchUtil.MatchCache desc;
	}

	public class NameDescSearchTermsCache : IComparable, SearchUtil.IScore
	{
		public FuzzySearch.Match SearchTermsScore { get; private set; }

		public void Bind(string searchStringUpper)
		{
			this.nameDesc.Bind(searchStringUpper);
			this.SearchTermsScore = FuzzySearch.ScoreTokens(searchStringUpper, this.searchTerms);
		}

		public void Reset()
		{
			this.nameDesc.Reset();
			this.SearchTermsScore = FuzzySearch.Match.NONE;
		}

		public int Score
		{
			get
			{
				return Math.Max(this.nameDesc.Score, this.SearchTermsScore.score);
			}
		}

		public bool IsPassingScore()
		{
			return this.Score >= 79;
		}

		public int CompareTo(object obj)
		{
			SearchUtil.NameDescSearchTermsCache nameDescSearchTermsCache = (SearchUtil.NameDescSearchTermsCache)obj;
			int score = this.Score;
			int score2 = nameDescSearchTermsCache.Score;
			int num = -score.CompareTo(score2);
			if (num != 0)
			{
				return num;
			}
			SearchUtil.TieBreaker tieBreaker = new SearchUtil.TieBreaker(score);
			tieBreaker.Consider<SearchUtil.MatchCache>(this.nameDesc.name, nameDescSearchTermsCache.nameDesc.name);
			tieBreaker.Consider(this.SearchTermsScore.score, nameDescSearchTermsCache.SearchTermsScore.score);
			return tieBreaker.Consider<SearchUtil.MatchCache>(this.nameDesc.desc, nameDescSearchTermsCache.nameDesc.desc);
		}

		public SearchUtil.NameDescCache nameDesc;

		public IReadOnlyList<string> searchTerms;
	}

	public class BuildingDefCache : IComparable, SearchUtil.IScore
	{
		public SearchUtil.NameDescCache BestRecipe { get; private set; }

		public void Bind(string searchStringUpper)
		{
			this.nameDescSearchTerms.Bind(searchStringUpper);
			this.effect.Bind(searchStringUpper);
			this.BestRecipe = null;
			foreach (SearchUtil.NameDescCache nameDescCache in this.recipes)
			{
				nameDescCache.Bind(searchStringUpper);
				if (this.BestRecipe == null || nameDescCache.CompareTo(this.BestRecipe) == -1)
				{
					this.BestRecipe = nameDescCache;
				}
			}
		}

		public void Reset()
		{
			this.nameDescSearchTerms.Reset();
			this.effect.Reset();
			foreach (SearchUtil.NameDescCache nameDescCache in this.recipes)
			{
				nameDescCache.Reset();
			}
			this.BestRecipe = null;
		}

		public int Score
		{
			get
			{
				return Math.Max(this.nameDescSearchTerms.Score, Math.Max(this.effect.Score, (this.BestRecipe == null) ? 0 : this.BestRecipe.Score));
			}
		}

		public bool IsPassingScore()
		{
			return this.Score >= 79;
		}

		public int CompareTo(object obj)
		{
			SearchUtil.BuildingDefCache buildingDefCache = (SearchUtil.BuildingDefCache)obj;
			int score = this.Score;
			int score2 = buildingDefCache.Score;
			int num = -score.CompareTo(score2);
			if (num != 0)
			{
				return num;
			}
			SearchUtil.TieBreaker tieBreaker = new SearchUtil.TieBreaker(score);
			tieBreaker.Consider<SearchUtil.MatchCache>(this.nameDescSearchTerms.nameDesc.name, buildingDefCache.nameDescSearchTerms.nameDesc.name);
			tieBreaker.Consider(this.nameDescSearchTerms.SearchTermsScore.score, buildingDefCache.nameDescSearchTerms.SearchTermsScore.score);
			if (!tieBreaker.IsTieBroken)
			{
				int num2 = this.techTier.CompareTo(buildingDefCache.techTier);
				if (num2 != 0)
				{
					return num2;
				}
			}
			tieBreaker.Consider<SearchUtil.MatchCache>(this.effect, buildingDefCache.effect);
			return tieBreaker.Consider<SearchUtil.MatchCache>(this.nameDescSearchTerms.nameDesc.desc, buildingDefCache.nameDescSearchTerms.nameDesc.desc);
		}

		public SearchUtil.NameDescSearchTermsCache nameDescSearchTerms;

		public SearchUtil.MatchCache effect;

		public List<SearchUtil.NameDescCache> recipes;

		public int techTier;
	}

	public class TechItemCache : IComparable, SearchUtil.IScore
	{
		public SearchUtil.NameDescCache BestRecipe { get; private set; }

		public void Bind(string searchStringUpper)
		{
			this.nameDescSearchTerms.Bind(searchStringUpper);
			this.BestRecipe = null;
			foreach (SearchUtil.NameDescCache nameDescCache in this.recipes)
			{
				nameDescCache.Bind(searchStringUpper);
				if (this.BestRecipe == null || nameDescCache.CompareTo(this.BestRecipe) == -1)
				{
					this.BestRecipe = nameDescCache;
				}
			}
		}

		public void Reset()
		{
			this.nameDescSearchTerms.Reset();
			foreach (SearchUtil.NameDescCache nameDescCache in this.recipes)
			{
				nameDescCache.Reset();
			}
			this.BestRecipe = null;
		}

		public int Score
		{
			get
			{
				return Math.Max(this.nameDescSearchTerms.Score, (this.BestRecipe == null) ? 0 : this.BestRecipe.Score);
			}
		}

		public bool IsPassingScore()
		{
			return this.Score >= 79;
		}

		public int CompareTo(object obj)
		{
			SearchUtil.TechItemCache techItemCache = (SearchUtil.TechItemCache)obj;
			int score = this.Score;
			int score2 = techItemCache.Score;
			int num = -score.CompareTo(score2);
			if (num != 0)
			{
				return num;
			}
			SearchUtil.TieBreaker tieBreaker = new SearchUtil.TieBreaker(score);
			tieBreaker.Consider<SearchUtil.MatchCache>(this.nameDescSearchTerms.nameDesc.name, techItemCache.nameDescSearchTerms.nameDesc.name);
			tieBreaker.Consider(this.nameDescSearchTerms.SearchTermsScore.score, techItemCache.nameDescSearchTerms.SearchTermsScore.score);
			if (!tieBreaker.IsTieBroken)
			{
				int num2 = this.tier.CompareTo(techItemCache.tier);
				if (num2 != 0)
				{
					return num2;
				}
			}
			tieBreaker.Consider<SearchUtil.MatchCache>(this.nameDescSearchTerms.nameDesc.desc, techItemCache.nameDescSearchTerms.nameDesc.desc);
			return tieBreaker.Consider<SearchUtil.NameDescCache>(this.BestRecipe, techItemCache.BestRecipe);
		}

		public SearchUtil.NameDescSearchTermsCache nameDescSearchTerms;

		public List<SearchUtil.NameDescCache> recipes;

		public int tier;
	}

	public class TechCache : IComparable
	{
		public SearchUtil.TechItemCache BestItem { get; private set; }

		public void Bind(string searchStringUpper)
		{
			this.tech.Bind(searchStringUpper);
			this.BestItem = null;
			foreach (KeyValuePair<string, SearchUtil.TechItemCache> keyValuePair in this.techItems)
			{
				keyValuePair.Value.Bind(searchStringUpper);
				if (this.BestItem == null || keyValuePair.Value.CompareTo(this.BestItem) == -1)
				{
					this.BestItem = keyValuePair.Value;
				}
			}
		}

		public void Reset()
		{
			this.tech.Reset();
			foreach (KeyValuePair<string, SearchUtil.TechItemCache> keyValuePair in this.techItems)
			{
				keyValuePair.Value.Reset();
			}
			this.BestItem = null;
		}

		public int Score
		{
			get
			{
				return Math.Max(this.tech.Score, (this.BestItem == null) ? 0 : this.BestItem.Score);
			}
		}

		public bool IsPassingScore()
		{
			return this.Score >= 79;
		}

		public int CompareTo(object obj)
		{
			SearchUtil.TechCache techCache = (SearchUtil.TechCache)obj;
			int score = this.Score;
			int score2 = techCache.Score;
			int num = -score.CompareTo(score2);
			if (num != 0)
			{
				return num;
			}
			SearchUtil.TieBreaker tieBreaker = new SearchUtil.TieBreaker(score);
			tieBreaker.Consider<SearchUtil.MatchCache>(this.tech.nameDesc.name, techCache.tech.nameDesc.name);
			tieBreaker.Consider(this.tech.SearchTermsScore.score, techCache.tech.SearchTermsScore.score);
			if (!tieBreaker.IsTieBroken)
			{
				int num2 = this.tier.CompareTo(techCache.tier);
				if (num2 != 0)
				{
					return num2;
				}
			}
			tieBreaker.Consider<SearchUtil.MatchCache>(this.tech.nameDesc.desc, techCache.tech.nameDesc.desc);
			return tieBreaker.Consider<SearchUtil.TechItemCache>(this.BestItem, techCache.BestItem);
		}

		public SearchUtil.NameDescSearchTermsCache tech;

		public Dictionary<string, SearchUtil.TechItemCache> techItems;

		public int tier;
	}

	public class SubcategoryCache : IComparable
	{
		public SearchUtil.BuildingDefCache BestBuildingDef { get; private set; }

		public void Bind(string searchStringUpper)
		{
			this.subcategory.Bind(searchStringUpper);
			this.BestBuildingDef = null;
			foreach (SearchUtil.BuildingDefCache buildingDefCache in this.buildingDefs)
			{
				buildingDefCache.Bind(searchStringUpper);
				if (this.BestBuildingDef == null || buildingDefCache.CompareTo(this.BestBuildingDef) == -1)
				{
					this.BestBuildingDef = buildingDefCache;
				}
			}
		}

		public void Reset()
		{
			this.subcategory.Reset();
			foreach (SearchUtil.BuildingDefCache buildingDefCache in this.buildingDefs)
			{
				buildingDefCache.Reset();
			}
			this.BestBuildingDef = null;
		}

		public int Score
		{
			get
			{
				return Math.Max(this.subcategory.Score, (this.BestBuildingDef == null) ? 0 : this.BestBuildingDef.Score);
			}
		}

		public bool IsPassingScore()
		{
			return this.Score >= 79;
		}

		public int CompareTo(object obj)
		{
			SearchUtil.SubcategoryCache subcategoryCache = (SearchUtil.SubcategoryCache)obj;
			int score = this.Score;
			int score2 = subcategoryCache.Score;
			int num = -score.CompareTo(score2);
			if (num != 0)
			{
				return num;
			}
			SearchUtil.TieBreaker tieBreaker = new SearchUtil.TieBreaker(score);
			tieBreaker.Consider<SearchUtil.MatchCache>(this.subcategory, subcategoryCache.subcategory);
			return tieBreaker.Consider<SearchUtil.BuildingDefCache>(this.BestBuildingDef, subcategoryCache.BestBuildingDef);
		}

		public SearchUtil.MatchCache subcategory;

		public HashSet<SearchUtil.BuildingDefCache> buildingDefs;
	}
}
