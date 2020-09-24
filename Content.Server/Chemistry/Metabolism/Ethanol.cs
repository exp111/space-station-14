using Content.Server.GameObjects.Components.Nutrition;
using Content.Shared.Chemistry;
using Content.Shared.Interfaces.Chemistry;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Serialization;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Content.Server.Chemistry.Metabolism
{
    public class Ethanol : IMetabolizable
    {
        //Rate of metabolism in units / second
        private ReagentUnit _metabolismRate;
        public ReagentUnit MetabolismRate => _metabolismRate;

        //How much thirst is satiated when 1u of the reagent is metabolized
        private float _hydrationFactor;
        public float HydrationFactor => _hydrationFactor;

        private float _alcoholFactor;
        public float AlhocolFactor => _alcoholFactor;

        void IExposeData.ExposeData(ObjectSerializer serializer)
        {
            serializer.DataField(ref _metabolismRate, "rate", ReagentUnit.New(1));
            serializer.DataField(ref _hydrationFactor, "nutrimentFactor", 30.0f);
            serializer.DataField(ref _alcoholFactor, "alcoholFactor", 1.0f);
        }

        //Remove reagent at set rate, satiate thirst if a ThirstComponent can be found
        ReagentUnit IMetabolizable.Metabolize(IEntity solutionEntity, string reagentId, float tickTime)
        {
            var metabolismAmount = MetabolismRate * tickTime;
            if (solutionEntity.TryGetComponent(out ThirstComponent thirst))
                thirst.UpdateThirst(metabolismAmount.Float() * HydrationFactor);
            if (solutionEntity.TryGetComponent(out DrunkComponent drunk))
                drunk.UpdateDrunk(metabolismAmount.Float() * AlhocolFactor);


            //Return amount of reagent to be removed
            return metabolismAmount;
        }
    }
}
