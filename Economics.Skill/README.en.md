# Economics.skill skill plug -in

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Shao Si Ming
- Source: None
- A plugin that can release skills

> [! Note]
> Need to install front plug -ins: Economicsapi, Economics.rpg (this warehouse)
> Using AI style function may cause a large amount of bandwidth for force and
 
## Update log


```
V1.1.0.1
添加 无敌帧，锁定怪物，AI样式，传送玩家，移除画圆配置，改用循环实现
修复: 弹幕AI无法生效，持续时间无法生效

V1.0.0.1
修复:物品消耗
```

## Configuration Note

- In the trigger mode `Kill`   `Fight`   `initiative` three models, any of them cannot be combined, killing and hitting itself is an active manifestation.

## instruction

|grammar|Authority|illustrate|
| --------------------- |: -----------------------:|: ---------------------:|
|/Skill Buy [Skill Index]|economics.skill.use|Purchase skills|
|/Skill Del [Skill Index]|economics.skill.use|Unbinding skills|
|/Skill MS|economics.skill.use|View the binding skills|
|/skill deeely|economics.skill.use|All the skills to unbind the handheld weapon|
|/Skill Clear|economics.skill.use|Unbind all skills|
|/Skill Reset|economics.skill.admin|Reset|

## Configuration
> Configuration file location: TSHOCK/ECONOMICS/SKILL.JSON
```json
{
   "购买主动技能最大数量": 1,
   "单武器绑定最大技能数量": 1,
   "被动绑定最大技能数量": 4,
   "禁止拉怪表": [],
   "最大显示页": 20,
   "技能列表": [
    {
       "名称": "",
       "喊话": "",
       "技能唯一": false,
       "全服唯一": false,
       "技能价格": 0,
       "限制等级": [],
       "限制进度": [],
       "触发设置": {
         "触发模式": [
           "主动" //触发模式 CD 主动 打击 击杀 死亡 血量 蓝量 冲刺 装备 跳跃
        ],
         "冷却": 0,
         "血量": 0,
         "蓝量": 0,
         "物品条件": []
      },
       "伤害敌怪": {
         "伤害": 0,
         "范围": 0
      },
       "范围命令": {
         "命令": [],
         "范围": 0
      },
       "治愈": {
         "血量": 0,
         "魔力": 0,
         "范围": 0
      },
       "清理弹幕": {
         "启用": false,
         "范围": 0
      },
       "拉怪": {
         "X轴调整": 0,
         "Y轴调整": 0,
         "范围": 0
      },
       "传送": {
         "启用": false,
         "X轴位置": 0,
         "Y轴位置": 0
      },
       "无敌": {
         "启用": true, //无敌帧，不保证完全无敌，算是我留的小坑，而且不想改，觉得这样挺好。
         "时长": 2000
      },
       "范围Buff": {
         "Buff列表": [],
         "范围": 0
      },
       "弹幕": [
        {
           "弹幕ID": 132,
           "伤害": 40,
           "击退": 1.0,
           "起始角度": 2,
           "X轴起始位置": 0,
           "Y轴起始位置": 0,
           "X轴速度": 0.0,
           "Y轴速度": 0.0,
           "自动方向": true,
           "持续时间": -1,
           "AI": [
            0.0,
            0.0,
            0.0
          ],
           "AI样式": 0, //目前只有0有效
           "射速": 10.0, 
           "锁定怪物配置": {
             "启用": true,
             "弹幕锁定敌怪": true, 
             "以锁定敌怪为中心": true,
             "锁定血量最少": true, //不开启则锁定距离最近 锁定方式: Boss > 小怪
             "范围": 200
          },
           "延迟": 0,
           "弹幕循环": {
             "配置": [
              {
                 "次数": 5,
                 "X递增": 0,
                 "Y递增": 0,
                 "角度递增": 20,
                 "圆面半径": 20, 把老版本画圆挪过来了
                 "反向发射": false,
                 "延迟": 100,
                 "跟随玩家位置": false,
                 "根据角度计算新的点": true 配合画圆使用
              }
            ]
          }
        }
      ]
    }
  ]
}
```

## feedback

- Co -maintained plug -in library: https://github.com/Controllerdestiny/tshockplugin
- The official group of Trhub.cn or Tshock in the domestic community