using System;
using System.Collections.Generic;

namespace rail
{
	public class RailCallBackHelper
	{
		private RailCallBackHelper()
		{
		}

		public static RailCallBackHelper Instance
		{
			get
			{
				if (RailCallBackHelper.instance_ == null)
				{
					object obj = RailCallBackHelper.locker_;
					lock (obj)
					{
						if (RailCallBackHelper.instance_ == null)
						{
							RailCallBackHelper.instance_ = new RailCallBackHelper();
						}
					}
				}
				return RailCallBackHelper.instance_;
			}
		}

		public void RegisterCallback(RAILEventID event_id, RailEventCallBackHandler handler)
		{
			object obj = RailCallBackHelper.locker_;
			lock (obj)
			{
				if (RailCallBackHelper.eventHandlers_.ContainsKey(event_id))
				{
					Dictionary<RAILEventID, RailEventCallBackHandler> dictionary = RailCallBackHelper.eventHandlers_;
					dictionary[event_id] = (RailEventCallBackHandler)Delegate.Combine(dictionary[event_id], handler);
				}
				else
				{
					RailCallBackHelper.eventHandlers_.Add(event_id, handler);
					RAIL_API_PINVOKE.CSharpRailRegisterEvent((int)event_id, RailCallBackHelper.delegate_);
				}
			}
		}

		public void UnregisterCallback(RAILEventID event_id, RailEventCallBackHandler handler)
		{
			object obj = RailCallBackHelper.locker_;
			lock (obj)
			{
				if (RailCallBackHelper.eventHandlers_.ContainsKey(event_id))
				{
					Dictionary<RAILEventID, RailEventCallBackHandler> dictionary = RailCallBackHelper.eventHandlers_;
					dictionary[event_id] = (RailEventCallBackHandler)Delegate.Remove(dictionary[event_id], handler);
					if (RailCallBackHelper.eventHandlers_[event_id] == null)
					{
						RAIL_API_PINVOKE.CSharpRailUnRegisterEvent((int)event_id, RailCallBackHelper.delegate_);
						RailCallBackHelper.eventHandlers_.Remove(event_id);
					}
				}
			}
		}

		public void UnregisterCallback(RAILEventID event_id)
		{
			object obj = RailCallBackHelper.locker_;
			lock (obj)
			{
				RAIL_API_PINVOKE.CSharpRailUnRegisterEvent((int)event_id, RailCallBackHelper.delegate_);
				if (RailCallBackHelper.eventHandlers_[event_id] != null)
				{
					RailCallBackHelper.eventHandlers_.Remove(event_id);
				}
			}
		}

		public void UnregisterAllCallback()
		{
			object obj = RailCallBackHelper.locker_;
			lock (obj)
			{
				RAIL_API_PINVOKE.CSharpRailUnRegisterAllEvent();
				RailCallBackHelper.eventHandlers_.Clear();
			}
		}

