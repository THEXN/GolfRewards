using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TestPlugin
{
    // 配置文件类
    public class ConfigFile
    {
        // 读取配置文件
        public static ConfigFile Read(string path)
        {
            // 如果文件不存在，则返回默认配置
            if (!File.Exists(path))
            {
                return new ConfigFile
                {
                    奖励表 = new List<奖励节>
                    {
                        new 奖励节(0, 0, new List<物品节> { new 物品节(757, 1, 0) }, new List<string> { "/time noon" }),
                        new 奖励节(0, 0, new List<物品节> { new 物品节(757, 1, 0) }, new List<string> { "/time night" })
                    }
                };
            }

            // 从文件中读取配置
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var streamReader = new StreamReader(fileStream))
            {
                var configFile = JsonConvert.DeserializeObject<ConfigFile>(streamReader.ReadToEnd());
                // 如果有自定义配置处理方法，则执行
                ConfigR?.Invoke(configFile);
                return configFile;
            }
        }

        // 从流中读取配置
        public static ConfigFile Read(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                var configFile = JsonConvert.DeserializeObject<ConfigFile>(streamReader.ReadToEnd());
                ConfigR?.Invoke(configFile);
                return configFile;
            }
        }

        // 写入配置文件
        public void Write(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                Write(fileStream);
            }
        }

        // 写入配置到流
        public void Write(Stream stream)
        {
            var value = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (var streamWriter = new StreamWriter(stream))
            {
                streamWriter.Write(value);
            }
        }

        // 奖励表
        public List<奖励节> 奖励表 = new List<奖励节>();

        // 自定义配置处理方法
        public static Action<ConfigFile> ConfigR;
    }
}
