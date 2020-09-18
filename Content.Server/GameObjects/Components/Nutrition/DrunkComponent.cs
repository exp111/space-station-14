using Content.Server.GameObjects.Components.Mobs;
using Content.Shared.GameObjects.Components.Mobs;
using Content.Shared.GameObjects.Components.Nutrition;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Content.Server.GameObjects.Components.Nutrition
{
    [RegisterComponent]
    internal class DrunkComponent : SharedDrunkComponent
    {
        private float _baseDecayRate;
        [ViewVariables(VVAccess.ReadWrite)]
        public float BaseDecayRate
        {
            get => _baseDecayRate;
            set
            {
                _baseDecayRate = value;
            }
        }

        //TODO: maybe other baseDecayRate?

        [ViewVariables(VVAccess.ReadWrite)]
        private float _currentDrunk = 0.0f;

        private DrunkThreshold _currentDrunkThreshold = DrunkThreshold.No;
        [ViewVariables(VVAccess.ReadWrite)]
        public override DrunkThreshold CurrentDrunkThreshold => _currentDrunkThreshold;

        //TODO: alcoholResistance
        [ViewVariables(VVAccess.ReadWrite)]
        private float _alcoholResistance = 0.0f;

        [ViewVariables(VVAccess.ReadOnly)]
        //TODO: find proper values for the tresholds
        public Dictionary<DrunkThreshold, float> DrunkThresholds { get; } = new Dictionary<DrunkThreshold, float>
        {
            {DrunkThreshold.Blackout, 600.0f},
            {DrunkThreshold.Drunk, 450.0f},
            {DrunkThreshold.Tipsy, 300.0f},
            {DrunkThreshold.No, 100.0f},
        };

        //TODO: drunk status effects images
        public static readonly Dictionary<DrunkThreshold, string> DrunkThresholdImages = new Dictionary<DrunkThreshold, string>
        {
            {DrunkThreshold.Blackout, "/Textures/Interface/StatusEffects/Drunk/Blackout.png"},
            {DrunkThreshold.Drunk, "/Textures/Interface/StatusEffects/Drunk/Drunk.png"},
            {DrunkThreshold.Tipsy, "/Textures/Interface/StatusEffects/Drunk/Tipsy.png"},
        };

        protected override void Startup()
        {
            base.Startup();

            //TODO: Random tolerance?
        }

        public override void ExposeData(ObjectSerializer serializer)
        {
            base.ExposeData(serializer);
            serializer.DataField(ref _baseDecayRate, "base_decay_rate", 0.1f);
        }

        public void OnUpdate(float frametime)
        {
            _currentDrunk -= frametime * BaseDecayRate; //TODO: actualDecayRate?
            var calculatedThirstThreshold = GetDrunkThreshold(_currentDrunk);
            if (calculatedThirstThreshold != _currentDrunkThreshold)
            {
                _currentDrunkThreshold = calculatedThirstThreshold;
                DrunkThresholdEffect();
                Dirty();
            }

            //TODO: random move thingy if drunk enough
            //TODO: dmg when too drunk
        }

        public void ResetDrunk()
        {
            _currentDrunk = 0f;
            _currentDrunkThreshold = DrunkThreshold.No;
            DrunkThresholdEffect();
            Dirty();
        }

        private DrunkThreshold GetDrunkThreshold(float drink)
        {
            DrunkThreshold result = DrunkThreshold.No;
            var value = DrunkThresholds[DrunkThreshold.Blackout];
            foreach (var threshold in DrunkThresholds)
            {
                if (threshold.Value <= value && threshold.Value >= drink)
                {
                    result = threshold.Key;
                    value = threshold.Value;
                }
            }

            return result;
        }

        private void DrunkThresholdEffect()
        {
            Owner.TryGetComponent(out ServerStatusEffectsComponent statusEffectsComponent);

            if (DrunkThresholdImages.TryGetValue(_currentDrunkThreshold, out var statusTexture))
            {
                statusEffectsComponent?.ChangeStatusEffectIcon(StatusEffect.Drunk, statusTexture);
            }
            else
            {
                statusEffectsComponent?.RemoveStatusEffect(StatusEffect.Drunk);
            }

            /*switch (_currentDrunkThreshold)
            {
                case DrunkThreshold.Blackout:
                    _actualDecayRate = _baseDecayRate;
                    return;
                case DrunkThreshold.Drunk:
                    _actualDecayRate = _baseDecayRate * 0.8f;
                    return;
                case DrunkThreshold.Tipsy:
                    _actualDecayRate = _baseDecayRate * 0.6f;
                    return;
                case DrunkThreshold.No:
                    return;
                default:
                    Logger.ErrorS("thirst", $"No drunk threshold found for {_currentDrunkThreshold}");
                    throw new ArgumentOutOfRangeException($"No thirst drunk found for {_currentDrunkThreshold}");
            }*/
        }

        public void UpdateDrunk(float amount)
        {
            _currentDrunk = Math.Min(_currentDrunk + amount, DrunkThresholds[DrunkThreshold.Blackout]);
        }
        public override ComponentState GetComponentState()
        {
            return new DrunkComponentState(_currentDrunkThreshold);
        }
    }
}
