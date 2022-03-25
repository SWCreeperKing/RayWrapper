﻿using RayWrapper.Var_Interfaces;

namespace RayWrapper.Vars
{
    public abstract class Tooltip
    {
        public enum ScreenQuadrant
        {
            TopLeft = 1,
            TopRight = 2,
            BottomLeft = 3,
            BottomRight = 4
        }
        
        private Actionable<string> _data;

        public Tooltip(Actionable<string> data) => _data = data;
        
        /// <summary>
        /// Use this to draw the tooltip
        /// </summary>
        public void Draw() => GameBox.tooltips.Add(this);

        /// <summary>
        /// Do not call this to draw, use <see cref="Tooltip.Draw()"/>
        /// This is for the process of rendering the tooltip
        /// </summary>
        public void RenderTooltip(ScreenQuadrant screenQuad) => RenderTooltip(screenQuad, _data);

        /// <summary>
        /// Do not call this to draw, use <see cref="Tooltip.Draw()"/>
        /// This is for the process of rendering the tooltip
        /// </summary>
        protected abstract void RenderTooltip(ScreenQuadrant screenQuad, string data);
    }
}