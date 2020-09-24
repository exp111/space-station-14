using Content.Server.GameObjects.Components.Body.Circulatory;
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
    public class AlcoholicDrink : IMetabolizable
    {
        //Rate of metabolism in units / second
        private ReagentUnit _metabolismRate;
        public ReagentUnit MetabolismRate => _metabolismRate;

        // How much Ethanol should be given per unit of this reagent
        private float _ethanolFactor;
        public float EthanolFactor => _ethanolFactor;

        void IExposeData.ExposeData(ObjectSerializer serializer)
        {
            serializer.DataField(ref _metabolismRate, "rate", ReagentUnit.New(1));
            serializer.DataField(ref _ethanolFactor, "ethanolFactor", 1.0f);
        }

        // Transform the drink into ethanol
        ReagentUnit IMetabolizable.Metabolize(IEntity solutionEntity, string reagentId, float tickTime)
        {
            var metabolismAmount = MetabolismRate * tickTime;
            if (solutionEntity.TryGetComponent(out BloodstreamComponent bloodstream))
            {
                if (!bloodstream.Solution.TryAddReagent("chem.Ethanol", metabolismAmount * EthanolFactor, out _))
                    return ReagentUnit.Zero;
            }

            return metabolismAmount;
        }
    }
}
