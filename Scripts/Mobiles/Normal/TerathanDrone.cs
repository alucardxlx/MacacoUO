using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a terathan drone corpse")]
    public class TerathanDrone : BaseCreature
    {
        [Constructable]
        public TerathanDrone()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "terathan trabalhador";
            this.Body = 71;
            this.BaseSoundID = 594;

            this.SetStr(36, 65);
            this.SetDex(96, 145);
            this.SetInt(21, 45);

            this.SetHits(1222, 1239);
            this.SetMana(0);

            this.SetDamage(16, 42);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 25);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 15, 25);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 15, 25);

            this.SetSkill(SkillName.Poisoning, 40.1, 60.0);
            this.SetSkill(SkillName.MagicResist, 30.1, 45.0);
            this.SetSkill(SkillName.Tactics, 30.1, 50.0);
            this.SetSkill(SkillName.Wrestling, 150.1, 150.1);

            this.Fame = 2200;
            this.Karma = -2200;

            this.VirtualArmor = 44;
			
            this.PackItem(new SpidersSilk(2));

            PackItem(new CristalTherathan());
        }

        public TerathanDrone(Serial serial)
            : base(serial)
        {
        }

        public override void AlterMeleeDamageTo(Mobile to, ref int damage)
        {
            base.AlterMeleeDamageTo(to, ref damage);
            if (to is BaseCreature)
                damage *= 4;
        }

        public override int Meat
        {
            get
            {
                return 4;
            }
        }

        public override TribeType Tribe { get { return TribeType.Terathan; } }

        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.TerathansAndOphidians;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.LV4);
            // TODO: weapon?
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

            if (this.BaseSoundID == 589)
                this.BaseSoundID = 594;
        }
    }
}
