using System;
using System.Collections.Generic;
using System.Linq;

namespace TestPlugin
{
    public class 奖励节
    {
        public int 球洞坐标X { get; set; }
        public int 球洞坐标Y { get; set; }
        public int 最少杆数 { get; set; }
        public int 最多杆数 { get; set; }
        public bool 本位置仅领取该奖励 { get; set; }
        public List<物品节> 物品节 { get; set; }
        public List<string> 指令节 { get; set; }

        public 奖励节(int x, int y, List<物品节> item, List<string> cmd)
        {
            球洞坐标X = x;
            球洞坐标Y = y;
            最少杆数 = 0;
            最多杆数 = 999;
            物品节 = item;
            指令节 = cmd;
            本位置仅领取该奖励 = false;
        }

        public 物品节 GetRandomItems()
        {
            if (物品节.Count == 0)
                return null;

            int maxValue = 物品节.Sum(t => t.相对概率);
            int num = LC.LRadom.Next(1, maxValue);
            foreach (物品节 item in 物品节)
            {
                if (item.相对概率 >= num)
                    return item;
                num -= item.相对概率;
            }
            return null;
        }

        public string GetRandomCMD()
        {
            if (指令节.Count == 0)
                return null;

            int count = 指令节.Count;
            int num = LC.LRadom.Next(1, count);
            return 指令节[num - 1];
        }

        public string 命中提示 = "高尔夫击中奖励球";
        public string 备注 = "";
    }
}

