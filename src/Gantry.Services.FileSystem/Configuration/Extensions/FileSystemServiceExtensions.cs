﻿using System;
using Gantry.Core;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Enums;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Gantry.Services.FileSystem.Configuration.Extensions
{
    /// <summary>
    ///     Extension methods for use within the File System Service.
    /// </summary>
    public static class FileSystemServiceExtensions
    {
        /// <summary>
        ///     Registers a settings file with the FileSystem Service. This will copy a default implementation of the file from:
        ///      • An embedded resource.
        ///      • The mod's unpack directory.
        ///      • The mod's assets folder.
        /// 
        ///     If no default implementation can be found, a new file is created, at the correct location.
        /// </summary>
        /// <param name="fileSystem">The <see cref="IFileSystemService"/> to use to register the file.</param>
        /// <param name="fileName">The name of the file, including file extension.</param>
        /// <param name="scope">The scope of the file, be it global, or per-world.</param>
        public static IFileSystemService RegisterSettingsFile(
            this IFileSystemService fileSystem,
            string fileName,
            FileScope scope)
        {
            fileSystem.RegisterFile(fileName, scope);
            var file = JsonSettingsFile.FromJsonFile(fileSystem.GetJsonFile(fileName));
            switch (scope)
            {
                case FileScope.Global:
                    ApiEx.Run(() => ModSettings.ClientGlobal = file, () => ModSettings.ServerGlobal = file);
                    break;
                case FileScope.World:
                    ApiEx.Run(() => ModSettings.ClientWorld = file, () => ModSettings.ServerWorld = file);
                    break;
                case FileScope.Local:
                    ApiEx.Run(() => ModSettings.ClientLocal = file, () => ModSettings.ServerLocal = file);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
            }

            return fileSystem;
        }

        /// <summary>
        ///     Registers settings files.
        /// </summary>
        public static IFileSystemService RegisterSettingsFiles(this IFileSystemService fileSystem)
        {
            var side = ApiEx.Side.ToString().ToLower();
            return fileSystem
                .RegisterSettingsFile($"settings-world-{side}.json", FileScope.World)
                .RegisterSettingsFile($"settings-global-{side}.json", FileScope.Global);
        }
    }
}
