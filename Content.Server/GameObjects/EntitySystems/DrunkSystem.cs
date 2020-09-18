using Content.Server.GameObjects.Components.Nutrition;
using Content.Server.Interfaces.Chat;
using Content.Shared.GameObjects.Components.Observer;
using Robust.Shared.GameObjects.Systems;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;
using System.Text;

namespace Content.Server.GameObjects.EntitySystems
{
    public class DrunkSystem : EntitySystem
    {
        private float _accumulatedFrameTime;

        public override void Initialize()
        {
            var chatManager = IoCManager.Resolve<IChatManager>();
            chatManager.RegisterChatTransform(DrunkSpeech);
        }

        public string DrunkSpeech(IEntity speaker, string message)
        {
            if (!speaker.TryGetComponent(out DrunkComponent drunk))
                return message;

            if (drunk.CurrentDrunkThreshold > Shared.GameObjects.Components.Nutrition.DrunkThreshold.Drunk)
            {
                return $"DRUNK: {message}";
            }
            return message;
        }

        public override void Update(float frameTime)
        {
            _accumulatedFrameTime += frameTime;

            if (_accumulatedFrameTime > 1)
            {
                foreach (var component in ComponentManager.EntityQuery<DrunkComponent>())
                {
                    component.OnUpdate(_accumulatedFrameTime);
                }
                _accumulatedFrameTime -= 1;
            }
        }
    }
}
