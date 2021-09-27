/* File Comments
 *****************************************************************************
 * File        : ConsoleProgressBar.cs
 * Description : 控制台进度条
 * Version     : 1.0
 * Created     : 2020/02/23
 * Author      : Hant
 ****************************************************************************/

using System;
using System.Text;

namespace Hant.Helper
{
    /// <summary>
    /// 控制台进度条
    /// </summary>
    public class ConsoleProgressBar
    {
        /// <summary>
        /// 进度描述格式化模板
        /// </summary>
        private const string DESC_FORMAT = "\0{0,4:F0}%\n";

        /// <summary>
        /// 进度描述内容的长度
        /// </summary>
        private const int DESC_LENGTH = 5;

        /// <summary>
        /// 同步锁
        /// </summary>
        private static readonly object locker = new object();

        private string title;
        private char blockContent;
        private int blockWidth;
        private double minValue;
        private double maxValue;
        private int barLeft;
        private int barTop;
        private int blockStartIndex;
        private double blockSingleModulus;
        private int descStartIndex;
        private char[] output;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="title">进度条标题</param>
        /// <param name="blockContent">进度符内容</param>
        /// <param name="blockWidth">进度符区域的宽度</param>
        /// <param name="minValue">最小进度值</param>
        /// <param name="maxValue">最大进度值</param>
        public ConsoleProgressBar(
            string title = "",
            char blockContent = '*',
            int blockWidth = 32,
            double minValue = 0d,
            double maxValue = 100d)
        {
            this.title = String.Empty.Equals(title) ? title : $"{title}:\0";
            this.blockContent = blockContent;
            this.blockWidth = blockWidth;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.blockSingleModulus = ((maxValue - minValue) / blockWidth);

            this.barLeft = Console.CursorLeft;
            this.barTop = Console.CursorTop;
            this.blockStartIndex = this.title.Length + 1;
            this.descStartIndex = this.title.Length + this.blockWidth + 2;

            this.output = CreateOutput(
                this.title,
                this.blockWidth);
        }

        /// <summary>
        /// 构建输出
        /// </summary>
        /// <param name="title">进度条标题</param>
        /// <param name="blockWidth">进度符区域的宽度</param>
        /// <returns>输出内容</returns>
        private char[] CreateOutput(
            in string title,
            in int blockWidth)
        {
            var contentLength = title.Length + 2 + blockWidth + DESC_LENGTH;
            var strBuilder = new StringBuilder(contentLength);

            strBuilder.Append(title);
            strBuilder.Append('[');

            for (int i = 0; i < blockWidth; i++)
            {
                strBuilder.Append('\0');
            }

            strBuilder.Append(']');
            strBuilder.Append(String.Format(DESC_FORMAT, 0));

            return strBuilder.ToString().ToCharArray();
        }

        /// <summary>
        /// 更新输出
        /// </summary>
        /// <param name="output">输出内容</param>
        /// <param name="blockStartIndex">进度符起始索引</param>
        /// <param name="singleBlockProgress">单一进度符表示的进度</param>
        /// <param name="fillModulus">填充系数</param>
        /// <returns>更新后的输出内容</returns>
        private char[] UpdateOutput(
            in char[] output,
            in int blockWidth,
            in int blockStartIndex,
            in char blockContent,
            in double singleBlockProgress,
            in int descStartIndex,
            in double fillModulus)
        {
            for (int i = 0; i < blockWidth; i++)
            {
                output[blockStartIndex + i] =
                    singleBlockProgress * i <= fillModulus ?
                    blockContent : '\0';
            }

            var desc = String.Format(DESC_FORMAT, fillModulus).ToCharArray();

            for (int i = 0; i < desc.Length - 1; i++)
            {
                output[descStartIndex + i] = desc[i];
            }

            return output;
        }

        /// <summary>
        /// 更新进度
        /// </summary>
        /// <param name="value">当前进度值</param>
        public void UpdateProgress(double value)
        {
            lock (locker)
            {
                Console.CursorVisible = false;

                var currentCursorPosition =
                   (left: Console.CursorLeft, top: Console.CursorTop);
                var fillModulus = CalculateFillModulus(
                    value,
                    this.minValue,
                    this.maxValue);
                this.output = UpdateOutput(
                    this.output,
                    this.blockWidth,
                    this.blockStartIndex,
                    this.blockContent,
                    this.blockSingleModulus,
                    this.descStartIndex,
                    fillModulus);

                EraseOutput(this.barLeft, this.barTop, this.output.Length);

                // reset cursor to bar start position
                Console.SetCursorPosition(this.barLeft, this.barTop);

                DisplayOutput(this.output);

                // reset cursor to original position
                Console.SetCursorPosition(
                    currentCursorPosition.left,
                    currentCursorPosition.top);

                Console.CursorVisible = true;
            }
        }

        /// <summary>
        /// 计算填充系数
        /// </summary>
        /// <param name="value">当前进度值</param>
        /// <param name="minValue">最小进度值</param>
        /// <param name="maxValue">最大进度值</param>
        /// <returns>填充系数</returns>
        private double CalculateFillModulus(
            in double value,
            in double minValue,
            in double maxValue)
        {
            return value >= minValue ?
                ((value - minValue) / (maxValue - minValue)) * 100 : 0;
        }

        /// <summary>
        /// 显示输出内容
        /// </summary>
        private void DisplayOutput(char[] output)
        {
            Console.Write(this.output);
        }

        /// <summary>
        /// 抹去输出内容
        /// </summary>
        /// <param name="cursorLeft">光标左起位置</param>
        /// <param name="cursorTop">光标上起位置</param>
        /// <param name="width">要擦除的宽度</param>
        public void EraseOutput(
            in int cursorLeft,
            in int cursorTop,
            in int width)
        {
            Console.SetCursorPosition(cursorLeft, cursorTop);

            for (int i = 0; i < width; i++)
            {
                Console.Write("\0");
            }
        }

        /// <summary>
        /// 显示进度条
        /// </summary>
        /// <returns>当前实例</returns>
        public ConsoleProgressBar Show()
        {
            lock (locker)
            {
                DisplayOutput(this.output);
                return this;
            }
        }
    }
}