﻿
using AAAGR_io.Engine;

namespace AAAGR_io.Game_Assets.Interfaces
{
    public enum SpriteName
    { 
        None,
        Skull,
        Color,
        Zergling,
    }
    public interface IAnimateable
    {
        public Animator GetAnimator();
    }
}
