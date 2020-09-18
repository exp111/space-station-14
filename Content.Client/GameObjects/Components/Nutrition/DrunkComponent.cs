using Content.Shared.GameObjects.Components.Nutrition;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Content.Client.GameObjects.Components.Nutrition
{
    [RegisterComponent]
    internal class DrunkComponent : SharedDrunkComponent
    {
        private DrunkThreshold _currentDrunkTreshold;
        [ViewVariables(VVAccess.ReadWrite)]
        public override DrunkThreshold CurrentDrunkThreshold => _currentDrunkTreshold;

        public override void HandleComponentState(ComponentState? curState, ComponentState? nextState)
        {
            if (!(curState is DrunkComponentState thirst))
            {
                return;
            }

            _currentDrunkTreshold = thirst.CurrentThreshold;
            //TODO: do/remove screen vignette effect
        }
    }
}
