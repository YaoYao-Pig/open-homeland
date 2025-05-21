# Open WorldLand

## 第三方依赖

1. XChart：用于图表的管理和创建https://github.com/XCharts-Team/XCharts

2. LiteJson:用于Json读取，直接在Unity插件商店里面下载就好

## 项目展示：

![image-20250521201449447](assets/image-20250521201449447.png)

2025.5.21 添加了基于大模型的BGM生成以及音频可视化展示——“星环”

![image-20250521201332248](assets/image-20250521201332248.png)

## 路线设计

结合大模型完成视觉和听觉效果的提升：

1. 设计一套pipeline，通过输入项目的相关信息和数据，输出的是程序化控制地形的参数
2. 设计一套pipeine，通过输入项目的相关信息和数据，对应的输出对应的项目BGM。

也就是从视听两个角度，设计一个pipeline帮助我们更好的从可视化和可听效果上提高感知效果

## 项目介绍：

OpenHomeLand是一个通过视听效果展示开源世界的项目

本项目分成两个部分：

1. 视觉效果：主要使用了程序化的星球生成技术，针对不同的项目的指标生成独一无二的星球世界
2. 听觉效果：依靠文本到音频的大模型，根据项目的活跃度以及从项目的develop以及readme中总结出的RAG，提取项目promt以及活跃度，openRank序列。从而生成对应的音频BGM

## 前端部署

项目提供了一个前端部署页面，考虑到包体的大小，因此Build后的包将以压缩包的形式发布，并且也可以自己导出。前端结构如下：

\-- Web

\---- src

\---- unity

\---- index.html

\---- script.js

\---- style.cs 

打包的文件放在unity文件夹下面

```python
python -m http.server 8000
```
![](./Readme/f1.png)

## 演示视频

gif:(gif大小比较大，请等待加载)

![gif](https://github.com/YaoYao-Pig/open-homeland/blob/main/Readme/perform.gif)

视频：

[Video](./Readme/perform.mp4)

## PPT

复赛PPT位置在复赛文件夹下

## 开发日志

Log