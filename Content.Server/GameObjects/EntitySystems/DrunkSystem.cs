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
using System.Text.RegularExpressions;

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

        private readonly Regex _drunkRegex = new Regex("(k|nk|ck|s|th|nt|t)", RegexOptions.IgnoreCase);
        private string Drunkify(string message)
        {
            // Taken from Goonstation accents.dm at e788c98304bfe25d0eb8188c22332edd1426714d
            //k => g, nk/ck => gh
            //s => sh
            //t => ff, th => du, nt => thf
            return _drunkRegex.Replace(message, DrunkMatcher);
        }

        private string DrunkMatcher(Match match)
        {
            var upper = match.Length > 0 && char.IsUpper(match.Value[0]);
            return match.Value.ToLower() switch
            {
                "k" => upper ? "G" : "g",
                "nk" => upper ? "GH" : "gh",
                "ck" => upper ? "GH" : "gh",
                "s" => upper ? "SH" : "sh",
                "th" => upper ? "DU" : "du",
                "nt" => upper ? "THF" : "thf",
                "t" => upper ? "FF" : "ff",
                _ => match.Value,
            };
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
