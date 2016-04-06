using Symbioz.DofusProtocol.Messages;
using Symbioz.Enums;
using Symbioz.PathProvider;
using Symbioz.World.Models.Fights.Fighters;
using Symbioz.World.PathProvider;
using Symbioz.World.Records.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symbioz.Providers.SpellEffectsProvider.Effects
{
    class MovementEffects
    {
        [EffectHandler(EffectsEnum.Eff_RepelsTo)] // Peur du sram
        public static void RepelsTo(Fighter fighter, SpellLevelRecord level, ExtendedSpellEffect effect, List<Fighter> affecteds, short castcellid)
        {
            var dir = ShapesProvider.GetDirectionFromTwoCells(fighter.CellId, castcellid);
            var movedCellAmount = (short)(PathHelper.GetDistanceBetween(fighter.CellId, castcellid));
            fighter.Fight.Reply(movedCellAmount.ToString());
            var line = ShapesProvider.GetLineFromDirection(fighter.CellId,movedCellAmount, dir);
            if (line.Count > 0)
            {
                var target = fighter.Fight.GetFighter(line.First());
                if (target != null)
                {
                    line.Remove(target.CellId);
                    List<short> cells = fighter.Fight.BreakAtFirstObstacles(line);
                    if (cells.Count > 0)
                        target.Slide(fighter.ContextualId, cells);
                }
            }
        }
        [EffectHandler(EffectsEnum.Eff_1106)] // symétrie esprit félin frappe xélor etc
        public static void SymetryMove(Fighter fighter, SpellLevelRecord level, ExtendedSpellEffect effect, List<Fighter> affecteds, short castcellid)
        {
            var direction = ShapesProvider.GetDirectionFromTwoCells(fighter.CellId,castcellid);
            var line = ShapesProvider.GetLineFromDirection(fighter.CellId, 2, direction);
            short destinationCell = line.Last();
            if (fighter.Fight.IsObstacle(destinationCell))
            {
                var target = fighter.Fight.GetFighter(destinationCell);
                if (target != null)
                {
                    SwitchPosition(fighter, null, null, new List<Fighter>() { target }, destinationCell);
                }
            }
            else
            {
                fighter.Teleport(destinationCell);
            }
        }
        [EffectHandler(EffectsEnum.Eff_SwitchPosition)]
        public static void SwitchPosition(Fighter fighter, SpellLevelRecord level, ExtendedSpellEffect effect, List<Fighter> affecteds, short castcellid)
        {
            var target = affecteds.Find(x => x.CellId == castcellid);
            if (target != null)
            {
                target.Teleport(fighter.CellId);
                fighter.Teleport(castcellid);
            }
        }
        [EffectHandler(EffectsEnum.Eff_Teleport)]  //Bond de Iop
        public static void Teleport(Fighter fighter, SpellLevelRecord level, ExtendedSpellEffect effect, List<Fighter> affecteds, short castcellid)
        {
            if (fighter.Fight.IsObstacle(castcellid))
                return;
            fighter.Fight.Send(new SequenceStartMessage(5, fighter.ContextualId));
            fighter.Fight.Send(new GameActionFightTeleportOnSameMapMessage(4, fighter.ContextualId, fighter.ContextualId, castcellid));
            fighter.Fight.Send(new SequenceEndMessage(2, fighter.ContextualId, 5));
            fighter.ApplyFighterEvent(FighterEventType.ON_TELEPORTED, castcellid);
            fighter.CellId = castcellid;
        }
        [EffectHandler(EffectsEnum.Eff_1041)]  //Appui du zobal 
        public static void PushCaster(Fighter fighter, SpellLevelRecord level, ExtendedSpellEffect effect, List<Fighter> affecteds, short castcellid)
        {
            foreach (var target in affecteds)
            {
                List<short> line = ShapesProvider.GetLineFromOposedDirection(target.CellId, (byte)(effect.BaseEffect.DiceNum + 1), ShapesProvider.GetDirectionFromTwoCells(fighter.CellId, target.CellId));

                List<short> cells = new List<short>();
                foreach (var cell in line)
                {
                    if (!fighter.Fight.IsObstacle(cell) || cell == fighter.CellId)
                        cells.Add(cell);
                    else
                        break;
                }
                if (cells.Count > 0)
                {
                    fighter.Slide(fighter.ContextualId, cells);
                }
            }

        }
        [EffectHandler(EffectsEnum.Eff_BePulled)]
        public static void BePulled(Fighter fighter, SpellLevelRecord level, ExtendedSpellEffect effect, List<Fighter> affecteds, short castcellid)
        {
            foreach (var target in affecteds)
            {
                var direction = ShapesProvider.GetDirectionFromTwoCells(target.CellId, fighter.CellId);
                List<short> line = ShapesProvider.GetLineFromDirection(target.CellId, (byte)effect.BaseEffect.DiceNum, direction);
                List<short> cells = fighter.Fight.BreakAtFirstObstacles(line);
                cells.Reverse();

                if (cells.Count > 0)
                {
                    fighter.Slide(fighter.ContextualId, cells);
                }
            }
        }
        [EffectHandler(EffectsEnum.Eff_PushBack_1103)]
        public static void PushBack1103(Fighter fighter, SpellLevelRecord level, ExtendedSpellEffect effect, List<Fighter> affecteds, short castcellid)
        {
            affecteds.Remove(fighter);
            PushBack(fighter, level, effect, affecteds, castcellid);
        }
        [EffectHandler(EffectsEnum.Eff_PullForward)]
        public static void PullForward(Fighter fighter, SpellLevelRecord level, ExtendedSpellEffect effect, List<Fighter> affecteds, short castcellid)
        {
            foreach (var target in affecteds)
            {
                DirectionsEnum direction;
                if (target.CellId != castcellid)
                {
                    direction = ShapesProvider.GetDirectionFromTwoCells(target.CellId, castcellid);
                }
                else
                {
                   direction = ShapesProvider.GetDirectionFromTwoCells(target.CellId, fighter.CellId);
                }
                List<short> line = ShapesProvider.GetLineFromDirection(target.CellId, effect.BaseEffect.DiceNum, direction);
                List<short> cells = fighter.Fight.BreakAtFirstObstacles(line);
                if (cells.Count > 0)
                {
                    target.Slide(fighter.ContextualId, cells);
                }
            }
        }
        [EffectHandler(EffectsEnum.Eff_PushBack)]
        public static void PushBack(Fighter fighter, SpellLevelRecord level, ExtendedSpellEffect effect, List<Fighter> affecteds, short castcellid)
        {
            foreach (var target in affecteds)
            {
                DirectionsEnum direction = 0;

                if (castcellid == target.CellId)
                {
                    direction = ShapesProvider.GetDirectionFromTwoCells(fighter.CellId, target.CellId);
                }
                else
                {
                    direction = ShapesProvider.GetDirectionFromTwoCells(castcellid, target.CellId);
                }
                List<short> line = ShapesProvider.GetLineFromDirection(target.CellId, (byte)effect.BaseEffect.DiceNum, direction);
                List<short> cells = fighter.Fight.BreakAtFirstObstacles(line);
                if (cells.Count > 0)
                {
                    target.Slide(fighter.ContextualId, cells);
                }
            }

        }
    }
}
