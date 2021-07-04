using System;
using Server.Fronteira.Talentos;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells.Third;

namespace Server.Spells
{
    public abstract class MagerySpell : Spell
    {
        public override void SayMantra()
        {
            if (Info.Mantra != null && Info.Mantra.Length > 0 && (m_Caster.Player || (m_Caster is BaseCreature && ((BaseCreature)m_Caster).ShowSpellMantra)))
            {
                var eable = m_Caster.Map.GetClientsInRange(m_Caster.Location);
                foreach (NetState state in eable)
                {
                    if (state.Mobile.CanSee(m_Caster))
                    {
                        if(state.Mobile == m_Caster || state.Mobile.Skills.Magery.Value > 50)
                        {
                            m_Caster.PrivateOverheadMessage(Info.Mantra, state.Mobile);
                        } else
                        {
                            m_Caster.PrivateOverheadMessage("* conjurando *", state.Mobile);
                        }
                    }
                }
            }
        }

        // Magias q vao ficar mais dificeis castar andando
        public static readonly Type[] MovementNerfWhenRepeated =
        {
            typeof(TeleportSpell)
        };

        public virtual int GetMinSkill { get { return 0; } }
        private static readonly int[] m_ManaTable = new int[] { 4, 6, 9, 11, 14, 20, 40, 50 };
        private const double ChanceOffset = 20.0, ChanceLength = 100.0 / 7.0;
        public MagerySpell(Mobile caster, Item scroll, SpellInfo info)
            : base(caster, scroll, info)
        {
        }

        public abstract SpellCircle Circle { get; }
        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds((3 + (int)Circle) * CastDelaySecondsPerTick);
            }
        }
        public override bool ConsumeReagents()
        {
            if (base.ConsumeReagents())
                return true;

            if (ArcaneGem.ConsumeCharges(Caster, (Core.SE ? 1 : 1 + (int)Circle)))
                return true;

            return false;
        }

        public override bool ValidateCast(Mobile from)
        {
            /*
            if (from.RP && from.Player)
            {
                if(((PlayerMobile)from).Talentos.Tem(Talento.ArmaduraMagica);
                if (talento >= 1)
                    return true;
            }
            */

            var circleMax = CicloArmadura(from);
            if (circleMax < (int)this.Circle + 1)
            {
                from.SendMessage("Esta armadura e muito pesada para esta magia");
                return false;
            }
            return true;
        }

        public override int SkillNeeded
        {
            get
            {
                if (Circle == SpellCircle.Eighth)
                    return 100;
                if (Circle == SpellCircle.Seventh)
                    return 80;
                if (Circle == SpellCircle.Sixth)
                    return 65;

                return (int)((double)(Circle + 1) * 11);
            }
        }

        public override void GetCastSkills(out double min, out double max)
        {
            int circle = (int)Circle;

            if (Scroll != null)
                circle -= 2;

            double avg = ChanceLength * circle;

            min = avg - ChanceOffset;
            max = avg + ChanceOffset;

            var m = GetMinSkill;
            if (m != 0)
            {
                min = m;
            }
        }

        public override int GetMana()
        {
            if (Scroll is BaseWand)
                return 0;

            return m_ManaTable[(int)Circle];
        }

        public virtual bool CheckResisted(Mobile target)
        {
            if (target == Caster)
                return false;

            double n = GetResistPercent(target);

            Shard.Debug("Resist % " + n, target);

            n /= 100.0;

            if (n <= 0.0)
                return false;

            if (n >= 1.0)
                return true;

            int maxSkill = (1 + (int)Circle) * 10;
            maxSkill += (1 + ((int)Circle / 6)) * 25;

            if (target.Skills[SkillName.MagicResist].Value < maxSkill && target != Caster)
                target.CheckSkillMult(SkillName.MagicResist, 0.0, target.Skills[SkillName.MagicResist].Cap);

            var resisted = (n >= Utility.RandomDouble());

            if (resisted)
            {
                Caster.PlaySound(0x1E6);
                target.FixedEffect(0x42CF, 10, 5);
            }
            return resisted;
        }

        public virtual double GetResistPercentForCircle(Mobile target, SpellCircle circle)
        {
            if (!Shard.POL_STYLE)
            {
                double value = GetResistSkill(target);
                if(target.Player && !Caster.Player)
                {
                    value += (int)(value * (target.GetBonusElemento(ElementoPvM.Agua) + target.GetBonusElemento(ElementoPvM.Luz) + target.GetBonusElemento(ElementoPvM.Escuridao) + target.GetBonusElemento(ElementoPvM.Gelo)));
                }
                double firstPercent = value / 5.0;
                double secondPercent = value - (((Caster.Skills[CastSkill].Value - 20.0) / 5.0) + (1 + (int)circle) * 5.0);
                return (firstPercent > secondPercent ? firstPercent : secondPercent) / 2.0; // Seems should be about half of what stratics says.
            }
            else
            {
                var resist = target.Skills[SkillName.MagicResist].Value;
                if (target.Player && !Caster.Player)
                {
                    resist += (int)(resist * Caster.GetBonusElemento(ElementoPvM.Agua));
                }
                if (target.Player)
                {
                    if(((PlayerMobile)target).Talentos.Tem(Talento.PeleArcana))
                        resist += 10;
                }

                if (Caster.Player && Caster.RP)
                {
                    if (((PlayerMobile)Caster).Talentos.Tem(Talento.MentePerfurante))
                        resist -= 10;
                }

                var cap = resist / 5;

                var magery = Caster.Skills[CastSkill].Value;
                var circ = 1 + (double)circle;

                var chance = ((magery * 2) / 10 + circ * circ);

                if (Shard.DebugEnabled)
                    Shard.Debug("Chance Base: " + chance + " circulo " + circ);

                chance = resist - chance;
                if (chance < cap)
                    chance = cap;

                if (Shard.SPHERE_STYLE)
                    chance *= 0.35; // sem pre cast mais dificil de resistir
                else
                    chance *= 0.80;

                if (Caster is BaseCreature && target is PlayerMobile)
                    chance /= 1.5;

                Shard.Debug("Chance RS: " + chance, target);

                return chance;
            }
        }

        public virtual double GetResistPercent(Mobile target)
        {

            return GetResistPercentForCircle(target, Circle);
        }

        public override TimeSpan GetCastDelay()
        {
            if (!Core.ML && Scroll is BaseWand)
                return TimeSpan.Zero;

            if (!Core.AOS)
            {
                if (Caster is BaseCreature)
                    return TimeSpan.FromSeconds(0.7 + 0.40 * (int)Circle);

                var pl = Caster as PlayerMobile;
                if (!Shard.POL_STYLE || pl == null || (pl.Talentos.Tem(Talento.Cajados) && Caster.Weapon is BaseStaff))
                    // Delay T2A
                    return TimeSpan.FromSeconds(0.5 + (0.25 * (int)(1+Circle)));
                else
                {
                    // delay POL/Mystic
                    return TimeSpan.FromSeconds(0.5 + (0.5 * (int)Circle));
                }
            }
            return base.GetCastDelay();
        }
    }
}
