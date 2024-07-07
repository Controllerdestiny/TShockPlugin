﻿using TShockAPI;
using static Plugin.Plugin;

namespace Plugin
{
    internal class Comds
    {
        public static async void Comd(CommandArgs args)
        {
            TSPlayer plr = args.Player;
            if (args == null || !plr.RealPlayer ) return;

            if (args.Parameters.Count == 0)
            {
                args.Player.SendMessage(
                    "【出生点监狱】\n" +
                    "/rm 数量 —— 建监狱\n" +
                    "/rm 重置 —— 重置自动建房开关", 250, 247, 105);
            }

            if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "reset" || args.Parameters[0].ToLower() == "重置")
            {
                Config.Enabled = true;
                args.Player.SendMessage("【出生点监狱】开关已打开，请重启服务器。",250, 247, 105);
                Config.Write();
                return;
            }

            int total = 3;
            if (args.Parameters.Count > 1)
            {
                if (NeedInGame() || NeedWaitTask())
                {
                    return;
                }

                if (!int.TryParse(args.Parameters[1], out total))
                {
                    plr.SendErrorMessage("输入的监狱数量不对");
                    return;
                }

                if (total < 1 || total > 1000)
                {
                    total = 3;
                }
            }
            await AsyncGenRoom(isRight: plr.TPlayer.direction != -1, plr: plr, posX: plr.TileX, posY: plr.TileY + 3, total: total, needCenter: true);

            bool NeedInGame()
            {
                return Utils.NeedInGame(plr);
            }

            bool NeedWaitTask()
            {
                return TileHelper.NeedWaitTask(plr);
            }
        }

    }
}
