using System;

namespace Server.Items
{
    public class GreaterCurePotion : BaseCurePotion
    {
		public override string DefaultName
        {
            get { return "Poção de Cura Maior"; }            
        }
		
        private static readonly CureLevelInfo[] m_OldLevelInfo = new CureLevelInfo[]
        {
            new CureLevelInfo(Poison.Lesser, 1.00), // 100% chance to cure lesser poison
            new CureLevelInfo(Poison.Regular, 1.00), // 100% chance to cure regular poison
            new CureLevelInfo(Poison.Greater, 1.00), // 100% chance to cure greater poison
            new CureLevelInfo(Poison.Deadly, 0.9), //  75% chance to cure deadly poison
            new CureLevelInfo(Poison.Lethal, 0.85)//  25% chance to cure lethal poison
        };
        private static readonly CureLevelInfo[] m_AosLevelInfo = new CureLevelInfo[]
        {
            new CureLevelInfo(Poison.Lesser, 1.00),
            new CureLevelInfo(Poison.Regular, 1.00),
            new CureLevelInfo(Poison.Greater, 0.75),
            new CureLevelInfo(Poison.Deadly, 0.45),
            new CureLevelInfo(Poison.Lethal, 0.25)
        };
        [Constructable]
        public GreaterCurePotion()
            : base(PotionEffect.CuraMaior)
        {
        }

        public GreaterCurePotion(Serial serial)
            : base(serial)
        {
        }

        public override CureLevelInfo[] LevelInfo
        {
            get
            {
                return Core.AOS ? m_AosLevelInfo : m_OldLevelInfo;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
