using System;

public interface IInputHandler
{
	void OnInputEnable(ControlSchema schema);
	void OnInputDisable();
}
