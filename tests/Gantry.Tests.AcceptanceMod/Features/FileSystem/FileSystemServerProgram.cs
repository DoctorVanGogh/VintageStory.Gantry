﻿using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using Gantry.Core.DependencyInjection;
using Gantry.Core.ModSystems;
using Gantry.Services.FileSystem.Abstractions.Contracts;
using Gantry.Services.FileSystem.Configuration;
using Gantry.Tests.AcceptanceMod.Features.FileSystem.Abstractions;
using Gantry.Tests.AcceptanceMod.Features.FileSystem.Settings;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Gantry.Tests.AcceptanceMod.Features.FileSystem
{
    internal sealed class FileSystemServerProgram : ServerModSystem
    {
        private IFileSystemService _fileSystemService;

        private IMessageProvider _worldSettings;
        private IMessageProvider _globalSettings;

        private IMessageProvider _embeddedWorldSettings;
        private IMessageProvider _embeddedGlobalSettings;

        public override void StartServerSide(ICoreServerAPI api)
        {
            _fileSystemService = IOC.Services.Resolve<IFileSystemService>();
            _worldSettings = ModSettings.World.Feature<ServerWorldSettings>();
            _globalSettings = ModSettings.Global.Feature<ServerGlobalSettings>();
            _embeddedWorldSettings = _fileSystemService.ParseEmbeddedJsonFile<EmbeddedJsonSettings>("embedded-world-server.json");
            _embeddedGlobalSettings = _fileSystemService.ParseEmbeddedJsonFile<EmbeddedJsonSettings>("embedded-global-server.json");

            api.RegisterCommand("filesystemtest", "", "", Handler);
        }

        private void Handler(IServerPlayer player, int groupId, CmdArgs args)
        {
            var scope = args.PopWord("world");
            var type = args.PopWord("file");
            var provider = type switch
            {
                "file" when scope is "world" => _worldSettings,
                "file" when scope is "global" => _globalSettings,
                "embedded" when scope is "world" => _embeddedWorldSettings,
                "embedded" when scope is "global" => _embeddedGlobalSettings,
                _ => _worldSettings
            };
            Sapi.SendMessage(player, groupId, provider.Message, EnumChatType.CommandSuccess);
        }
    }
}