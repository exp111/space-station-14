﻿using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Components.UserInterface;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Content.Shared.GameObjects.Components
{
    public class SharedGasAnalyzerComponent : Component
    {
        public override string Name => "GasAnalyzer";

        [Serializable, NetSerializable]
        public enum GasAnalyzerUiKey
        {
            Key,
        }

        [Serializable, NetSerializable]
        public class GasAnalyzerBoundUserInterfaceState : BoundUserInterfaceState
        {
            public float Pressure;
            public float Temperature;
            public GasEntry[] Gases;
            public string Error;

            public GasAnalyzerBoundUserInterfaceState(float pressure, float temperature, GasEntry[] gases, string error = null)
            {
                Pressure = pressure;
                Temperature = temperature;
                Gases = gases;
                Error = error;
            }
        }

        [Serializable, NetSerializable]
        public struct GasEntry
        {
            public readonly string Name;
            public readonly float Amount;

            public GasEntry(string name, float amount)
            {
                Name = name;
                Amount = amount;
            }

            public override string ToString()
            {
                return Loc.GetString("{0}: {1} mol", Name, Amount);
            }
        }

        [Serializable, NetSerializable]
        public class GasAnalyzerRefreshMessage : BoundUserInterfaceMessage
        {
            public GasAnalyzerRefreshMessage() {}
        }
    }
}
