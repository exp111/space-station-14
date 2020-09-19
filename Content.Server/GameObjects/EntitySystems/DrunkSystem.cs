using Content.Server.GameObjects.Components.Nutrition;
using Content.Server.Interfaces.Chat;
using Content.Shared.GameObjects.Components.Nutrition;
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

            return drunk.CurrentDrunkThreshold switch
            {
                //TODO: DrunkThreshold d when d > DrunkThreshold.Blackout => SuperDrunkify(message),
                DrunkThreshold d when d >= DrunkThreshold.Drunk => Drunkify(message),
                _ => message,
            };
        }

        private string Drunkify(string message)
        {
            // Algo taken from Goonstation accents.dm at e788c98304bfe25d0eb8188c22332edd1426714d
            //k => g, nk/ck => gh
            //s => sh
            //t => ff, th => du, nt => thf
            return message.
                Replace("k", "g").Replace("nk", "gh").Replace("nk", "gh").
                Replace("s", "sh").
                Replace("th", "du").Replace("nt", "hf").Replace("t", "ff");
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
