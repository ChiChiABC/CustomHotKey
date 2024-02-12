# 自定义热键(CustomHotKey)
自定义热键，提高使用效率。使用AvaloniaUI，MVVM设计模式开发。
# 系统要求
* 根据AvaloniaUI官网的描述，可以在Windows7运行，但不保证稳定性
* Windows 8.1
* Windows 10
* Windows 11
# 基本概念
## 热键组(HotKeyGroup)
每个热键组对应一个`.chkey`文件，它包含一个热键组需要的所有信息(例如触发条件，任务列表，名称等) 当热键组存储的热键都被按下时，会依次执行热键组存储的热键任务
### .chkey文件的结构
`.chkey`文件本质是json格式的文件，结构如下:
```json
{
  "Name": "GroupName", // 名称
  "Description": "GroupDescription", // 描述
  "HotKeys": [ // 需要被按下的热键
    67,
    72,
    75
  ],
  "KeyTasks": [ // 热键任务列表
    {
      "TaskType": "RunCommand", // 任务类型
      "Args": [ // 任务参数列表
        "notepad.exe"
      ]
    }
  ]
}
```
## 热键任务(KeyTask)
热键任务会在热键组内的热键都被按下(即满足触发条件)时执行，根据热键任务的类型不同，执行后的效果也不同。目前已有的热键任务类型：
* RunCommand (运行命令)
* OpenFile (打开文件)
## 热键任务参数(KeyTaskArg)
热键任务执行时会根据自身存储的热键参数产生不同效果，例如RunCommand任务的每一条参数都是会被执行的命令，OpenFile任务的每一条参数都是会被打开的文件。
