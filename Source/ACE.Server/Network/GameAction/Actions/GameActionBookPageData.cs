using System;

namespace ACE.Server.Network.GameAction.Actions
{
    public static class GameActionBookPageData
    {
        [GameAction(GameActionType.BookPageData)]
        public static void Handle(ClientMessage message, ISession session)
        {
            var bookGuid = message.Payload.ReadGuid(session);
            var pageNum = message.Payload.ReadInt32();     // 0-based

            Console.WriteLine($"0xAE - BookPageData({bookGuid:X16}, {pageNum}) - unused?");

            session.Player.ReadBookPage(bookGuid, pageNum);
        }
    }
}
