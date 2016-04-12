using Symbioz.DofusProtocol.Messages;
using Symbioz.Network.Clients;
using Symbioz.Network.Messages;
using Symbioz.Enums;
using Symbioz.World.Records.Guilds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symbioz.World.Handlers
{
    public class GuildsHandler
    {
        [MessageHandler]
        public static void HandleGuildCreationRequest(GuildCreationValidMessage message, WorldClient client)
        {
            if (client.Character.Record.GuildId != 0)
            {
                client.Send(new GuildCreationResultMessage((sbyte)GuildCreationResultEnum.GUILD_CREATE_ERROR_ALREADY_IN_GUILD));
                return;
            }
            if (GuildRecord.CanCreateGuild(message.guildName))
            {
                GuildRecord newGuild = new GuildRecord(GuildRecord.PopNextId(), message.guildName, message.guildEmblem);
                newGuild.AddElement(); // met l'element en cache pour la sauvegarde en bdd (l'ajoute a la liste dans GuildRecord)
                client.Character.Record.GuildId = newGuild.Id;
                client.Send(new GuildCreationResultMessage((sbyte)GuildCreationResultEnum.GUILD_CREATE_OK));
            }
            else
            {
                client.Send(new GuildCreationResultMessage((sbyte)GuildCreationResultEnum.GUILD_CREATE_ERROR_NAME_ALREADY_EXISTS));
            }
        }
        /*[MessageHandler]
        public static void HandleGuildGetInformations(Guild)*/
    }
}