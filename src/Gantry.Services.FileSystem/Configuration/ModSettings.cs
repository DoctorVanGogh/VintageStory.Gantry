﻿using System;
using Gantry.Core;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Enums;
using HarmonyLib;

namespace Gantry.Services.FileSystem.Configuration
{
    /// <summary>
    ///     Globally accessible settings files for the mod. Populated via the <see cref="IFileSystemService"/>.
    /// </summary>
    public static class ModSettings
    {
        /// <summary>
        ///     The global mod settings; these settings will persist through each gameworld.
        /// </summary>
        /// <value>The global settings.</value>
        internal static Abstractions.IJsonSettingsFile ClientGlobal { get; set; }

        /// <summary>
        ///     The per-world mod settings; these settings can change within each gameworld.
        /// </summary>
        /// <value>The per-world settings.</value>
        internal static Abstractions.IJsonSettingsFile ClientWorld { get; set; }

        /// <summary>
        ///     The global mod settings; these settings will persist through each gameworld.
        /// </summary>
        /// <value>The global settings.</value>
        internal static Abstractions.IJsonSettingsFile ServerGlobal { get; set; }

        /// <summary>
        ///     The per-world mod settings; these settings can change within each gameworld.
        /// </summary>
        /// <value>The per-world settings.</value>
        internal static Abstractions.IJsonSettingsFile ServerWorld { get; set; }

        /// <summary>
        ///     The global mod settings; these settings will persist through each gameworld.
        /// </summary>
        /// <value>The global settings.</value>
        public static Abstractions.IJsonSettingsFile Global => ApiEx.OneOf(ClientGlobal, ServerGlobal);

        /// <summary>
        ///     The per-world mod settings; these settings can change within each gameworld.
        /// </summary>
        /// <value>The per-world settings.</value>
        public static Abstractions.IJsonSettingsFile World => ApiEx.OneOf(ClientWorld, ServerWorld);

        /// <summary>
        ///     The mod settings for a specific <see cref="FileScope"/>.
        /// </summary>
        /// <value>The global settings.</value>
        public static Abstractions.IJsonSettingsFile For(FileScope scope)
        {
            return scope switch
            {
                FileScope.Global => Global,
                FileScope.World => World,
                _ => throw new ArgumentOutOfRangeException(nameof(scope))
            };
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public static void Dispose()
        {
            FeaturePatcher.UnpatchAll();
            ApiEx.Run(ClientDispose, ServerDispose);
        }

        private static void ClientDispose()
        {
            ClientWorld?.Dispose();
            ClientGlobal = null;

            ClientGlobal?.Dispose();
            ClientWorld = null;
        }

        private static void ServerDispose()
        {
            ServerGlobal?.Dispose();
            ServerGlobal = null;

            ServerWorld?.Dispose();
            ServerWorld = null;
        }

        internal static Harmony FeaturePatcher { get; } = new ($"{ModEx.ModInfo.ModID}_ObservableFeatures");
    }
}