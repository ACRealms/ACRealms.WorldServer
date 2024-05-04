using ACE.Common.Extensions;
using ACE.Entity;
using ACE.Server.Network.Enum;
using ACE.Server.Network.Managers;

namespace ACE.Server.Network.GameMessages.Messages
{
    public class GameMessageCharacterCreateResponse : GameMessage
    {
        public CharacterGenerationVerificationResponse Response { get; init; }
        public ObjectGuid Guid { get; init; }
        public string CharName { get; init; }

        public GameMessageCharacterCreateResponse(CharacterGenerationVerificationResponse response, ObjectGuid guid, string charName)
            : base(GameMessageOpcode.CharacterCreateResponse, GameMessageGroup.UIQueue)
        {
            if (TestMode)
            {
                Response = response;
                Guid = guid;
                CharName = CharName;
            }

            Writer.Write((uint)response);

            if (response == CharacterGenerationVerificationResponse.Ok)
            {
                Writer.WriteGuid(guid);
                Writer.WriteString16L(charName);
                Writer.Write(0u);
            }
        }
    }
}
