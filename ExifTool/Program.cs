using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ExifToolHelper
{
    internal class Program
    {
        /// <summary>
        /// 工具路径 exe
        /// </summary>
        internal static string toolPath;
        /// <summary>
        /// 工具目录
        /// </summary>
        internal static string toolDirectory;
        /// <summary>
        /// 工作路径 要遍历的文件组的目录
        /// </summary>
        internal static string workPath;
        /// <summary>
        /// 要筛选的扩展名
        /// </summary>
        internal static string extension;

        [STAThread]
        static void Main(string[] args)
        {
            TryGetExiftool();

            if (SelectPath.SelectFolder("指定工作目录(对目录下所有符合条件的文件执行预制的命令)", out string localPath))
            {
                workPath = localPath;
                Console.WriteLine($"请输入文件后缀,对工作目录下的所有该后缀文件运行预制命令");
                extension = Console.ReadLine();

                SetCommand();

                RunExiftool();
            }
            else
            {
                Console.WriteLine($"未选择工作路径或者选择失败，输入任意键退出！");
                Console.ReadKey();
            }
        }

        /// <summary>
        ///检查有无工具
        /// </summary>
        internal static void TryGetExiftool()
        {
            var currentDirectory = Environment.CurrentDirectory;
            string normalPath1 = $"{currentDirectory}\\exiftool.exe";
            string normalPath2 = $"{currentDirectory}\\exiftool(-k).exe";
            string loaclConfig = $"{currentDirectory}\\ExiftoolHelper.txt";

            if (File.Exists(normalPath1))
                toolPath = normalPath1;
            else if (File.Exists(normalPath2))
                toolPath = normalPath2;
            else if (File.Exists(loaclConfig))
                using (StreamReader streamReader = new StreamReader(File.Open(loaclConfig, FileMode.Open)))
                    toolPath = streamReader.ReadToEnd();
            else if (SelectPath.SelectFolder("未找到exiftool.exe 请指定其位置", out string localPath))
            {
                toolPath = localPath;
                using (StreamWriter streamWriter = new StreamWriter(File.Create(loaclConfig)))
                    streamWriter.WriteLine(localPath);
            }

            toolDirectory = toolPath.Substring(0, toolPath.LastIndexOf('\\'));
        }
        /// <summary>
        /// 设置临时命令
        /// </summary>
        internal static void SetCommand()
        {
            List<string> fileList = new List<string>();
            foreach (var path in Directory.GetFiles(workPath))
                if (path.EndsWith(extension))
                    fileList.Add(path);

            List<string> commands = new List<string>();
            using (StreamReader streamReader = new StreamReader(File.Open($"{toolDirectory}\\command.txt", FileMode.Open)))
                while (streamReader.Peek() > -1)
                    commands.Add(streamReader.ReadLine());

            foreach (var path in fileList)
                foreach (var command in commands)
                {
                    if (command == "%path%")
                        File.AppendAllText($"{toolDirectory}\\temp.txt", $"{path}\n", Encoding.GetEncoding("GB2312"));
                    else
                        File.AppendAllText($"{toolDirectory}\\temp.txt", $"{command}\n", Encoding.UTF8);
                }

            File.Delete($"{toolDirectory}\\command.txt");
        }
        /// <summary>
        /// 运行工具
        /// </summary>
        private static void RunExiftool()
        {
            using (Process pExifTool = new Process())
            {
                string command = $"\"{toolPath}\" -@ temp.txt";
                pExifTool.StartInfo = new ProcessStartInfo("cmd", $"/c \"{@command}\"");
                pExifTool.StartInfo.RedirectStandardOutput = true;

                pExifTool.StartInfo.RedirectStandardError = true;
                pExifTool.ErrorDataReceived += new DataReceivedEventHandler(ETErrorHandler);

                pExifTool.StartInfo.UseShellExecute = false;
                pExifTool.StartInfo.CreateNoWindow = true;
                pExifTool.Start();
                pExifTool.BeginErrorReadLine();

                Console.WriteLine("开始运行脚本！");

                string line = string.Empty;
                do
                {
                    line = pExifTool.StandardOutput.ReadLine();
                    Console.WriteLine($"普通输出{line}");

                }
                while (line != null);

                Console.WriteLine("全部运行完成！");
                Console.ReadKey();
            }
        }
        /// <summary>
        /// 错误输出回调
        /// </summary>
        /// <param name="sendingProcess"></param>
        /// <param name="errLine"></param>
        private static void ETErrorHandler(object sendingProcess, DataReceivedEventArgs errLine)
        {
            if (!string.IsNullOrEmpty(errLine.Data))
                Console.WriteLine($"错误信息:{errLine.Data}");
        }
    }
}
