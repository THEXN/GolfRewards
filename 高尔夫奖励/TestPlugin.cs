using System;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace TestPlugin
{
    [ApiVersion(2, 1)]
    public class TestPlugin : TerrariaPlugin
    {
        public override string Name => "高尔夫奖励";
        public override string Author => "GK 阁下 由 鸽子 定制，肝帝熙恩更新适配1449";
        public override string Description => "将高尔夫打入球洞会得到奖励.";
        public override Version Version => new Version(1, 0, 0, 4);

        public TestPlugin(Main game) : base(game)
        {
            base.Order = 5;
            LC.RI();
        }

        private void OnInitialize(EventArgs args)
        {
            LC.RC();

            // 注册物块坐标确认命令
            Commands.ChatCommands.Add(new Command("物块坐标", CMD2, "物块坐标")
            {
                HelpText = "输入/物块坐标后敲击物块确认坐标"
            });

            // 注册重读高尔夫奖励配置命令
            Commands.ChatCommands.Add(new Command("重读高尔夫奖励", CMD, "reload")
            {
                HelpText = "输入/重读高尔夫奖励重读配置"
            });
        }


        public override void Initialize()
        {
            // 注册网络数据处理事件
            ServerApi.Hooks.NetGetData.Register(this, GetData);

            // 注册游戏初始化事件
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);

            // 注册玩家欢迎事件
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer);

            // 注册玩家离开事件
            ServerApi.Hooks.ServerLeave.Register(this, OnLeave);

            // 注册砖块编辑事件
            GetDataHandlers.TileEdit += TileEdit;

            // 注册高尔夫球落入球洞事件
            GetDataHandlers.LandGolfBallInCup += Golf;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 取消注册网络数据处理事件
                ServerApi.Hooks.NetGetData.Deregister(this, GetData);

                // 取消注册游戏初始化事件
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);

                // 取消注册玩家欢迎事件
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreetPlayer);

                // 取消注册玩家离开事件
                ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);

                // 取消注册砖块编辑事件
                GetDataHandlers.TileEdit -= TileEdit;

                // 取消注册高尔夫球落入球洞事件
                GetDataHandlers.LandGolfBallInCup -= Golf;
            }
            base.Dispose(disposing);
        }


        private void CMD(CommandArgs args)
		{
			LC.RC();
			args.Player.SendSuccessMessage("[高尔夫奖励] 重读配置完毕!");
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002818 File Offset: 0x00000A18
		private void CMD2(CommandArgs args)
		{
			LPlayer[] lplayers = LC.LPlayers;
			lock (lplayers)
			{
				bool flag2 = LC.LPlayers[args.Player.Index] != null;
				if (flag2)
				{
					LC.LPlayers[args.Player.Index].tip = true;
				}
			}
			args.Player.SendSuccessMessage("敲击物块以确认其坐标!");
		}


		private void OnGreetPlayer(GreetPlayerEventArgs e)
		{
			LPlayer[] lplayers = LC.LPlayers;
			lock (lplayers)
			{
				LC.LPlayers[e.Who] = new LPlayer(e.Who);
			}
		}


		private void OnLeave(LeaveEventArgs e)
		{
			LPlayer[] lplayers = LC.LPlayers;
			lock (lplayers)
			{
				bool flag2 = LC.LPlayers[e.Who] != null;
				if (flag2)
				{
					LC.LPlayers[e.Who] = null;
				}
			}
		}

        private void TileEdit(object sender, GetDataHandlers.TileEditEventArgs args)
        {
            if (args.Handled)
                return;

            TSPlayer tsplayer = TShock.Players[args.Player.Index];
            if (tsplayer == null)
                return;

            LPlayer[] lplayers = LC.LPlayers;
            lock (lplayers)
            {
                if (LC.LPlayers[args.Player.Index] != null && LC.LPlayers[args.Player.Index].tip)
                {
                    LC.LPlayers[args.Player.Index].tip = false;
                    tsplayer.SendInfoMessage($"目标坐标为: X{args.X} Y{args.Y}");
                }
            }
        }


        public void Golf(object sender, GetDataHandlers.LandGolfBallInCupEventArgs args)
        {
            if (args.Handled)
                return;

            var player = args.Player;
            if (player == null)
                return;

            int tileX = (int)args.TileX;
            int tileY = (int)args.TileY;
            int hits = (int)args.Hits;

            foreach (var 奖励节 in LC.LConfig.奖励表)
            {
                if (奖励节.球洞坐标X != tileX || 奖励节.球洞坐标Y != tileY)
                    continue;

                if (hits <= 奖励节.最少杆数 || hits > 奖励节.最多杆数)
                    continue;

                if (!string.IsNullOrEmpty(奖励节.命中提示))
                    TShock.Utils.Broadcast(奖励节.命中提示, byte.MaxValue, 215, 0);

                var randomItems = 奖励节.GetRandomItems();
                if (randomItems != null && randomItems.物品ID != 0 && randomItems.物品数量 > 0)
                    player.GiveItem(randomItems.物品ID, randomItems.物品数量, randomItems.物品前缀);

                var randomCMD = 奖励节.GetRandomCMD();
                if (!string.IsNullOrEmpty(randomCMD))
                {
                    bool cmdSuccess = Commands.HandleCommand(TSPlayer.Server, randomCMD.Replace("{name}", player.Name));
                    if (!cmdSuccess)
                        Console.WriteLine($"指令 {randomCMD} 执行失败！ ");
                }

                if (奖励节.本位置仅领取该奖励)
                    break;
            }
        }

        private void GetData(GetDataEventArgs args)
		{
		}
	}
}
