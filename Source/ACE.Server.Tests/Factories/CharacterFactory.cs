using ACE.Database.Models.Auth;
using ACE.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ACE.Database.Models.Shard;
using ACE.Entity;
using ACE.Server.Network;
using ACE.Server.Network.Handlers;
using ACE.Server.Factories;
using ACRealms.Tests.Fixtures.Network;
using ACE.Server.Network.GameMessages.Messages;

namespace ACRealms.Tests.Factories
{
    internal class CharacterNotCreatedException : Exception
    {
        ACE.Server.Network.Enum.CharacterGenerationVerificationResponse Response { get; init; }
        public CharacterNotCreatedException(ACE.Server.Network.Enum.CharacterGenerationVerificationResponse response)
        {
            Response = response;
        }

        public override string ToString() => $"The character was not created successfully: ${Response}";
    }

    internal class CharacterFactory : Factory<Character, CharacterFactory>
    {
        public Account Account { get; init; }
        public FakeSession Session { get; init; }
        CharacterCreateInfo CharacterCreateInfo { get; init; }
        public string CharacterName { get; init; }

        public override Func<Character> Builder() => () =>
        {
            var session = Session ?? FakeSessionFactory.Make();
            var characterName = CharacterName ?? $"Generated {CurrentIndex}";
            var characterCreateInfo = CharacterCreateInfo ?? PlayerFactoryEx.CreateCharacterCreateInfo(characterName, 40, 70, 100, 100, 10, 10, true);
            CharacterHandler.CharacterCreateEx(characterCreateInfo, session);
            var response = session.WaitForMessage<GameMessageCharacterCreateResponse>();
            if (response.Response != ACE.Server.Network.Enum.CharacterGenerationVerificationResponse.Ok)
                throw new CharacterNotCreatedException(response.Response);

            return session.Characters.Find(x => x.Id == response.Guid.Full);
        };
    }
}
