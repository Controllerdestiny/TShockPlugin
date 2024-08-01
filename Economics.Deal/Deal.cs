﻿using System.Reflection;
using EconomicsAPI.Configured;
using Terraria;
using TerrariaApi.Server;
using TShockAPI.Hooks;

namespace Economics.Deal;

[ApiVersion(2, 1)]
public class Deal : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => new Version(1, 0, 0, 1);

    internal static string PATH = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "Deal.json");

    internal static Config Config { get; set; }

    public Deal(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Config = ConfigHelper.LoadConfig<Config>(PATH);
        GeneralHooks.ReloadEvent += (e) => Config = ConfigHelper.LoadConfig(PATH, Config);
    }
}
