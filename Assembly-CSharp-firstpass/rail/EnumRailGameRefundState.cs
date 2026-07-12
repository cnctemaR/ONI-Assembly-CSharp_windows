using System;

namespace rail
{
	public enum EnumRailGameRefundState
	{
		kRailGameRefundStateUnknown,
		kRailGameRefundStateApplyReceived = 1000,
		kRailGameRefundStateUserCancelApply = 1100,
		kRailGameRefundStateAdminCancelApply,
		kRailGameRefundStateRefundApproved = 1150,
		kRailGameRefundStateRefundSuccess = 1200,
		kRailGameRefundStateRefundFailed
	}
}
