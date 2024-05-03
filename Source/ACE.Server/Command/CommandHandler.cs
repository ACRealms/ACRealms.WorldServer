using ACE.Server.Network;

namespace ACE.Server.Command
{
    public delegate void CommandHandler(ISession session, params string[] parameters);
}
