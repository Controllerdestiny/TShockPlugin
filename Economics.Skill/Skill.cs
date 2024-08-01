﻿using System.Reflection;
using Economics.Skill.DB;
using Economics.Skill.Events;
using Economics.Skill.Setting;
using EconomicsAPI.Configured;
using EconomicsAPI.EventArgs.PlayerEventArgs;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Economics.Skill;

[ApiVersion(2, 1)]
public class Skill : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => new(1, 2, 0, 9);

    internal static string PATH = Path.Combine(EconomicsAPI.Economics.SaveDirPath, "Skill.json");

    public long TimerCount;

    internal static Config Config { get; set; } = new();

    internal static PlayerSKillManager PlayerSKillManager { get; set; }

    public Skill(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        LoadConfig();
        PlayerSKillManager = new();
        ServerApi.Hooks.NpcStrike.Register(this, OnStrike);
        ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        ServerApi.Hooks.ProjectileAIUpdate.Register(this, OnAiUpdate);
        GetDataHandlers.PlayerUpdate.Register(OnPlayerUpdate);
        GetDataHandlers.PlayerHP.Register(OnHP);
        GetDataHandlers.PlayerMana.Register(OnMP);
        GetDataHandlers.KillMe.Register(KillMe);
        GetDataHandlers.NewProjectile.Register(OnNewProj);
        GetDataHandlers.PlayerDamage.Register(OnPlayerDamage);
        EconomicsAPI.Events.PlayerHandler.OnPlayerKillNpc += OnKillNpc;
        EconomicsAPI.Events.PlayerHandler.OnPlayerCountertop += OnPlayerCountertop;
        GeneralHooks.ReloadEvent += e => LoadConfig();
        On.Terraria.Projectile.Update += Projectile_Update;
    }

    private void OnPlayerDamage(object? sender, GetDataHandlers.PlayerDamageEventArgs e)
    {
        PlayerSparkSkillHandler.Adapter(e.Player, Enumerates.SkillSparkType.Struck);
    }

    private void OnAiUpdate(ProjectileAiUpdateEventArgs args)
    {
        if (!string.IsNullOrEmpty(args.Projectile.miscText))
            AIStyle.AI(args.Projectile);
    }

    private void Projectile_Update(On.Terraria.Projectile.orig_Update orig, Projectile self, int i)
    {
        if (self.timeLeft <= 0 || !self.active)
            self.Kill();
        else
            AIStyle.AI(self);
        orig(self, i);
    }

    private void KillMe(object? sender, GetDataHandlers.KillMeEventArgs e)
    {
        PlayerSparkSkillHandler.Adapter(e.Player, Enumerates.SkillSparkType.Death);
    }

    private void OnPlayerCountertop(PlayerCountertopArgs args)
    {
        var skill = PlayerSKillManager.QuerySkill(args.Player.Name);
        var msg = skill.Any() ? string.Join(",", skill.Select(x => x.Skill == null ? "无效技能" : x.Skill.Name)) : "无";
        args.Messages.Add(new($"绑定技能: {msg}", 12));
    }

    private void OnUpdate(EventArgs args)
    {
        TimerCount++;
        if ((TimerCount % 6) == 0)
        {
            SkillCD.SendGodPacket();
            SkillCD.Updata();
        }
    }

    private void OnNewProj(object? sender, GetDataHandlers.NewProjectileEventArgs e)
    {
        if (e.Player.TPlayer.controlUseItem)
            PlayerSparkSkillHandler.Adapter(e, Enumerates.SkillSparkType.Take);
    }

    private void OnMP(object? sender, GetDataHandlers.PlayerManaEventArgs e)
    {
        PlayerSparkSkillHandler.Adapter(e.Player, Enumerates.SkillSparkType.MP);
    }

    private void OnHP(object? sender, GetDataHandlers.PlayerHPEventArgs e)
    {
        PlayerSparkSkillHandler.Adapter(e.Player, Enumerates.SkillSparkType.HP);
    }

    private void OnStrike(NpcStrikeEventArgs args)
    {
        var tsply = TShock.Players[args.Player.whoAmI];
        if (tsply != null)
            PlayerSparkSkillHandler.Adapter(tsply, Enumerates.SkillSparkType.Strike);
    }

    private void OnKillNpc(PlayerKillNpcArgs args)
    {
        if (args.Player != null)
            PlayerSparkSkillHandler.Adapter(args.Player, Enumerates.SkillSparkType.Kill);
    }

    private void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        if (e.Player.TPlayer.ItemTimeIsZero && e.Player.TPlayer.controlUseItem && e.Player.TPlayer.HeldItem.shoot == 0)
        {
            PlayerSparkSkillHandler.Adapter(e.Player, Enumerates.SkillSparkType.Take);
        }

        if (e.Player.TPlayer.dashDelay == -1)
        {
            PlayerSparkSkillHandler.Adapter(e.Player, Enumerates.SkillSparkType.Dash);
        }
        if (e.Player.TPlayer.jump > 0)
        {
            PlayerSparkSkillHandler.Adapter(e.Player, Enumerates.SkillSparkType.Jump);
        }
    }

    private static void LoadConfig()
    {
        if (File.Exists(PATH))
        {
            Config = ConfigHelper.LoadConfig<Config>(PATH);
        }
        else
        {
            Config = new Config()
            {
                SkillContexts = new()
                {
                    new Model.SkillContext()
                    {
                        BuffOption = new()
                        {
                            Buffs = new()
                        },
                        Projectiles = new()
                        {
                            new()
                            {
                                ProjectileCycle = new()
                                {
                                    ProjectileCycles = new()
                                    {
                                        new()
                                    }
                                }
                            }
                        }
                    }
                }
            };
            Config = ConfigHelper.LoadConfig(PATH, Config);
        }
    }
}
