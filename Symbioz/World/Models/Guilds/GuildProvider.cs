using Symbioz.DofusProtocol.Messages;
using Symbioz.Helper;
using Symbioz.World.Records.Guilds;
using System;
using System.Collections.Generic;
using System.Linq;
using Symbioz.Enums;
using System.Text;
using System.Threading.Tasks;
using Symbioz.Network.Servers;
using Symbioz.DofusProtocol.Types;
using Symbioz.Network.Clients;

namespace Symbioz.World.Models.Guilds
{
    public class GuildProvider : Singleton<GuildProvider>
    {
        public static GuildRightsBitEnum DEFAULT_JOIN_RANK = GuildRightsBitEnum.GUILD_RIGHT_NONE;
        /// <summary>
        /// Créer le GuildRecord et le CharacterGuildRecord, et les met en cache pour les sauvegarder en db.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public GuildRecord CreateGuild(Character owner, GuildCreationValidMessage message)
        {
            GuildRecord guild = new GuildRecord(GuildRecord.PopNextId(), message.guildName, message.guildEmblem.symbolShape,
                   message.guildEmblem.symbolColor, message.guildEmblem.backgroundShape, message.guildEmblem.backgroundColor, 1, 0);
            guild.AddElement();
            JoinGuild(guild, owner, GuildRightsBitEnum.GUILD_RIGHT_BOSS, (ushort)GuildRightsBitEnum.GUILD_RIGHT_BOSS);
            return guild;
        }
        public void JoinGuild(GuildRecord guild, Character character, GuildRightsBitEnum rights, ushort rank)
        {
            CharacterGuildRecord characterGuild = new CharacterGuildRecord(character.Id, guild.Id, rank, 0, 0, (uint)rights);
            characterGuild.AddElement();
            character.HumanOptions.Add(new HumanOptionGuild(guild.GetGuildInformations()));
            character.Client.Send(new GuildJoinedMessage(guild.GetGuildInformations(), (uint)rights, true));
            character.RefreshOnMapInstance();

        }
        public void LeaveGuild(Character character)
        {
            CharacterGuildRecord.GetCharacterGuild(character.Id).RemoveElement();
            character.HumanOptions.RemoveAll(x => x is HumanOptionGuild);
            character.Client.Send(new GuildLeftMessage());
            character.RefreshOnMapInstance();
        }
        public bool HasGuild(int characterId)
        {
            return CharacterGuildRecord.HasGuild(characterId);
        }
        public static WorldClient[] GetMembers(int guildId)
        {
            List<WorldClient> clients = new List<WorldClient>();
            foreach (var member in CharacterGuildRecord.GetMembers(guildId))
            {
                clients.Add(WorldServer.Instance.GetOnlineClient((int)member.id));
            }
            clients.RemoveAll(x => x == null);
            return clients.ToArray();
        }
        public int ConnectedMembersCount(int guildId)
        {
            int count = 0;
            foreach (var member in CharacterGuildRecord.GetMembers(guildId))
            {
                if (WorldServer.Instance.IsConnected((int)member.id))
                    count++;
            }
            return count;
        }
    }
}
