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
        public CharacterNotCreatedException(ACE.Server.Network.Enum.CharacterGenerationVerificationResponse response)
            : base($"The character was not created successfully: {response}")
        {
        }
    }

    internal record CharacterFactory : Factory<Character, CharacterFactory>
    {
        public AccountFactory Account { get; init; } = new AccountFactory();
        Func<CharacterFactory, CharacterCreateInfo> CharacterCreateInfo { get; init; } = (fac) => PlayerFactoryEx.CreateCharacterCreateInfo(fac.CharacterName, 40, 70, 100, 100, 10, 10, true);
        public string CharacterName { get; init; } = $"Generated {CurrentIndex}";

        protected override Func<Character> Builder() => () =>
        {
            var account = Account.Create();
            var sessionFactory = new FakeSessionFactory() { Account = AccountFactory.Identity(account) };
            var session = sessionFactory.Create();
            var characterCreateInfo = CharacterCreateInfo(this);
            CharacterHandler.CharacterCreateEx(characterCreateInfo, session);
            var response = session.WaitForMessage<GameMessageCharacterCreateResponse>();
            if (response.Response != ACE.Server.Network.Enum.CharacterGenerationVerificationResponse.Ok)
                throw new CharacterNotCreatedException(response.Response);

            return session.Characters.Find(x => x.Id == response.Guid.Full);
        };
    }
}
