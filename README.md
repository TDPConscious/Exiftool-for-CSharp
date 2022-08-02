# Exiftool-for-CSharp
如果使用exiftool向mp4 metadata 写入英文数据的话不会有问题 直接使用命令行批处理就行  
但如果写入中文 部分数据会出现吞字的情况 所以有了此工具  
核心方法来源于https://exiftool.org/forum/index.php?topic=11286.0  
感谢该社区人员提供的思路  
## 使用方法
exiftool目录下创建command.txt，并在command内部写入命令  
与命令行的命令相同，不过每个参数间隔都要换行，资源路径用 %path% 代替，例如:  

-ItemList:Title=Title  
-Microsoft:Subtitle=Subtitle  
-Microsoft:Category=Category1;Category2  
-ItemList:Comment=Comment  
%path%

更多示例可自行下载
