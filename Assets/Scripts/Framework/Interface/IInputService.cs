using System;
using System.Collections.Generic;

public interface IInputService:IService
{
    Msg_PlayerInput GetInput();
}
