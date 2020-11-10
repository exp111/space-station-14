using Content.Shared.Interfaces;
using Content.Shared.Interfaces.GameObjects.Components;
using Content.Shared.Utility;
using Robust.Shared.GameObjects;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Random;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using System;
using System.Linq;

namespace Content.Server.GameObjects.Components.Adminbus
{
    [RegisterComponent]
    internal class RandomComponentGun : Component, IAfterInteract
    {
        public override string Name => "RandomComponentGun";

        void IAfterInteract.AfterInteract(AfterInteractEventArgs eventArgs)
        {
            var target = eventArgs.Target;
            if (target == null)
                return;

            if (!eventArgs.InRangeUnobstructed(popup: true))
                return;

            var random = IoCManager.Resolve<IRobustRandom>();
            var compFactory = IoCManager.Resolve<IComponentFactory>();

            var types = compFactory.AllRegisteredTypes.ToList();
            Type selected;
            do
            {
                selected = random.Pick(types);
            } while (target.HasComponent(selected));

            if (selected == null)
                eventArgs.User.PopupMessage("Already has all components.");

            // Generic fuckery to add the comp
            var ensure = typeof(IEntity).GetMethod("AddComponent");
            if (ensure == null)
                return;
            var method = ensure.MakeGenericMethod(selected);
            method.Invoke(target, null);

            Robust.Shared.Log.Logger.Debug($"Added component {selected.Name} to {target.Name} ({target.Uid})");
        }
    }
}
