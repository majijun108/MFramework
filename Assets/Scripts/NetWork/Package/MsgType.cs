using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//消息类型
public enum MsgType
{
    C2S_ReqRoomInfo = 0,//请求房间信息
    Count,
    Max = 255//最大个数 byte
}
