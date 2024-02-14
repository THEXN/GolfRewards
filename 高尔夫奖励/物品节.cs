using System;

namespace TestPlugin
{
    public class 物品节
    {
        public int 物品ID { get; set; }
        public int 物品数量 { get; set; }

        public 物品节(int id, int stack, int prefix)
        {
            物品ID = id;
            物品数量 = stack;
            物品前缀 = prefix;
        }

        public int 相对概率 = 1;
        public int 物品前缀 = 0;
        public string 备注 = "";
    }
}
