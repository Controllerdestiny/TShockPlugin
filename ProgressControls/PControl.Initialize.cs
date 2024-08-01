﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace ProgressControl
{
    public partial class PControl : TerrariaPlugin
    {
        /// <summary>
        /// 普通控制boss进度的指令
        /// </summary>
        /// <param name="args"></param>
        private void PCO(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                string othermess = "";
                //boss进度数据输出
                if (config.OpenAutoControlProgressLock)
                {
                    double lv = (DateTime.Now - config.StartServerDate).TotalHours;
                    Dictionary<string, double> keyValuePairs = new Dictionary<string, double>();
                    Dictionary<string, double> sortpairs = new Dictionary<string, double>();
                    foreach (var v in config.ProgressLockTimeForStartServerDate)
                    {
                        keyValuePairs.Add(v.Key, v.Value);
                    }
                    //排序
                    while (keyValuePairs.Count > 0)
                    {
                        double min = double.MaxValue;
                        string key = "";
                        foreach (var v in keyValuePairs)
                        {
                            if (v.Value < min)
                            {
                                key = v.Key;
                                min = v.Value;
                            }
                        }
                        if (key != "")
                        {
                            sortpairs.Add(key, min);
                            keyValuePairs.Remove(key);
                        }
                    }
                    string mess = args.Player.IsLoggedIn ? "[i:3868]已解锁Boss：\n" : "@已解锁Boss：\n";
                    int count = 0;
                    //把排好序的数据输出
                    foreach (var v in sortpairs)
                    {
                        if (v.Value < lv)
                        {
                            mess += $"{v.Key}  ";
                            count++;
                            if (count == 8)
                            {
                                mess += "\n";
                                count = 0;
                            }
                        }
                        else
                        {
                            mess += $"{(count != 0 ? "\n" : "")}{v.Key} 还剩{HoursToM(v.Value - lv, (args.Player.IsLoggedIn ? "28FFB8" : ""))}解锁";
                            count = 1;
                        }
                    }
                    //npc的************************************************************************************
                    Dictionary<int, double> keyValuePairsnpc = new Dictionary<int, double>();
                    Dictionary<int, double> sortpairsnpc = new Dictionary<int, double>();
                    foreach (var v in config.CustomNPCIDLockTimeForStartServerDate)
                    {
                        keyValuePairsnpc.Add(v.Key, v.Value);
                    }
                    //排序
                    while (keyValuePairsnpc.Count > 0)
                    {
                        double min = double.MaxValue;
                        int key = -114514;
                        foreach (var v in keyValuePairsnpc)
                        {
                            if (v.Value < min)
                            {
                                key = v.Key;
                                min = v.Value;
                            }
                        }
                        if (key != -114514)
                        {
                            sortpairsnpc.Add(key, min);
                            keyValuePairsnpc.Remove(key);
                        }
                    }
                    //把排好序的数据输出
                    foreach (var v in sortpairsnpc)
                    {
                        if (v.Value >= lv)
                            mess += $"\n[{v.Key}]{Lang.GetNPCNameValue(v.Key)} 还剩{HoursToM(v.Value - lv, (args.Player.IsLoggedIn ? "28FFB8" : ""))}解锁";
                    }
                    args.Player.SendMessage(mess, TextColor());
                }
                else
                    othermess += args.Player.IsLoggedIn ? "[i:3868]" + MtoM("NPC进度控制未开启", "28FFB8") + "\n" : "NPC进度控制未开启\n";

                //自动重置数据输出
                if (config.OpenAutoReset && !countdownReset.enable)
                {
                    if (args.Player.IsLoggedIn)
                        args.Player.SendInfoMessage($"[i:3099]世界将于{HoursToM((config.StartServerDate.AddHours(config.HowLongTimeOfAotuResetServer) - DateTime.Now).TotalHours, "EA00FF")}后重置");
                    else
                        args.Player.SendInfoMessage($"@世界将于{HoursToM((config.StartServerDate.AddHours(config.HowLongTimeOfAotuResetServer) - DateTime.Now).TotalHours)}后重置");
                }
                else if (countdownReset.enable)
                {
                    if (args.Player.IsLoggedIn)
                        args.Player.SendInfoMessage($"[i:709]世界将于{HoursToM(countdownReset.time * 1.0 / 3600, "EA00FF")}后重置");
                    else
                        args.Player.SendInfoMessage($"@世界将于{HoursToM(countdownReset.time * 1.0 / 3600)}后重置");
                }
                else
                    othermess += args.Player.IsLoggedIn ? "[i:3099]" + MtoM("世界自动重置未开启", "EA00FF") + "\n" : "世界自动重置未开启\n";

                //自动重启数据输出
                if (config.AutoRestartServer && !countdownRestart.enable)
                {
                    if (args.Player.IsLoggedIn)
                        args.Player.SendInfoMessage($"[i:17]服务器将于{HoursToM((config.LasetServerRestartDate.AddHours(config.HowLongTimeOfRestartServer) - DateTime.Now).TotalHours, "FF9000")}后重启");
                    else
                        args.Player.SendInfoMessage($"@服务器将于{HoursToM((config.LasetServerRestartDate.AddHours(config.HowLongTimeOfRestartServer) - DateTime.Now).TotalHours)}后重启");
                }
                else if (countdownRestart.enable)
                {
                    if (args.Player.IsLoggedIn)
                        args.Player.SendInfoMessage($"[i:17]服务器将于{HoursToM(countdownRestart.time * 1.0 / 3600, "FF9000")}后重启");
                    else
                        args.Player.SendInfoMessage($"@服务器将于{HoursToM(countdownRestart.time * 1.0 / 3600)}后重启");
                }
                else
                    othermess += args.Player.IsLoggedIn ? "[i:17]" + MtoM("服务器自动重启未开启", "FF9000") + "\n" : "服务器自动重启未开启\n";

                //自动执行指令输出
                if (config.OpenAutoCommand && !countdownCom.enable)
                {
                    if (args.Player.IsLoggedIn)
                        args.Player.SendInfoMessage($"[i:903]服务器将于{HoursToM((config.LasetAutoCommandDate.AddHours(config.HowLongTimeOfAutoCommand) - DateTime.Now).TotalHours, "00A8FF")}后执行指令");
                    else
                        args.Player.SendInfoMessage($"@服务器将于{HoursToM((config.LasetAutoCommandDate.AddHours(config.HowLongTimeOfAutoCommand) - DateTime.Now).TotalHours)}后执行指令");
                }
                else if (countdownCom.enable)
                {
                    if (args.Player.IsLoggedIn)
                        args.Player.SendInfoMessage($"[i:903]服务器将于{HoursToM(countdownCom.time * 1.0 / 3600, "00A8FF")}后执行指令");
                    else
                        args.Player.SendInfoMessage($"@服务器将于{HoursToM(countdownCom.time * 1.0 / 3600)}后执行指令");
                }
                else
                    othermess += args.Player.IsLoggedIn ? "[i:903]" + MtoM("服务器自动执行指令未开启", "00A8FF") : "服务器自动执行指令未开启";
                othermess = othermess.Trim();
                if (!string.IsNullOrWhiteSpace(othermess))
                    args.Player.SendInfoMessage(othermess);
                return;
            }
            else if (args.Parameters.Count == 1)
            {
                if (args.Parameters[0].Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    args.Player.SendMessage(
                        //都能使用
                        "输入 /pco help   来获取该插件的帮助\n" +
                        "输入 /pco   来查看当前服务器的自动化计划简化信息\n" +
                        //p_admin
                        "输入 /pco now   来将开服日期、上次重启日期和上次自动执行指令日期调整到现在\n" +
                        "输入 /pco delfile   使用指令删除配置文件中选定的文件\n" +
                        "输入 /pco copy   使用指令复制配置文件中选定的文件到指定目录\n" +
                        //p_npc
                        "输入 /pco npc help   来查看NPC封禁计划的帮助指令\n" +
                        //p_com
                        "输入 /pco com help   来查看执行指令计划的帮助指令\n" +
                        //p_reload
                        "输入 /pco reload help   来查看重启计划的帮助指令\n" +
                        //p_reset
                        "输入 /pco reset help   来查看重置计划的帮助指令\n" +
                        //上面四个指令任意一个的分配
                        "输入 /pco mess   来查看当前服务器的自动化计划详细信息", TextColor());
                }
                else if (args.Parameters[0].Equals("now", StringComparison.OrdinalIgnoreCase))
                {
                    if (!args.Player.HasPermission(p_admin))
                    {
                        args.Player.SendInfoMessage($"权限不足！[{p_admin}]");
                        return;
                    }
                    config.StartServerDate = DateTime.Now;
                    config.LasetServerRestartDate = DateTime.Now;
                    config.LasetAutoCommandDate = DateTime.Now;
                    config.SaveConfigFile();
                    args.Player.SendSuccessMessage("定位成功，所有已开启的自动计划将从现在开始计时");
                }
                else if (args.Parameters[0].Equals("delfile", StringComparison.OrdinalIgnoreCase))
                {
                    if (!args.Player.HasPermission(p_admin))
                    {
                        args.Player.SendInfoMessage($"权限不足！[{p_admin}]");
                        return;
                    }
                    try
                    {
                        if (config.Command_PcoDelFile_DeletePath.Count > 0)
                        {
                            foreach (string v in config.Command_PcoDelFile_DeletePath)
                            {
                                if (string.IsNullOrWhiteSpace(v))
                                    continue;
                                if (File.Exists(v))//删除指定文件
                                    File.Delete(v);
                                if (Directory.Exists(v))//删除指定文件夹
                                    Directory.Delete(v, true);
                                //分析删除后缀为 *.txt(举例) 的文件
                                string[] files = v.Split('/');
                                string theDirectory = v.Remove(v.Length - files[files.Length - 1].Length);
                                files[files.Length - 1] = files[files.Length - 1].Trim();
                                if (files[files.Length - 1].StartsWith("*.") && Directory.Exists(theDirectory))
                                {
                                    DirectoryInfo dir = new DirectoryInfo(theDirectory);
                                    foreach (FileSystemInfo i in dir.GetFileSystemInfos())
                                    {
                                        if (i.Extension.Equals(files[files.Length - 1].TrimStart('*'), StringComparison.OrdinalIgnoreCase) && File.Exists(i.FullName))
                                        {
                                            File.Delete(i.FullName);      //删除指定后缀名文件
                                            args.Player.SendSuccessMessage(i.Name + " 删除成功");
                                            TShock.Log.Info(i.Name + " 删除成功");
                                        }
                                    }
                                }
                            }
                        }
                        else
                            args.Player.SendInfoMessage("你没有指出那些文件需要删除，请在配置文件中设置");
                    }
                    catch (Exception ex)
                    {
                        TSPlayer.All.SendErrorMessage("使用指令删除哪些文件发生错误：" + ex.ToString());
                        TShock.Log.Error("使用指令删除哪些文件发生错误：" + ex.ToString());
                        Console.WriteLine("使用指令删除哪些文件发生错误：" + ex.ToString());
                    }
                }
                else if (args.Parameters[0].Equals("copy", StringComparison.OrdinalIgnoreCase))
                {
                    if (!args.Player.HasPermission(p_admin))
                    {
                        args.Player.SendInfoMessage($"权限不足！[{p_admin}]");
                        return;
                    }
                    try
                    {
                        if (config.Command_PcoCopy_CopyPath.Count > 0)
                        {
                            string currentDate = DateTime.UtcNow.ToString(" yyyy-MM-dd"); // 获取当前UTC时间并格式化
                            foreach (string v in config.Command_PcoCopy_CopyPath)
                            {
                                if (string.IsNullOrWhiteSpace(v))
                                    continue;

                                string sourcePath = v;
                                string baseDestinationName = Path.GetFileNameWithoutExtension(v); // 基础名称不包含扩展名
                                string destinationFolder = Path.Combine(config.Command_PcoCopy_PastePath, baseDestinationName + currentDate);

                                if (File.Exists(sourcePath)) // 复制指定文件
                                {
                                    string destinationPath = Path.ChangeExtension(destinationFolder, Path.GetExtension(v));
                                    File.Copy(sourcePath, destinationPath, config.Command_PcoCopy_CoverFile);
                                    args.Player.SendSuccessMessage($"文件'{Path.GetFileName(v)}' 已复制到 '{destinationPath}'");
                                }
                                else if (Directory.Exists(v)) // 如果是目录，则递归复制整个目录，并在目标目录名后添加日期
                                {
                                    DirectoryCopy(v, destinationFolder, config.Command_PcoCopy_CoverFile);
                                    args.Player.SendSuccessMessage($"目录'{v}' 内容已复制到 '{destinationFolder}'");
                                }
                                else if (v.EndsWith("*.txt", StringComparison.OrdinalIgnoreCase) && Directory.Exists(Path.GetDirectoryName(v))) // 复制指定后缀名的文件
                                {
                                    string directory = Path.GetDirectoryName(v);
                                    string extension = v.Substring(v.LastIndexOf('.') + 1);
                                    foreach (string file in Directory.GetFiles(directory, $"*.{extension}", SearchOption.AllDirectories))
                                    {
                                        string relativePath = file.Substring(directory.Length + 1); // 获取相对路径
                                        string destFolder = Path.Combine(destinationFolder, Path.GetDirectoryName(relativePath)); // 目标目录结构
                                        string destFile = Path.Combine(destFolder, Path.GetFileName(file) + currentDate + Path.GetExtension(file)); // 文件名后添加日期
                                        Directory.CreateDirectory(destFolder);
                                        File.Copy(file, destFile, config.Command_PcoCopy_CoverFile);
                                        args.Player.SendSuccessMessage($"文件'{Path.GetFileName(file)}' 已复制到 '{destFile}'");
                                    }
                                }
                                else
                                {
                                    args.Player.SendInfoMessage($"路径'{v}' 既不是有效文件也不是目录，或指定的后缀名复制规则不适用。");
                                }
                            }
                        }
                        else
                        {
                            args.Player.SendInfoMessage("你没有指定要复制的文件或文件夹，请在配置文件中设置。");
                        }
                    }
                    catch (Exception ex)
                    {
                        TSPlayer.All.SendErrorMessage("使用指令复制文件时发生错误：" + ex.ToString());
                        TShock.Log.Error("使用指令复制文件时发生错误：" + ex.ToString());
                        Console.WriteLine("使用指令复制文件时发生错误：" + ex.ToString());
                    }
                }
                else if (args.Parameters[0].Equals("mess", StringComparison.OrdinalIgnoreCase) || args.Parameters[0].Equals("view", StringComparison.OrdinalIgnoreCase))
                {
                    if (!args.Player.HasPermission(p_admin) && !args.Player.HasPermission(p_npc) && !args.Player.HasPermission(p_com) && !args.Player.HasPermission(p_reload) && !args.Player.HasPermission(p_reset))
                    {
                        args.Player.SendInfoMessage($"权限不足！至少拥有四种计划中一个的控制权限或[{p_admin}]");
                    }
                    else
                    {
                        //3个自动化设置的信息
                        string clock_npc, mess_npc = "", clock_reset, mess_reset = "", clock_reload, mess_reload = "", clock_com, mess_com = "";


                        #region NPC进度数据输出
                        clock_npc = args.Player.IsLoggedIn ? "[i:3868]" + MtoM("NPC进度控制计划：", "28FFB8") : "@NPC进度控制计划：";
                        if (config.OpenAutoControlProgressLock)
                        {
                            double lv = (DateTime.Now - config.StartServerDate).TotalHours;
                            Dictionary<string, double> keyValuePairs = new Dictionary<string, double>();
                            Dictionary<string, double> sortpairs = new Dictionary<string, double>();
                            foreach (var v in config.ProgressLockTimeForStartServerDate)
                            {
                                keyValuePairs.Add(v.Key, v.Value);
                            }
                            //排序
                            while (keyValuePairs.Count > 0)
                            {
                                double min = double.MaxValue;
                                string key = "";
                                foreach (var v in keyValuePairs)
                                {
                                    if (v.Value < min)
                                    {
                                        key = v.Key;
                                        min = v.Value;
                                    }
                                }
                                if (key != "")
                                {
                                    sortpairs.Add(key, min);
                                    keyValuePairs.Remove(key);
                                }
                            }
                            //把排好序的数据输出
                            int count = 0;
                            foreach (var v in sortpairs)
                            {
                                if (v.Value >= lv)
                                {
                                    count++;
                                    if (args.Player.IsLoggedIn)
                                        mess_npc += $"[{v.Key}{HoursToM(v.Value - lv, "28FFB8")}] ";
                                    else
                                        mess_npc += $"[{v.Key}{HoursToM(v.Value - lv)}] ";
                                    if (count == 4)
                                    {
                                        mess_npc += "\n";
                                        count = 0;
                                    }
                                }
                            }
                            mess_npc = mess_npc.Trim('\n');
                            mess_npc += "\n";
                            //对npc封禁进行整理*******************************************************************
                            Dictionary<int, double> keyValuePairsnpc = new Dictionary<int, double>();
                            Dictionary<int, double> sortpairsnpc = new Dictionary<int, double>();
                            foreach (var v in config.CustomNPCIDLockTimeForStartServerDate)
                            {
                                keyValuePairsnpc.Add(v.Key, v.Value);
                            }
                            //排序
                            while (keyValuePairsnpc.Count > 0)
                            {
                                double min = double.MaxValue;
                                int key = -114514;
                                foreach (var v in keyValuePairsnpc)
                                {
                                    if (v.Value < min)
                                    {
                                        key = v.Key;
                                        min = v.Value;
                                    }
                                }
                                if (key != -114514)
                                {
                                    sortpairsnpc.Add(key, min);
                                    keyValuePairsnpc.Remove(key);
                                }
                            }
                            //把排好序的数据输出
                            count = 0;
                            foreach (var v in sortpairsnpc)
                            {
                                if (v.Value >= lv)
                                {
                                    count++;
                                    if (args.Player.IsLoggedIn)
                                        mess_npc += $"[({v.Key}){Lang.GetNPCNameValue(v.Key)}{HoursToM(v.Value - lv, "28FFB8")}] ";
                                    else
                                        mess_npc += $"[({v.Key}){Lang.GetNPCNameValue(v.Key)}{HoursToM(v.Value - lv)}] ";
                                    if (count == 5)
                                    {
                                        mess_npc += "\n";
                                        count = 0;
                                    }
                                }
                            }
                            if (string.IsNullOrWhiteSpace(mess_npc))
                                clock_npc += args.Player.IsLoggedIn ? MtoM("结束", "FFFFFF") : "结束";
                        }
                        else
                            clock_npc += args.Player.IsLoggedIn ? MtoM("未", "FFFFFF") + "开启" : "未开启";
                        mess_npc = mess_npc.Trim('\n');
                        #endregion


                        #region 重置执行信息
                        clock_reset = args.Player.IsLoggedIn ? "[i:3099]" + MtoM("重置计划：", "EA00FF") : "@重置计划：";
                        if (!config.OpenAutoReset && !countdownReset.enable)
                        {
                            if (args.Player.IsLoggedIn)
                                clock_reset += MtoM("未", "FFFFFF") + $"开启 Time:{config.HowLongTimeOfAotuResetServer:0.00}";
                            else
                                clock_reset += $"未开启 Time:{config.HowLongTimeOfAotuResetServer:0.00}";
                        }
                        else if (config.OpenAutoReset && !countdownReset.enable)
                            mess_reset = $"世界将在{HoursToM((config.StartServerDate.AddHours(config.HowLongTimeOfAotuResetServer) - DateTime.Now).TotalHours, (args.Player.IsLoggedIn ? "EA00FF" : ""))}后开始自动重置\n";
                        else
                            mess_reset = $"世界将在{HoursToM(countdownReset.time * 1.0 / 3600, (args.Player.IsLoggedIn ? "EA00FF" : ""))}后开始手动重置\n";
                        if (config.OpenAutoReset || countdownReset.enable)
                        {
                            if (args.Player.IsLoggedIn)
                                clock_reset += MtoM("已", "FFFFFF") + $"开启 Time:{config.HowLongTimeOfAotuResetServer:0.00}";
                            else
                                clock_reset += $"已开启 Time:{config.HowLongTimeOfAotuResetServer:0.00}";
                        }

                        string size;
                        if (config.MapSizeForAfterReset == 1)
                            size = "小";
                        else if (config.MapSizeForAfterReset == 2)
                            size = "中";
                        else if (config.MapSizeForAfterReset == 3)
                            size = "大";
                        else
                            size = "错误，请检查数据填写是否有误";
                        string mode;
                        if (config.MapDifficultyForAfterReset == 0)
                            mode = "普通";
                        else if (config.MapDifficultyForAfterReset == 1)
                            mode = "专家";
                        else if (config.MapDifficultyForAfterReset == 2)
                            mode = "大师";
                        else if (config.MapDifficultyForAfterReset == 3)
                            mode = "旅途";
                        else
                            mode = "错误，请检查数据填写是否有误";

                        //自动生成地图还是挑选备用地图
                        if (config.ExpectedUsageWorldFileNameForAotuReset.Count > 0 && ExistWorldNamePlus(config.ExpectedUsageWorldFileNameForAotuReset.First(), out string world) && !(world == Main.worldName && config.DeleteWorldForReset))
                        {
                            mess_reset += $"使用提供的地图：{world}，最多在线人数：{config.AfterResetPeople}，端口：{config.AfterResetPort}，是否重置玩家数据：{config.ResetTSCharacter}，服务器密码：{config.AfterResetServerPassword}，自动删图：{config.DeleteWorldForReset}";
                        }
                        else
                        {
                            string temp;
                            if (config.ExpectedUsageWorldFileNameForAotuReset.Count > 0)
                            {
                                if (!config.DeleteWorldForReset)
                                    temp = config.AddNumberFile(CorrectFileName(config.ExpectedUsageWorldFileNameForAotuReset.First()));
                                else
                                    temp = config.AddNumberFile(CorrectFileName(config.ExpectedUsageWorldFileNameForAotuReset.First()), Main.worldName);
                            }
                            else
                            {
                                if (!config.DeleteWorldForReset)
                                    temp = config.AddNumberFile(CorrectFileName(config.WorldNameForAfterReset));
                                else
                                    temp = config.AddNumberFile(CorrectFileName(config.WorldNameForAfterReset), Main.worldName);
                            }
                            mess_reset +=
                            $"生成地图名称：{temp}，地图大小：{size}，模式：{mode}，种子：{(string.IsNullOrWhiteSpace(config.WorldSeedForAfterReset) ? "随机" : config.WorldSeedForAfterReset)}，最多在线人数：{config.AfterResetPeople}" +
                            $"，端口：{config.AfterResetPort}，服务器密码：{(string.IsNullOrWhiteSpace(config.AfterResetServerPassword) ? "无" : config.AfterResetServerPassword)}，是否重置玩家数据：{config.ResetTSCharacter}，自动删图：{config.DeleteWorldForReset}";
                        }
                        #endregion


                        #region 重启执行信息
                        clock_reload = args.Player.IsLoggedIn ? "[i:17]" + MtoM("重启计划：", "FF9000") : "@重启计划：";
                        if (!config.AutoRestartServer && !countdownRestart.enable)
                        {
                            if (args.Player.IsLoggedIn)
                                clock_reload += MtoM("未", "FFFFFF") + $"开启 Time:{config.HowLongTimeOfRestartServer:0.00}";
                            else
                                clock_reload += $"未开启 Time:{config.HowLongTimeOfRestartServer:0.00}";
                        }
                        else if (config.AutoRestartServer && !countdownRestart.enable)
                            mess_reload = $"服务器将在{HoursToM((config.LasetServerRestartDate.AddHours(config.HowLongTimeOfRestartServer) - DateTime.Now).TotalHours, (args.Player.IsLoggedIn ? "FF9000" : ""))}后开始自动重启\n";
                        else
                            mess_reload = $"服务器将在{HoursToM(countdownRestart.time * 1.0 / 3600, (args.Player.IsLoggedIn ? "FF9000" : ""))}后开始手动重启\n";
                        if (config.AutoRestartServer || countdownRestart.enable)
                        {
                            if (args.Player.IsLoggedIn)
                                clock_reload += MtoM("已", "FFFFFF") + $"开启 Time:{config.HowLongTimeOfRestartServer:0.00}";
                            else
                                clock_reload += $"已开启 Time:{config.HowLongTimeOfRestartServer:0.00}";
                        }

                        mess_reload += $"地图名称：{Main.worldName}，最多在线人数：{config.AfterRestartPeople}，端口：{config.AfterRestartPort}，服务器密码：{(string.IsNullOrWhiteSpace(config.AfterRestartServerPassword) ? "无" : config.AfterRestartServerPassword)}";
                        #endregion


                        #region 指令执行信息
                        clock_com = args.Player.IsLoggedIn ? "[i:903]" + MtoM("指令计划：", "00A8FF") : "@指令计划：";
                        if (!config.OpenAutoCommand && !countdownCom.enable)
                        {
                            if (args.Player.IsLoggedIn)
                                clock_com += MtoM("未", "FFFFFF") + $"开启 Time:{config.HowLongTimeOfAutoCommand:0.00}";
                            else
                                clock_com += $"未开启 Time:{config.HowLongTimeOfAutoCommand:0.00}";
                        }
                        else if (config.OpenAutoCommand && !countdownCom.enable)
                            mess_com = $"服务器将在{HoursToM((config.LasetAutoCommandDate.AddHours(config.HowLongTimeOfAutoCommand) - DateTime.Now).TotalHours, (args.Player.IsLoggedIn ? "00A8FF" : ""))}后开始自动执行指令\n";
                        else
                            mess_com = $"服务器将在{HoursToM(countdownCom.time * 1.0 / 3600, (args.Player.IsLoggedIn ? "00A8FF" : ""))}后开始手动执行指令\n";
                        if (config.OpenAutoCommand || countdownCom.enable)
                        {
                            if (args.Player.IsLoggedIn)
                                clock_com += MtoM("已", "FFFFFF") + $"开启 Time:{config.HowLongTimeOfAutoCommand:0.00}";
                            else
                                clock_com += $"已开启 Time:{config.HowLongTimeOfAutoCommand:0.00}";
                        }

                        mess_com += "执行时广播通告：" + config.AutoCommandOfBroadcast + "，要执行的指令：";
                        string mess_com_temp = "";
                        int count_com = 1;
                        foreach (string v in config.AutoCommandList)
                        {
                            if (!string.IsNullOrWhiteSpace(v))
                            {
                                string t = v;
                                while (t.EndsWith(' ') || t.EndsWith('/') || t.EndsWith('.') || t.StartsWith(' ') || t.StartsWith('/') || t.StartsWith('.') || t.Contains("  "))
                                {
                                    t = t.Trim(' ', '/', '.');
                                    t = t.Replace("  ", " ");
                                }
                                mess_com_temp += "/" + t + ", ";
                                if (count_com == 10)
                                {
                                    mess_com_temp += "\n";
                                    count_com = 0;
                                }
                                count_com++;
                            }
                        }
                        while (mess_com_temp.EndsWith('\n') || mess_com_temp.EndsWith(',') || mess_com_temp.EndsWith(' '))
                            mess_com_temp = mess_com_temp.TrimEnd('\n', ',', ' ');
                        if (string.IsNullOrWhiteSpace(mess_com_temp))
                            mess_com += "无";
                        else
                            mess_com += mess_com_temp;
                        #endregion


                        args.Player.SendInfoMessage(
                            //npc封禁计划
                            $"{clock_npc}\n{(string.IsNullOrWhiteSpace(mess_npc) ? "" : mess_npc + "\n")}" +
                            //自动重置计划
                            $"{clock_reset}\n{mess_reset}\n" +
                            //自动重启计划
                            $"{clock_reload}\n{mess_reload}\n" +
                            //自动指令计划
                            $"{clock_com}\n{mess_com}");
                    }
                }
                else if (args.Parameters[0].Equals("npc", StringComparison.OrdinalIgnoreCase))
                    args.Player.SendInfoMessage("输入 /pco npc help   来查看NPC封禁计划的帮助指令");
                else if (args.Parameters[0].Equals("com", StringComparison.OrdinalIgnoreCase))
                    args.Player.SendInfoMessage("输入 /pco com help   来查看执行指令计划的帮助指令");
                else if (args.Parameters[0].Equals("reload", StringComparison.OrdinalIgnoreCase))
                    args.Player.SendInfoMessage("输入 /pco reload help   来查看重启计划的帮助指令");
                else if (args.Parameters[0].Equals("reset", StringComparison.OrdinalIgnoreCase))
                    args.Player.SendInfoMessage("输入 /pco reset help   来查看重置计划的帮助指令");
                else
                    args.Player.SendInfoMessage("输入 /pco help 来获取该插件的帮助");
                return;
            }

            //参数 >= 2  *******************************************************************************************************
            /// npc 指令
            if (args.Parameters[0].Equals("npc", StringComparison.OrdinalIgnoreCase))
            {
                if (!args.Player.HasPermission(p_npc) && !args.Player.HasPermission(p_admin))
                {
                    args.Player.SendInfoMessage($"权限不足！[{p_npc}]或[{p_admin}]");
                    return;
                }
                if (args.Parameters.Count == 2)
                {
                    if (args.Parameters[1].Equals("help", StringComparison.OrdinalIgnoreCase))
                    {
                        args.Player.SendMessage(
                        "输入 /pco npc act   自动控制NPC进度计划启用，再次使用则关闭\n" +
                        "输入 /pco npc os <±num/±H:M:S>   来将自动控制NPC的全体(包括Boss)解锁时刻推迟或提前num时或H时M分S秒，num可为小数\n" +
                        "输入 /pco npc os <id/name> <±num/±H:M:S>   来将自动控制某个NPC(包括Boss)解锁时刻推迟或提前num时或H时M分S秒，num可为小数\n" +
                        "输入 /pco npc add <id/name> <num/H:M:S>   来添加或更新一个NPC(不包括Boss)的封禁限制，参数格式同上，但不为负\n" +
                        "输入 /pco npc del <id/name>   来删除一个NPC(不包括Boss)的封禁限制", TextColor());
                    }
                    else if (args.Parameters[1].Equals("act", StringComparison.OrdinalIgnoreCase))
                    {
                        if (config.OpenAutoControlProgressLock)
                            args.Player.SendSuccessMessage("已取消NPC的封禁限制计划");
                        else
                            args.Player.SendSuccessMessage("已开启NPC的封禁限制计划");
                        config.OpenAutoControlProgressLock = !config.OpenAutoControlProgressLock;
                        config.SaveConfigFile();
                    }
                    else
                        args.Player.SendInfoMessage("输入 /pco npc help   来查看NPC封禁计划的帮助指令");
                }
                else
                {
                    if (args.Parameters[1].Equals("os", StringComparison.OrdinalIgnoreCase) || args.Parameters[1].Equals("offset", StringComparison.OrdinalIgnoreCase))
                    {
                        if (args.Parameters.Count == 3)
                        {
                            double addtime = HMSorHoursToHours(args.Parameters[2], out string reason);
                            if (reason != "")
                            {
                                args.Player.SendInfoMessage(reason);
                                return;
                            }
                            string[] keys = config.ProgressLockTimeForStartServerDate.Keys.ToArray();
                            foreach (var x in keys)
                            {
                                config.ProgressLockTimeForStartServerDate[x] += addtime;
                            }
                            int[] keys2 = config.CustomNPCIDLockTimeForStartServerDate.Keys.ToArray();
                            foreach (var x in keys2)
                            {
                                config.CustomNPCIDLockTimeForStartServerDate[x] += addtime;
                            }
                            config.SaveConfigFile();
                            string st;
                            if (addtime > 0)
                                st = "推迟" + (args.Player.IsLoggedIn ? HoursToM(addtime, "28FFB8") : HoursToM(addtime));
                            else if (addtime < 0)
                                st = "提前" + (args.Player.IsLoggedIn ? HoursToM(-1 * addtime, "28FFB8") : HoursToM(-1 * addtime));
                            else
                                st = "正常";
                            if (!config.OpenAutoControlProgressLock)
                            {
                                args.Player.SendWarningMessage("警告，未开启自动控制NPC进度计划，你的修改不会有任何效果");
                                args.Player.SendSuccessMessage($"定位成功，NPC将{st}解锁");
                            }
                            else
                                SendMessageAllAndMe(args.Player, $"定位成功，NPC将{st}解锁", Color.LimeGreen);
                        }
                        else if (args.Parameters.Count == 4)
                        {
                            double addtime = HMSorHoursToHours(args.Parameters[3], out string reason);
                            if (reason != "")
                            {
                                args.Player.SendInfoMessage(reason);
                                return;
                            }
                            var keypairs = FindNPCNameAndIDByNetid(args.Parameters[2]);
                            if (keypairs.Count == 0)
                            {
                                args.Player.SendInfoMessage("未找到该生物");
                            }
                            else if (keypairs.Count == 1)
                            {
                                int id = keypairs.First().Key;
                                string name = keypairs.First().Value;
                                if (config.CustomNPCIDLockTimeForStartServerDate.ContainsKey(id))
                                {
                                    config.CustomNPCIDLockTimeForStartServerDate[id] += addtime;
                                    config.SaveConfigFile();
                                    string st;
                                    if (addtime > 0)
                                        st = "推迟" + (args.Player.IsLoggedIn ? HoursToM(addtime, "28FFB8") : HoursToM(addtime));
                                    else if (addtime < 0)
                                        st = "提前" + (args.Player.IsLoggedIn ? HoursToM(-1 * addtime, "28FFB8") : HoursToM(-1 * addtime));
                                    else
                                        st = "正常";
                                    if (!config.OpenAutoControlProgressLock)
                                    {
                                        args.Player.SendWarningMessage("警告，未开启自动控制NPC进度计划，你的修改不会有任何效果");
                                        args.Player.SendSuccessMessage($"定位成功，{name}将{st}解锁");
                                    }
                                    else
                                        SendMessageAllAndMe(args.Player, $"定位成功，{name}将{st}解锁", Color.LimeGreen);
                                }
                                else
                                {
                                    switch (id)
                                    {
                                        case NPCID.KingSlime:
                                            Function(args.Player, "史莱姆王", addtime);
                                            break;
                                        case NPCID.EyeofCthulhu:
                                            Function(args.Player, "克苏鲁之眼", addtime);
                                            break;
                                        case NPCID.EaterofWorldsHead:
                                        case NPCID.EaterofWorldsBody:
                                        case NPCID.EaterofWorldsTail:
                                            Function(args.Player, "世界吞噬者", addtime);
                                            break;
                                        case NPCID.BrainofCthulhu:
                                            Function(args.Player, "克苏鲁之脑", addtime);
                                            break;
                                        case NPCID.QueenBee:
                                            Function(args.Player, "蜂后", addtime);
                                            break;
                                        case NPCID.Deerclops:
                                            Function(args.Player, "巨鹿", addtime);
                                            break;
                                        case NPCID.SkeletronHead:
                                        case NPCID.SkeletronHand:
                                            Function(args.Player, "骷髅王", addtime);
                                            break;
                                        case NPCID.WallofFlesh:
                                        case NPCID.WallofFleshEye:
                                        case NPCID.TheHungry:
                                        case NPCID.TheHungryII:
                                            Function(args.Player, "血肉墙", addtime);
                                            break;
                                        case NPCID.QueenSlimeBoss:
                                            Function(args.Player, "史莱姆皇后", addtime);
                                            break;
                                        case 125://双子
                                        case 126:
                                            Function(args.Player, "双子魔眼", addtime);
                                            break;
                                        case NPCID.TheDestroyer:
                                        case NPCID.TheDestroyerBody:
                                        case NPCID.TheDestroyerTail:
                                        case NPCID.Probe:
                                            Function(args.Player, "毁灭者", addtime);
                                            break;
                                        case 127:
                                        case 128:
                                        case 129:
                                        case 130:
                                        case 131:
                                            Function(args.Player, "机械骷髅王", addtime);
                                            break;
                                        case NPCID.Plantera:
                                        case NPCID.PlanterasTentacle:
                                            Function(args.Player, "世纪之花", addtime);
                                            break;
                                        case NPCID.GolemFistLeft:
                                        case NPCID.GolemFistRight:
                                        case NPCID.Golem:
                                        case NPCID.GolemHead:
                                            Function(args.Player, "石巨人", addtime);
                                            break;
                                        case NPCID.DukeFishron:
                                            Function(args.Player, "猪龙鱼公爵", addtime);
                                            break;
                                        case NPCID.HallowBoss:
                                            Function(args.Player, "光之女皇", addtime);
                                            break;
                                        case NPCID.CultistBoss:
                                            Function(args.Player, "拜月教教徒", addtime);
                                            break;
                                        case NPCID.LunarTowerSolar:
                                        case NPCID.LunarTowerVortex:
                                        case NPCID.LunarTowerStardust:
                                        case NPCID.LunarTowerNebula:
                                            Function(args.Player, "四柱", addtime);
                                            break;
                                        case NPCID.MoonLordCore:
                                        case NPCID.MoonLordHead:
                                        case NPCID.MoonLordHand:
                                        case NPCID.MoonLordLeechBlob:
                                            Function(args.Player, "月亮领主", addtime);
                                            break;
                                        default:
                                            args.Player.SendInfoMessage($"修改失败，没有{name}的封禁计划");
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                int cout = 0;
                                string text = "目标过多，你想查找的是？\n";
                                foreach (var v in keypairs)
                                {
                                    cout++;
                                    if (cout == 10)
                                    {
                                        text += $"[{v.Value}:{v.Key}], \n";
                                        cout = 0;
                                    }
                                    else
                                        text += $"[{v.Value}:{v.Key}], ";
                                }
                                while (text.EndsWith('\n') || text.EndsWith(' ') || text.EndsWith(','))
                                {
                                    text = text.TrimEnd('\n', ' ', ',');
                                }
                                args.Player.SendInfoMessage(text);
                            }
                        }
                        else
                            args.Player.SendInfoMessage("输入 /pco npc help   来查看NPC封禁计划的帮助指令");
                    }
                    else if (args.Parameters[1].Equals("add", StringComparison.OrdinalIgnoreCase))
                    {
                        if (args.Parameters.Count != 4)
                        {
                            args.Player.SendInfoMessage("缺少封禁时长的参数");
                            return;
                        }
                        var npcs = FindNPCNameAndIDByNetid(args.Parameters[2]);
                        if (npcs.Count > 1)
                        {
                            int cout = 0;
                            string text = "目标过多，你想查找的是？\n";
                            foreach (var v in npcs)
                            {
                                cout++;
                                if (cout == 10)
                                {
                                    text += $"[{v.Value}:{v.Key}], \n";
                                    cout = 0;
                                }
                                else
                                    text += $"[{v.Value}:{v.Key}], ";
                            }
                            while (text.EndsWith('\n') || text.EndsWith(' ') || text.EndsWith(','))
                            {
                                text = text.TrimEnd('\n', ' ', ',');
                            }
                            args.Player.SendInfoMessage(text);
                        }
                        else if (npcs.Count == 1)
                        {
                            double num = HMSorHoursToHours(args.Parameters[3], out string reason);
                            if (reason != "")
                            {
                                args.Player.SendInfoMessage(reason);
                                return;
                            }
                            if (config.CustomNPCIDLockTimeForStartServerDate.TryGetValue(npcs.First().Key, out double temp))
                            {
                                config.CustomNPCIDLockTimeForStartServerDate[npcs.First().Key] = num;
                                double d = (config.StartServerDate.AddHours(num) - DateTime.Now).TotalHours;
                                if (d > 0)
                                    args.Player.SendSuccessMessage($"NPC:{npcs.First().Value} 已更新成功，将在从现在起{HoursToM(d, (args.Player.IsLoggedIn ? "28FFB8" : ""))}后解锁");
                                else
                                    args.Player.SendSuccessMessage($"NPC:{npcs.First().Value} 已更新成功，且目前已解锁");
                            }
                            else
                            {
                                config.CustomNPCIDLockTimeForStartServerDate.Add(npcs.First().Key, num);
                                double d = (config.StartServerDate.AddHours(num) - DateTime.Now).TotalHours;
                                if (d > 0)
                                    args.Player.SendSuccessMessage($"NPC:{npcs.First().Value} 添加成功，将在从现在起{HoursToM(d, (args.Player.IsLoggedIn ? "28FFB8" : ""))}后解锁");
                                else
                                    args.Player.SendSuccessMessage($"NPC:{npcs.First().Value} 添加成功，且目前已解锁");
                            }
                            config.SaveConfigFile();
                        }
                        else
                            args.Player.SendInfoMessage("未找到该生物");
                    }
                    else if (args.Parameters[1].Equals("del", StringComparison.OrdinalIgnoreCase))
                    {
                        var npcs = FindNPCNameAndIDByNetid(args.Parameters[2]);
                        if (npcs.Count == 1)
                        {
                            if (config.CustomNPCIDLockTimeForStartServerDate.Remove(npcs.First().Key))
                            {
                                config.SaveConfigFile();
                                args.Player.SendSuccessMessage($"NPC:{npcs.First().Value} 已删除成功");
                            }
                            else
                            {
                                args.Player.SendSuccessMessage($"NPC:{npcs.First().Value} 未在计划中找到，删除失败");
                            }
                        }
                        else if (npcs.Count > 1)
                        {
                            int cout = 0;
                            string text = "目标过多，你想查找的是？\n";
                            foreach (var v in npcs)
                            {
                                cout++;
                                if (cout == 10)
                                {
                                    text += $"[{v.Value}:{v.Key}], \n";
                                    cout = 0;
                                }
                                else
                                    text += $"[{v.Value}:{v.Key}], ";
                            }
                            while (text.EndsWith('\n') || text.EndsWith(' ') || text.EndsWith(','))
                            {
                                text = text.TrimEnd('\n', ' ', ',');
                            }
                            args.Player.SendInfoMessage(text);
                        }
                        else
                            args.Player.SendInfoMessage("未找到该生物");
                    }
                    else
                        args.Player.SendInfoMessage("输入 /pco npc help   来查看NPC封禁计划的帮助指令");
                }
            }
            /// com 指令
            else if (args.Parameters[0].Equals("com", StringComparison.OrdinalIgnoreCase))
            {
                if (!args.Player.HasPermission(p_com) && !args.Player.HasPermission(p_admin))
                {
                    args.Player.SendInfoMessage($"权限不足！[{p_com}]或[{p_admin}]");
                    return;
                }
                if (args.Parameters.Count == 2)
                {
                    if (args.Parameters[1].Equals("help", StringComparison.OrdinalIgnoreCase))
                    {
                        args.Player.SendMessage(
                            "输入 /pco com act   自动执行指令计划启用，再次使用关闭\n" +
                            "输入 /pco com os <±num/±H:M:S>   将自动执行指令的时间推迟或提前num时或H时M分S秒，num可为小数\n" +
                            "输入 /pco com hand <±num/±H:M:S>   手动执行指令计划启用，在num时或H时M分S秒后开始执行，若不填则立刻执行，小于0则关闭当前存在的手动计划，其优先级大于自动执行指令\n" +
                            "输入 /pco com add <string>   在计划里添加一个要执行的指令\n" +
                            "输入 /pco com del <string>   在计划里删除一个要执行的指令\n" +
                            "输入 /pco com bc   执行指令时向游戏内发送广播，再次使用关闭\n" +
                            "输入 /pco com stop   关闭手动执行指令计划", TextColor());
                    }
                    else if (args.Parameters[1].Equals("act", StringComparison.OrdinalIgnoreCase))
                    {
                        if (config.CheckPerm)
                        {
                            if (!args.Player.HasPermission(p_admin))
                            {
                                foreach (var x in config.AutoCommandList)
                                {
                                    if (CorrectCommand(x).StartsWith("pco npc", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_npc))
                                    {
                                        args.Player.SendInfoMessage($"权限不足[{p_npc}]，不能调整含有NPC计划系列指令的计划");
                                        return;
                                    }
                                    if (CorrectCommand(x).StartsWith("pco reload", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_reload))
                                    {
                                        args.Player.SendInfoMessage($"权限不足[{p_reload}]，不能调整含有重启计划系列指令的计划");
                                        return;
                                    }
                                    if (CorrectCommand(x).StartsWith("pco reset", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_reset))
                                    {
                                        args.Player.SendInfoMessage($"权限不足[{p_reset}]，不能调整含有重置计划系列指令的计划");
                                        return;
                                    }
                                }
                            }
                            //禁止有人通过这个指令来越权使用别的指令
                            foreach (var x in config.AutoCommandList)
                            {
                                bool can = true;
                                foreach (var v in getAllComCannotRun(args.Player))
                                {
                                    if ((x + " ").StartsWith(v + " "))
                                    {
                                        can = false; break;
                                    }
                                }
                                if (!can)
                                {
                                    args.Player.SendInfoMessage($"你的权限不足以涵盖该指令：/{x}，请不要试图越权");
                                    return;
                                }
                            }
                        }
                        if (config.OpenAutoCommand)
                        {
                            SendMessageAllAndMe(args.Player, "已取消自动执行指令的计划", Color.LimeGreen);
                        }
                        else
                        {
                            if (config.HowLongTimeOfAutoCommand < 0)
                            {
                                args.Player.SendInfoMessage("自动执行指令倒计时必须大于等于0，请使用指令 /pco com os 调整");
                                return;
                            }
                            if (config.LasetAutoCommandDate.AddHours(config.HowLongTimeOfAutoCommand) <= DateTime.Now.AddMinutes(1))
                            {
                                args.Player.SendInfoMessage("自动执行指令倒计时过短，可能即将开始自动执行指令");
                            }
                            if (!args.Player.IsLoggedIn)
                                args.Player.SendSuccessMessage($"自动执行指令计划已启用，将在从现在起{HoursToM((config.LasetAutoCommandDate.AddHours(config.HowLongTimeOfAutoCommand) - DateTime.Now).TotalHours)}后自动执行指令");
                            TSPlayer.All.SendSuccessMessage($"自动执行指令计划已启用，将在从现在起{HoursToM((config.LasetAutoCommandDate.AddHours(config.HowLongTimeOfAutoCommand) - DateTime.Now).TotalHours, "00A8FF")}后自动执行指令");
                        }
                        config.OpenAutoCommand = !config.OpenAutoCommand;
                        config.SaveConfigFile();
                    }
                    else if (args.Parameters[1].Equals("hand", StringComparison.OrdinalIgnoreCase))
                    {
                        if (config.CheckPerm)
                        {
                            if (!args.Player.HasPermission(p_admin))
                            {
                                foreach (var x in config.AutoCommandList)
                                {
                                    if (CorrectCommand(x).StartsWith("pco npc", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_npc))
                                    {
                                        args.Player.SendInfoMessage($"权限不足[{p_npc}]，不能发起含有NPC计划系列指令的计划");
                                        return;
                                    }
                                    if (CorrectCommand(x).StartsWith("pco reload", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_reload))
                                    {
                                        args.Player.SendInfoMessage($"权限不足[{p_reload}]，不能发起含有重启计划系列指令的计划");
                                        return;
                                    }
                                    if (CorrectCommand(x).StartsWith("pco reset", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_reset))
                                    {
                                        args.Player.SendInfoMessage($"权限不足[{p_reset}]，不能发起含有重置计划系列指令的计划");
                                        return;
                                    }
                                }
                            }
                            //禁止有人通过这个指令来越权使用别的指令
                            foreach (var x in config.AutoCommandList)
                            {
                                bool can = true;
                                foreach (var v in getAllComCannotRun(args.Player))
                                {
                                    if ((x + " ").StartsWith(v + " "))
                                    {
                                        can = false; break;
                                    }
                                }
                                if (!can)
                                {
                                    args.Player.SendInfoMessage($"你的权限不足以涵盖该指令：/{x}，请不要试图越权");
                                    return;
                                }
                            }
                        }
                        ActiveCommands();
                        if (!args.Player.IsLoggedIn && args.Player.Name != "Server")
                            args.Player.SendSuccessMessage("服务器自动执行指令成功");
                    }
                    else if (args.Parameters[1].Equals("bc", StringComparison.OrdinalIgnoreCase))
                    {
                        if (config.AutoCommandOfBroadcast)
                            args.Player.SendSuccessMessage("执行指令时的通知广播已关闭");
                        else
                            args.Player.SendSuccessMessage("执行指令时的通知广播已开启");
                        config.AutoCommandOfBroadcast = !config.AutoCommandOfBroadcast;
                        config.SaveConfigFile();
                    }
                    else if (args.Parameters[1].Equals("stop", StringComparison.OrdinalIgnoreCase))
                    {
                        if (countdownCom.enable || thread_com.IsAlive)
                        {
                            countdownCom.enable = false;
                            countdownCom.time = 0;
                            args.Player.SendSuccessMessage("手动执行指令计划已关闭");
                        }
                        else
                            args.Player.SendSuccessMessage("手动执行指令计划未开启，无需关闭");
                    }
                    else
                        args.Player.SendInfoMessage("输入 /pco com help   来查看指令计划的帮助指令");
                }
                else
                {
                    if (args.Parameters[1].Equals("os", StringComparison.OrdinalIgnoreCase) || args.Parameters[1].Equals("offset", StringComparison.OrdinalIgnoreCase))
                    {
                        if (config.CheckPerm)
                        {
                            if (!args.Player.HasPermission(p_admin))
                            {
                                foreach (var x in config.AutoCommandList)
                                {
                                    if (CorrectCommand(x).StartsWith("pco npc", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_npc))
                                    {
                                        args.Player.SendInfoMessage($"权限不足[{p_npc}]，不能调整含有NPC计划系列指令的计划");
                                        return;
                                    }
                                    if (CorrectCommand(x).StartsWith("pco reload", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_reload))
                                    {
                                        args.Player.SendInfoMessage($"权限不足[{p_reload}]，不能调整含有重启计划系列指令的计划");
                                        return;
                                    }
                                    if (CorrectCommand(x).StartsWith("pco reset", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_reset))
                                    {
                                        args.Player.SendInfoMessage($"权限不足[{p_reset}]，不能调整含有重置计划系列指令的计划");
                                        return;
                                    }
                                }
                            }
                            foreach (var x in config.AutoCommandList)
                            {
                                bool can = true;
                                //禁止有人通过这个指令来越权使用别的指令
                                foreach (var v in getAllComCannotRun(args.Player))
                                {
                                    if ((x + " ").StartsWith(v + " "))
                                    {
                                        can = false; break;
                                    }
                                }
                                if (!can)
                                {
                                    args.Player.SendInfoMessage($"你的权限不足以涵盖该指令：/{x}，请不要试图越权");
                                    return;
                                }
                            }
                        }
                        double addtime = HMSorHoursToHours(args.Parameters[2], out string reason);
                        if (reason != "")
                        {
                            args.Player.SendInfoMessage(reason);
                            return;
                        }
                        config.HowLongTimeOfAutoCommand += addtime;
                        if (config.HowLongTimeOfAutoCommand < 0)
                            config.HowLongTimeOfAutoCommand = 0;
                        config.SaveConfigFile();
                        string st;
                        if (addtime > 0)
                            st = "推迟" + (args.Player.IsLoggedIn ? HoursToM(addtime, "00A8FF") : HoursToM(addtime));
                        else if (addtime < 0)
                            st = "提前" + (args.Player.IsLoggedIn ? HoursToM(-1 * addtime, "00A8FF") : HoursToM(-1 * addtime));
                        else
                            st = "时间不变";
                        double h = (config.LasetAutoCommandDate.AddHours(config.HowLongTimeOfAutoCommand) - DateTime.Now).TotalHours;
                        if (!config.OpenAutoCommand)
                            args.Player.SendWarningMessage("警告，未开启自动执行指令的计划，你的修改不会有任何效果");
                        else
                            TSPlayer.All.SendInfoMessage("自动执行指令计划已修改");
                        if (!args.Player.IsLoggedIn)
                            args.Player.SendSuccessMessage($"定位成功，下次自动执行指令将{st}，" + (h > 0 ? $"即从现在起{HoursToM(h)}后" : "你提前的太多了，指令将立刻执行"));
                        else
                            args.Player.SendSuccessMessage($"定位成功，下次自动执行指令将{st}，" + (h > 0 ? $"即从现在起{HoursToM(h, "00A8FF")}后" : "你提前的太多了，指令将立刻执行"));
                    }
                    else if (args.Parameters[1].Equals("hand", StringComparison.OrdinalIgnoreCase))
                    {
                        if (config.CheckPerm)
                        {
                            if (!args.Player.HasPermission(p_admin))
                            {
                                foreach (var x in config.AutoCommandList)
                                {
                                    if (CorrectCommand(x).StartsWith("pco npc", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_npc))
                                    {
                                        args.Player.SendInfoMessage($"权限不足[{p_npc}]，不能发起含有NPC计划系列指令的计划");
                                        return;
                                    }
                                    if (CorrectCommand(x).StartsWith("pco reload", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_reload))
                                    {
                                        args.Player.SendInfoMessage($"权限不足[{p_reload}]，不能发起含有重启计划系列指令的计划");
                                        return;
                                    }
                                    if (CorrectCommand(x).StartsWith("pco reset", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_reset))
                                    {
                                        args.Player.SendInfoMessage($"权限不足[{p_reset}]，不能发起含有重置计划系列指令的计划");
                                        return;
                                    }
                                }
                            }
                            //禁止有人通过这个指令来越权使用别的指令
                            foreach (var x in config.AutoCommandList)
                            {
                                bool can = true;
                                foreach (var v in getAllComCannotRun(args.Player))
                                {
                                    if ((x + " ").StartsWith(v + " "))
                                    {
                                        can = false; break;
                                    }
                                }
                                if (!can)
                                {
                                    args.Player.SendInfoMessage($"你的权限不足以涵盖该指令：/{x}，请不要试图越权");
                                    return;
                                }
                            }
                        }
                        double h = HMSorHoursToHours(args.Parameters[2], out string reason);
                        if (reason != "")
                        {
                            args.Player.SendInfoMessage(reason);
                            return;
                        }
                        int num = (int)(h * 3600);
                        if (num >= 0)
                        {
                            if (countdownRestart.enable && countdownRestart.time < num)
                            {
                                args.Player.SendWarningMessage("警告：当前存在手动重启计划，且时间小于你输入的数值，你的手动执行指令计划将不会生效！");
                            }
                            if (!countdownRestart.enable && config.AutoRestartServer && config.LasetServerRestartDate.AddHours(config.HowLongTimeOfRestartServer) < DateTime.Now.AddHours(h))
                            {
                                args.Player.SendWarningMessage("警告：当前存在自动重启计划，且时间小于你输入的数值，你的手动执行指令计划将不会生效！");
                            }
                            if (countdownReset.enable && countdownReset.time < num)
                            {
                                args.Player.SendWarningMessage("警告：当前存在手动重置计划，且时间小于你输入的数值，你的手动执行指令计划将不会生效！");
                            }
                            if (!countdownReset.enable && config.OpenAutoReset && config.StartServerDate.AddHours(config.HowLongTimeOfAotuResetServer) < DateTime.Now.AddHours(h))
                            {
                                args.Player.SendWarningMessage("警告：当前存在自动重置计划，且时间小于你输入的数值，你的手动执行指令计划将不会生效！");
                            }

                            if (countdownCom.enable && thread_com != null && thread_com.IsAlive)
                            {
                                countdownCom.time = num;
                                TSPlayer.All.SendMessage($"手动执行指令计划已重新开始，服务器将在{HoursToM(h, "00A8FF")}后执行指令", Color.Yellow);
                                if (!args.Player.IsLoggedIn)
                                    args.Player.SendInfoMessage($"手动执行指令计划已重新开始，服务器将在{HoursToM(h)}后执行指令");
                            }
                            else if (thread_com == null || !thread_com.IsAlive)
                            {
                                TSPlayer.All.SendMessage($"手动执行指令计划开启，服务器将在{HoursToM(h, "00A8FF")}后执行指令", Color.Yellow);
                                if (!args.Player.IsLoggedIn)
                                    args.Player.SendInfoMessage($"手动执行指令计划开启，服务器将在{HoursToM(h)}后执行指令");
                                countdownCom.enable = true;
                                countdownCom.time = num;
                                thread_com = new Thread(thread_comFun);
                                thread_com.IsBackground = true;
                                thread_com.Start();
                            }
                            else
                                args.Player.SendInfoMessage("计划时机不恰当，请1秒后重试");
                        }
                        else
                        {
                            if (countdownCom.enable)
                            {
                                countdownCom.enable = false;
                                countdownCom.time = 0;
                                SendMessageAllAndMe(args.Player, "手动执行指令计划已关闭", Color.LimeGreen);
                            }
                            else
                                args.Player.SendSuccessMessage("未开启手动执行指令计划");
                        }
                    }
                    else if (args.Parameters[1].Equals("add", StringComparison.OrdinalIgnoreCase))
                    {
                        string mess = "";
                        for (int i = 2; i < args.Parameters.Count; i++)
                        {
                            mess += args.Parameters[i] + " ";
                        }
                        mess = CorrectCommand(mess);
                        if (config.CheckPerm)
                        {
                            //禁止有人通过这个指令来越权使用别的指令
                            bool can = true;
                            foreach (var v in getAllComCannotRun(args.Player))
                            {
                                if ((mess + " ").StartsWith(v + " "))
                                {
                                    can = false; break;
                                }
                            }
                            if (!can)
                            {
                                args.Player.SendInfoMessage($"你的权限不足以涵盖该指令：/{mess}，请不要试图越权");
                                return;
                            }
                            //这几个权限被我写在方法里面特判了，所以这里也要特判
                            if (!args.Player.HasPermission(p_admin))
                            {
                                if (mess.StartsWith("pco npc", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_npc))
                                {
                                    args.Player.SendInfoMessage($"权限不足[{p_npc}]，不能添加含有NPC计划系列指令的计划");
                                    return;
                                }
                                if (mess.StartsWith("pco reload", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_reload))
                                {
                                    args.Player.SendInfoMessage($"权限不足[{p_reload}]，不能添加含有重启计划系列指令的计划");
                                    return;
                                }
                                if (mess.StartsWith("pco reset", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_reset))
                                {
                                    args.Player.SendInfoMessage($"权限不足[{p_reset}]，不能添加含有重置计划系列指令的计划");
                                    return;
                                }
                            }
                        }
                        if (mess.StartsWith("pco com", StringComparison.OrdinalIgnoreCase))
                        {
                            args.Player.SendInfoMessage($"禁止套娃");
                            return;
                        }
                        if (string.IsNullOrWhiteSpace(mess))
                        {
                            args.Player.SendErrorMessage("添加失败，不要添加空指令");
                        }
                        else if (config.AutoCommandList.Add(mess))
                        {
                            args.Player.SendSuccessMessage($"已将指令 /{mess} 添加成功！");
                            config.SaveConfigFile();
                        }
                        else
                            args.Player.SendErrorMessage("添加失败，请检查是否已添加过");
                    }
                    else if (args.Parameters[1].Equals("del", StringComparison.OrdinalIgnoreCase))
                    {
                        string mess = "";
                        for (int i = 2; i < args.Parameters.Count; i++)
                        {
                            mess += args.Parameters[i] + " ";
                        }
                        mess = CorrectCommand(mess);

                        if (config.CheckPerm)
                        {
                            //禁止有人通过这个指令来越权使用别的指令
                            bool can = true;
                            foreach (var v in getAllComCannotRun(args.Player))
                            {
                                if ((mess + " ").StartsWith(v + " "))
                                {
                                    can = false; break;
                                }
                            }
                            if (!can)
                            {
                                args.Player.SendInfoMessage($"你的权限不足以涵盖该指令：/{mess}，请不要试图越权");
                                return;
                            }
                            if (!args.Player.HasPermission(p_admin))
                            {
                                if (mess.StartsWith("pco npc", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_npc))
                                {
                                    args.Player.SendInfoMessage($"权限不足[{p_npc}]，不能删除含有NPC计划系列指令的计划");
                                    return;
                                }
                                if (mess.StartsWith("pco reload", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_reload))
                                {
                                    args.Player.SendInfoMessage($"权限不足[{p_reload}]，不能删除含有重启计划系列指令的计划");
                                    return;
                                }
                                if (mess.StartsWith("pco reset", StringComparison.OrdinalIgnoreCase) && !args.Player.HasPermission(p_reset))
                                {
                                    args.Player.SendInfoMessage($"权限不足[{p_reset}]，不能删除含有重置计划系列指令的计划");
                                    return;
                                }
                            }
                        }
                        if (config.AutoCommandList.Remove(mess))
                        {
                            config.SaveConfigFile();
                            args.Player.SendSuccessMessage($"已将指令 /{mess} 删除成功！");
                        }
                        else
                        {
                            List<string> list = new List<string>();
                            config.AutoCommandList.ForEach(x =>
                            {
                                if (CorrectCommand(x).Equals(mess, StringComparison.OrdinalIgnoreCase))
                                {
                                    list.Add(x);
                                }
                            });
                            bool flag = false;
                            if (list.Count > 0)
                            {
                                list.ForEach(x =>
                                {
                                    if (config.AutoCommandList.Remove(x))
                                    {
                                        flag = true;
                                        args.Player.SendSuccessMessage($"已将等价指令 /{x} 删除成功！");
                                    }
                                });
                                config.SaveConfigFile();
                                if (!flag)
                                    args.Player.SendErrorMessage("删除失败，请检查是否存在该指令");
                            }
                            else
                                args.Player.SendErrorMessage("删除失败，请检查是否存在该指令");
                        }
                    }
                    else
                        args.Player.SendInfoMessage("输入 /pco com help   来查看指令计划的帮助指令");
                }
            }
            /// reload 指令
            else if (args.Parameters[0].Equals("reload", StringComparison.OrdinalIgnoreCase))
            {
                if (!args.Player.HasPermission(p_reload) && !args.Player.HasPermission(p_admin))
                {
                    args.Player.SendInfoMessage($"权限不足！[{p_reload}]或[{p_admin}]");
                    return;
                }
                if (args.Parameters.Count == 2)
                {
                    if (args.Parameters[1].Equals("help", StringComparison.OrdinalIgnoreCase))
                    {
                        args.Player.SendMessage(
                            //2
                            "输入 /pco reload act   自动重启服务器计划启用，再次使用关闭\n" +
                            //3
                            "输入 /pco reload os <±num/±H:M:S>   将自动重启服务器的时间推迟或提前num时或H时M分S秒，num可为小数\n" +
                            //2 || 3
                            "输入 /pco reload hand <±num/±H:M:S>   手动重启服务器计划启用，在num时或H时M分S秒后开始重启，若不填则立刻重启，小于0则关闭当前存在的手动计划，其优先级大于自动重启\n" +
                            //3
                            "输入 /pco reload maxplayers <num>   来设置下次重启地图时的最多在线玩家\n" +
                            //3
                            "输入 /pco reload port <num>   来设置下次重启地图时的端口\n" +
                            //2 || 3
                            "输入 /pco reload password <string>   来设置下次重启地图时的密码\n" +
                            "输入 /pco reload stop   关闭手动重启服务器计划", TextColor());
                    }
                    else if (args.Parameters[1].Equals("act", StringComparison.OrdinalIgnoreCase))
                    {
                        if (config.AutoRestartServer)
                        {
                            config.AutoRestartServer = !config.AutoRestartServer;
                            config.SaveConfigFile();
                            SendMessageAllAndMe(args.Player, "自动重启计划已关闭", Color.LimeGreen);
                        }
                        else
                        {
                            double temp = (DateTime.Now.AddMinutes(AvoidTime) - config.LasetServerRestartDate.AddHours(config.HowLongTimeOfRestartServer)).TotalHours;
                            if (temp >= 0)
                            {
                                args.Player.SendInfoMessage($"自动重启服务器倒计时过短，需Time大于 {temp:0.00} 来避免立刻重启(即上次重启时间{AvoidTime}后才可自动执行)，避免产生错误，修改失败");
                            }
                            else
                            {
                                if (!args.Player.IsLoggedIn)
                                    args.Player.SendSuccessMessage($"自动重启计划已启用，将在从现在起{HoursToM((config.LasetServerRestartDate.AddHours(config.HowLongTimeOfRestartServer) - DateTime.Now).TotalHours)}后自动重启");
                                TSPlayer.All.SendMessage($"自动重启计划已启用，将在从现在起{HoursToM((config.LasetServerRestartDate.AddHours(config.HowLongTimeOfRestartServer) - DateTime.Now).TotalHours, "FF9000")}后自动重启", Color.LimeGreen);
                                config.AutoRestartServer = !config.AutoRestartServer;
                                config.SaveConfigFile();
                            }
                        }
                    }
                    else if (args.Parameters[1].Equals("hand", StringComparison.OrdinalIgnoreCase))
                    {
                        RestartGame();
                    }
                    else if (args.Parameters[1].Equals("password", StringComparison.OrdinalIgnoreCase))
                    {
                        config.AfterRestartServerPassword = "";
                        config.SaveConfigFile();
                        args.Player.SendSuccessMessage("下次重置服务器的密码已取消");
                    }
                    else if (args.Parameters[1].Equals("stop", StringComparison.OrdinalIgnoreCase))
                    {
                        if (countdownRestart.enable || thread_reload.IsAlive)
                        {
                            countdownRestart.enable = false;
                            countdownRestart.time = 0;
                            args.Player.SendSuccessMessage("手动重启计划已关闭");
                        }
                        else
                            args.Player.SendSuccessMessage("手动重启计划未开启，无需关闭");
                    }
                    else
                        args.Player.SendInfoMessage("输入 /pco reload help   来查看指令计划的帮助指令");
                }
                else
                {
                    if (args.Parameters[1].Equals("os", StringComparison.OrdinalIgnoreCase) || args.Parameters[1].Equals("offset", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!config.AutoRestartServer)
                        {
                            args.Player.SendWarningMessage("警告，并未开启自动重启计划，你的修改不会有任何效果");
                        }
                        double num = HMSorHoursToHours(args.Parameters[2], out string reason);
                        if (reason != "")
                        {
                            args.Player.SendInfoMessage(reason);
                            return;
                        }
                        if (config.LasetServerRestartDate.AddHours(config.HowLongTimeOfRestartServer + num) < DateTime.Now.AddMinutes(AvoidTime))
                        {
                            double temp = (DateTime.Now.AddMinutes(AvoidTime) - config.LasetServerRestartDate.AddHours(config.HowLongTimeOfRestartServer)).TotalHours;
                            args.Player.SendInfoMessage($"重启服务器倒计时过短，需Time大于 {temp:0.00} 来避免立刻重启(最短重启时间{AvoidTime}分钟)，修改失败。若要立刻重启，请使用 /pco reload hand 指令");
                        }
                        else
                        {
                            config.HowLongTimeOfRestartServer += num;
                            config.SaveConfigFile();
                            if (args.Player.IsLoggedIn)
                                args.Player.SendSuccessMessage($"时间已修改为 {config.HowLongTimeOfRestartServer:0.00} 即从现在起{HoursToM((config.LasetServerRestartDate.AddHours(config.HowLongTimeOfRestartServer) - DateTime.Now).TotalHours, "FF9000")}后开始重启");
                            else
                                args.Player.SendSuccessMessage($"时间已修改为 {config.HowLongTimeOfRestartServer:0.00} 即从现在起{HoursToM((config.LasetServerRestartDate.AddHours(config.HowLongTimeOfRestartServer) - DateTime.Now).TotalHours)}后开始重启");
                            if (config.AutoRestartServer)
                                TSPlayer.All.SendMessage("自动重启计划已修改", Color.Yellow);
                        }
                    }
                    else if (args.Parameters[1].Equals("hand", StringComparison.OrdinalIgnoreCase))
                    {
                        double h = HMSorHoursToHours(args.Parameters[2], out string reason);
                        if (reason != "")
                        {
                            args.Player.SendInfoMessage(reason);
                            return;
                        }
                        int num = (int)(h * 3600);
                        if (num >= 0)
                        {
                            if (config.AutoRestartServer && config.LasetServerRestartDate.AddHours(config.HowLongTimeOfRestartServer) < DateTime.Now.AddHours(h))
                            {
                                args.Player.SendWarningMessage("警告：手动重启计划倒计时时间太长，已超过自动重启计划时间，如果突然使用stop指令中断，自动重启可能立刻开始生效，不建议这么做");
                            }
                            if (countdownReset.enable && countdownReset.time < num)
                            {
                                args.Player.SendWarningMessage("警告：当前存在手动重置计划，且时间小于你输入的数值，你的手动重启计划将不会生效！");
                            }
                            if (!countdownReset.enable && config.OpenAutoReset && config.StartServerDate.AddHours(config.HowLongTimeOfAotuResetServer) < DateTime.Now.AddHours(h))
                            {
                                args.Player.SendWarningMessage("警告：当前存在自动重置计划，且时间小于你输入的数值，你的手动重启计划将不会生效！");
                            }

                            if (countdownRestart.enable && thread_reload != null && thread_reload.IsAlive)
                            {
                                countdownRestart.time = num;
                                TSPlayer.All.SendMessage($"手动重启计划已重新开始，服务器将在{HoursToM(h, "FF9000")}后重启", Color.Yellow);
                                if (!args.Player.IsLoggedIn)
                                    args.Player.SendInfoMessage($"手动重启计划已重新开始，服务器将在{HoursToM(h)}后重启");
                            }
                            else if (thread_reload == null || !thread_reload.IsAlive)
                            {
                                TSPlayer.All.SendMessage($"手动重启计划开启，服务器将在{HoursToM(h, "FF9000")}后重启", Color.Yellow);
                                if (!args.Player.IsLoggedIn)
                                    args.Player.SendInfoMessage($"手动重启计划开启，服务器将在{HoursToM(h)}后重启");
                                countdownRestart.enable = true;
                                countdownRestart.time = num;
                                thread_reload = new Thread(thread_reloadFun);
                                thread_reload.IsBackground = true;
                                thread_reload.Start();
                            }
                            else
                                args.Player.SendInfoMessage("计划时机不恰当，请1秒后重试");
                        }
                        else
                        {
                            if (countdownRestart.enable)
                            {
                                countdownRestart.enable = false;
                                countdownRestart.time = 0;
                                SendMessageAllAndMe(args.Player, "手动重启计划已关闭", Color.LimeGreen);
                            }
                            else
                                SendMessageAllAndMe(args.Player, "未开启手动重启计划", Color.LimeGreen);
                        }
                    }
                    else if (args.Parameters[1].Equals("maxplayers", StringComparison.OrdinalIgnoreCase))
                    {
                        if (int.TryParse(args.Parameters[2], out int num) && num >= 0 && num < 200)
                        {
                            args.Player.SendSuccessMessage("下次重启地图的玩家上限成功修改为：" + num);
                            config.AfterRestartPeople = num;
                            config.SaveConfigFile();
                        }
                        else
                            args.Player.SendInfoMessage("请输入整数，不要输入负数，数字不要过大");
                    }
                    else if (args.Parameters[1].Equals("port", StringComparison.OrdinalIgnoreCase))
                    {
                        config.AfterRestartPort = args.Parameters[2];
                        config.SaveConfigFile();
                        args.Player.SendSuccessMessage("下次重启地图时的端口为：" + args.Parameters[2]);
                    }
                    else if (args.Parameters[1].Equals("password", StringComparison.OrdinalIgnoreCase))
                    {
                        config.AfterRestartServerPassword = args.Parameters[2];
                        config.SaveConfigFile();
                        args.Player.SendSuccessMessage("下次重置服务器的密码修改成功");
                    }
                    else
                        args.Player.SendInfoMessage("输入 /pco reload help   来查看指令计划的帮助指令");
                }
            }
            /// reset 指令
            else if (args.Parameters[0].Equals("reset", StringComparison.OrdinalIgnoreCase))
            {
                if (!args.Player.HasPermission(p_reset) && !args.Player.HasPermission(p_admin))
                {
                    args.Player.SendInfoMessage($"权限不足！[{p_reset}]或[{p_admin}]");
                    return;
                }
                if (args.Parameters.Count == 2)
                {
                    if (args.Parameters[1].Equals("help", StringComparison.OrdinalIgnoreCase))
                    {
                        args.Player.SendMessage(
                            //2
                            "输入 /pco reset act   自动重置服务器计划启用，再次使用关闭\n" +
                            "输入 /pco reset os <±num/±H:M:S>   将自动重置世界的时间推迟或提前num时或H时M分S秒，num可为小数\n" +
                            //2 || 3
                            "输入 /pco reset hand <±num/±H:M:S>   手动重置世界计划启用，在num时或H时M分S秒后开始重置，若不填则立刻重置，小于0则关闭当前存在的手动计划，其优先级大于自动重置\n" +
                            "输入 /pco reset name <string>   来设置自动重置地图的地图名字\n" +
                            "输入 /pco reset size <小1/中2/大3(只填数字)>   来设置下次重置时地图的大小\n" +
                            "输入 /pco reset mode <普通0/专家1/大师2/旅途3(只填数字)>   来设置下次重置地图时的模式\n" +
                            "输入 /pco reset seed <string>   来设置下次重置地图时的地图种子，不填时设为随机\n" +
                            "输入 /pco reset maxplayers <num>   来设置下次重置地图时的最多在线玩家\n" +
                            //2
                            "输入 /pco reset resetplayers   来设置下次重置地图时清理玩家数据，再次使用取消\n" +
                            "输入 /pco reset port <num>   来设置下次重置地图时的端口\n" +
                            //2 || 3
                            "输入 /pco reset password <string>   来设置下次重置地图时的密码\n" +
                            //2
                            "输入 /pco reset delworld   来设置下次重置地图时删除当前地图，再次使用取消\n" +
                            "输入 /pco reset addname <string>   来添加你提供的用来重置的地图的名称\n" +
                            "输入 /pco reset delname <string>   来删除你提供的用来重置的地图的名称\n" +
                            //2
                            "输入 /pco reset listname   来列出你提供的所有地图名称\n" +
                            "输入 /pco reset stop    关闭手动重置世界计划", TextColor());
                    }
                    else if (args.Parameters[1].Equals("act", StringComparison.OrdinalIgnoreCase))
                    {
                        if (config.OpenAutoReset)
                        {
                            SendMessageAllAndMe(args.Player, "自动重置计划已关闭", Color.LimeGreen);
                            config.OpenAutoReset = !config.OpenAutoReset;
                            config.SaveConfigFile();
                        }
                        else
                        {
                            double temp = (DateTime.Now.AddMinutes(AvoidTime) - config.StartServerDate.AddHours(config.HowLongTimeOfAotuResetServer)).TotalHours;
                            if (temp >= 0)
                            {
                                args.Player.SendInfoMessage($"自动重置世界倒计时过短，需大于 {temp:0.00} 来避免立刻重置(即上次重置时间{AvoidTime}后才可自动执行)，避免产生错误，修改失败");
                            }
                            else
                            {
                                if (!args.Player.IsLoggedIn)
                                    args.Player.SendSuccessMessage($"自动重置计划已启用，将在从现在起{HoursToM((config.StartServerDate.AddHours(config.HowLongTimeOfAotuResetServer) - DateTime.Now).TotalHours)}后自动重置");
                                TSPlayer.All.SendSuccessMessage($"自动重置计划已启用，将在从现在起{HoursToM((config.StartServerDate.AddHours(config.HowLongTimeOfAotuResetServer) - DateTime.Now).TotalHours, "EA00FF")}后自动重置");
                                config.OpenAutoReset = !config.OpenAutoReset;
                                config.SaveConfigFile();
                            }
                        }
                    }
                    else if (args.Parameters[1].Equals("hand", StringComparison.OrdinalIgnoreCase))
                    {
                        ResetGame();
                    }
                    else if (args.Parameters[1].Equals("seed", StringComparison.OrdinalIgnoreCase))
                    {
                        config.WorldSeedForAfterReset = "";
                        config.SaveConfigFile();
                        args.Player.SendInfoMessage("已将地图种子设为随机");
                    }
                    else if (args.Parameters[1].Equals("password", StringComparison.OrdinalIgnoreCase))
                    {
                        config.AfterResetServerPassword = "";
                        config.SaveConfigFile();
                        args.Player.SendSuccessMessage("下次重置服务器的密码已取消");
                    }
                    else if (args.Parameters[1].Equals("listname", StringComparison.OrdinalIgnoreCase) || args.Parameters[1].Equals("list", StringComparison.OrdinalIgnoreCase))
                    {
                        string text = "";
                        if (config.ExpectedUsageWorldFileNameForAotuReset.Count == 0)
                        {
                            text += $"你没有提供任何名字，服务器将按<自动重置后的地图名称>: {config.AddNumberFile(CorrectFileName(config.WorldNameForAfterReset))} 生成地图";
                        }
                        else
                        {
                            int count = 1;
                            foreach (var v in config.ExpectedUsageWorldFileNameForAotuReset)
                            {
                                if (ExistWorldNamePlus(v, out string world))
                                {
                                    text += $"{world} 地图存在，第 {count} 次重置地图将自动调用\n";
                                }
                                else
                                {
                                    string name = config.AddNumberFile(CorrectFileName(v));
                                    text += $"{name} 地图不存在，第 {count} 次重置地图将自动生成一个\n";
                                }
                                count++;
                            }
                        }
                        text = text.Trim('\n');
                        args.Player.SendInfoMessage(text);
                    }
                    else if (args.Parameters[1].Equals("resetplayers", StringComparison.OrdinalIgnoreCase))
                    {
                        if (config.ResetTSCharacter)
                            args.Player.SendSuccessMessage("下次重置地图" + MtoM("不", "FFFFFF") + "会自动重置玩家数据");
                        else
                            args.Player.SendSuccessMessage("下次重置地图" + MtoM("会", "FFFFFF") + "自动重置玩家数据");
                        config.ResetTSCharacter = !config.ResetTSCharacter;
                        config.SaveConfigFile();
                    }
                    else if (args.Parameters[1].Equals("delworld", StringComparison.OrdinalIgnoreCase))
                    {
                        if (config.DeleteWorldForReset)
                            args.Player.SendSuccessMessage("下次重置地图" + MtoM("不", "FFFFFF") + "会自动删掉当前地图");
                        else
                            args.Player.SendSuccessMessage("下次重置地图" + MtoM("会", "FFFFFF") + "自动删掉当前地图");
                        config.DeleteWorldForReset = !config.DeleteWorldForReset;
                        config.SaveConfigFile();
                    }
                    else if (args.Parameters[1].Equals("stop", StringComparison.OrdinalIgnoreCase))
                    {
                        if (countdownReset.enable || thread_reset.IsAlive)
                        {
                            countdownReset.enable = false;
                            countdownReset.time = 0;
                            args.Player.SendSuccessMessage("手动重置计划已关闭");
                        }
                        else
                            args.Player.SendSuccessMessage("手动重置计划未开启，无需关闭");
                    }
                    else
                        args.Player.SendInfoMessage("输入 /pco reset help   来查看重置计划的帮助指令");
                }
                else
                {
                    if (args.Parameters[1].Equals("os", StringComparison.OrdinalIgnoreCase) || args.Parameters[1].Equals("offset", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!config.OpenAutoReset)
                        {
                            args.Player.SendWarningMessage("警告，并未开启自动重置计划，你的修改不会有任何效果");
                        }
                        double num = HMSorHoursToHours(args.Parameters[2], out string reason);
                        if (reason != "")
                        {
                            args.Player.SendInfoMessage(reason);
                            return;
                        }
                        if (config.StartServerDate.AddHours(config.HowLongTimeOfAotuResetServer + num) < DateTime.Now.AddMinutes(AvoidTime))
                        {
                            double temp = (DateTime.Now.AddMinutes(AvoidTime) - config.StartServerDate.AddHours(config.HowLongTimeOfAotuResetServer)).TotalHours;
                            args.Player.SendInfoMessage($"重置世界倒计时过短，需Time大于 {temp:0.00} 来避免立刻重置(最短重置时间5分钟)，修改失败。若要立刻重置，请使用 /pco reset hand 指令");
                        }
                        else
                        {
                            config.HowLongTimeOfAotuResetServer += num;
                            config.SaveConfigFile();
                            if (args.Player.IsLoggedIn)
                                args.Player.SendSuccessMessage($"时间已修改为 {config.HowLongTimeOfAotuResetServer:0.00} 即从现在起{HoursToM((config.StartServerDate.AddHours(config.HowLongTimeOfAotuResetServer) - DateTime.Now).TotalHours, "EA00FF")}后开始重置");
                            else
                                args.Player.SendSuccessMessage($"时间已修改为 {config.HowLongTimeOfAotuResetServer:0.00} 即从现在起{HoursToM((config.StartServerDate.AddHours(config.HowLongTimeOfAotuResetServer) - DateTime.Now).TotalHours)}后开始重置");
                            if (config.OpenAutoReset)
                                TSPlayer.All.SendMessage("自动重置计划已修改", Color.Yellow);
                        }
                    }
                    else if (args.Parameters[1].Equals("hand", StringComparison.OrdinalIgnoreCase))
                    {
                        double h = HMSorHoursToHours(args.Parameters[2], out string reason);
                        if (reason != "")
                        {
                            args.Player.SendInfoMessage(reason);
                            return;
                        }
                        int num = (int)(h * 3600);
                        if (num >= 0)
                        {
                            if (config.OpenAutoReset && config.StartServerDate.AddHours(config.HowLongTimeOfAotuResetServer) < DateTime.Now.AddHours(h))
                            {
                                args.Player.SendWarningMessage("警告：手动重置计划倒计时时间太长，已超过自动重置计划时间，如果突然使用stop指令中断，自动重置可能立刻开始生效，不建议这么做");
                            }
                            if (countdownRestart.enable && countdownRestart.time < num)
                            {
                                args.Player.SendWarningMessage("警告：当前存在手动重启计划，且时间小于你输入的数值，你的手动重置计划将不会生效！");
                            }
                            if (!countdownRestart.enable && config.AutoRestartServer && config.LasetServerRestartDate.AddHours(config.HowLongTimeOfRestartServer) < DateTime.Now.AddHours(h))
                            {
                                args.Player.SendWarningMessage("警告：当前存在自动重启计划，且时间小于你输入的数值，你的手动重置计划将不会生效！");
                            }

                            if (countdownReset.enable && thread_reset != null && thread_reset.IsAlive)
                            {
                                countdownReset.time = num;
                                TSPlayer.All.SendMessage($"手动重置计划已重新开始，服务器将在{HoursToM(h, "EA00FF")}后重置", Color.Yellow);
                                if (!args.Player.IsLoggedIn)
                                    args.Player.SendInfoMessage($"手动重置计划已重新开始，服务器将在{HoursToM(h)}后重置");
                            }
                            else if (thread_reset == null || !thread_reset.IsAlive)
                            {
                                TSPlayer.All.SendMessage($"手动重置计划开启，服务器将在{HoursToM(h, "EA00FF")}后重置", Color.Yellow);
                                if (!args.Player.IsLoggedIn)
                                    args.Player.SendInfoMessage($"手动重置计划开启，服务器将在{HoursToM(h)}后重置");
                                countdownReset.enable = true;
                                countdownReset.time = num;
                                thread_reset = new Thread(thread_resetFun);
                                thread_reset.IsBackground = true;
                                thread_reset.Start();
                            }
                            else
                                args.Player.SendInfoMessage("计划时机不恰当，请1秒后重试");
                        }
                        else
                        {
                            if (countdownReset.enable)
                            {
                                countdownReset.enable = false;
                                countdownReset.time = 0;
                                SendMessageAllAndMe(args.Player, "手动重置计划已关闭", Color.LimeGreen);
                            }
                            else
                                SendMessageAllAndMe(args.Player, "未开启手动重置计划", Color.LimeGreen);
                        }
                    }
                    else if (args.Parameters[1].Equals("name", StringComparison.OrdinalIgnoreCase))
                    {
                        string worldname = CorrectFileName(args.Parameters[2]);
                        if (args.Parameters[2] != worldname)
                            args.Player.SendErrorMessage("你输入的地图名称带有非法字符，已将非法字符过滤，改为：" + worldname);
                        config.WorldNameForAfterReset = worldname;
                        config.SaveConfigFile();
                        args.Player.SendSuccessMessage("修改成功，自动重置后的地图名称修改为：" + worldname);
                    }
                    else if (args.Parameters[1].Equals("size", StringComparison.OrdinalIgnoreCase))
                    {
                        if (int.TryParse(args.Parameters[2], out int num) && (num == 1 || num == 2 || num == 3))
                        {
                            if (num == 1)
                                args.Player.SendSuccessMessage("下次重置地图的大小成功修改为：小");
                            else if (num == 2)
                                args.Player.SendSuccessMessage("下次重置地图的大小成功修改为：中");
                            else
                                args.Player.SendSuccessMessage("下次重置地图的大小成功修改为：大");
                            config.MapSizeForAfterReset = num;
                            config.SaveConfigFile();
                        }
                        else
                            args.Player.SendInfoMessage("请输入数字1,2或3");
                    }
                    else if (args.Parameters[1].Equals("mode", StringComparison.OrdinalIgnoreCase))
                    {
                        if (int.TryParse(args.Parameters[2], out int num) && (num == 0 || num == 1 || num == 2 || num == 3))
                        {
                            if (num == 0)
                                args.Player.SendSuccessMessage("下次重置地图的模式成功修改为：普通");
                            else if (num == 1)
                                args.Player.SendSuccessMessage("下次重置地图的模式成功修改为：专家");
                            else if (num == 2)
                                args.Player.SendSuccessMessage("下次重置地图的模式成功修改为：大师");
                            else
                                args.Player.SendSuccessMessage("下次重置地图的模式成功修改为：旅途");
                            config.MapDifficultyForAfterReset = num;
                            config.SaveConfigFile();
                        }
                        else
                            args.Player.SendInfoMessage("请输入数字0,1,2或3");
                    }
                    else if (args.Parameters[1].Equals("seed", StringComparison.OrdinalIgnoreCase))
                    {
                        args.Player.SendSuccessMessage("下次重置地图的种子成功修改为：" + args.Parameters[2]);
                        config.WorldSeedForAfterReset = args.Parameters[2];
                        config.SaveConfigFile();
                    }
                    else if (args.Parameters[1].Equals("maxplayers", StringComparison.OrdinalIgnoreCase))
                    {
                        if (int.TryParse(args.Parameters[2], out int num) && num >= 0 && num < 200)
                        {
                            args.Player.SendSuccessMessage("下次重置地图的玩家上限成功修改为：" + num);
                            config.AfterResetPeople = num;
                            config.SaveConfigFile();
                        }
                        else
                            args.Player.SendInfoMessage("请输入整数，不要输入负数，数字不要过大");
                    }
                    else if (args.Parameters[1].Equals("port", StringComparison.OrdinalIgnoreCase))
                    {
                        config.AfterResetPort = args.Parameters[2];
                        config.SaveConfigFile();
                        args.Player.SendSuccessMessage("下次重置地图时的端口为：" + args.Parameters[2]);
                    }
                    else if (args.Parameters[1].Equals("password", StringComparison.OrdinalIgnoreCase))
                    {
                        config.AfterResetServerPassword = args.Parameters[2];
                        config.SaveConfigFile();
                        args.Player.SendSuccessMessage("下次重置服务器的密码修改成功");
                    }
                    else if (args.Parameters[1].Equals("addname", StringComparison.OrdinalIgnoreCase) || args.Parameters[1].Equals("add", StringComparison.OrdinalIgnoreCase))
                    {
                        string worldname = CorrectFileName(args.Parameters[2]);
                        if (args.Parameters[2] != worldname)
                            args.Player.SendErrorMessage("你输入的地图名称带有非法字符或不必要的后缀，已过滤为：" + worldname);
                        if (config.ExpectedUsageWorldFileNameForAotuReset.Add(worldname))
                        {
                            if (ExistWorldNamePlus(worldname, out string world))
                                args.Player.SendSuccessMessage($"{worldname} 添加成功，该名称的地图存在：{world}，重置时将直接读取");
                            else
                                args.Player.SendSuccessMessage($"{worldname} 添加成功，该名称的地图不存在，重置时将生成一个");
                            config.SaveConfigFile();
                        }
                        else
                            args.Player.SendInfoMessage("添加失败，该名称已存在");
                    }
                    else if (args.Parameters[1].Equals("delname", StringComparison.OrdinalIgnoreCase) || args.Parameters[1].Equals("del", StringComparison.OrdinalIgnoreCase))
                    {
                        string worldname = CorrectFileName(args.Parameters[2]);
                        if (args.Parameters[2] != worldname)
                            args.Player.SendErrorMessage("你输入的地图名称带有非法字符或不必要的后缀，已过滤为：" + worldname);
                        if (config.ExpectedUsageWorldFileNameForAotuReset.Remove(worldname))
                        {
                            args.Player.SendSuccessMessage($"删除成功");
                            config.SaveConfigFile();
                        }
                        else
                            args.Player.SendInfoMessage("删除失败，该名称可能不存在");
                    }
                    else
                        args.Player.SendInfoMessage("输入 /pco reset help 来获取该插件的帮助");
                }
            }
            else
                args.Player.SendInfoMessage("输入 /pco help 来获取该插件的帮助");
        }

        #region 辅助方法用于递归复制目录
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool overwrite)
        {
            var dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"源目录不存在或找不到: {sourceDirName}");

            var dirs = dir.GetDirectories();
            var files = dir.GetFiles();

            Directory.CreateDirectory(destDirName);

            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, overwrite);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, overwrite);
            }
        }
        #endregion

        /// <summary>
        /// npc更新的
        /// </summary>
        /// <param name = "args" ></ param >
        private void NPCAIUpdate(NpcAiUpdateEventArgs args)
        {
            if (!config.OpenAutoControlProgressLock || args == null || args.Npc == null || !args.Npc.active || Main.time % 2 != 0)
                return;

            switch (args.Npc.netID)
            {
                case NPCID.KingSlime:
                    Function(args.Npc, "史莱姆王");
                    break;
                case NPCID.EyeofCthulhu:
                    Function(args.Npc, "克苏鲁之眼");
                    break;
                case NPCID.EaterofWorldsHead:
                    Function(args.Npc, "世界吞噬者");
                    break;
                case NPCID.EaterofWorldsBody:
                case NPCID.EaterofWorldsTail:
                    Function(args.Npc, "世界吞噬者", false);
                    break;
                case NPCID.BrainofCthulhu:
                    Function(args.Npc, "克苏鲁之脑");
                    break;
                case NPCID.Creeper:
                    Function(args.Npc, "克苏鲁之脑", false);
                    break;
                case NPCID.QueenBee:
                    Function(args.Npc, "蜂后");
                    break;
                case NPCID.Deerclops:
                    Function(args.Npc, "巨鹿");
                    break;
                case NPCID.SkeletronHead:
                    Function(args.Npc, "骷髅王");
                    break;
                case NPCID.SkeletronHand:
                    Function(args.Npc, "骷髅王", false);
                    break;
                case NPCID.WallofFlesh:
                    Function(args.Npc, "血肉墙");
                    break;
                case NPCID.WallofFleshEye:
                case NPCID.TheHungry:
                case NPCID.TheHungryII:
                    Function(args.Npc, "血肉墙", false);
                    break;
                case NPCID.QueenSlimeBoss:
                    Function(args.Npc, "史莱姆皇后");
                    break;
                case 125://双子
                case 126:
                    Function(args.Npc, "双子魔眼");
                    break;
                case NPCID.TheDestroyer:
                    Function(args.Npc, "毁灭者");
                    break;
                case NPCID.TheDestroyerBody:
                case NPCID.TheDestroyerTail:
                case NPCID.Probe:
                    Function(args.Npc, "毁灭者", false);
                    break;
                case 127://机械骷髅王
                    Function(args.Npc, "机械骷髅王");
                    break;
                case 128:
                case 129:
                case 130:
                case 131:
                    Function(args.Npc, "机械骷髅王", false);
                    break;
                case NPCID.Plantera:
                    Function(args.Npc, "世纪之花");
                    break;
                case NPCID.PlanterasTentacle:
                    Function(args.Npc, "世纪之花", false);
                    break;
                case NPCID.GolemFistLeft:
                case NPCID.GolemFistRight:
                    Function(args.Npc, "石巨人", false);
                    break;
                case NPCID.Golem:
                case NPCID.GolemHead:
                    Function(args.Npc, "石巨人");
                    break;
                case NPCID.DukeFishron:
                    Function(args.Npc, "猪龙鱼公爵");
                    break;
                case NPCID.HallowBoss:
                    Function(args.Npc, "光之女皇");
                    break;
                case NPCID.CultistBoss:
                    Function(args.Npc, "拜月教教徒");
                    break;
                case NPCID.LunarTowerSolar:
                case NPCID.LunarTowerVortex:
                case NPCID.LunarTowerStardust:
                case NPCID.LunarTowerNebula:
                    Function(args.Npc, "四柱");
                    break;
                case NPCID.MoonLordCore:
                    Function(args.Npc, "月亮领主");
                    break;
                case NPCID.MoonLordHead:
                case NPCID.MoonLordHand:
                case NPCID.MoonLordLeechBlob:
                    Function(args.Npc, "月亮领主", false);
                    break;
                default: break;
            }

            if ((int)Main.time % 5 == 0 && config.CustomNPCIDLockTimeForStartServerDate.TryGetValue(args.Npc.netID, out double num))
            {
                TimeSpan span = config.StartServerDate.AddHours(num) - DateTime.Now;
                if (span.TotalHours >= 0)
                {
                    args.Npc.active = false;
                    args.Npc.type = 0;
                    TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", args.Npc.whoAmI);
                    //TSPlayer.All.SendInfoMessage($"{args.Npc.FullName} 未到解锁时间，还剩{HoursToM(span.TotalHours, "28FFB8")}");
                }
            }
        }


        /// <summary>
        /// npc受击的
        /// </summary>
        /// <param name="args"></param>
        private void NPCStrike(NpcStrikeEventArgs args)
        {
            if (!config.OpenAutoControlProgressLock || args == null || args.Npc == null || !args.Npc.active)
                return;

            switch (args.Npc.netID)
            {
                case NPCID.KingSlime:
                    Function(args.Npc, "史莱姆王", false);
                    break;
                case NPCID.EyeofCthulhu:
                    Function(args.Npc, "克苏鲁之眼", false);
                    break;
                case NPCID.EaterofWorldsHead:
                case NPCID.EaterofWorldsBody:
                case NPCID.EaterofWorldsTail:
                    Function(args.Npc, "世界吞噬者", false);
                    break;
                case NPCID.BrainofCthulhu:
                case NPCID.Creeper:
                    Function(args.Npc, "克苏鲁之脑", false);
                    break;
                case NPCID.QueenBee:
                    Function(args.Npc, "蜂后", false);
                    break;
                case NPCID.Deerclops:
                    Function(args.Npc, "巨鹿", false);
                    break;
                case NPCID.SkeletronHead:
                case NPCID.SkeletronHand:
                    Function(args.Npc, "骷髅王", false);
                    break;
                case NPCID.WallofFlesh:
                case NPCID.WallofFleshEye:
                case NPCID.TheHungry:
                case NPCID.TheHungryII:
                    Function(args.Npc, "血肉墙", false);
                    break;
                case NPCID.QueenSlimeBoss:
                    Function(args.Npc, "史莱姆皇后", false);
                    break;
                case 125://双子
                case 126:
                    Function(args.Npc, "双子魔眼", false);
                    break;
                case NPCID.TheDestroyer:
                case NPCID.TheDestroyerBody:
                case NPCID.TheDestroyerTail:
                case NPCID.Probe:
                    Function(args.Npc, "毁灭者", false);
                    break;
                case 127://机械骷髅王
                case 128:
                case 129:
                case 130:
                case 131:
                    Function(args.Npc, "机械骷髅王", false);
                    break;
                case NPCID.Plantera:
                case NPCID.PlanterasTentacle:
                    Function(args.Npc, "世纪之花", false);
                    break;
                case NPCID.GolemFistLeft:
                case NPCID.GolemFistRight:
                case NPCID.Golem:
                case NPCID.GolemHead:
                    Function(args.Npc, "石巨人", false);
                    break;
                case NPCID.DukeFishron:
                    Function(args.Npc, "猪龙鱼公爵", false);
                    break;
                case NPCID.HallowBoss:
                    Function(args.Npc, "光之女皇", false);
                    break;
                case NPCID.CultistBoss:
                    Function(args.Npc, "拜月教教徒", false);
                    break;
                case NPCID.LunarTowerSolar:
                case NPCID.LunarTowerVortex:
                case NPCID.LunarTowerStardust:
                case NPCID.LunarTowerNebula:
                    Function(args.Npc, "四柱", false);
                    break;
                case NPCID.MoonLordCore:
                case NPCID.MoonLordHead:
                case NPCID.MoonLordHand:
                case NPCID.MoonLordLeechBlob:
                    Function(args.Npc, "月亮领主", false);
                    break;
                default: break;
            }
        }


        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="args"></param>
        private void PostInit(EventArgs args)
        {
            config = Config.LoadConfigFile();


            //重置时间在现在之前，那么取消重置
            if ((DateTime.Now - config.StartServerDate).TotalHours >= config.HowLongTimeOfAotuResetServer && config.OpenAutoReset)
            {
                TSPlayer.Server.SendErrorMessage("自动重置已过期，现已关闭自动重置并将开服日期设定为现在，详情看ProgressControl.json配置文件（ProgressControl插件）");
                config.StartServerDate = DateTime.Now;
                config.OpenAutoReset = false;
                config.HowLongTimeOfAotuResetServer = 0;
                config.SaveConfigFile();
            }
            if ((DateTime.Now - config.LasetServerRestartDate).TotalHours >= config.HowLongTimeOfRestartServer && config.AutoRestartServer)
            {
                TSPlayer.Server.SendErrorMessage("自动重启已过期，现已将上次重启日期设定为现在，如果你不希望开启自动重启可以关闭，详情看ProgressControl.json配置文件（ProgressControl插件）");
                config.LasetServerRestartDate = DateTime.Now;
                config.SaveConfigFile();
            }
            if ((DateTime.Now - config.LasetAutoCommandDate).TotalHours >= config.HowLongTimeOfAutoCommand && config.OpenAutoCommand)
            {
                TSPlayer.Server.SendErrorMessage("自动执行指令已过期，现已将上次执行指令的日期设定为现在，如果你不希望开启自动执行指令可以关闭，详情看ProgressControl.json配置文件（ProgressControl插件）");
                config.LasetAutoCommandDate = DateTime.Now;
                config.SaveConfigFile();
            }
            thread_auto.IsBackground = true;
            thread_auto.Start();
            //更新npc的死亡次数触发器
            NpcAndKillCountTrigger.Clear();
            foreach (var v in config.NpcKillCountForAutoReset)
            {
                NpcAndKillCountTrigger.TryAdd(v.Key, true);
            }
        }


        /// <summary>
        /// 在npc死时触发
        /// </summary>
        /// <param name="args"></param>
        private void OnNPCKilled(NpcKilledEventArgs args)
        {
            if (config.NpcKillCountForAutoReset.TryGetValue(args.npc.netID, out var arrayList))
            {
                if (arrayList.Count < 1 || arrayList[0] == null)
                    return;
                long num = (long)arrayList[0]!;
                if (Main.BestiaryTracker.Kills.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[args.npc.netID]) < num)
                {
                    NpcAndKillCountTrigger[args.npc.netID] = false;
                }
                else if (NpcAndKillCountTrigger[args.npc.netID] == false || Main.BestiaryTracker.Kills.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[args.npc.netID]) == num)
                {
                    for (int i = 1; i < arrayList.Count; i++)
                    {
                        TSPlayer.All.SendInfoMessage($"NPC:{args.npc.FullName} 已被击杀 {arrayList[0]} 次，开始执行指令");
                        Commands.HandleCommand(TSPlayer.Server, "/" + arrayList[i]);
                    }
                    NpcAndKillCountTrigger[args.npc.netID] = true;
                }
            }
        }
    }
}
