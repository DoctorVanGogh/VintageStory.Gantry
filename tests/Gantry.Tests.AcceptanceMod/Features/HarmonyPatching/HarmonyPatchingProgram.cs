﻿using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using Gantry.Core.DependencyInjection;
using Gantry.Core.ModSystems;
using Gantry.Services.HarmonyPatches;
using Gantry.Tests.AcceptanceMod.Features.HarmonyPatching.Patches;
using HarmonyLib;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

// ReSharper disable StringLiteralTypo

namespace Gantry.Tests.AcceptanceMod.Features.HarmonyPatching
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal sealed class HarmonyPatchingProgram : UniversalModSystem
    {
        private IHarmonyPatchingService _harmony;
        private readonly OriginalClass _originalClass = new();
        public override void Start(ICoreAPI api) => _harmony = IOC.Services.Resolve<IHarmonyPatchingService>();
        public override void StartClientSide(ICoreClientAPI api) => Should_Patch_OriginalClass_On_Client(api);
        public override void StartServerSide(ICoreServerAPI sapi) => Should_Patch_OriginalClass_On_Server(sapi);


        private void Should_Patch_OriginalClass_On_Client(ICoreClientAPI api)
        {
            // Arrange
            const string postfixName = nameof(OriginalClassManualPatches.Patch_OriginalClass_OriginalClientMethod_Postfix);
            var method = typeof(OriginalClass).GetMethod(nameof(OriginalClass.OriginalClientMethod));
            var postfix = typeof(OriginalClassManualPatches).GetMethod(postfixName);

            // Act
            _harmony.Default.Patch(method, postfix: new HarmonyMethod(postfix));

            // Assert
            api.RegisterCommand("harmonytest", "", "",
                (_, _) => _originalClass.OriginalClientMethod(api));
        }

        private void Should_Patch_OriginalClass_On_Server(ICoreServerAPI sapi)
        {
            // Arrange
            const string postfixName = nameof(OriginalClassManualPatches.Patch_OriginalClass_OriginalServerMethod_Postfix);
            var method = typeof(OriginalClass).GetMethod(nameof(OriginalClass.OriginalServerMethod));
            var postfix = typeof(OriginalClassManualPatches).GetMethod(postfixName);

            // Act
            _harmony.Default.Patch(method, postfix: new HarmonyMethod(postfix));

            // Assert
            sapi.RegisterCommand("harmonytest", "", "", (player, id, _) =>
                _originalClass.OriginalServerMethod(sapi, player, id));
        }
    }
}