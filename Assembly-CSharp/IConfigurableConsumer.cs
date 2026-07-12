using System;

public interface IConfigurableConsumer
{
	IConfigurableConsumerOption[] GetSettingOptions();

	IConfigurableConsumerOption GetSelectedOption();

	void SetSelectedOption(IConfigurableConsumerOption option);
}
