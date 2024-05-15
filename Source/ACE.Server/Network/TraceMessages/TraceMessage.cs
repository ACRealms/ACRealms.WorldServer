using System;
using System.IO;
using log4net;
using ACE.Server.Network.GameMessages;
using ACE.Server.Network;

namespace ACRealms.Server.Network.TraceMessages
{
    public abstract class TraceMessage(GameMessageOpcode _op = GameMessageOpcode.None, GameMessageGroup _group = GameMessageGroup.InvalidQueue)
        : GameMessage(_op, _group)
    {
    }
}
