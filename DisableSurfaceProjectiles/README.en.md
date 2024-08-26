## Surface projectiles prohibit surface barrage.

- Author: feather science
- - 出处: [github](https://github.com/1242509682/ProhibitSurfaceProjectiles) 
- This is a Tshock server plug-in, which is mainly used to prohibit the generation of barrage when players in the server are on the surface of the world.
- Especially for those who maliciously use explosives to destroy the server map.

## Update log

```
- 1.0.6
- 补全卸载函数
- 1.0.5
- 移除了计时器，使用OnWorldload方法实现加载完地图后再创建配置文件，
- 方便计算出准确的Main.worldSurface地表值
- 1.0.4
- 添加了个计时器，20秒后再创建计算好Main.worldSurface世界地表值的配置文件，
- 配置文件可支持正常地图与颠倒世界，将指令同步到配置文件总开关。
- 1.0.3
- 给插件加了个指令开关与权限，并在开启时获取所有ID带有名称的列表，
- 名字显示不全的改为“未知”。
- 开关指令名：/禁地表弹幕 （该指令的权限同名）
- 1.0.2
- 对config预设了更多的弹幕类型，涵盖了主要破坏地图的手段
- 1.0.1
- 加入了Config配置文件，玩家可通过Config设置拦截的弹幕ID
```
## instruction

|grammar|limit of authority|explain|
| -------------- |:-----------------:|:------:|
|/reload|without|Overloaded configuration file|
|/forbidden table barrage|Off-limits watch barrage|Function switch|
|without|Inspection-free surface barrage|Do not test it.|

## deploy
> Configuration file location: tshock/ forbidden list barrage.json.
```json
   "配置说明1": "(注意：颠倒和正常地表只能开启一个，高度阈值数值649为1倍 正常种子：大世界10384（16倍）",
   "配置说明2": "(颠倒地图种子：小世界25960（40倍）中世界31476（48.5倍） 大世界35370（54.5倍）",
   "启用": true,
   "开启正常高度限制": true,
   "正常限制高度阈值": 10384,
   "开启颠倒高度限制": false,
   "颠倒限制高度阈值": 25960
   "禁用地表弹幕id": [
    28,
    29,
    37,
    65,
    68,
    99,
    108,
    136,
    137,
    138,
    139,
    142,
    143,
    144,
    146,
    147,
    149,
    164,
    339,
    341,
    354,
    453,
    516,
    519,
    637,
    716,
    718,
    727,
    773,
    780,
    781,
    782,
    783,
    784,
    785,
    786,
    787,
    788,
    789,
    790,
    791,
    792,
    796,
    797,
    798,
    799,
    800,
    801,
    804,
    805,
    806,
    807,
    809,
    810,
    863,
    868,
    869,
    904,
    905,
    906,
    910,
    911,
    949,
    1013,
    1014
  ]
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.