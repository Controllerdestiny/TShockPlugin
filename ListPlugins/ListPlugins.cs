﻿using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

[ApiVersion(2, 1)]
public class ListPlugins : TerrariaPlugin
{
    public override string Name => "查已装插件";
    public override Version Version => new Version(1, 0, 4);
    public override string Author => "iheart 修改：羽学，肝帝熙恩";
    public override string Description => "用指令查已装插件";

    public ListPlugins(Main game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new Command("ListPlugin", ListPluginsCommand, "插件列表", "pllist"));
    }

    private void ListPluginsCommand(CommandArgs args)
    {
        try
        {
            var pluginInfos = ServerApi.Plugins.Select(p => new
            {
                Name = p.Plugin.Name,
                Author = p.Plugin.Author,
                Version = p.Plugin.Version,
                Description = p.Plugin.Description
            });

            if (!pluginInfos.Any())
            {
                args.Player.SendInfoMessage("没有安装任何插件。");
                return;
            }

            StringBuilder msgBuilder = new StringBuilder();
            msgBuilder.AppendLine("插件列表：");
            foreach (var plugin in pluginInfos)
            {
                msgBuilder.AppendLine(FormatPluginInfo(plugin));
            }

            args.Player.SendInfoMessage(msgBuilder.ToString());
        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.ToString());
            args.Player.SendErrorMessage("获取插件列表时发生错误。");
        }
    }

    private string FormatPluginInfo(dynamic plugin)
    {
        Random random = new Random();
        string colorTag = $"[c/{random.Next(0, 16777216):X}:";
        string formattedName = colorTag + plugin.Name.Replace("]", "]" + colorTag + "]") + "]";
        return $"{formattedName} - 版本: {plugin.Version} - 作者: {plugin.Author}" +
               (plugin.Description != null ? $", 描述: {plugin.Description}" : "");
    }
}
