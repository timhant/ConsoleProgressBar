# ConsoleProgressBar
控制台进度条 .Net Console Progress Bar 

![image](https://github.com/HANTIAN4444/ConsoleProgressBar/blob/master/result/20200224.gif)  

按照自己的想法使用 C# 写了一个控制台的进度条，  
想要实现 pip 下载安装过程中的那种效果，  
已完成的代码在单线程下工作的ok，多线程下还有问题需要解决。  
  
如果有朋友正好能用上欢迎帮我优化完善代码，我也借着这个机会多多学习。  
: )  
  
2020-2-24  
在 PowerShell   中，使用静态锁的话会有若干进度条进度显示不出来的情况；  
在 Command      中，使用静态锁时没有发现什么问题；  
在 New Terminal 中，无法擦除原先的进度显示，每次更新都会重新打印一行；  
参考了几个 github 上 console progress bar 的例子（没有把所有的都研究一遍..）好像大家都没有考虑多线程时候的问题，难道是我想多了么？  
下面继续研究 PowerShell 和 New Terminal 中为什么会出现显示异常的原因。  
