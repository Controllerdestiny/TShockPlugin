﻿using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI.Chat;
using TShockAPI;
using TShockAPI.DB;

namespace CaiBot;

public class CaiBotPlayer : TSPlayer
{
    private readonly List<string> _commandOutput = new();

    public CaiBotPlayer()
        : base("CaiBot")
    {
        Group = new SuperAdminGroup();
        AwaitingResponse = new Dictionary<string, Action<object>>();
        Account = new UserAccount
        {
            Name = "CaiBot"
        };
    }

    public override void SendMessage(string msg, Color color)
    {
        SendMessage(msg, color.R, color.G, color.B);
    }

    public override void SendMessage(string msg, byte red, byte green, byte blue)
    {
        string result1 = "";
        foreach (TextSnippet item in ChatManager.ParseMessage(msg, new Color(red, green, blue))) result1 += item.Text;

        Regex regex = new(@"\[i(tem)?(?:\/s(?<Stack>\d{1,4}))?(?:\/p(?<Prefix>\d{1,3}))?:(?<NetID>-?\d{1,4})\]");

        string result = regex.Replace(result1, m =>
        {
            string netId = m.Groups["NetID"].Value;
            string prefix = m.Groups["Prefix"].Success ? m.Groups["Prefix"].Value : "0";
            string stack = m.Groups["Stack"].Success ? m.Groups["Stack"].Value : "0";
            if (stack == "0")
                return "";
            if (stack == "1")
                if (prefix == "0")
                    return $"[{Lang.GetItemName(int.Parse(netId))}]";
                else
                    return
                        $"[{Lang.prefix[int.Parse(prefix)]} {Lang.GetItemName(int.Parse(netId))}]"; //return $"[{Terraria.Lang.prefix[int.Parse(netID)]}]";
            return $"[{Lang.prefix[int.Parse(prefix)]} {Lang.GetItemName(int.Parse(netId))} ({stack})]";
        });
        _commandOutput.Add(result);
    }

    public override void SendInfoMessage(string msg)
    {
        SendMessage(msg, Color.Yellow);
    }

    public override void SendSuccessMessage(string msg)
    {
        SendMessage(msg, Color.Green);
    }

    public override void SendWarningMessage(string msg)
    {
        SendMessage(msg, Color.OrangeRed);
    }

    public override void SendErrorMessage(string msg)
    {
        SendMessage(msg, Color.Red);
    }

    public List<string> GetCommandOutput()
    {
        return _commandOutput;
    }
}