		[MonoPInvokeCallback(typeof(RailEventCallBackFunction))]
		public static void OnRailCallBack(RAILEventID event_id, IntPtr data)
		{
			RailEventCallBackHandler railEventCallBackHandler = RailCallBackHelper.eventHandlers_[event_id];
			if (railEventCallBackHandler != null)
			{
				if (event_id <= RAILEventID.kRailEventShowFloatingNotifyWindow)
				{
					if (event_id <= RAILEventID.kRailEventAssetsAssetsChanged)
					{
						if (event_id <= RAILEventID.kRailEventAchievementGlobalAchievementReceived)
						{
							if (event_id <= RAILEventID.kRailEventSystemStateChanged)
							{
								if (event_id == RAILEventID.kRailEventFinalize)
								{
									RailFinalize railFinalize = new RailFinalize();
									RailConverter.Cpp2Csharp(data, railFinalize);
									railEventCallBackHandler(event_id, railFinalize);
									return;
								}
								if (event_id != RAILEventID.kRailEventSystemStateChanged)
								{
									return;
								}
								RailSystemStateChanged railSystemStateChanged = new RailSystemStateChanged();
								RailConverter.Cpp2Csharp(data, railSystemStateChanged);
								railEventCallBackHandler(event_id, railSystemStateChanged);
								return;
							}
							else
							{
								switch (event_id)
								{
								case RAILEventID.kRailPlatformNotifyEventJoinGameByGameServer:
								{
									RailPlatformNotifyEventJoinGameByGameServer railPlatformNotifyEventJoinGameByGameServer = new RailPlatformNotifyEventJoinGameByGameServer();
									RailConverter.Cpp2Csharp(data, railPlatformNotifyEventJoinGameByGameServer);
									railEventCallBackHandler(event_id, railPlatformNotifyEventJoinGameByGameServer);
									return;
								}
								case RAILEventID.kRailPlatformNotifyEventJoinGameByRoom:
								{
									RailPlatformNotifyEventJoinGameByRoom railPlatformNotifyEventJoinGameByRoom = new RailPlatformNotifyEventJoinGameByRoom();
									RailConverter.Cpp2Csharp(data, railPlatformNotifyEventJoinGameByRoom);
									railEventCallBackHandler(event_id, railPlatformNotifyEventJoinGameByRoom);
									return;
								}
								case RAILEventID.kRailPlatformNotifyEventJoinGameByUser:
								{
									RailPlatformNotifyEventJoinGameByUser railPlatformNotifyEventJoinGameByUser = new RailPlatformNotifyEventJoinGameByUser();
									RailConverter.Cpp2Csharp(data, railPlatformNotifyEventJoinGameByUser);
									railEventCallBackHandler(event_id, railPlatformNotifyEventJoinGameByUser);
									return;
								}
								default:
									switch (event_id)
									{
									case RAILEventID.kRailEventStatsPlayerStatsReceived:
									{
										PlayerStatsReceived playerStatsReceived = new PlayerStatsReceived();
										RailConverter.Cpp2Csharp(data, playerStatsReceived);
										railEventCallBackHandler(event_id, playerStatsReceived);
										return;
									}
									case RAILEventID.kRailEventStatsPlayerStatsStored:
									{
										PlayerStatsStored playerStatsStored = new PlayerStatsStored();
										RailConverter.Cpp2Csharp(data, playerStatsStored);
										railEventCallBackHandler(event_id, playerStatsStored);
										return;
									}
									case RAILEventID.kRailEventStatsNumOfPlayerReceived:
									{
										NumberOfPlayerReceived numberOfPlayerReceived = new NumberOfPlayerReceived();
										RailConverter.Cpp2Csharp(data, numberOfPlayerReceived);
										railEventCallBackHandler(event_id, numberOfPlayerReceived);
										return;
									}
									case RAILEventID.kRailEventStatsGlobalStatsReceived:
									{
										GlobalStatsRequestReceived globalStatsRequestReceived = new GlobalStatsRequestReceived();
										RailConverter.Cpp2Csharp(data, globalStatsRequestReceived);
										railEventCallBackHandler(event_id, globalStatsRequestReceived);
										return;
									}
									default:
										switch (event_id)
										{
										case RAILEventID.kRailEventAchievementPlayerAchievementReceived:
										{
											PlayerAchievementReceived playerAchievementReceived = new PlayerAchievementReceived();
											RailConverter.Cpp2Csharp(data, playerAchievementReceived);
											railEventCallBackHandler(event_id, playerAchievementReceived);
											return;
										}
										case RAILEventID.kRailEventAchievementPlayerAchievementStored:
										{
											PlayerAchievementStored playerAchievementStored = new PlayerAchievementStored();
											RailConverter.Cpp2Csharp(data, playerAchievementStored);
											railEventCallBackHandler(event_id, playerAchievementStored);
											return;
										}
										case RAILEventID.kRailEventAchievementGlobalAchievementReceived:
										{
											GlobalAchievementReceived globalAchievementReceived = new GlobalAchievementReceived();
											RailConverter.Cpp2Csharp(data, globalAchievementReceived);
											railEventCallBackHandler(event_id, globalAchievementReceived);
											return;
										}
										default:
											return;
										}
										break;
									}
									break;
								}
							}
						}
						else if (event_id <= RAILEventID.kRailEventGameServerRemoveFavoriteGameServer)
						{
							switch (event_id)
							{
							case RAILEventID.kRailEventLeaderboardReceived:
							{
								LeaderboardReceived leaderboardReceived = new LeaderboardReceived();
								RailConverter.Cpp2Csharp(data, leaderboardReceived);
								railEventCallBackHandler(event_id, leaderboardReceived);
								return;
							}
							case RAILEventID.kRailEventLeaderboardEntryReceived:
							{
								LeaderboardEntryReceived leaderboardEntryReceived = new LeaderboardEntryReceived();
								RailConverter.Cpp2Csharp(data, leaderboardEntryReceived);
								railEventCallBackHandler(event_id, leaderboardEntryReceived);
								return;
							}
							case RAILEventID.kRailEventLeaderboardUploaded:
							{
								LeaderboardUploaded leaderboardUploaded = new LeaderboardUploaded();
								RailConverter.Cpp2Csharp(data, leaderboardUploaded);
								railEventCallBackHandler(event_id, leaderboardUploaded);
								return;
							}
							case RAILEventID.kRailEventLeaderboardAttachSpaceWork:
							{
								LeaderboardAttachSpaceWork leaderboardAttachSpaceWork = new LeaderboardAttachSpaceWork();
								RailConverter.Cpp2Csharp(data, leaderboardAttachSpaceWork);
								railEventCallBackHandler(event_id, leaderboardAttachSpaceWork);
								return;
							}
							case RAILEventID.kRailEventLeaderboardAsyncCreated:
							{
								LeaderboardCreated leaderboardCreated = new LeaderboardCreated();
								RailConverter.Cpp2Csharp(data, leaderboardCreated);
								railEventCallBackHandler(event_id, leaderboardCreated);
								return;
							}
							default:
								switch (event_id)
								{
								case RAILEventID.kRailEventGameServerListResult:
								{
									GetGameServerListResult getGameServerListResult = new GetGameServerListResult();
									RailConverter.Cpp2Csharp(data, getGameServerListResult);
									railEventCallBackHandler(event_id, getGameServerListResult);
									return;
								}
								case RAILEventID.kRailEventGameServerCreated:
								{
									CreateGameServerResult createGameServerResult = new CreateGameServerResult();
									RailConverter.Cpp2Csharp(data, createGameServerResult);
									railEventCallBackHandler(event_id, createGameServerResult);
									return;
								}
								case RAILEventID.kRailEventGameServerSetMetadataResult:
								{
									SetGameServerMetadataResult setGameServerMetadataResult = new SetGameServerMetadataResult();
									RailConverter.Cpp2Csharp(data, setGameServerMetadataResult);
									railEventCallBackHandler(event_id, setGameServerMetadataResult);
									return;
								}
								case RAILEventID.kRailEventGameServerGetMetadataResult:
								{
									GetGameServerMetadataResult getGameServerMetadataResult = new GetGameServerMetadataResult();
									RailConverter.Cpp2Csharp(data, getGameServerMetadataResult);
									railEventCallBackHandler(event_id, getGameServerMetadataResult);
									return;
								}
								case RAILEventID.kRailEventGameServerGetSessionTicket:
								{
									AsyncAcquireGameServerSessionTicketResponse asyncAcquireGameServerSessionTicketResponse = new AsyncAcquireGameServerSessionTicketResponse();
									RailConverter.Cpp2Csharp(data, asyncAcquireGameServerSessionTicketResponse);
									railEventCallBackHandler(event_id, asyncAcquireGameServerSessionTicketResponse);
									return;
								}
								case RAILEventID.kRailEventGameServerAuthSessionTicket:
								{
									GameServerStartSessionWithPlayerResponse gameServerStartSessionWithPlayerResponse = new GameServerStartSessionWithPlayerResponse();
									RailConverter.Cpp2Csharp(data, gameServerStartSessionWithPlayerResponse);
									railEventCallBackHandler(event_id, gameServerStartSessionWithPlayerResponse);
									return;
								}
								case RAILEventID.kRailEventGameServerPlayerListResult:
								{
									GetGameServerPlayerListResult getGameServerPlayerListResult = new GetGameServerPlayerListResult();
									RailConverter.Cpp2Csharp(data, getGameServerPlayerListResult);
									railEventCallBackHandler(event_id, getGameServerPlayerListResult);
									return;
								}
								case RAILEventID.kRailEventGameServerRegisterToServerListResult:
								{
									GameServerRegisterToServerListResult gameServerRegisterToServerListResult = new GameServerRegisterToServerListResult();
									RailConverter.Cpp2Csharp(data, gameServerRegisterToServerListResult);
									railEventCallBackHandler(event_id, gameServerRegisterToServerListResult);
									return;
								}
								case RAILEventID.kRailEventGameServerFavoriteGameServers:
								{
									AsyncGetFavoriteGameServersResult asyncGetFavoriteGameServersResult = new AsyncGetFavoriteGameServersResult();
									RailConverter.Cpp2Csharp(data, asyncGetFavoriteGameServersResult);
									railEventCallBackHandler(event_id, asyncGetFavoriteGameServersResult);
									return;
								}
								case RAILEventID.kRailEventGameServerAddFavoriteGameServer:
								{
									AsyncAddFavoriteGameServerResult asyncAddFavoriteGameServerResult = new AsyncAddFavoriteGameServerResult();
									RailConverter.Cpp2Csharp(data, asyncAddFavoriteGameServerResult);
									railEventCallBackHandler(event_id, asyncAddFavoriteGameServerResult);
									return;
								}
								case RAILEventID.kRailEventGameServerRemoveFavoriteGameServer:
								{
									AsyncRemoveFavoriteGameServerResult asyncRemoveFavoriteGameServerResult = new AsyncRemoveFavoriteGameServerResult();
									RailConverter.Cpp2Csharp(data, asyncRemoveFavoriteGameServerResult);
									railEventCallBackHandler(event_id, asyncRemoveFavoriteGameServerResult);
									return;
								}
								default:
									return;
								}
								break;
							}
						}
						else
						{
							switch (event_id)
							{
							case RAILEventID.kRailEventUserSpaceGetMySubscribedWorksResult:
							{
								AsyncGetMySubscribedWorksResult asyncGetMySubscribedWorksResult = new AsyncGetMySubscribedWorksResult();
								RailConverter.Cpp2Csharp(data, asyncGetMySubscribedWorksResult);
								railEventCallBackHandler(event_id, asyncGetMySubscribedWorksResult);
								return;
							}
							case RAILEventID.kRailEventUserSpaceGetMyFavoritesWorksResult:
							{
								AsyncGetMyFavoritesWorksResult asyncGetMyFavoritesWorksResult = new AsyncGetMyFavoritesWorksResult();
								RailConverter.Cpp2Csharp(data, asyncGetMyFavoritesWorksResult);
								railEventCallBackHandler(event_id, asyncGetMyFavoritesWorksResult);
								return;
							}
							case RAILEventID.kRailEventUserSpaceQuerySpaceWorksResult:
							{
								AsyncQuerySpaceWorksResult asyncQuerySpaceWorksResult = new AsyncQuerySpaceWorksResult();
								RailConverter.Cpp2Csharp(data, asyncQuerySpaceWorksResult);
								railEventCallBackHandler(event_id, asyncQuerySpaceWorksResult);
								return;
							}
							case RAILEventID.kRailEventUserSpaceUpdateMetadataResult:
							{
								AsyncUpdateMetadataResult asyncUpdateMetadataResult = new AsyncUpdateMetadataResult();
								RailConverter.Cpp2Csharp(data, asyncUpdateMetadataResult);
								railEventCallBackHandler(event_id, asyncUpdateMetadataResult);
								return;
							}
							case RAILEventID.kRailEventUserSpaceSyncResult:
							{
								SyncSpaceWorkResult syncSpaceWorkResult = new SyncSpaceWorkResult();
								RailConverter.Cpp2Csharp(data, syncSpaceWorkResult);
								railEventCallBackHandler(event_id, syncSpaceWorkResult);
								return;
							}
							case RAILEventID.kRailEventUserSpaceSubscribeResult:
							{
								AsyncSubscribeSpaceWorksResult asyncSubscribeSpaceWorksResult = new AsyncSubscribeSpaceWorksResult();
								RailConverter.Cpp2Csharp(data, asyncSubscribeSpaceWorksResult);
								railEventCallBackHandler(event_id, asyncSubscribeSpaceWorksResult);
								return;
							}
							case RAILEventID.kRailEventUserSpaceModifyFavoritesWorksResult:
							{
								AsyncModifyFavoritesWorksResult asyncModifyFavoritesWorksResult = new AsyncModifyFavoritesWorksResult();
								RailConverter.Cpp2Csharp(data, asyncModifyFavoritesWorksResult);
								railEventCallBackHandler(event_id, asyncModifyFavoritesWorksResult);
								return;
							}
							case RAILEventID.kRailEventUserSpaceRemoveSpaceWorkResult:
							{
								AsyncRemoveSpaceWorkResult asyncRemoveSpaceWorkResult = new AsyncRemoveSpaceWorkResult();
								RailConverter.Cpp2Csharp(data, asyncRemoveSpaceWorkResult);
								railEventCallBackHandler(event_id, asyncRemoveSpaceWorkResult);
								return;
							}
							case RAILEventID.kRailEventUserSpaceVoteSpaceWorkResult:
							{
								AsyncVoteSpaceWorkResult asyncVoteSpaceWorkResult = new AsyncVoteSpaceWorkResult();
								RailConverter.Cpp2Csharp(data, asyncVoteSpaceWorkResult);
								railEventCallBackHandler(event_id, asyncVoteSpaceWorkResult);
								return;
							}
							case RAILEventID.kRailEventUserSpaceSearchSpaceWorkResult:
							{
								AsyncSearchSpaceWorksResult asyncSearchSpaceWorksResult = new AsyncSearchSpaceWorksResult();
								RailConverter.Cpp2Csharp(data, asyncSearchSpaceWorksResult);
								railEventCallBackHandler(event_id, asyncSearchSpaceWorksResult);
								return;
							}
							case RAILEventID.kRailEventUserSpaceQuerySpaceWorksResultV2:
								break;
							case RAILEventID.kRailEventUserSpaceDownloadProgress:
							{
								UserSpaceDownloadProgress userSpaceDownloadProgress = new UserSpaceDownloadProgress();
								RailConverter.Cpp2Csharp(data, userSpaceDownloadProgress);
								railEventCallBackHandler(event_id, userSpaceDownloadProgress);
								return;
							}
							case RAILEventID.kRailEventUserSpaceDownloadResult:
							{
								UserSpaceDownloadResult userSpaceDownloadResult = new UserSpaceDownloadResult();
								RailConverter.Cpp2Csharp(data, userSpaceDownloadResult);
								railEventCallBackHandler(event_id, userSpaceDownloadResult);
								return;
							}
							case RAILEventID.kRailEventUserSpaceRateSpaceWorkResult:
							{
								AsyncRateSpaceWorkResult asyncRateSpaceWorkResult = new AsyncRateSpaceWorkResult();
								RailConverter.Cpp2Csharp(data, asyncRateSpaceWorkResult);
								railEventCallBackHandler(event_id, asyncRateSpaceWorkResult);
								return;
							}
							case RAILEventID.kRailEventUserSpaceQuerySpaceWorksInfoResult:
							{
								AsyncQuerySpaceWorksInfoResult asyncQuerySpaceWorksInfoResult = new AsyncQuerySpaceWorksInfoResult();
								RailConverter.Cpp2Csharp(data, asyncQuerySpaceWorksInfoResult);
								railEventCallBackHandler(event_id, asyncQuerySpaceWorksInfoResult);
								return;
							}
							default:
								switch (event_id)
								{
								case RAILEventID.kRailEventStorageQueryQuotaResult:
								{
									AsyncQueryQuotaResult asyncQueryQuotaResult = new AsyncQueryQuotaResult();
									RailConverter.Cpp2Csharp(data, asyncQueryQuotaResult);
									railEventCallBackHandler(event_id, asyncQueryQuotaResult);
									return;
								}
								case RAILEventID.kRailEventStorageShareToSpaceWorkResult:
								{
									ShareStorageToSpaceWorkResult shareStorageToSpaceWorkResult = new ShareStorageToSpaceWorkResult();
									RailConverter.Cpp2Csharp(data, shareStorageToSpaceWorkResult);
									railEventCallBackHandler(event_id, shareStorageToSpaceWorkResult);
									return;
								}
								case RAILEventID.kRailEventStorageAsyncReadFileResult:
								{
									AsyncReadFileResult asyncReadFileResult = new AsyncReadFileResult();
									RailConverter.Cpp2Csharp(data, asyncReadFileResult);
									railEventCallBackHandler(event_id, asyncReadFileResult);
									return;
								}
								case RAILEventID.kRailEventStorageAsyncWriteFileResult:
								{
									AsyncWriteFileResult asyncWriteFileResult = new AsyncWriteFileResult();
									RailConverter.Cpp2Csharp(data, asyncWriteFileResult);
									railEventCallBackHandler(event_id, asyncWriteFileResult);
									return;
								}
								case RAILEventID.kRailEventStorageAsyncListStreamFileResult:
								{
									AsyncListFileResult asyncListFileResult = new AsyncListFileResult();
									RailConverter.Cpp2Csharp(data, asyncListFileResult);
									railEventCallBackHandler(event_id, asyncListFileResult);
									return;
								}
								case RAILEventID.kRailEventStorageAsyncRenameStreamFileResult:
								{
									AsyncRenameStreamFileResult asyncRenameStreamFileResult = new AsyncRenameStreamFileResult();
									RailConverter.Cpp2Csharp(data, asyncRenameStreamFileResult);
									railEventCallBackHandler(event_id, asyncRenameStreamFileResult);
									return;
								}
								case RAILEventID.kRailEventStorageAsyncDeleteStreamFileResult:
								{
									AsyncDeleteStreamFileResult asyncDeleteStreamFileResult = new AsyncDeleteStreamFileResult();
									RailConverter.Cpp2Csharp(data, asyncDeleteStreamFileResult);
									railEventCallBackHandler(event_id, asyncDeleteStreamFileResult);
									return;
								}
								case RAILEventID.kRailEventStorageAsyncReadStreamFileResult:
								{
									AsyncReadStreamFileResult asyncReadStreamFileResult = new AsyncReadStreamFileResult();
									RailConverter.Cpp2Csharp(data, asyncReadStreamFileResult);
									railEventCallBackHandler(event_id, asyncReadStreamFileResult);
									return;
								}
								case RAILEventID.kRailEventStorageAsyncWriteStreamFileResult:
								{
									AsyncWriteStreamFileResult asyncWriteStreamFileResult = new AsyncWriteStreamFileResult();
									RailConverter.Cpp2Csharp(data, asyncWriteStreamFileResult);
									railEventCallBackHandler(event_id, asyncWriteStreamFileResult);
									return;
								}
								default:
									switch (event_id)
									{
									case RAILEventID.kRailEventAssetsRequestAllAssetsFinished:
									{
										RequestAllAssetsFinished requestAllAssetsFinished = new RequestAllAssetsFinished();
										RailConverter.Cpp2Csharp(data, requestAllAssetsFinished);
										railEventCallBackHandler(event_id, requestAllAssetsFinished);
										break;
									}
									case RAILEventID.kRailEventAssetsCompleteConsumeByExchangeAssetsToFinished:
									{
										CompleteConsumeByExchangeAssetsToFinished completeConsumeByExchangeAssetsToFinished = new CompleteConsumeByExchangeAssetsToFinished();
										RailConverter.Cpp2Csharp(data, completeConsumeByExchangeAssetsToFinished);
										railEventCallBackHandler(event_id, completeConsumeByExchangeAssetsToFinished);
										return;
									}
									case RAILEventID.kRailEventAssetsExchangeAssetsFinished:
									{
										ExchangeAssetsFinished exchangeAssetsFinished = new ExchangeAssetsFinished();
										RailConverter.Cpp2Csharp(data, exchangeAssetsFinished);
										railEventCallBackHandler(event_id, exchangeAssetsFinished);
										return;
									}
									case RAILEventID.kRailEventAssetsExchangeAssetsToFinished:
									{
										ExchangeAssetsToFinished exchangeAssetsToFinished = new ExchangeAssetsToFinished();
										RailConverter.Cpp2Csharp(data, exchangeAssetsToFinished);
										railEventCallBackHandler(event_id, exchangeAssetsToFinished);
										return;
									}
									case RAILEventID.kRailEventAssetsDirectConsumeFinished:
									{
										DirectConsumeAssetsFinished directConsumeAssetsFinished = new DirectConsumeAssetsFinished();
										RailConverter.Cpp2Csharp(data, directConsumeAssetsFinished);
										railEventCallBackHandler(event_id, directConsumeAssetsFinished);
										return;
									}
									case RAILEventID.kRailEventAssetsStartConsumeFinished:
									{
										StartConsumeAssetsFinished startConsumeAssetsFinished = new StartConsumeAssetsFinished();
										RailConverter.Cpp2Csharp(data, startConsumeAssetsFinished);
										railEventCallBackHandler(event_id, startConsumeAssetsFinished);
										return;
									}
									case RAILEventID.kRailEventAssetsUpdateConsumeFinished:
									{
										UpdateConsumeAssetsFinished updateConsumeAssetsFinished = new UpdateConsumeAssetsFinished();
										RailConverter.Cpp2Csharp(data, updateConsumeAssetsFinished);
										railEventCallBackHandler(event_id, updateConsumeAssetsFinished);
										return;
									}
									case RAILEventID.kRailEventAssetsCompleteConsumeFinished:
									{
										CompleteConsumeAssetsFinished completeConsumeAssetsFinished = new CompleteConsumeAssetsFinished();
										RailConverter.Cpp2Csharp(data, completeConsumeAssetsFinished);
										railEventCallBackHandler(event_id, completeConsumeAssetsFinished);
										return;
									}
									case RAILEventID.kRailEventAssetsSplitFinished:
									{
										SplitAssetsFinished splitAssetsFinished = new SplitAssetsFinished();
										RailConverter.Cpp2Csharp(data, splitAssetsFinished);
										railEventCallBackHandler(event_id, splitAssetsFinished);
										return;
									}
									case RAILEventID.kRailEventAssetsSplitToFinished:
									{
										SplitAssetsToFinished splitAssetsToFinished = new SplitAssetsToFinished();
										RailConverter.Cpp2Csharp(data, splitAssetsToFinished);
										railEventCallBackHandler(event_id, splitAssetsToFinished);
										return;
									}
									case RAILEventID.kRailEventAssetsMergeFinished:
									{
										MergeAssetsFinished mergeAssetsFinished = new MergeAssetsFinished();
										RailConverter.Cpp2Csharp(data, mergeAssetsFinished);
										railEventCallBackHandler(event_id, mergeAssetsFinished);
										return;
									}
									case RAILEventID.kRailEventAssetsMergeToFinished:
									{
										MergeAssetsToFinished mergeAssetsToFinished = new MergeAssetsToFinished();
										RailConverter.Cpp2Csharp(data, mergeAssetsToFinished);
										railEventCallBackHandler(event_id, mergeAssetsToFinished);
										return;
									}
									case RAILEventID.kRailEventAssetsRequestAllProductFinished:
										break;
									case RAILEventID.kRailEventAssetsUpdateAssetPropertyFinished:
									{
										UpdateAssetsPropertyFinished updateAssetsPropertyFinished = new UpdateAssetsPropertyFinished();
										RailConverter.Cpp2Csharp(data, updateAssetsPropertyFinished);
										railEventCallBackHandler(event_id, updateAssetsPropertyFinished);
										return;
									}
									case RAILEventID.kRailEventAssetsAssetsChanged:
									{
										RailAssetsChanged railAssetsChanged = new RailAssetsChanged();
										RailConverter.Cpp2Csharp(data, railAssetsChanged);
										railEventCallBackHandler(event_id, railAssetsChanged);
										return;
									}
									default:
										return;
									}
									break;
								}
								break;
							}
						}
					}
					else if (event_id <= RAILEventID.kRailEventRoomSetRoomMaxMemberResult)
					{
						if (event_id <= RAILEventID.kRailEventUtilsGameSettingMetadataChanged)
						{
							if (event_id == RAILEventID.kRailEventUtilsGetImageDataResult)
							{
								RailGetImageDataResult railGetImageDataResult = new RailGetImageDataResult();
								RailConverter.Cpp2Csharp(data, railGetImageDataResult);
								railEventCallBackHandler(event_id, railGetImageDataResult);
								return;
							}
							if (event_id != RAILEventID.kRailEventUtilsGameSettingMetadataChanged)
							{
								return;
							}
							RailGameSettingMetadataChanged railGameSettingMetadataChanged = new RailGameSettingMetadataChanged();
							RailConverter.Cpp2Csharp(data, railGameSettingMetadataChanged);
							railEventCallBackHandler(event_id, railGameSettingMetadataChanged);
							return;
						}
						else
						{
							switch (event_id)
							{
							case RAILEventID.kRailEventInGamePurchaseAllProductsInfoReceived:
							{
								RailInGamePurchaseRequestAllProductsResponse railInGamePurchaseRequestAllProductsResponse = new RailInGamePurchaseRequestAllProductsResponse();
								RailConverter.Cpp2Csharp(data, railInGamePurchaseRequestAllProductsResponse);
								railEventCallBackHandler(event_id, railInGamePurchaseRequestAllProductsResponse);
								return;
							}
							case RAILEventID.kRailEventInGamePurchaseAllPurchasableProductsInfoReceived:
							{
								RailInGamePurchaseRequestAllPurchasableProductsResponse railInGamePurchaseRequestAllPurchasableProductsResponse = new RailInGamePurchaseRequestAllPurchasableProductsResponse();
								RailConverter.Cpp2Csharp(data, railInGamePurchaseRequestAllPurchasableProductsResponse);
								railEventCallBackHandler(event_id, railInGamePurchaseRequestAllPurchasableProductsResponse);
								return;
							}
							case RAILEventID.kRailEventInGamePurchasePurchaseProductsResult:
							{
								RailInGamePurchasePurchaseProductsResponse railInGamePurchasePurchaseProductsResponse = new RailInGamePurchasePurchaseProductsResponse();
								RailConverter.Cpp2Csharp(data, railInGamePurchasePurchaseProductsResponse);
								railEventCallBackHandler(event_id, railInGamePurchasePurchaseProductsResponse);
								return;
							}
							case RAILEventID.kRailEventInGamePurchaseFinishOrderResult:
							{
								RailInGamePurchaseFinishOrderResponse railInGamePurchaseFinishOrderResponse = new RailInGamePurchaseFinishOrderResponse();
								RailConverter.Cpp2Csharp(data, railInGamePurchaseFinishOrderResponse);
								railEventCallBackHandler(event_id, railInGamePurchaseFinishOrderResponse);
								return;
							}
							case RAILEventID.kRailEventInGamePurchasePurchaseProductsToAssetsResult:
							{
								RailInGamePurchasePurchaseProductsToAssetsResponse railInGamePurchasePurchaseProductsToAssetsResponse = new RailInGamePurchasePurchaseProductsToAssetsResponse();
								RailConverter.Cpp2Csharp(data, railInGamePurchasePurchaseProductsToAssetsResponse);
								railEventCallBackHandler(event_id, railInGamePurchasePurchaseProductsToAssetsResponse);
								return;
							}
							default:
								switch (event_id)
								{
								case RAILEventID.kRailEventInGameStorePurchasePayWindowDisplayed:
								{
									RailInGameStorePurchasePayWindowDisplayed railInGameStorePurchasePayWindowDisplayed = new RailInGameStorePurchasePayWindowDisplayed();
									RailConverter.Cpp2Csharp(data, railInGameStorePurchasePayWindowDisplayed);
									railEventCallBackHandler(event_id, railInGameStorePurchasePayWindowDisplayed);
									return;
								}
								case RAILEventID.kRailEventInGameStorePurchasePayWindowClosed:
								{
									RailInGameStorePurchasePayWindowClosed railInGameStorePurchasePayWindowClosed = new RailInGameStorePurchasePayWindowClosed();
									RailConverter.Cpp2Csharp(data, railInGameStorePurchasePayWindowClosed);
									railEventCallBackHandler(event_id, railInGameStorePurchasePayWindowClosed);
									return;
								}
								case RAILEventID.kRailEventInGameStorePurchasePaymentResult:
								{
									RailInGameStorePurchaseResult railInGameStorePurchaseResult = new RailInGameStorePurchaseResult();
									RailConverter.Cpp2Csharp(data, railInGameStorePurchaseResult);
									railEventCallBackHandler(event_id, railInGameStorePurchaseResult);
									return;
								}
								default:
									switch (event_id)
									{
									case RAILEventID.kRailEventRoomGetRoomListResult:
									{
										GetRoomListResult getRoomListResult = new GetRoomListResult();
										RailConverter.Cpp2Csharp(data, getRoomListResult);
										railEventCallBackHandler(event_id, getRoomListResult);
										return;
									}
									case RAILEventID.kRailEventRoomCreated:
									{
										CreateRoomResult createRoomResult = new CreateRoomResult();
										RailConverter.Cpp2Csharp(data, createRoomResult);
										railEventCallBackHandler(event_id, createRoomResult);
										return;
									}
									case RAILEventID.kRailEventRoomGetRoomMembersResult:
									{
										GetRoomMembersResult getRoomMembersResult = new GetRoomMembersResult();
										RailConverter.Cpp2Csharp(data, getRoomMembersResult);
										railEventCallBackHandler(event_id, getRoomMembersResult);
										return;
									}
									case RAILEventID.kRailEventRoomJoinRoomResult:
									{
										JoinRoomResult joinRoomResult = new JoinRoomResult();
										RailConverter.Cpp2Csharp(data, joinRoomResult);
										railEventCallBackHandler(event_id, joinRoomResult);
										return;
									}
									case RAILEventID.kRailEventRoomKickOffMemberResult:
									{
										KickOffMemberResult kickOffMemberResult = new KickOffMemberResult();
										RailConverter.Cpp2Csharp(data, kickOffMemberResult);
										railEventCallBackHandler(event_id, kickOffMemberResult);
										return;
									}
									case RAILEventID.kRailEventRoomSetRoomMetadataResult:
									{
										SetRoomMetadataResult setRoomMetadataResult = new SetRoomMetadataResult();
										RailConverter.Cpp2Csharp(data, setRoomMetadataResult);
										railEventCallBackHandler(event_id, setRoomMetadataResult);
										return;
									}
									case RAILEventID.kRailEventRoomGetRoomMetadataResult:
									{
										GetRoomMetadataResult getRoomMetadataResult = new GetRoomMetadataResult();
										RailConverter.Cpp2Csharp(data, getRoomMetadataResult);
										railEventCallBackHandler(event_id, getRoomMetadataResult);
										return;
									}
									case RAILEventID.kRailEventRoomGetMemberMetadataResult:
									{
										GetMemberMetadataResult getMemberMetadataResult = new GetMemberMetadataResult();
										RailConverter.Cpp2Csharp(data, getMemberMetadataResult);
										railEventCallBackHandler(event_id, getMemberMetadataResult);
										return;
									}
									case RAILEventID.kRailEventRoomSetMemberMetadataResult:
									{
										SetMemberMetadataResult setMemberMetadataResult = new SetMemberMetadataResult();
										RailConverter.Cpp2Csharp(data, setMemberMetadataResult);
										railEventCallBackHandler(event_id, setMemberMetadataResult);
										return;
									}
									case RAILEventID.kRailEventRoomLeaveRoomResult:
									{
										LeaveRoomResult leaveRoomResult = new LeaveRoomResult();
										RailConverter.Cpp2Csharp(data, leaveRoomResult);
										railEventCallBackHandler(event_id, leaveRoomResult);
										return;
									}
									case RAILEventID.kRailEventRoomGetAllDataResult:
									{
										GetAllRoomDataResult getAllRoomDataResult = new GetAllRoomDataResult();
										RailConverter.Cpp2Csharp(data, getAllRoomDataResult);
										railEventCallBackHandler(event_id, getAllRoomDataResult);
										return;
									}
									case RAILEventID.kRailEventRoomGetUserRoomListResult:
									{
										GetUserRoomListResult getUserRoomListResult = new GetUserRoomListResult();
										RailConverter.Cpp2Csharp(data, getUserRoomListResult);
										railEventCallBackHandler(event_id, getUserRoomListResult);
										return;
									}
									case RAILEventID.kRailEventRoomClearRoomMetadataResult:
									{
										ClearRoomMetadataResult clearRoomMetadataResult = new ClearRoomMetadataResult();
										RailConverter.Cpp2Csharp(data, clearRoomMetadataResult);
										railEventCallBackHandler(event_id, clearRoomMetadataResult);
										return;
									}
									case RAILEventID.kRailEventRoomOpenRoomResult:
									{
										OpenRoomResult openRoomResult = new OpenRoomResult();
										RailConverter.Cpp2Csharp(data, openRoomResult);
										railEventCallBackHandler(event_id, openRoomResult);
										return;
									}
									case RAILEventID.kRailEventRoomSetRoomTagResult:
									{
										SetRoomTagResult setRoomTagResult = new SetRoomTagResult();
										RailConverter.Cpp2Csharp(data, setRoomTagResult);
										railEventCallBackHandler(event_id, setRoomTagResult);
										return;
									}
									case RAILEventID.kRailEventRoomGetRoomTagResult:
									{
										GetRoomTagResult getRoomTagResult = new GetRoomTagResult();
										RailConverter.Cpp2Csharp(data, getRoomTagResult);
										railEventCallBackHandler(event_id, getRoomTagResult);
										return;
									}
									case RAILEventID.kRailEventRoomSetNewRoomOwnerResult:
									{
										SetNewRoomOwnerResult setNewRoomOwnerResult = new SetNewRoomOwnerResult();
										RailConverter.Cpp2Csharp(data, setNewRoomOwnerResult);
										railEventCallBackHandler(event_id, setNewRoomOwnerResult);
										return;
									}
									case RAILEventID.kRailEventRoomSetRoomTypeResult:
									{
										SetRoomTypeResult setRoomTypeResult = new SetRoomTypeResult();
										RailConverter.Cpp2Csharp(data, setRoomTypeResult);
										railEventCallBackHandler(event_id, setRoomTypeResult);
										return;
									}
									case RAILEventID.kRailEventRoomSetRoomMaxMemberResult:
									{
										SetRoomMaxMemberResult setRoomMaxMemberResult = new SetRoomMaxMemberResult();
										RailConverter.Cpp2Csharp(data, setRoomMaxMemberResult);
										railEventCallBackHandler(event_id, setRoomMaxMemberResult);
										return;
									}
									default:
										return;
									}
									break;
								}
								break;
							}
						}
					}
					else if (event_id <= RAILEventID.kRailEventPlayerGetPlayerMetadataResult)
					{
						switch (event_id)
						{
						case RAILEventID.kRailEventRoomNotifyMetadataChanged:
						{
							NotifyMetadataChange notifyMetadataChange = new NotifyMetadataChange();
							RailConverter.Cpp2Csharp(data, notifyMetadataChange);
							railEventCallBackHandler(event_id, notifyMetadataChange);
							return;
						}
						case RAILEventID.kRailEventRoomNotifyMemberChanged:
						{
							NotifyRoomMemberChange notifyRoomMemberChange = new NotifyRoomMemberChange();
							RailConverter.Cpp2Csharp(data, notifyRoomMemberChange);
							railEventCallBackHandler(event_id, notifyRoomMemberChange);
							return;
						}
						case RAILEventID.kRailEventRoomNotifyMemberkicked:
						{
							NotifyRoomMemberKicked notifyRoomMemberKicked = new NotifyRoomMemberKicked();
							RailConverter.Cpp2Csharp(data, notifyRoomMemberKicked);
							railEventCallBackHandler(event_id, notifyRoomMemberKicked);
							return;
						}
						case RAILEventID.kRailEventRoomNotifyRoomDestroyed:
						{
							NotifyRoomDestroy notifyRoomDestroy = new NotifyRoomDestroy();
							RailConverter.Cpp2Csharp(data, notifyRoomDestroy);
							railEventCallBackHandler(event_id, notifyRoomDestroy);
							return;
						}
						case RAILEventID.kRailEventRoomNotifyRoomOwnerChanged:
						{
							NotifyRoomOwnerChange notifyRoomOwnerChange = new NotifyRoomOwnerChange();
							RailConverter.Cpp2Csharp(data, notifyRoomOwnerChange);
							railEventCallBackHandler(event_id, notifyRoomOwnerChange);
							return;
						}
						case RAILEventID.kRailEventRoomNotifyRoomDataReceived:
						{
							RoomDataReceived roomDataReceived = new RoomDataReceived();
							RailConverter.Cpp2Csharp(data, roomDataReceived);
							railEventCallBackHandler(event_id, roomDataReceived);
							return;
						}
						case RAILEventID.kRailEventRoomNotifyRoomGameServerChanged:
						{
							NotifyRoomGameServerChange notifyRoomGameServerChange = new NotifyRoomGameServerChange();
							RailConverter.Cpp2Csharp(data, notifyRoomGameServerChange);
							railEventCallBackHandler(event_id, notifyRoomGameServerChange);
							return;
						}
						default:
							switch (event_id)
							{
							case RAILEventID.kRailEventFriendsSetMetadataResult:
							{
								RailFriendsSetMetadataResult railFriendsSetMetadataResult = new RailFriendsSetMetadataResult();
								RailConverter.Cpp2Csharp(data, railFriendsSetMetadataResult);
								railEventCallBackHandler(event_id, railFriendsSetMetadataResult);
								return;
							}
							case RAILEventID.kRailEventFriendsGetMetadataResult:
							{
								RailFriendsGetMetadataResult railFriendsGetMetadataResult = new RailFriendsGetMetadataResult();
								RailConverter.Cpp2Csharp(data, railFriendsGetMetadataResult);
								railEventCallBackHandler(event_id, railFriendsGetMetadataResult);
								return;
							}
							case RAILEventID.kRailEventFriendsClearMetadataResult:
							{
								RailFriendsClearMetadataResult railFriendsClearMetadataResult = new RailFriendsClearMetadataResult();
								RailConverter.Cpp2Csharp(data, railFriendsClearMetadataResult);
								railEventCallBackHandler(event_id, railFriendsClearMetadataResult);
								return;
							}
							case RAILEventID.kRailEventFriendsGetInviteCommandLine:
							{
								RailFriendsGetInviteCommandLine railFriendsGetInviteCommandLine = new RailFriendsGetInviteCommandLine();
								RailConverter.Cpp2Csharp(data, railFriendsGetInviteCommandLine);
								railEventCallBackHandler(event_id, railFriendsGetInviteCommandLine);
								return;
							}
							case RAILEventID.kRailEventFriendsReportPlayedWithUserListResult:
							{
								RailFriendsReportPlayedWithUserListResult railFriendsReportPlayedWithUserListResult = new RailFriendsReportPlayedWithUserListResult();
								RailConverter.Cpp2Csharp(data, railFriendsReportPlayedWithUserListResult);
								railEventCallBackHandler(event_id, railFriendsReportPlayedWithUserListResult);
								return;
							}
							case (RAILEventID)12007:
							case (RAILEventID)12008:
							case (RAILEventID)12009:
								break;
							case RAILEventID.kRailEventFriendsFriendsListChanged:
							{
								RailFriendsListChanged railFriendsListChanged = new RailFriendsListChanged();
								RailConverter.Cpp2Csharp(data, railFriendsListChanged);
								railEventCallBackHandler(event_id, railFriendsListChanged);
								return;
							}
							case RAILEventID.kRailEventFriendsGetFriendPlayedGamesResult:
							{
								RailFriendsQueryFriendPlayedGamesResult railFriendsQueryFriendPlayedGamesResult = new RailFriendsQueryFriendPlayedGamesResult();
								RailConverter.Cpp2Csharp(data, railFriendsQueryFriendPlayedGamesResult);
								railEventCallBackHandler(event_id, railFriendsQueryFriendPlayedGamesResult);
								return;
							}
							case RAILEventID.kRailEventFriendsQueryPlayedWithFriendsListResult:
							{
								RailFriendsQueryPlayedWithFriendsListResult railFriendsQueryPlayedWithFriendsListResult = new RailFriendsQueryPlayedWithFriendsListResult();
								RailConverter.Cpp2Csharp(data, railFriendsQueryPlayedWithFriendsListResult);
								railEventCallBackHandler(event_id, railFriendsQueryPlayedWithFriendsListResult);
								return;
							}
							case RAILEventID.kRailEventFriendsQueryPlayedWithFriendsTimeResult:
							{
								RailFriendsQueryPlayedWithFriendsTimeResult railFriendsQueryPlayedWithFriendsTimeResult = new RailFriendsQueryPlayedWithFriendsTimeResult();
								RailConverter.Cpp2Csharp(data, railFriendsQueryPlayedWithFriendsTimeResult);
								railEventCallBackHandler(event_id, railFriendsQueryPlayedWithFriendsTimeResult);
								return;
							}
							case RAILEventID.kRailEventFriendsQueryPlayedWithFriendsGamesResult:
							{
								RailFriendsQueryPlayedWithFriendsGamesResult railFriendsQueryPlayedWithFriendsGamesResult = new RailFriendsQueryPlayedWithFriendsGamesResult();
								RailConverter.Cpp2Csharp(data, railFriendsQueryPlayedWithFriendsGamesResult);
								railEventCallBackHandler(event_id, railFriendsQueryPlayedWithFriendsGamesResult);
								return;
							}
							case RAILEventID.kRailEventFriendsAddFriendResult:
							{
								RailFriendsAddFriendResult railFriendsAddFriendResult = new RailFriendsAddFriendResult();
								RailConverter.Cpp2Csharp(data, railFriendsAddFriendResult);
								railEventCallBackHandler(event_id, railFriendsAddFriendResult);
								return;
							}
							case RAILEventID.kRailEventFriendsOnlineStateChanged:
							{
								RailFriendsOnlineStateChanged railFriendsOnlineStateChanged = new RailFriendsOnlineStateChanged();
								RailConverter.Cpp2Csharp(data, railFriendsOnlineStateChanged);
								railEventCallBackHandler(event_id, railFriendsOnlineStateChanged);
								return;
							}
							case RAILEventID.kRailEventFriendsMetadataChanged:
							{
								RailFriendsMetadataChanged railFriendsMetadataChanged = new RailFriendsMetadataChanged();
								RailConverter.Cpp2Csharp(data, railFriendsMetadataChanged);
								railEventCallBackHandler(event_id, railFriendsMetadataChanged);
								return;
							}
							default:
								switch (event_id)
								{
								case RAILEventID.kRailEventSessionTicketGetSessionTicket:
								{
									AcquireSessionTicketResponse acquireSessionTicketResponse = new AcquireSessionTicketResponse();
									RailConverter.Cpp2Csharp(data, acquireSessionTicketResponse);
									railEventCallBackHandler(event_id, acquireSessionTicketResponse);
									return;
								}
								case RAILEventID.kRailEventSessionTicketAuthSessionTicket:
								{
									StartSessionWithPlayerResponse startSessionWithPlayerResponse = new StartSessionWithPlayerResponse();
									RailConverter.Cpp2Csharp(data, startSessionWithPlayerResponse);
									railEventCallBackHandler(event_id, startSessionWithPlayerResponse);
									return;
								}
								case RAILEventID.kRailEventPlayerGetGamePurchaseKey:
								{
									PlayerGetGamePurchaseKeyResult playerGetGamePurchaseKeyResult = new PlayerGetGamePurchaseKeyResult();
									RailConverter.Cpp2Csharp(data, playerGetGamePurchaseKeyResult);
									railEventCallBackHandler(event_id, playerGetGamePurchaseKeyResult);
									return;
								}
								case RAILEventID.kRailEventQueryPlayerBannedStatus:
								{
									QueryPlayerBannedStatus queryPlayerBannedStatus = new QueryPlayerBannedStatus();
									RailConverter.Cpp2Csharp(data, queryPlayerBannedStatus);
									railEventCallBackHandler(event_id, queryPlayerBannedStatus);
									return;
								}
								case RAILEventID.kRailEventPlayerGetAuthenticateURL:
								{
									GetAuthenticateURLResult getAuthenticateURLResult = new GetAuthenticateURLResult();
									RailConverter.Cpp2Csharp(data, getAuthenticateURLResult);
									railEventCallBackHandler(event_id, getAuthenticateURLResult);
									return;
								}
								case RAILEventID.kRailEventPlayerAntiAddictionGameOnlineTimeChanged:
								{
									RailAntiAddictionGameOnlineTimeChanged railAntiAddictionGameOnlineTimeChanged = new RailAntiAddictionGameOnlineTimeChanged();
									RailConverter.Cpp2Csharp(data, railAntiAddictionGameOnlineTimeChanged);
									railEventCallBackHandler(event_id, railAntiAddictionGameOnlineTimeChanged);
									return;
								}
								case RAILEventID.kRailEventPlayerGetEncryptedGameTicketResult:
								{
									RailGetEncryptedGameTicketResult railGetEncryptedGameTicketResult = new RailGetEncryptedGameTicketResult();
									RailConverter.Cpp2Csharp(data, railGetEncryptedGameTicketResult);
									railEventCallBackHandler(event_id, railGetEncryptedGameTicketResult);
									return;
								}
								case RAILEventID.kRailEventPlayerGetPlayerMetadataResult:
								{
									RailGetPlayerMetadataResult railGetPlayerMetadataResult = new RailGetPlayerMetadataResult();
									RailConverter.Cpp2Csharp(data, railGetPlayerMetadataResult);
									railEventCallBackHandler(event_id, railGetPlayerMetadataResult);
									return;
								}
								default:
									return;
								}
								break;
							}
							break;
						}
					}
					else
					{
						switch (event_id)
						{
						case RAILEventID.kRailEventUsersGetUsersInfo:
						{
							RailUsersInfoData railUsersInfoData = new RailUsersInfoData();
							RailConverter.Cpp2Csharp(data, railUsersInfoData);
							railEventCallBackHandler(event_id, railUsersInfoData);
							return;
						}
						case RAILEventID.kRailEventUsersNotifyInviter:
						{
							RailUsersNotifyInviter railUsersNotifyInviter = new RailUsersNotifyInviter();
							RailConverter.Cpp2Csharp(data, railUsersNotifyInviter);
							railEventCallBackHandler(event_id, railUsersNotifyInviter);
							return;
						}
						case RAILEventID.kRailEventUsersRespondInvitation:
						{
							RailUsersRespondInvitation railUsersRespondInvitation = new RailUsersRespondInvitation();
							RailConverter.Cpp2Csharp(data, railUsersRespondInvitation);
							railEventCallBackHandler(event_id, railUsersRespondInvitation);
							return;
						}
						case RAILEventID.kRailEventUsersInviteJoinGameResult:
						{
							RailUsersInviteJoinGameResult railUsersInviteJoinGameResult = new RailUsersInviteJoinGameResult();
							RailConverter.Cpp2Csharp(data, railUsersInviteJoinGameResult);
							railEventCallBackHandler(event_id, railUsersInviteJoinGameResult);
							return;
						}
						case RAILEventID.kRailEventUsersInviteUsersResult:
						{
							RailUsersInviteUsersResult railUsersInviteUsersResult = new RailUsersInviteUsersResult();
							RailConverter.Cpp2Csharp(data, railUsersInviteUsersResult);
							railEventCallBackHandler(event_id, railUsersInviteUsersResult);
							return;
						}
						case RAILEventID.kRailEventUsersGetInviteDetailResult:
						{
							RailUsersGetInviteDetailResult railUsersGetInviteDetailResult = new RailUsersGetInviteDetailResult();
							RailConverter.Cpp2Csharp(data, railUsersGetInviteDetailResult);
							railEventCallBackHandler(event_id, railUsersGetInviteDetailResult);
							return;
						}
						case RAILEventID.kRailEventUsersCancelInviteResult:
						{
							RailUsersCancelInviteResult railUsersCancelInviteResult = new RailUsersCancelInviteResult();
							RailConverter.Cpp2Csharp(data, railUsersCancelInviteResult);
							railEventCallBackHandler(event_id, railUsersCancelInviteResult);
							return;
						}
						case RAILEventID.kRailEventUsersGetUserLimitsResult:
						{
							RailUsersGetUserLimitsResult railUsersGetUserLimitsResult = new RailUsersGetUserLimitsResult();
							RailConverter.Cpp2Csharp(data, railUsersGetUserLimitsResult);
							railEventCallBackHandler(event_id, railUsersGetUserLimitsResult);
							return;
						}
						case RAILEventID.kRailEventUsersShowChatWindowWithFriendResult:
						{
							RailShowChatWindowWithFriendResult railShowChatWindowWithFriendResult = new RailShowChatWindowWithFriendResult();
							RailConverter.Cpp2Csharp(data, railShowChatWindowWithFriendResult);
							railEventCallBackHandler(event_id, railShowChatWindowWithFriendResult);
							return;
						}
						case RAILEventID.kRailEventUsersShowUserHomepageWindowResult:
						{
							RailShowUserHomepageWindowResult railShowUserHomepageWindowResult = new RailShowUserHomepageWindowResult();
							RailConverter.Cpp2Csharp(data, railShowUserHomepageWindowResult);
							railEventCallBackHandler(event_id, railShowUserHomepageWindowResult);
							return;
						}
						default:
						{
							if (event_id == RAILEventID.kRailEventShowFloatingWindow)
							{
								ShowFloatingWindowResult showFloatingWindowResult = new ShowFloatingWindowResult();
								RailConverter.Cpp2Csharp(data, showFloatingWindowResult);
								railEventCallBackHandler(event_id, showFloatingWindowResult);
								return;
							}
							if (event_id != RAILEventID.kRailEventShowFloatingNotifyWindow)
							{
								return;
							}
							ShowNotifyWindow showNotifyWindow = new ShowNotifyWindow();
							RailConverter.Cpp2Csharp(data, showNotifyWindow);
							railEventCallBackHandler(event_id, showNotifyWindow);
							return;
						}
						}
					}
				}
				else if (event_id <= RAILEventID.kRailEventSmallObjectServiceQueryObjectStateResult)
				{
					if (event_id <= RAILEventID.kRailEventVoiceChannelSpeakingUsersChangedEvent)
					{
						if (event_id <= RAILEventID.kRailEventNetworkCreateRawSessionFailed)
						{
							switch (event_id)
							{
							case RAILEventID.kRailEventBrowserCreateResult:
							{
								CreateBrowserResult createBrowserResult = new CreateBrowserResult();
								RailConverter.Cpp2Csharp(data, createBrowserResult);
								railEventCallBackHandler(event_id, createBrowserResult);
								return;
							}
							case RAILEventID.kRailEventBrowserReloadResult:
							{
								ReloadBrowserResult reloadBrowserResult = new ReloadBrowserResult();
								RailConverter.Cpp2Csharp(data, reloadBrowserResult);
								railEventCallBackHandler(event_id, reloadBrowserResult);
								return;
							}
							case RAILEventID.kRailEventBrowserCloseResult:
							{
								CloseBrowserResult closeBrowserResult = new CloseBrowserResult();
								RailConverter.Cpp2Csharp(data, closeBrowserResult);
								railEventCallBackHandler(event_id, closeBrowserResult);
								return;
							}
							case RAILEventID.kRailEventBrowserJavascriptEvent:
							{
								JavascriptEventResult javascriptEventResult = new JavascriptEventResult();
								RailConverter.Cpp2Csharp(data, javascriptEventResult);
								railEventCallBackHandler(event_id, javascriptEventResult);
								return;
							}
							case RAILEventID.kRailEventBrowserTryNavigateNewPageRequest:
							{
								BrowserTryNavigateNewPageRequest browserTryNavigateNewPageRequest = new BrowserTryNavigateNewPageRequest();
								RailConverter.Cpp2Csharp(data, browserTryNavigateNewPageRequest);
								railEventCallBackHandler(event_id, browserTryNavigateNewPageRequest);
								return;
							}
							case RAILEventID.kRailEventBrowserPaint:
							{
								BrowserNeedsPaintRequest browserNeedsPaintRequest = new BrowserNeedsPaintRequest();
								RailConverter.Cpp2Csharp(data, browserNeedsPaintRequest);
								railEventCallBackHandler(event_id, browserNeedsPaintRequest);
								return;
							}
							case RAILEventID.kRailEventBrowserDamageRectPaint:
							{
								BrowserDamageRectNeedsPaintRequest browserDamageRectNeedsPaintRequest = new BrowserDamageRectNeedsPaintRequest();
								RailConverter.Cpp2Csharp(data, browserDamageRectNeedsPaintRequest);
								railEventCallBackHandler(event_id, browserDamageRectNeedsPaintRequest);
								return;
							}
							case RAILEventID.kRailEventBrowserNavigeteResult:
							{
								BrowserRenderNavigateResult browserRenderNavigateResult = new BrowserRenderNavigateResult();
								RailConverter.Cpp2Csharp(data, browserRenderNavigateResult);
								railEventCallBackHandler(event_id, browserRenderNavigateResult);
								return;
							}
							case RAILEventID.kRailEventBrowserStateChanged:
							{
								BrowserRenderStateChanged browserRenderStateChanged = new BrowserRenderStateChanged();
								RailConverter.Cpp2Csharp(data, browserRenderStateChanged);
								railEventCallBackHandler(event_id, browserRenderStateChanged);
								return;
							}
							case RAILEventID.kRailEventBrowserTitleChanged:
							{
								BrowserRenderTitleChanged browserRenderTitleChanged = new BrowserRenderTitleChanged();
								RailConverter.Cpp2Csharp(data, browserRenderTitleChanged);
								railEventCallBackHandler(event_id, browserRenderTitleChanged);
								return;
							}
							default:
								switch (event_id)
								{
								case RAILEventID.kRailEventNetworkCreateSessionRequest:
								{
									CreateSessionRequest createSessionRequest = new CreateSessionRequest();
									RailConverter.Cpp2Csharp(data, createSessionRequest);
									railEventCallBackHandler(event_id, createSessionRequest);
									return;
								}
								case RAILEventID.kRailEventNetworkCreateSessionFailed:
								{
									CreateSessionFailed createSessionFailed = new CreateSessionFailed();
									RailConverter.Cpp2Csharp(data, createSessionFailed);
									railEventCallBackHandler(event_id, createSessionFailed);
									return;
								}
								case RAILEventID.kRailEventNetworkCreateRawSessionRequest:
								{
									NetworkCreateRawSessionRequest networkCreateRawSessionRequest = new NetworkCreateRawSessionRequest();
									RailConverter.Cpp2Csharp(data, networkCreateRawSessionRequest);
									railEventCallBackHandler(event_id, networkCreateRawSessionRequest);
									return;
								}
								case RAILEventID.kRailEventNetworkCreateRawSessionFailed:
								{
									NetworkCreateRawSessionFailed networkCreateRawSessionFailed = new NetworkCreateRawSessionFailed();
									RailConverter.Cpp2Csharp(data, networkCreateRawSessionFailed);
									railEventCallBackHandler(event_id, networkCreateRawSessionFailed);
									return;
								}
								default:
									return;
								}
								break;
							}
						}
						else
						{
							switch (event_id)
							{
							case RAILEventID.kRailEventDlcInstallStart:
							{
								DlcInstallStart dlcInstallStart = new DlcInstallStart();
								RailConverter.Cpp2Csharp(data, dlcInstallStart);
								railEventCallBackHandler(event_id, dlcInstallStart);
								return;
							}
							case RAILEventID.kRailEventDlcInstallStartResult:
							{
								DlcInstallStartResult dlcInstallStartResult = new DlcInstallStartResult();
								RailConverter.Cpp2Csharp(data, dlcInstallStartResult);
								railEventCallBackHandler(event_id, dlcInstallStartResult);
								return;
							}
							case RAILEventID.kRailEventDlcInstallProgress:
							{
								DlcInstallProgress dlcInstallProgress = new DlcInstallProgress();
								RailConverter.Cpp2Csharp(data, dlcInstallProgress);
								railEventCallBackHandler(event_id, dlcInstallProgress);
								return;
							}
							case RAILEventID.kRailEventDlcInstallFinished:
							{
								DlcInstallFinished dlcInstallFinished = new DlcInstallFinished();
								RailConverter.Cpp2Csharp(data, dlcInstallFinished);
								railEventCallBackHandler(event_id, dlcInstallFinished);
								return;
							}
							case RAILEventID.kRailEventDlcUninstallFinished:
							{
								DlcUninstallFinished dlcUninstallFinished = new DlcUninstallFinished();
								RailConverter.Cpp2Csharp(data, dlcUninstallFinished);
								railEventCallBackHandler(event_id, dlcUninstallFinished);
								return;
							}
							case RAILEventID.kRailEventDlcCheckAllDlcsStateReadyResult:
							{
								CheckAllDlcsStateReadyResult checkAllDlcsStateReadyResult = new CheckAllDlcsStateReadyResult();
								RailConverter.Cpp2Csharp(data, checkAllDlcsStateReadyResult);
								railEventCallBackHandler(event_id, checkAllDlcsStateReadyResult);
								return;
							}
							case RAILEventID.kRailEventDlcQueryIsOwnedDlcsResult:
							{
								QueryIsOwnedDlcsResult queryIsOwnedDlcsResult = new QueryIsOwnedDlcsResult();
								RailConverter.Cpp2Csharp(data, queryIsOwnedDlcsResult);
								railEventCallBackHandler(event_id, queryIsOwnedDlcsResult);
								return;
							}
							case RAILEventID.kRailEventDlcOwnershipChanged:
							{
								DlcOwnershipChanged dlcOwnershipChanged = new DlcOwnershipChanged();
								RailConverter.Cpp2Csharp(data, dlcOwnershipChanged);
								railEventCallBackHandler(event_id, dlcOwnershipChanged);
								return;
							}
							case RAILEventID.kRailEventDlcRefundChanged:
							{
								DlcRefundChanged dlcRefundChanged = new DlcRefundChanged();
								RailConverter.Cpp2Csharp(data, dlcRefundChanged);
								railEventCallBackHandler(event_id, dlcRefundChanged);
								return;
							}
							default:
								switch (event_id)
								{
								case RAILEventID.kRailEventScreenshotTakeScreenshotFinished:
								{
									TakeScreenshotResult takeScreenshotResult = new TakeScreenshotResult();
									RailConverter.Cpp2Csharp(data, takeScreenshotResult);
									railEventCallBackHandler(event_id, takeScreenshotResult);
									return;
								}
								case RAILEventID.kRailEventScreenshotTakeScreenshotRequest:
								{
									ScreenshotRequestInfo screenshotRequestInfo = new ScreenshotRequestInfo();
									RailConverter.Cpp2Csharp(data, screenshotRequestInfo);
									railEventCallBackHandler(event_id, screenshotRequestInfo);
									return;
								}
								case RAILEventID.kRailEventScreenshotPublishScreenshotFinished:
								{
									PublishScreenshotResult publishScreenshotResult = new PublishScreenshotResult();
									RailConverter.Cpp2Csharp(data, publishScreenshotResult);
									railEventCallBackHandler(event_id, publishScreenshotResult);
									return;
								}
								default:
									switch (event_id)
									{
									case RAILEventID.kRailEventVoiceChannelCreateResult:
									{
										CreateVoiceChannelResult createVoiceChannelResult = new CreateVoiceChannelResult();
										RailConverter.Cpp2Csharp(data, createVoiceChannelResult);
										railEventCallBackHandler(event_id, createVoiceChannelResult);
										return;
									}
									case RAILEventID.kRailEventVoiceChannelDataCaptured:
									{
										VoiceDataCapturedEvent voiceDataCapturedEvent = new VoiceDataCapturedEvent();
										RailConverter.Cpp2Csharp(data, voiceDataCapturedEvent);
										railEventCallBackHandler(event_id, voiceDataCapturedEvent);
										return;
									}
									case RAILEventID.kRailEventVoiceChannelJoinedResult:
									{
										JoinVoiceChannelResult joinVoiceChannelResult = new JoinVoiceChannelResult();
										RailConverter.Cpp2Csharp(data, joinVoiceChannelResult);
										railEventCallBackHandler(event_id, joinVoiceChannelResult);
										return;
									}
									case RAILEventID.kRailEventVoiceChannelLeaveResult:
									{
										LeaveVoiceChannelResult leaveVoiceChannelResult = new LeaveVoiceChannelResult();
										RailConverter.Cpp2Csharp(data, leaveVoiceChannelResult);
										railEventCallBackHandler(event_id, leaveVoiceChannelResult);
										return;
									}
									case RAILEventID.kRailEventVoiceChannelAddUsersResult:
									{
										VoiceChannelAddUsersResult voiceChannelAddUsersResult = new VoiceChannelAddUsersResult();
										RailConverter.Cpp2Csharp(data, voiceChannelAddUsersResult);
										railEventCallBackHandler(event_id, voiceChannelAddUsersResult);
										return;
									}
									case RAILEventID.kRailEventVoiceChannelRemoveUsersResult:
									{
										VoiceChannelRemoveUsersResult voiceChannelRemoveUsersResult = new VoiceChannelRemoveUsersResult();
										RailConverter.Cpp2Csharp(data, voiceChannelRemoveUsersResult);
										railEventCallBackHandler(event_id, voiceChannelRemoveUsersResult);
										return;
									}
									case RAILEventID.kRailEventVoiceChannelInviteEvent:
									{
										VoiceChannelInviteEvent voiceChannelInviteEvent = new VoiceChannelInviteEvent();
										RailConverter.Cpp2Csharp(data, voiceChannelInviteEvent);
										railEventCallBackHandler(event_id, voiceChannelInviteEvent);
										return;
									}
									case RAILEventID.kRailEventVoiceChannelMemberChangedEvent:
									{
										VoiceChannelMemeberChangedEvent voiceChannelMemeberChangedEvent = new VoiceChannelMemeberChangedEvent();
										RailConverter.Cpp2Csharp(data, voiceChannelMemeberChangedEvent);
										railEventCallBackHandler(event_id, voiceChannelMemeberChangedEvent);
										return;
									}
									case RAILEventID.kRailEventVoiceChannelUsersSpeakingStateChangedEvent:
									{
										VoiceChannelUsersSpeakingStateChangedEvent voiceChannelUsersSpeakingStateChangedEvent = new VoiceChannelUsersSpeakingStateChangedEvent();
										RailConverter.Cpp2Csharp(data, voiceChannelUsersSpeakingStateChangedEvent);
										railEventCallBackHandler(event_id, voiceChannelUsersSpeakingStateChangedEvent);
										return;
									}
									case RAILEventID.kRailEventVoiceChannelPushToTalkKeyChangedEvent:
									{
										VoiceChannelPushToTalkKeyChangedEvent voiceChannelPushToTalkKeyChangedEvent = new VoiceChannelPushToTalkKeyChangedEvent();
										RailConverter.Cpp2Csharp(data, voiceChannelPushToTalkKeyChangedEvent);
										railEventCallBackHandler(event_id, voiceChannelPushToTalkKeyChangedEvent);
										return;
									}
									case RAILEventID.kRailEventVoiceChannelSpeakingUsersChangedEvent:
									{
										VoiceChannelSpeakingUsersChangedEvent voiceChannelSpeakingUsersChangedEvent = new VoiceChannelSpeakingUsersChangedEvent();
										RailConverter.Cpp2Csharp(data, voiceChannelSpeakingUsersChangedEvent);
										railEventCallBackHandler(event_id, voiceChannelSpeakingUsersChangedEvent);
										return;
									}
									default:
										return;
									}
									break;
								}
								break;
							}
						}
					}
					else if (event_id <= RAILEventID.kRailEventIMEHelperTextInputSelectedResult)
					{
						if (event_id == RAILEventID.kRailEventAppQuerySubscribeWishPlayStateResult)
						{
							QuerySubscribeWishPlayStateResult querySubscribeWishPlayStateResult = new QuerySubscribeWishPlayStateResult();
							RailConverter.Cpp2Csharp(data, querySubscribeWishPlayStateResult);
							railEventCallBackHandler(event_id, querySubscribeWishPlayStateResult);
							return;
						}
						if (event_id == RAILEventID.kRailEventTextInputShowTextInputWindowResult)
						{
							RailTextInputResult railTextInputResult = new RailTextInputResult();
							RailConverter.Cpp2Csharp(data, railTextInputResult);
							railEventCallBackHandler(event_id, railTextInputResult);
							return;
						}
						if (event_id != RAILEventID.kRailEventIMEHelperTextInputSelectedResult)
						{
							return;
						}
						RailIMEHelperTextInputSelectedResult railIMEHelperTextInputSelectedResult = new RailIMEHelperTextInputSelectedResult();
						RailConverter.Cpp2Csharp(data, railIMEHelperTextInputSelectedResult);
						railEventCallBackHandler(event_id, railIMEHelperTextInputSelectedResult);
						return;
					}
					else
					{
						if (event_id == RAILEventID.kRailEventIMEHelperTextInputCompositionStateChanged)
						{
							RailIMEHelperTextInputCompositionState railIMEHelperTextInputCompositionState = new RailIMEHelperTextInputCompositionState();
							RailConverter.Cpp2Csharp(data, railIMEHelperTextInputCompositionState);
							railEventCallBackHandler(event_id, railIMEHelperTextInputCompositionState);
							return;
						}
						if (event_id == RAILEventID.kRailEventHttpSessionResponseResult)
						{
							RailHttpSessionResponse railHttpSessionResponse = new RailHttpSessionResponse();
							RailConverter.Cpp2Csharp(data, railHttpSessionResponse);
							railEventCallBackHandler(event_id, railHttpSessionResponse);
							return;
						}
						if (event_id != RAILEventID.kRailEventSmallObjectServiceQueryObjectStateResult)
						{
							return;
						}
						RailSmallObjectStateQueryResult railSmallObjectStateQueryResult = new RailSmallObjectStateQueryResult();
						RailConverter.Cpp2Csharp(data, railSmallObjectStateQueryResult);
						railEventCallBackHandler(event_id, railSmallObjectStateQueryResult);
						return;
					}
				}
				else if (event_id <= RAILEventID.kRailEventInGameCoinRequestCoinInfoResult)
				{
					if (event_id <= RAILEventID.kRailEventZoneServerSwitchPlayerSelectedZoneResult)
					{
						if (event_id == RAILEventID.kRailEventSmallObjectServiceDownloadResult)
						{
							RailSmallObjectDownloadResult railSmallObjectDownloadResult = new RailSmallObjectDownloadResult();
							RailConverter.Cpp2Csharp(data, railSmallObjectDownloadResult);
							railEventCallBackHandler(event_id, railSmallObjectDownloadResult);
							return;
						}
						if (event_id != RAILEventID.kRailEventZoneServerSwitchPlayerSelectedZoneResult)
						{
							return;
						}
						RailSwitchPlayerSelectedZoneResult railSwitchPlayerSelectedZoneResult = new RailSwitchPlayerSelectedZoneResult();
						RailConverter.Cpp2Csharp(data, railSwitchPlayerSelectedZoneResult);
						railEventCallBackHandler(event_id, railSwitchPlayerSelectedZoneResult);
						return;
					}
					else
					{
						if (event_id == RAILEventID.kRailEventGroupChatQueryGroupsInfoResult)
						{
							RailQueryGroupsInfoResult railQueryGroupsInfoResult = new RailQueryGroupsInfoResult();
							RailConverter.Cpp2Csharp(data, railQueryGroupsInfoResult);
							railEventCallBackHandler(event_id, railQueryGroupsInfoResult);
							return;
						}
						if (event_id == RAILEventID.kRailEventGroupChatOpenGroupChatResult)
						{
							RailOpenGroupChatResult railOpenGroupChatResult = new RailOpenGroupChatResult();
							RailConverter.Cpp2Csharp(data, railOpenGroupChatResult);
							railEventCallBackHandler(event_id, railOpenGroupChatResult);
							return;
						}
						if (event_id != RAILEventID.kRailEventInGameCoinRequestCoinInfoResult)
						{
							return;
						}
						RailInGameCoinRequestCoinInfoResponse railInGameCoinRequestCoinInfoResponse = new RailInGameCoinRequestCoinInfoResponse();
						RailConverter.Cpp2Csharp(data, railInGameCoinRequestCoinInfoResponse);
						railEventCallBackHandler(event_id, railInGameCoinRequestCoinInfoResponse);
						return;
					}
				}
				else if (event_id <= RAILEventID.kRailEventAntiAddictionQueryGameOnlineTimeResult)
				{
					if (event_id == RAILEventID.kRailEventInGameCoinPurchaseCoinsResult)
					{
						RailInGameCoinPurchaseCoinsResponse railInGameCoinPurchaseCoinsResponse = new RailInGameCoinPurchaseCoinsResponse();
						RailConverter.Cpp2Csharp(data, railInGameCoinPurchaseCoinsResponse);
						railEventCallBackHandler(event_id, railInGameCoinPurchaseCoinsResponse);
						return;
					}
					switch (event_id)
					{
					case RAILEventID.kRailEventInGameActivityQueryGameActivityResult:
					{
						RailQueryGameActivityResult railQueryGameActivityResult = new RailQueryGameActivityResult();
						RailConverter.Cpp2Csharp(data, railQueryGameActivityResult);
						railEventCallBackHandler(event_id, railQueryGameActivityResult);
						return;
					}
					case RAILEventID.kRailEventInGameActivityOpenGameActivityWindowResult:
					{
						RailOpenGameActivityWindowResult railOpenGameActivityWindowResult = new RailOpenGameActivityWindowResult();
						RailConverter.Cpp2Csharp(data, railOpenGameActivityWindowResult);
						railEventCallBackHandler(event_id, railOpenGameActivityWindowResult);
						return;
					}
					case RAILEventID.kRailEventInGameActivityNotifyNewGameActivities:
					{
						RailNotifyNewGameActivities railNotifyNewGameActivities = new RailNotifyNewGameActivities();
						RailConverter.Cpp2Csharp(data, railNotifyNewGameActivities);
						railEventCallBackHandler(event_id, railNotifyNewGameActivities);
						return;
					}
					case RAILEventID.kRailEventInGameActivityGameActivityPlayerEvent:
					{
						RailGameActivityPlayerEvent railGameActivityPlayerEvent = new RailGameActivityPlayerEvent();
						RailConverter.Cpp2Csharp(data, railGameActivityPlayerEvent);
						railEventCallBackHandler(event_id, railGameActivityPlayerEvent);
						return;
					}
					default:
					{
						if (event_id != RAILEventID.kRailEventAntiAddictionQueryGameOnlineTimeResult)
						{
							return;
						}
						RailQueryGameOnlineTimeResult railQueryGameOnlineTimeResult = new RailQueryGameOnlineTimeResult();
						RailConverter.Cpp2Csharp(data, railQueryGameOnlineTimeResult);
						railEventCallBackHandler(event_id, railQueryGameOnlineTimeResult);
						return;
					}
					}
				}
				else
				{
					if (event_id == RAILEventID.kRailEventAntiAddictionCustomizeAntiAddictionActions)
					{
						RailCustomizeAntiAddictionActions railCustomizeAntiAddictionActions = new RailCustomizeAntiAddictionActions();
						RailConverter.Cpp2Csharp(data, railCustomizeAntiAddictionActions);
						railEventCallBackHandler(event_id, railCustomizeAntiAddictionActions);
						return;
					}
					if (event_id == RAILEventID.kRailThirdPartyAccountLoginResult)
					{
						RailThirdPartyAccountLoginResult railThirdPartyAccountLoginResult = new RailThirdPartyAccountLoginResult();
						RailConverter.Cpp2Csharp(data, railThirdPartyAccountLoginResult);
						railEventCallBackHandler(event_id, railThirdPartyAccountLoginResult);
						return;
					}
					if (event_id != RAILEventID.kRailThirdPartyAccountLoginNotifyQrCodeInfo)
					{
						return;
					}
					RailNotifyThirdPartyAccountQrCodeInfo railNotifyThirdPartyAccountQrCodeInfo = new RailNotifyThirdPartyAccountQrCodeInfo();
					RailConverter.Cpp2Csharp(data, railNotifyThirdPartyAccountQrCodeInfo);
					railEventCallBackHandler(event_id, railNotifyThirdPartyAccountQrCodeInfo);
					return;
				}
			}
		}

		private static volatile RailCallBackHelper instance_;

		private static readonly object locker_ = new object();

		private static Dictionary<RAILEventID, RailEventCallBackHandler> eventHandlers_ = new Dictionary<RAILEventID, RailEventCallBackHandler>();

		private static RailEventCallBackFunction delegate_ = new RailEventCallBackFunction(RailCallBackHelper.OnRailCallBack);
	}
}
