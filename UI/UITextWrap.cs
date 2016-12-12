using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace Expeditions.UI
{
    public class UITextWrap : UIElement
    {
        private string _text = "";
        private int _maxLines = 10;
        private int _maxWidth = 460;
        private int _textHeight = 0;
        private bool _centred = false;
        private Color _colour;
        private Color _borderColour;
        public int TextHeight
        {
            get
            {
                if (_text == "") return 0;
                int noOfLines = 0;
                string[] array = Utils.WordwrapString(_text, Main.fontMouseText, _maxWidth, _maxLines, out noOfLines);
                noOfLines++;
                return noOfLines * 30;
            }
        }
        public UITextWrap(string text, int textMaxWidth, Color color, Color borderColour, bool centre)
        {
            this.SetText(text);
            _maxWidth = textMaxWidth;
            _centred = centre;
            _colour = color;
            _borderColour = borderColour;
        }

        public override void Recalculate()
        {
            this.SetText(this._text);
            base.Recalculate();
        }
        public void SetText(string text)
        {
            Vector2 vector = new Vector2(Main.fontMouseText.MeasureString(text).X, 16f);
            this._text = text;
            this.MinWidth.Set(vector.X + this.PaddingLeft + this.PaddingRight, 0f);
            this.MinHeight.Set(vector.Y + this.PaddingTop + this.PaddingBottom, 0f);
        }
        public void SetColour(Color colour)
        {
            this._colour = colour;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = base.GetDimensions();
            Vector2 pos = dimensions.Position();
            float offsetX = 0;
            if (_centred) offsetX = Main.fontMouseText.MeasureString(_text).X * 0.5f;

            //calculate wordwrap of text
            int noOfLines = 0;
            string[] array = Utils.WordwrapString(_text, Main.fontMouseText, _maxWidth, _maxLines, out noOfLines);
            noOfLines++;

            float colMod = (Main.mouseTextColor / 255f);
            Color textColor = new Color(
                (int)(_colour.R * colMod),
                (int)(_colour.G * colMod),
                (int)(_colour.B * colMod),
                (int)(_colour.A));

            //draw each line of text
            for (int i = 0; i < noOfLines; i++)
            {
                Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, array[i], (int)(pos.X - offsetX), (int)(pos.Y + i * 30), textColor, _borderColour, Vector2.Zero, 1f);
            }
        }
    }
}
