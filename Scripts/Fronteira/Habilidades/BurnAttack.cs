using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Fronteira.Talentos;

namespace Server.Items
{

    public class BurnAttack
    {
        private static readonly Dictionary<Mobile, BurnTimer> m_BleedTable = new Dictionary<Mobile, BurnTimer>();

        public BurnAttack()
        {
        }


        public static bool IsBurning(Mobile m)
        {
            return m_BleedTable.ContainsKey(m);
        }

        public static void BeginBurn(Mobile m, Mobile from, int damage, bool splintering = false)
        {
            BurnTimer timer = null;

            if (m_BleedTable.ContainsKey(m))
            {
                if (splintering)
                {
                    timer = m_BleedTable[m];
                    timer.Stop();
                }
                else
                {
                    return;
                }
            }

            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.FanDancerFanFire, 1075829, 1075830, TimeSpan.FromSeconds(10), m, String.Format("{0}\t{1}\t{2}", "1", "10", "2")));

            timer = new BurnTimer(from, m, damage, CheckBloodDrink(from));
            m_BleedTable[m] = timer;
            timer.Start();

            from.SendLocalizedMessage("Seu alvo esta queimando"); // Your target is bleeding!
            m.SendLocalizedMessage("Voce esta queimando"); // You are bleeding!


            m.NonlocalOverheadMessage(MessageType.Regular, 0x21, false, "* queimando *"); // You are bleeding profusely
 
            m.PlaySound(0x133);
            m.FixedParticles(0x377A, 244, 25, 9950, 31, 0, EffectLayer.Waist);
        }

        public static void DoBurn(Mobile m, Mobile from, int damage, bool blooddrinker)
        {
            if (m.Alive && !m.IsDeadBondedPet)
            {
                if (!m.Player)
                    damage *= 2;

                m.PlaySound(0x133);
                AOS.Damage(m, from, damage, false, 0, 0, 0, 0, 0, 0, 100, false, false, false);

                if (blooddrinker && from.Hits < from.HitsMax)
                {
                    from.SendLocalizedMessage("A sanguesuga te cura"); //The blood drinker effect heals you.
                    from.Heal(damage);
                }

                Blood blood = new Blood();
                blood.ItemID = Utility.Random(0x122A, 5);
                blood.MoveToWorld(m.Location, m.Map);
            }
            else
            {
                EndBurn(m, false);
            }
        }

        public static void EndBurn(Mobile m, bool message)
        {
            Timer t = null;

            if (m_BleedTable.ContainsKey(m))
            {
                t = m_BleedTable[m];
                m_BleedTable.Remove(m);
            }

            if (t == null)
                return;

            t.Stop();
            BuffInfo.RemoveBuff(m, BuffIcon.Bleed);

            if (message)
                m.SendLocalizedMessage("Voce nao esta mais queimando"); // The bleeding wounds have healed, you are no longer bleeding!
        }

        public static bool CheckBloodDrink(Mobile attacker)
        {
            return attacker.Weapon is BaseWeapon && ((BaseWeapon)attacker.Weapon).WeaponAttributes.BloodDrinker > 0;
        }

        private class BurnTimer : Timer
        {
            private readonly Mobile m_From;
            private readonly Mobile m_Mobile;
            private int m_Count;
            private int m_MaxCount;
            private readonly bool m_BloodDrinker;
            private int dmg;

            public BurnTimer(Mobile from, Mobile m, int dmg, bool blooddrinker)
                : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                this.dmg = dmg;
                m_From = from;
                m_Mobile = m;
                Priority = TimerPriority.TwoFiftyMS;
                m_BloodDrinker = blooddrinker;

                m_MaxCount = Spells.SkillMasteries.ResilienceSpell.UnderEffects(m) ? 3 : 5;
                this.dmg = this.dmg / m_MaxCount;
            }

            protected override void OnTick()
            {
                if (!m_Mobile.Alive || m_Mobile.Deleted)
                {
                    EndBurn(m_Mobile, true);
                }
                else
                {

                    DoBurn(m_Mobile, m_From, dmg, m_BloodDrinker);

                    if (++m_Count == m_MaxCount)
                        EndBurn(m_Mobile, true);
                }
            }
        }
    }
}
