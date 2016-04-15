using Symbioz.DofusProtocol.Types;
using Symbioz.Network.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symbioz.World.Models
{
    public class DungeonPartyCharacter
    {
        public static List<DungeonPartyCharacter> LoggedDungeonParyCharacters = new List<DungeonPartyCharacter>();

        public Character Character;
        public WorldClient Client;
        public List<ushort> DungeonsId;
        public DungeonPartyCharacter(Character character, WorldClient client, List<ushort> dungeonId)
        {
            this.Character = character;
            this.Client = client;
            this.DungeonsId = dungeonId;
        }

        public static void AddCharacter(Character character, List<ushort> dungeonsId)
        {
            DungeonPartyCharacter.LoggedDungeonParyCharacters.Add(new DungeonPartyCharacter(character, character.Client, dungeonsId));
        }

        public static void UpdateCharacter(Character character, List<ushort> dungeonsId)
        {
            DungeonPartyCharacter dpc = DungeonPartyCharacter.LoggedDungeonParyCharacters.Find(x => x.Character == character);
            dpc.DungeonsId = dungeonsId;
        }

        public static void RemoveCharacter(Character character)
        {
            DungeonPartyCharacter dpc = DungeonPartyCharacter.GetDPCByCharacterId(character.Id);
            DungeonPartyCharacter.LoggedDungeonParyCharacters.Remove(dpc);
        }

        public static DungeonPartyCharacter GetDPCByCharacterId(int characterId)
        {
            return DungeonPartyCharacter.LoggedDungeonParyCharacters.Find(x => x.Character.Id == characterId);
        }
        public static List<DungeonPartyFinderPlayer> GetCharactersForDungeon(ushort dungeonId)
        {
            List<DungeonPartyFinderPlayer> interested = new List<DungeonPartyFinderPlayer>();
            foreach(DungeonPartyCharacter dpc in DungeonPartyCharacter.LoggedDungeonParyCharacters)
            {
                if (dpc.DungeonsId.Contains(dungeonId))
                    interested.Add(dpc.GetDungeonPartyFinderPlayer());
            }
            return interested;
        }
        public DungeonPartyFinderPlayer GetDungeonPartyFinderPlayer()
        {
            Character c = this.Character;
            return new DungeonPartyFinderPlayer((uint)c.Id, c.Record.Name, c.Record.Breed, c.Record.Sex, c.Record.Level);
        }
    }
}
