using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace Expeditions.UI
{

    /// <summary>
    /// Visual only ItemSlot as a UIElement, for displaying items
    /// </summary>
    public class UIItemSlots : UIElement
    {
        /// <summary>Scale of item slot</summary>
        public const int itemSlotSize = (int)(50 * 0.6f + 4); //scale being used

        private int _context = 7;

        public Item[] Items;
        public int SlotHeight
        {
            get
            {
                if (Items == null || Items.Length <= 0) return 0;
                return itemSlotSize * (int)(1 + (Items.Length * itemSlotSize) / Width.Pixels);
            }
        }

        /// <summary> Create a new ItemSlot </summary>
        /// <param name="context">Set the context for how the itemslot is drawn, see Terraria.UI/ItemSlot.cs </param>
        public UIItemSlots(int context = 7)
        {
            _context = context;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (Items == null) return;
            CalculatedStyle dimensions = base.GetDimensions();
            Rectangle bounds = dimensions.ToRectangle();

            if (Items.Length > 0)
            {
                int noOfLines = (Items.Length * itemSlotSize) / bounds.Width;
                //Main.NewText(deliverables.Length + " / " + (bounds.Width / itemSlotSize));
                //Main.NewText(deliverables.Length + " items cover " + noOfLines + "lines");
                int i = 0;
                noOfLines++;
                for (int y = 0; y < noOfLines; y++)
                {
                    for (int x = 0; x < (bounds.Width / itemSlotSize); x++)
                    {
                        ExpeditionUI.DrawItemSlot(spriteBatch, Items[i],
                            bounds.Left + x * itemSlotSize, bounds.Y + y * itemSlotSize, _context);
                        i++;
                        if (i >= Items.Length) break;
                    }
                    if (i >= Items.Length) break;
                }
            }
        }
    }
}
