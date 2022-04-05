using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//消息类型
public enum MsgType
{
    C2S_ReqRoomInfo = 0,//请求房间信息
    S2C_RoomInfo,//服务器广播的房间信息
    C2S_ReqJoinRoom,//请求加入房间
    S2C_JoinRoom,//接受服务器加入房价
    C2S_ReqExitRoom,//请求退出房间
    S2C_ExitRoom,//服务器广播房间关闭
    Count,
    Max = 255//最大个数 byte
}
