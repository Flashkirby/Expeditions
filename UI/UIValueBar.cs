using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace Expeditions.UI
{
    public class UIValueBar : UIElement
    {
        private static Color barColour = new Color(43, 56, 101, 200);

        Texture2D bar = Main.colorBarTexture;
        Texture2D blip = Main.colorBlipTexture;
        Texture2D slider = Main.colorSliderTexture; //80 range with offset 3

        private List<Color> _blipColourList;

        private int _minValue = 0;
        private int _maxValue = 4;
        private int _lastIndex = 0;
        private int _index = 0;
        private int _dragVal = 0;
        private float _widthRange = 160f;
        private bool _dragging = false;
        private int _dailyColour = -1;
        public int Value
        {
            get { return _index; }
            set
            {
                if (value > _maxValue) value = _maxValue;
                if (value < _minValue) value = _minValue;
                _index = value;
                //Main.NewText("Scroll Value: " + _minValue + " < " + value + " > " + _maxValue);
            }
        }
        public int MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }
        public int MaxValue
        {
            get { return _maxValue; }
            set
            {
                if (value < _minValue) value = _minValue;
                _maxValue = value;
            }
        }
        public UIValueBar(int minValue, int maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            _widthRange = 160f;
            Width.Set(178f, 0f);
            Height.Set(25f, 0f);

            _blipColourList = new List<Color>();
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            base.MouseDown(evt);
            _dragging = true;
        }
        public override void MouseUp(UIMouseEvent evt)
        {
            base.MouseUp(evt);
            _dragging = false;
        }

        public void SetBlipColours(Color[] colours, int daily)
        {
            _blipColourList.Clear();
            foreach (Color c in colours)
            {
                _blipColourList.Add(c);
            }
            _dailyColour = daily;
        }
        public void SetBlipColours(Color[] colours)
        {
            SetBlipColours(colours, -1);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = base.GetDimensions();
            Vector2 pos = dimensions.Position();

            // Draw bar
            spriteBatch.Draw(bar, pos + new Vector2(0f, 4f), bar.Bounds, barColour);

            // Draw blips on bar
            int range = _maxValue - _minValue;
            Color c;
            for (int i = 0; i < range + 1; i++)
            {
                try { c = _blipColourList[i]; }
                catch { c = Color.LightSlateGray; }
                if (i == _dailyColour && _dailyColour >= 0) c = UIColour.Expert;
                spriteBatch.Draw(blip, pos + new Vector2(8f + (160f / range) * i, 8f), blip.Bounds, c);
            }


            // set to mouse
            if (_dragging) _dragVal = (int)(Main.mouseX - pos.X - 10);

            // limit slider to bar
            _dragVal = (int)Math.Max(Math.Min(_widthRange, _dragVal), 0f);

            //Set index to rounded position of dragVal in relation to range and distance across width
            _lastIndex = _index;
            if (_dragging) _index = (int)((0.5f + _minValue) + range * _dragVal / _widthRange);

            // Draw Slider
            spriteBatch.Draw(slider, pos + new Vector2(3f + _dragVal, 0f), slider.Bounds, Color.White);
            _dragVal = (int)(_widthRange / range * (_index - _minValue));
        }
    }
}
