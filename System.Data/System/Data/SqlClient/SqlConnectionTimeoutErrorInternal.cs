using System;
using System.Text;

namespace System.Data.SqlClient
{
	internal class SqlConnectionTimeoutErrorInternal
	{
		internal SqlConnectionTimeoutErrorPhase CurrentPhase
		{
			get
			{
				return this._currentPhase;
			}
		}

		public SqlConnectionTimeoutErrorInternal()
		{
			this._phaseDurations = new SqlConnectionTimeoutPhaseDuration[9];
			for (int i = 0; i < this._phaseDurations.Length; i++)
			{
				this._phaseDurations[i] = null;
			}
		}

		public void SetFailoverScenario(bool useFailoverServer)
		{
			this._isFailoverScenario = useFailoverServer;
		}

		public void SetInternalSourceType(SqlConnectionInternalSourceType sourceType)
		{
			this._currentSourceType = sourceType;
			if (this._currentSourceType == SqlConnectionInternalSourceType.RoutingDestination)
			{
				this._originalPhaseDurations = this._phaseDurations;
				this._phaseDurations = new SqlConnectionTimeoutPhaseDuration[9];
				this.SetAndBeginPhase(SqlConnectionTimeoutErrorPhase.PreLoginBegin);
			}
		}

		internal void ResetAndRestartPhase()
		{
			this._currentPhase = SqlConnectionTimeoutErrorPhase.PreLoginBegin;
			for (int i = 0; i < this._phaseDurations.Length; i++)
			{
				this._phaseDurations[i] = null;
			}
		}

		internal void SetAndBeginPhase(SqlConnectionTimeoutErrorPhase timeoutErrorPhase)
		{
			this._currentPhase = timeoutErrorPhase;
			if (this._phaseDurations[(int)timeoutErrorPhase] == null)
			{
				this._phaseDurations[(int)timeoutErrorPhase] = new SqlConnectionTimeoutPhaseDuration();
			}
			this._phaseDurations[(int)timeoutErrorPhase].StartCapture();
		}

		internal void EndPhase(SqlConnectionTimeoutErrorPhase timeoutErrorPhase)
		{
			this._phaseDurations[(int)timeoutErrorPhase].StopCapture();
		}

		internal void SetAllCompleteMarker()
		{
			this._currentPhase = SqlConnectionTimeoutErrorPhase.Complete;
		}

		internal string GetErrorMessage()
		{
			StringBuilder stringBuilder;
			string text;
			switch (this._currentPhase)
			{
			case SqlConnectionTimeoutErrorPhase.PreLoginBegin:
				stringBuilder = new StringBuilder(SQLMessage.Timeout_PreLogin_Begin());
				text = SQLMessage.Duration_PreLogin_Begin(this._phaseDurations[1].GetMilliSecondDuration());
				break;
			case SqlConnectionTimeoutErrorPhase.InitializeConnection:
				stringBuilder = new StringBuilder(SQLMessage.Timeout_PreLogin_InitializeConnection());
				text = SQLMessage.Duration_PreLogin_Begin(this._phaseDurations[1].GetMilliSecondDuration() + this._phaseDurations[2].GetMilliSecondDuration());
				break;
			case SqlConnectionTimeoutErrorPhase.SendPreLoginHandshake:
				stringBuilder = new StringBuilder(SQLMessage.Timeout_PreLogin_SendHandshake());
				text = SQLMessage.Duration_PreLoginHandshake(this._phaseDurations[1].GetMilliSecondDuration() + this._phaseDurations[2].GetMilliSecondDuration(), this._phaseDurations[3].GetMilliSecondDuration());
				break;
			case SqlConnectionTimeoutErrorPhase.ConsumePreLoginHandshake:
				stringBuilder = new StringBuilder(SQLMessage.Timeout_PreLogin_ConsumeHandshake());
				text = SQLMessage.Duration_PreLoginHandshake(this._phaseDurations[1].GetMilliSecondDuration() + this._phaseDurations[2].GetMilliSecondDuration(), this._phaseDurations[3].GetMilliSecondDuration() + this._phaseDurations[4].GetMilliSecondDuration());
				break;
			case SqlConnectionTimeoutErrorPhase.LoginBegin:
				stringBuilder = new StringBuilder(SQLMessage.Timeout_Login_Begin());
				text = SQLMessage.Duration_Login_Begin(this._phaseDurations[1].GetMilliSecondDuration() + this._phaseDurations[2].GetMilliSecondDuration(), this._phaseDurations[3].GetMilliSecondDuration() + this._phaseDurations[4].GetMilliSecondDuration(), this._phaseDurations[5].GetMilliSecondDuration());
				break;
			case SqlConnectionTimeoutErrorPhase.ProcessConnectionAuth:
				stringBuilder = new StringBuilder(SQLMessage.Timeout_Login_ProcessConnectionAuth());
				text = SQLMessage.Duration_Login_ProcessConnectionAuth(this._phaseDurations[1].GetMilliSecondDuration() + this._phaseDurations[2].GetMilliSecondDuration(), this._phaseDurations[3].GetMilliSecondDuration() + this._phaseDurations[4].GetMilliSecondDuration(), this._phaseDurations[5].GetMilliSecondDuration(), this._phaseDurations[6].GetMilliSecondDuration());
				break;
			case SqlConnectionTimeoutErrorPhase.PostLogin:
				stringBuilder = new StringBuilder(SQLMessage.Timeout_PostLogin());
				text = SQLMessage.Duration_PostLogin(this._phaseDurations[1].GetMilliSecondDuration() + this._phaseDurations[2].GetMilliSecondDuration(), this._phaseDurations[3].GetMilliSecondDuration() + this._phaseDurations[4].GetMilliSecondDuration(), this._phaseDurations[5].GetMilliSecondDuration(), this._phaseDurations[6].GetMilliSecondDuration(), this._phaseDurations[7].GetMilliSecondDuration());
				break;
			default:
				stringBuilder = new StringBuilder(SQLMessage.Timeout());
				text = null;
				break;
			}
			if (this._currentPhase != SqlConnectionTimeoutErrorPhase.Undefined && this._currentPhase != SqlConnectionTimeoutErrorPhase.Complete)
			{
				if (this._isFailoverScenario)
				{
					stringBuilder.Append("  ");
					stringBuilder.AppendFormat(null, SQLMessage.Timeout_FailoverInfo(), this._currentSourceType);
				}
				else if (this._currentSourceType == SqlConnectionInternalSourceType.RoutingDestination)
				{
					stringBuilder.Append("  ");
					stringBuilder.AppendFormat(null, SQLMessage.Timeout_RoutingDestination(), new object[]
					{
						this._originalPhaseDurations[1].GetMilliSecondDuration() + this._originalPhaseDurations[2].GetMilliSecondDuration(),
						this._originalPhaseDurations[3].GetMilliSecondDuration() + this._originalPhaseDurations[4].GetMilliSecondDuration(),
						this._originalPhaseDurations[5].GetMilliSecondDuration(),
						this._originalPhaseDurations[6].GetMilliSecondDuration(),
						this._originalPhaseDurations[7].GetMilliSecondDuration()
					});
				}
			}
			if (text != null)
			{
				stringBuilder.Append("  ");
				stringBuilder.Append(text);
			}
			return stringBuilder.ToString();
		}

		private SqlConnectionTimeoutPhaseDuration[] _phaseDurations;

		private SqlConnectionTimeoutPhaseDuration[] _originalPhaseDurations;

		private SqlConnectionTimeoutErrorPhase _currentPhase;

		private SqlConnectionInternalSourceType _currentSourceType;

		private bool _isFailoverScenario;
	}
}
