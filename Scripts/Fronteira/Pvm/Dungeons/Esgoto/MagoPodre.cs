using System;
using Server.Engines.Craft;
using Server.Items;
using Server.Ziden.Dungeons.Esgoto;

namespace Server.Mobiles
{
    [CorpseName("an evil mage corpse")]
    public class MagoPodre : BaseCreature
    {
        [Constructable]
        public MagoPodre()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Mago Putrido";

            Body = 0x191;

            SetStr(81, 105);
            SetDex(15, 50);
            SetInt(20, 30);

            SetHits(80, 80);

            SetDamage(2, 8);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Fire, 5, 10);
            SetResistance(ResistanceType.Poison, 5, 10);
            SetResistance(ResistanceType.Energy, 5, 10);

            SetSkill(SkillName.EvalInt, 0, 1);
            SetSkill(SkillName.Magery, 50.1, 50);
            SetSkill(SkillName.MagicResist, 75.0, 97.5);
            SetSkill(SkillName.Tactics, 65.0, 87.5);
            SetSkill(SkillName.Macing, 35.2,35.0);
            SetSkill(SkillName.Meditation, 80, 100);
            Fame = 2500;
            Karma = -2500;

            AddItem(new BlackStaff());
            var hue = Utility.RandomNeutralHue();
            VirtualArmor = 0;
            PackReg(3);
            AddItem(new Robe(hue)); // TODO: Proper hue
        }

        public override void GenerateLoot(bool spawning)
        {
            if (!spawning)
            {
                var r = new LanternaMagica();
                r.PartyLoot = false;

                var cont = Backpack;
                if (!cont.TryDropItem(this, r, false))
                {
                    cont.DropItem(r);
                }
                //Backpack.AddItem(r);
                r.PartyLoot = true;
            }
            base.GenerateLoot(spawning);
        }


        public override bool IsSmart { get { return false; } }

        public override int GetDeathSound()
        {
            return 0x423;
        }

        public override int GetHurtSound()
        {
            return 0x436;
        }

        public MagoPodre(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }

        public override int Meat
        {
            get
            {
                return 1;
            }
        }

        public override int TreasureMapLevel
        {
            get
            {
                return 0;
            }
        }


        public override bool OnBeforeDeath()
        {
            var manolos = this.GetLootingRights();
            foreach(var r in manolos)
            {
                if(r.m_HasRight && r.m_Mobile != null && r.m_Mobile is PlayerMobile)
                {
                    var p = (PlayerMobile)r.m_Mobile;
                    if(p.Wisp != null)
                    {
                        p.Wisp.MataMago();
                        if(p.Young)
                        {
                            p.PlaceInBackpack(new ParalyzeScroll());
                        }
                    }
                }
            }
            return base.OnBeforeDeath();
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
