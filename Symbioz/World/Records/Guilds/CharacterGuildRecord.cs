using Symbioz.DofusProtocol.Types;
using Symbioz.Network.Servers;
using Symbioz.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symbioz.World.Records.Guilds
{
    [Table("CharactersGuilds")]
    public class CharacterGuildRecord : ITable
    {
        public static List<CharacterGuildRecord> CharactersGuilds = new List<CharacterGuildRecord>();

        [Primary]
        public int CharacterId;

        public int GuildId;
        [Update]
        public ushort Rank;
        [Update]
        public ulong GivenExperience;
        [Update]
        public sbyte ExperienceGivenPercent;
        [Update]
        public uint Rights;

        public CharacterGuildRecord(int characterId, int guildId, ushort rank, ulong givenExperience, sbyte experienceGivenPercent, uint rights)
        {
            this.CharacterId = characterId;
            this.GuildId = guildId;
            this.Rank = rank;
            this.GivenExperience = givenExperience;
            this.ExperienceGivenPercent = experienceGivenPercent;
            this.Rights = rights;
        }

        public static GuildMember[] GetMembers(int guildId)
        {
            return CharactersGuilds.FindAll(x => x.GuildId == guildId).ConvertAll<GuildMember>(x => x.GetGuildMember()).ToArray();
        }
        public GuildMember GetGuildMember()
        {
            sbyte connected = (sbyte)(WorldServer.Instance.IsConnected(CharacterId) ? 1 : 0);
            CharacterRecord cRecord = CharacterRecord.GetCharacterRecordById(CharacterId);
            return new GuildMember((uint)CharacterId, cRecord.Level, cRecord.Name, cRecord.Breed, cRecord.Sex, Rank, GivenExperience, ExperienceGivenPercent,
                Rights, connected, cRecord.AlignmentSide, 0, 0, cRecord.AccountId, 0, WorldServer.Instance.GetOnlineClient(CharacterId).Character.PlayerStatus);
        }
        public static bool HasGuild(int characterId)
        {
            return CharactersGuilds.Find(x => x.CharacterId == characterId) != null;
        }
        public static CharacterGuildRecord GetCharacterGuild(int characterId)
        {
            return CharactersGuilds.Find(x => x.CharacterId == characterId);
        }
        public static int MembersCount(int guildId)
        {
            return GetMembers(guildId).Length;
        }
        public static void RemoveAll(int characterId)
        {
            CharactersGuilds.FindAll(x => x.CharacterId == characterId).ForEach(x => x.RemoveElement());
        }
    }
}
