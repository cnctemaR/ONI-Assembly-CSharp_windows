using System;

public interface ILogicRibbonBitSelector
{
	void SetBitSelection(int bit);

	int GetBitSelection();

	int GetBitDepth();

	string SideScreenTitle { get; }

	string SideScreenDescription { get; }

	bool SideScreenDisplayWriterDescription();

	bool SideScreenDisplayReaderDescription();

	bool IsBitActive(int bit);

	int GetOutputValue();

	int GetInputValue();

	void UpdateVisuals();
}
