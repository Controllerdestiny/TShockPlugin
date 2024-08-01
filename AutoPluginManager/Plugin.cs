﻿using System.IO.Compression;
using System.Reflection;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace AutoPluginManager;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Name => "AutoPluginManager";

    public override Version Version => new(2, 0, 0, 1);

    public override string Author => "少司命，Cai";

    public override string Description => "自动更新你的插件！";

    private const string ReleaseUrl = "https://github.com/Controllerdestiny/TShockPlugin/releases/download/V1.0.0.0/Plugins.zip";

    private const string PUrl = "https://github.moeyy.xyz/";

    private const string PluginsUrl = "https://raw.githubusercontent.com/Controllerdestiny/TShockPlugin/master/Plugins.json";

    private static readonly HttpClient _httpClient = new();

    private const string TempSaveDir = "TempFile";

    private const string TempZipName = "Plugins.zip";

    private readonly System.Timers.Timer _timer = new();

    public Plugin(Main game) : base(game)
    {

    }

    public override void Initialize()
    {
        Commands.ChatCommands.Add(new("AutoUpdatePlugin", PluginManager, "apm"));
        ServerApi.Hooks.GamePostInitialize.Register(this, AutoCheckUpdate, int.MinValue); //最低优先级
    }



    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == PluginManager);
            ServerApi.Hooks.GamePostInitialize.Deregister(this, AutoCheckUpdate);
            _timer.Stop();
            _timer.Dispose();
        }

        base.Dispose(disposing);
    }

    private void AutoCheckUpdate(EventArgs args)
    {
        _timer.AutoReset = true;
        _timer.Enabled = true;
        _timer.Interval = 60 * 30 * 1000;
        _timer.Elapsed += (_, _) =>
        {
            try
            {
                var updates = GetUpdates();
                if (updates.Any())
                {
                    TShock.Log.ConsoleInfo("[以下插件有新的版本更新]\n" + string.Join("\n", updates.Select(i => $"[{i.Name}] V{i.OldVersion} >>> V{i.NewVersion}")));
                    TShock.Log.ConsoleInfo("你可以使用命令/uplugin更新插件哦~");
                }

            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleInfo("[AutoUpdate]无法获取更新:" + ex.Message);
                return;
            }
        };
        _timer.Start();
    }

    private void PluginManager(CommandArgs args)
    {
        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "-c")
        {
            CheckCmd(args.Player);
        }
        else if (args.Parameters.Count >= 1 && args.Parameters[0].ToLower() == "-u")
        {
            var targets = Array.Empty<string>();
            if (args.Parameters.Count > 1)
                targets = args.Parameters[1].Split(",");
            UpdateCmd(args.Player, targets);
        }
        else if (args.Parameters.Count == 2 && args.Parameters[0].ToLower() == "-i")
        {
            var indexs = args.Parameters[1].Split(",").Select(x =>
            {
                if (int.TryParse(x, out var index))
                    return index;
                return -1;
            });
            InstallCmd(args.Player, indexs);
        }
        else if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "-l")
        {
            var repo = GetRepoPlugin();
            args.Player.SendInfoMessage("可安装插件列表:");
            for (int i = 0; i < repo.Count; i++)
                args.Player.SendInfoMessage($"{i + 1}.{repo[i].Name} {repo[i].Path} {repo[i].Version}");
        }
        else
        {
            args.Player.SendInfoMessage("apm -c 检测已安装插件更新");
            args.Player.SendInfoMessage("apm -u [插件名] 更新所有插件或指定插件");
            args.Player.SendInfoMessage("apm -i [序号] 安装指定插件");
            args.Player.SendInfoMessage("apm -l 查看可安装插件表");
        }
    }

    private void InstallCmd(TSPlayer Player, IEnumerable<int> target)
    {
        if (!target.Any())
        {
            Player.SendErrorMessage("无效参数，请附带需要安装插件的选择项!");
            return;
        }
        try
        {
            var plugins = GetRepoPlugin();
            var installs = new List<PluginVersionInfo>();
            foreach (var index in target)
            {
                if (index > 0 && index <= plugins.Count)
                {
                    installs.Add(plugins[index - 1]);
                }
            }
            if (installs.Count == 0)
            {
                Player.SendErrorMessage("序号无效，请附带需要安装插件的选择项!");
                return;
            }
            Player.SendInfoMessage("正在下载最新插件包...");
            DownLoadPlugin();
            Player.SendInfoMessage("正在解压插件包...");
            ExtractDirectoryZip();
            Player.SendInfoMessage("正在安装插件...");
            InstallPlugin(installs);
            Player.SendSuccessMessage("[安装完成]\n" + string.Join("\n", installs.Select(i => $"[{i.Name}] V{i.Version}")));
            Player.SendSuccessMessage("重启服务器后插件生效!");
        }
        catch (Exception ex)
        {
            Player.SendErrorMessage("安装插件出现错误:" + ex.Message);
        }
    }

    private void UpdateCmd(TSPlayer Player, string[] target)
    {
        try
        {
            var updates = GetUpdates();
            if (updates.Count == 0)
            {
                Player.SendSuccessMessage("你的插件全是最新版本，无需更新哦~");
                return;
            }
            if (target.Length != 0)
            {
                updates = updates.Where(i => target.Contains(i.Name)).ToList();
                if (!updates.Any())
                {
                    Player.SendErrorMessage($"{string.Join(",", target)} 无需更新!");
                    return;
                }
            }
            Player.SendInfoMessage("正在下载最新插件包...");
            DownLoadPlugin();
            Player.SendInfoMessage("正在解压插件包...");
            ExtractDirectoryZip();
            Player.SendInfoMessage("正在升级插件...");
            var success = UpdatePlugin(updates);
            if (success.Count == 0)
            {
                Player.SendSuccessMessage("更新了个寂寞~");
                return;
            }
            Player.SendSuccessMessage("[更新完成]\n" + string.Join("\n", success.Select(i => $"[{i.Name}] V{i.OldVersion} >>> V{i.NewVersion}")));
            Player.SendSuccessMessage("重启服务器后插件生效!");
        }
        catch (Exception ex)
        {
            Player.SendErrorMessage("自动更新出现错误:" + ex.Message);
            return;
        }
    }

    private void CheckCmd(TSPlayer Player)
    {
        try
        {
            var updates = GetUpdates();
            if (updates.Count == 0)
            {
                Player.SendSuccessMessage("你的插件全是最新版本，无需更新哦~");
                return;
            }
            Player.SendInfoMessage("[以下插件有新的版本更新]\n" + string.Join("\n", updates.Select(i => $"[{i.Name}] V{i.OldVersion} >>> V{i.NewVersion}")));
        }
        catch (Exception ex)
        {
            Player.SendErrorMessage("无法获取更新:" + ex.Message);
            return;
        }
    }

    #region 工具方法
    private static List<PluginUpdateInfo> GetUpdates()
    {
        var plugins = GetPlugins();
        var latestPluginList = GetRepoPlugin();
        List<PluginUpdateInfo> pluginUpdateList = new();
        foreach (var latestPluginInfo in latestPluginList)
            foreach (var plugin in plugins)
                if (plugin.Name == latestPluginInfo.Name && plugin.Version != latestPluginInfo.Version)
                    pluginUpdateList.Add(new PluginUpdateInfo(plugin.Name, plugin.Author, latestPluginInfo.Version, plugin.Version, plugin.Path, latestPluginInfo.Path));
        return pluginUpdateList;
    }

    private static List<PluginUpdateInfo> GetUpdates(List<PluginVersionInfo> latestPluginList)
    {
        List<PluginUpdateInfo> pluginUpdateList = new();
        var plugins = GetPlugins();
        foreach (var latestPluginInfo in latestPluginList)
            foreach (var plugin in plugins)
                if (plugin.Name == latestPluginInfo.Name && plugin.Version != latestPluginInfo.Version)
                    pluginUpdateList.Add(new PluginUpdateInfo(plugin.Name, plugin.Author, latestPluginInfo.Version, plugin.Version, plugin.Path, latestPluginInfo.Path));
        return pluginUpdateList;
    }

    private static List<PluginVersionInfo> GetRepoPlugin()
    {
        var plugins = GetPlugins();
        HttpClient httpClient = new();
        var response = httpClient.GetAsync(PUrl + PluginsUrl).Result;

        if (!response.IsSuccessStatusCode)
            throw new Exception("无法连接服务器");
        var json = response.Content.ReadAsStringAsync().Result;
        return JsonConvert.DeserializeObject<List<PluginVersionInfo>>(json) ?? new();
    }

    private static List<PluginVersionInfo> GetPlugins()
    {
        List<PluginVersionInfo> plugins = new();
        //获取已安装的插件，并且读取插件信息和AssemblyName
        foreach (var plugin in ServerApi.Plugins)
        {
            plugins.Add(new PluginVersionInfo()
            {
                AssemblyName = plugin.Plugin.GetType().Assembly.GetName().Name!,
                Author = plugin.Plugin.Author,
                Name = plugin.Plugin.Name,
                Description = plugin.Plugin.Description,
                Version = plugin.Plugin.Version.ToString()
            });
        }
        //反射拯救了TSAPI
        var type = typeof(ServerApi);
        var field = type.GetField("loadedAssemblies", BindingFlags.NonPublic | BindingFlags.Static)!;
        if (field.GetValue(null) is Dictionary<string, Assembly> loadedAssemblies)
            foreach (var (fileName, assembly) in loadedAssemblies)
                for (int i = 0; i < plugins.Count; i++)
                    if (plugins[i].AssemblyName == assembly.GetName().Name)
                        plugins[i].Path = fileName + ".dll";
        return plugins;
    }


    private static void DownLoadPlugin()
    {
        DirectoryInfo directoryInfo = new(TempSaveDir);
        if (!directoryInfo.Exists)
            directoryInfo.Create();
        HttpClient httpClient = new();
        var zipBytes = httpClient.GetByteArrayAsync(PUrl + ReleaseUrl).Result;
        File.WriteAllBytes(Path.Combine(directoryInfo.FullName, TempZipName), zipBytes);
    }

    private static void ExtractDirectoryZip()
    {
        DirectoryInfo directoryInfo = new(TempSaveDir);
        ZipFile.ExtractToDirectory(Path.Combine(directoryInfo.FullName, TempZipName), Path.Combine(directoryInfo.FullName, "Plugins"), true);
    }

    private static void InstallPlugin(List<PluginVersionInfo> plugininfos)
    {
        foreach (var info in plugininfos)
        {
            string sourcePath = Path.Combine(TempSaveDir, "Plugins", info.Path);
            string destinationPath = Path.Combine(ServerApi.ServerPluginsDirectoryPath, info.Path);
            File.Copy(sourcePath, destinationPath, true);
            //热添加插件emmm
            //var ass = Assembly.Load(File.ReadAllBytes(destinationPath));
        }
        if (Directory.Exists(TempSaveDir))
            Directory.Delete(TempSaveDir, true);
    }

    private static List<PluginUpdateInfo> UpdatePlugin(List<PluginUpdateInfo> pluginUpdateInfos)
    {
        for (int i = pluginUpdateInfos.Count - 1; i >= 0; i--)
        {
            var pluginUpdateInfo = pluginUpdateInfos[i];
            string sourcePath = Path.Combine(TempSaveDir, "Plugins", pluginUpdateInfo.RemotePath);
            string destinationPath = Path.Combine(ServerApi.ServerPluginsDirectoryPath, pluginUpdateInfo.LocalPath);
            // 确保目标目录存在
            string destinationDirectory = Path.GetDirectoryName(destinationPath)!;
            if (File.Exists(destinationPath))
            {
                File.Copy(sourcePath, destinationPath, true);
            }
            else
            {
                TShock.Log.ConsoleWarn($"[跳过更新]无法在本地找到插件{pluginUpdateInfo.Name}({destinationPath}),可能是云加载或使用-additionalplugins加载");
                pluginUpdateInfos.RemoveAt(i);  // 移除元素
            }
        }
        if (Directory.Exists(TempSaveDir))
            Directory.Delete(TempSaveDir, true);

        return pluginUpdateInfos;
    }
    #endregion
}
