using System;
using Server.Multis;
using Server.Network;

namespace Server.SkillHandlers
{
    public class Hiding
    {
        private static bool m_CombatOverride;
        public static bool CombatOverride
        {
            get
            {
                return m_CombatOverride;
            }
            set
            {
                m_CombatOverride = value;
            }
        }
        public static void Initialize()
        {
            SkillInfo.Table[21].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            if (m.Spell != null)
            {
                m.SendMessage("Voce esta ocupado fazendo outra coisa"); // You are busy doing something else and cannot hide.
                return TimeSpan.FromSeconds(1.0);
            }

            if (Server.Engines.VvV.ManaSpike.UnderEffects(m))
            {
                return TimeSpan.FromSeconds(1.0);
            }

            if (m.Target != null)
            {
                Targeting.Target.Cancel(m);
            }

            double bonus = 0.0;

            BaseHouse house = BaseHouse.FindHouseAt(m);

            if (house != null && house.IsFriend(m))
            {
                bonus = 100.0;
            }
            /*
            else
            {
                if (house == null)
                    house = BaseHouse.FindHouseAt(new Point3D(m.X - 1, m.Y, 127), m.Map, 16);

                if (house == null)
                    house = BaseHouse.FindHouseAt(new Point3D(m.X + 1, m.Y, 127), m.Map, 16);

                if (house == null)
                    house = BaseHouse.FindHouseAt(new Point3D(m.X, m.Y - 1, 127), m.Map, 16);

                if (house == null)
                    house = BaseHouse.FindHouseAt(new Point3D(m.X, m.Y + 1, 127), m.Map, 16);

                if (house != null)
                    bonus = 50.0;
            }

            */

            //int range = 18 - (int)(m.Skills[SkillName.Hiding].Value / 10);
            int skill = Math.Min(100, (int)m.Skills[SkillName.Hiding].Value);
            int range = Math.Min((int)((100 - skill) / 2) + 12, 18);	//Cap of 18 not OSI-exact, intentional difference

            bool badCombat = (!m_CombatOverride && m.Combatant is Mobile && m.InRange(m.Combatant.Location, range) && ((Mobile)m.Combatant).InLOS(m.Combatant));
            bool ok = (!badCombat /*&& m.CheckSkill( SkillName.Hiding, 0.0 - bonus, 100.0 - bonus )*/);

            if (ok)
            {
                if (!m_CombatOverride)
                {
                    IPooledEnumerable eable = m.GetMobilesInRange(range);

                    foreach (Mobile check in eable)
                    {
                        if (check.Combatant == m)
                        {
                            if(check.GetDistanceToSqrt(m) <= 4 || check.InLOS(m))
                            {
                                if (Shard.DebugEnabled)
                                    Shard.Debug("Hiding: Distancia alvo: " + check.GetDistanceToSqrt(m) + " Vendo alvo ? " + check.InLOS(m), check);

                                check.SendMessage("Voce nota " + m.Name + " tentando se esconder");
                                badCombat = true;
                                ok = false;
                                break;
                            }
                           
                        }
                    }

                    eable.Free();
                }

                ok = (!badCombat && m.CheckSkillMult(SkillName.Hiding, 0.0 - bonus, 100.0 - bonus));
            }

            if (badCombat)
            {
                m.RevealingAction();

                m.LocalOverheadMessage(MessageType.Regular, 0x22, 501237); // You can't seem to hide right now.

                //return TimeSpan.Zero;
                return TimeSpan.FromSeconds(10.0);
            }
            else 
            {
                if (ok)
                {
                    m.Hidden = true;
                    m.Warmode = false;
					Server.Spells.Sixth.InvisibilitySpell.RemoveTimer(m);
                    Server.Items.InvisibilityPotion.RemoveTimer(m);
                    m.LocalOverheadMessage(MessageType.Regular, 0, true, "* Voce se escondeu *"); // You have hidden yourself well.
                }
                else
                {
                    m.RevealingAction();

                    m.LocalOverheadMessage(MessageType.Regular, 0x22, false, "Voce nao consegue se esconder aqui"); // You can't seem to hide here.
                }

                return TimeSpan.FromSeconds(10.0);
            }
        }
    }
}
