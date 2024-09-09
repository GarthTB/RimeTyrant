# [Rime词库管理器](https://github.com/GarthTB/RimeTyrant)

跨平台、高性能的Rime输入法词库管理器

## 环境依赖（Windows）

- [.NET 8.0运行时](https://dotnet.microsoft.com/zh-cn/download/dotnet/8.0)

## 功能

- 添加：在词库中添加一个条目（词库有该词则词变红，有多码可选则码变红）
- 删除：在词库中删除一个条目
- 截短：将选中的词放到当前搜索的编码上，如果这个位置原本有码，则将其加长到对应的最短空码上
- 应用并保存：应用当前已完成的所有改动，覆写原词库文件
- 日志：记录所有的改动，以便查错和回溯

## 注意

不支持空格等空字符，不支持char类型不能包含的特别生僻字，不支持非Rime格式的词库文件。

保存词库时会按编码和优先级重排，所有不符合格式的行（包括注释和文件头）会堆在前面。

软件启动时，自动依次加载以下位置的词库。若某个位置中有多个词库，或一个词库也没有，则会提示“未能自动加载”：

- 程序上级目录（仅Win）
- Rime默认用户目录

一次性加载词库，点击保存时覆盖保存。其间任何在软件外的修改都会被覆盖。

清单刷新时，若未点击保存，则：

- 在清单中的手动修改**立即灭失**
- 添加、删除、截短的结果**不会**立即灭失

## 编码方案加入条件

1. 单字库和词库分离
2. 能从一个词中取出所有参与编码的字符
3. 能从取出的字符中挑出所有参与编码的码元
4. 能仅靠挑出的码元得到这个词的所有全码
5. 能靠某个全码推出其对应的指定长度的短码

---

## 更新日志

### [0.1.0] - 202409

- 发布！

---

## 前身

### [词器清单版](https://github.com/GarthTB/RimeLibrarian) - (20240622-20240901)

- WPF框架，仅适用于Windows
- 依赖[.NET 6.0运行时](https://dotnet.microsoft.com/zh-cn/download/dotnet/6.0)
- 仅适用于键道6输入法

### [词器v2](https://github.com/GarthTB/JDLibManager) - (20240605-20240620)

- WPF框架，仅适用于Windows
- 依赖[.NET 6.0运行时](https://dotnet.microsoft.com/zh-cn/download/dotnet/6.0)
- 仅适用于键道6输入法

### [词器v1](https://github.com/GarthTB/CiQi) - (20230513-20231223)

- WinForm框架，仅适用于Windows
- 依赖[.NET 6.0运行时](https://dotnet.microsoft.com/zh-cn/download/dotnet/6.0)
- 仅适用于键道6输入法