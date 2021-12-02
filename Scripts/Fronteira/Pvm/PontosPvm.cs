using Felladrin.Automations;
using Server.Commands;
using Server.Engines.Points;
using Server.Engines.VvV;
using Server.Fronteira.Elementos;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Regions;
using Server.Spells;
using System;

namespace Server.Ziden.Kills
{

    public class PontoTaming : PointsSystem
    {
        public override TextDefinition Name { get { return "Ponto Taming"; } }
        public override PointsType Loyalty { get { return PointsType.Taming; } }
        public override bool AutoAdd { get { return true; } }
        public override double MaxPoints { get { return double.MaxValue; } }
        public override bool ShowOnLoyaltyGump { get { return false; } }
        public static bool Enabled = true;
    }

    public class Exp : PointsSystem
    {
        public override TextDefinition Name { get { return "Exp"; } }
        public override PointsType Loyalty { get { return PointsType.Exp; } }
        public override bool AutoAdd { get { return true; } }
        public override double MaxPoints { get { return 1000; } }
        public override bool ShowOnLoyaltyGump { get { return true; } }
        public static bool Enabled = true;
    }

    public class PontosPvm : PointsSystem
    {
        public override TextDefinition Name { get { return "PvM"; } }
        public override PointsType Loyalty { get { return PointsType.PvM; } }
        public override bool AutoAdd { get { return true; } }
        public override double MaxPoints { get { return double.MaxValue; } }
        public override bool ShowOnLoyaltyGump { get { return true; } }
        public static bool Enabled = true;

        public static void Initialize()
        {
            EventSink.CreatureDeath += CreatureDeath;

            if (Shard.RP)
                return;

            CommandSystem.Register("pvm", AccessLevel.Player, Cmd);
        }

        [Usage("Kills")]
        private static void Cmd(CommandEventArgs e)
        {
            if (SpellHelper.CheckCombat(e.Mobile))
            {
                e.Mobile.SendMessage("Voce nao pode usar este comando no calor da batalha");
                return;
            }

            if (e.Mobile.RP)
            {
                e.Mobile.SendMessage("Voce nao pode usar este comando em um personagem RP");
                return;
            }

            e.Mobile.SendGump(new PvmRewardsGump(e.Mobile, e.Mobile as PlayerMobile));
        }

        public static void CreatureDeath(CreatureDeathEventArgs e)
        {
            BaseCreature bc = e.Creature as BaseCreature;

            var gold = e.Corpse.TotalGold;
 
            var pontos = bc.PontosPvm;

            if (pontos <= 0)
                return;

            if (bc.IsChampionSpawn && !(bc is BaseChampion))
                pontos = pontos / 4;

            var c = e.Corpse;
            var killer = e.Killer;

            var exp = pontos;
            var dg = false;
            if(!(bc.Region is DungeonRegion))
            {
                dg = true;
            } else
            {
                exp = (int)(exp * 1.5);
            }

            if (exp == 0)
                return;

            if (SkillCheck.BONUS_GERAL != 0)
                exp = (int)(exp * SkillCheck.BONUS_GERAL);

            if (Shard.DebugEnabled)
                Shard.Debug("Rolando XP " + exp);

            if (bc != null && bc.GetLootingRights() != null)
            {
                foreach (var m in bc.GetLootingRights())
                {
                    var pl = m.m_Mobile as PlayerMobile;
                    if (pl != null)
                    {
                        var ptPerto = DivideGold.DivideQuanto(pl);
                        var divisor = ptPerto == null ? 1 : ptPerto.Count;
                        PointsSystem.PontosOuro.AwardPoints(pl, gold / divisor);
                        if (Shard.EXP)
                        {
                            if(!pl.RP)
                            {
                                if(dg && !pl.IsCooldown("msgdg"))
                                {
                                    if(pl.Region is DungeonGuardedRegion)
                                    {
                                        pl.SetCooldown("msgdg", TimeSpan.FromHours(1));
                                        pl.SendMessage(78, "Dungeons com protecao como esta dano menos XP q o normal.");
                                    } else
                                    {
                                        pl.SetCooldown("msgdg", TimeSpan.FromHours(1));
                                        pl.SendMessage(78, "Monstros dentro de dungeons dao mais experiencia do que locais como este.");
                                    }
                                 
                                }

                                if(pl.Elemento != ElementoPvM.None)
                                {
                                    var expElem = pl.Elementos.GetExp(pl.Elemento);
                                    expElem += exp;
                                    var lvl = pl.Elementos.GetNivel(pl.Elemento);
                                    var maxExp = CustosUPElementos.CustoUpExp(lvl);
                                    if (expElem > maxExp)
                                    {
                                        expElem = (int)maxExp;
                                    }
                                    pl.Elementos.SetExp(pl.Elemento, expElem);
                                    var pct = (expElem / maxExp) * 100;
                                    pl.CloseGump(typeof(PctExpGump));
                                    pl.SendGump(new PctExpGump(pl, (int)pct, pl.Elemento.ToString()+" "+expElem+"/"+maxExp));
                                }

                                //pl.SendMessage(78, "Bonus de XP: Semana FULL EXP");
                                c.PrivateOverheadMessage(Network.MessageType.Regular, 66, false, string.Format("+{0} EXP", exp), pl.NetState);
                                PointsSystem.Exp.AwardPoints(pl, exp, false, false);
                                if (!pl.IsCooldown("xpp"))
                                {
                                    pl.SetCooldown("xpp", TimeSpan.FromHours(1));
                                    pl.SendMessage(78, "Digite .xp para usar sua EXP para subir skills");
                                }
                            } else
                            {
                                c.PrivateOverheadMessage(Network.MessageType.Regular, 66, false, string.Format("+{0} EXP", exp), pl.NetState);
                                pl.GanhaExpRP(pontos);
                            }
                        }
                        PointsSystem.PontosPvmEterno.AwardPoints(pl, pontos / 2, false, false);
                        var pts = PointsSystem.PontosPvm.AwardPoints(pl, pontos / 2, false, false);
                        if (pl.RP)
                        {
                            continue;
                        }
                      
                        if (pts > 500 && !pl.IsCooldown("pvmp"))
                        {
                            pl.SetCooldown("pvmp", TimeSpan.FromHours(1));
                            pl.SendMessage(78, "Voce pode trocar seus pontos PvM por recompensas usando o comando .pvm");
                        }

                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {

            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }
}
