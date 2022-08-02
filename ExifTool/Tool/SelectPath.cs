using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class SelectPath
{
    /// <summary>
    /// 选择文件路径
    /// </summary>
    public static bool SelectFile(string Title, out string path)
    {
        OpenSelectFileName openFileName = new OpenSelectFileName();
        openFileName.structSize = Marshal.SizeOf(openFileName);
        openFileName.filter = "执行文件(*.exe)\0*.exe";
        openFileName.file = new string(new char[256]);
        openFileName.maxFile = openFileName.file.Length;
        openFileName.fileTitle = new string(new char[64]);
        openFileName.maxFileTitle = openFileName.fileTitle.Length;
        openFileName.initialDir = Environment.CurrentDirectory;
        openFileName.title = Title;
        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

        if (LocalDialog.GetSaveFileName(openFileName))
        {
            path = openFileName.file;
            return true;
        }
        else
        {
            path = string.Empty;
            return false;
        }
    }
    /// <summary>
    /// 调用WindowsExploer 并返回所选文件夹路径
    /// </summary>
    /// <param name="dialogtitle">打开对话框的标题</param>
    /// <returns>所选文件夹路径</returns>
    public static bool SelectFolder(string Title, out string path)
    {
        FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
        folderBrowser.Description = Title;
        folderBrowser.ShowNewFolderButton = false;
        folderBrowser.RootFolder = Environment.SpecialFolder.Desktop;

        DialogResult result = folderBrowser.ShowDialog();
        if (result == DialogResult.OK)
        {
            path = folderBrowser.SelectedPath;
            return true;
        }
        else
        {
            path = string.Empty;
            return false;
        }
    }
}