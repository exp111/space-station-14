using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Content.Shared.GameObjects.Components.Nutrition
{
    public abstract class SharedDrunkComponent : Component
    {
        public override string Name => "Drunk";

        public sealed override uint? NetID => ContentNetIDs.DRUNK;

        public abstract DrunkThreshold CurrentDrunkThreshold { get; }

        [Serializable, NetSerializable]
        protected sealed class DrunkComponentState : ComponentState
        {
            public DrunkThreshold CurrentThreshold { get; }

            public DrunkComponentState(DrunkThreshold currentThreshold) : base(ContentNetIDs.DRUNK)
            {
                CurrentThreshold = currentThreshold;
            }
        }
    }

    //TODO: look at other codebases
    public enum DrunkThreshold : byte
    {
        No,
        Tipsy,
        Drunk,
        Blackout,
    }
}
