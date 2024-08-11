﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace VeinMiner
{
    [ApiVersion(2, 1)]
    public class VeinMiner : TerrariaPlugin
    {
        public override string Name => "VeinMiner";
        public override Version Version => new Version(1, 6, 0, 3);
        public override string Author => "Megghy|YSpoof|Maxthegreat99|肝帝熙恩";
        public override string Description => "VeinMiner by Megghy 适用于 TShock 5.2 支持！";

        internal static Config Config = new();
        public VeinMiner(Main game) : base(game)
        {

        }

        public override void Initialize()
        {
            Config.Load();
            Commands.ChatCommands.Add(new Command(
                permissions: "veinminer",
                cmd: delegate (CommandArgs args)
                {
                    var tsp = args.Player;
                    var result = tsp.GetData<VMStatus>("VeinMiner");
                    if (args.Parameters.Count >= 1)
                    {
                        result.EnableBroadcast = !result.EnableBroadcast;
                        tsp.SendMessage($"[c/95CFA6:<VeinMiner> 挖矿消息已{(result.EnableBroadcast ? "开启" : "关闭")}].", Color.White);
                    }
                    else
                    {
                        result.Enable = !result.Enable;
                        tsp.SendMessage($"[c/95CFA6:<VeinMiner> 已{(result.Enable ? "开启" : "关闭! | 要仅关闭挖矿消息提示请输入：/vm {任意参数}")}.]", Color.White);
                    }
                },
                "veinminer", "chain mining", "vm"));
            GetDataHandlers.TileEdit += OnTileEdit;
            TShockAPI.Hooks.GeneralHooks.ReloadEvent += Config.Load;
            ServerApi.Hooks.ServerJoin.Register(this, OnPlayerJoin);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GetDataHandlers.TileEdit -= OnTileEdit;
                TShockAPI.Hooks.GeneralHooks.ReloadEvent -= Config.Load;
                ServerApi.Hooks.ServerJoin.Deregister(this, OnPlayerJoin);
            }
            base.Dispose(disposing);
        }

        class VMStatus
        {
            public bool Enable = true;
            public bool EnableBroadcast = true;
        }

        void OnPlayerJoin(JoinEventArgs args)
        {
            if (TShock.Players[args.Who] is { } plr)
            {
                plr.SetData("VeinMiner", new VMStatus());
            }
        }

        void OnTileEdit(object o, GetDataHandlers.TileEditEventArgs args)
        {
            if (Main.tile[args.X, args.Y] is { } tile && args.Player.HasPermission("veinminer") && Config.Enable && args.Player.GetData<VMStatus>("VeinMiner").Enable && Config.Tile.Contains(tile.type) && args.Action == GetDataHandlers.EditAction.KillTile && args.EditData == 0)
            {
                args.Handled = true;
                Mine(args.Player, args.X, args.Y, tile.type);
            }
        }

        void Mine(TSPlayer plr, int x, int y, int type)
        {
            var list = GetVein(new(), x, y, type).Result;
            var count = list.Count;
            var item = Utils.GetItemFromTile(x, y);
            var mineableList = new List<Point>();

            // 过滤出可挖的矿石坐标
            foreach (var point in list)
            {
                if (point.Y > 0 && Config.NotMine.Contains(Main.tile[point.X, point.Y - 1].type))
                {
                    continue;
                }

                mineableList.Add(point);
            }

            var mineCount = mineableList.Count;
            if (mineCount > 0)
            {
                list = mineableList;

                if (Config.Exchange.Where(e => e.Type == type && mineCount >= e.MinSize).ToList() is { Count: > 0 } exchangeList)
                    exchangeList.ForEach(e =>
                    {
                        if (e.Item.Count <= plr.GetBlankSlot())
                        {
                            e.Item.ForEach(ex => plr.GiveItem(ex.Key, ex.Value));
                            if (e.OnlyGiveItem)
                                KillTileAndSend(list, true); 
                            else
                                GiveItem();
                            plr.SendMessage($"[c/95CFA6:<VeinMiner>] 挖掘了 [c/95CFA6: {mineCount} {(item.type == 0 ? "未知" : item.Name)}].", Color.White);
                            return;
                        }
                        else
                        {
                            plr.SendInfoMessage($"[c/95CFA6:<VeinMiner>] 背包已满，还需空位：[c/95CFA6:{e.Item.Count}] .");
                            plr.SendTileSquareCentered(x, y, 1);
                            return;
                        }
                    });
                else
                    GiveItem();

                void GiveItem()
                {
                    if (Config.PutInInventory)
                    {
                        if (plr.IsSpaceEnough(item.netID, mineCount)) 
                        {
                            plr.GiveItem(item.netID, mineCount); 
                            KillTileAndSend(list, true); 
                        }
                        else
                        {
                            WorldGen.KillTile(x, y);
                            plr.SendInfoMessage($"[c/95CFA6:<VeinMiner>] 背包已满，需额外空位：[c/95CFA6:{mineCount}] 以放入 [c/95CFA6:{item.Name}] .");
                        }
                    }
                    else
                        KillTileAndSend(list, false); 

                    if (plr.GetData<VMStatus>("VeinMiner").EnableBroadcast && Config.Broadcast && mineCount > 1)
                        plr.SendMessage($"[c/95CFA6:<VeinMiner>] 正在挖掘 [c/95CFA6:{mineCount} {(item.type == 0 ? "未知" : item.Name)}].", Color.White);
                }
            }
            else if (count > 0)
            {
                plr.SendMessage($"[c/95CFA6:<VeinMiner>] 无法挖取矿石，可能是因为矿石上方有不可破坏的物体.", Color.White);
            }
        }

        public static void KillTileAndSend(List<Point> list, bool noItem)
        {
            Task.Run(() =>
            {
                if (!list.Any())
                    return;

                list.ForEach(p =>
                {
                    WorldGen.KillTile(p.X, p.Y, false, false, noItem);
                    NetMessage.SendData(17, -1, -1, null, 4, p.X, p.Y, false.GetHashCode());
                });
            });
        }

        public static Task<List<Point>> GetVein(List<Point> list, int x, int y, int type)
        {
            return Task.Run(() =>
            {
                if (!list.Any(p => p.Equals(new Point(x, y))) && Main.tile[x, y] is { } tile && tile.active() && tile.type == type)
                {
                    if (list.Count > 5000) return list;
                    list.Add(new(x, y));
                    list = GetVein(list, x + 1, y, type).Result;
                    list = GetVein(list, x - 1, y, type).Result;
                    list = GetVein(list, x, y + 1, type).Result;
                    list = GetVein(list, x, y - 1, type).Result;
                    list = GetVein(list, x + 1, y + 1, type).Result;
                    list = GetVein(list, x + 1, y - 1, type).Result;
                    list = GetVein(list, x - 1, y + 1, type).Result;
                    list = GetVein(list, x - 1, y - 1, type).Result;
                }
                return list;
            });
        }
    }
}