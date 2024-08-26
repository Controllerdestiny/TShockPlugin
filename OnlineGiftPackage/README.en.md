# Online Gift Package online gift package

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: starry night flower and feather science
- - 出处: [github](https://gitee.com/star-night-flower/tshock-gift) 
- This is a Tshock server plug-in, which is mainly used to distribute random online rewards to online players in the server.
- After adding items in the online package. json file, the game send /Reload can automatically calculate the total probability of acquisition.
## Update log

```
- 1.0.1.2
- 完善卸载函数

- 1.1.1
- 1.完善了对“总概率”的Reload重载同步
- 2.优化了命令的显示排版
- 3.给命令加了个权限名
- 4.移除了配置文件里的“将未符合条件者记录后台”
-  
- 1.1.0
- 1.修复了使用/reload或重启服务器时，配置文件被原配置覆盖，无法正常读取修改过的变量问题。
- 2.把计算总概率的显示加入到了指令里：/在线礼包
- 3.删除配置文件使用/reload可得到个计算过的“总概率”值在配置文件里
- 4.配置文件加入了“跳过生命上限”的检测标准，可以决定高血量玩家无法获取在线礼包。
- 5.加入了每次发放礼包是否记录后台
- 6.“将未符合条件者记录后台”是羽学调试时观察用的，不建议开，后期更新会移除。
- 7.赠送礼包后会提醒玩家下次发放时间
-  
- 1.0.9
- 1.修复了不按时送物品的问题，  
- 2.移除了广播在线时长的方法，  
- 3.配置文件加入了大量预设物品方案，  
- 4.修复了/在线礼包指令不显示概率的问题，  
- 5.不再根据序列数量来发放物品而是发过一次自动重置为0+1从第一个序列开始发，  
- 6.解决了初始化配置文件时自动序列化问题  
- 7.加了个计算总概率的新方法。  
-  
- 1.0.8  
- 1.补充了配置文件缺失的变量名称  
- 2.增加了总概率选项  
- 3.玩家可自定义广播间隔时间，方便与触发时间同步  
- 4.再次尝试优化定时器
- 5.适配了.net 6.0  
-  
- 1.0.7  
- 1.优化了定时器  
- 2.config文件改名为【在线礼包.json】，并对其修改项合理汉化
```
## instruction

|grammar|limit of authority|explain|
| -------------- |:-----------------:|:------:|
|/online gift package|OnlineGiftPackage|Displays the probability table of all items in the package.|
|/reload|without|Automatic calculation of total probability|

## deploy
> Configuration file location: tshock/ online package. json
```json
{
   "启用": true,
   "总概率(自动更新)": 60,
   "发放间隔/秒": 1800,
   "跳过生命上限": 500,
   "每次发放礼包记录后台": false,
   "礼包列表": [
    {
       "物品名称": "铂金币",
       "物品ID": 74,
       "所占概率": 1,
       "物品数量": [
        2,
        5
      ]
    }
  ],
     "触发序列": {
     "1": "[c/55CDFF:服主]送了你1个礼包" 
  }
}
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.