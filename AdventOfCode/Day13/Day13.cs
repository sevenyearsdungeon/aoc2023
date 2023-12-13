using AdventOfCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Day13 : Solution
{
    static List<Panel> panels = new List<Panel>();
    static Day13()
    {
        Queue<string> lines = new Queue<string>(File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\day13\\day13.txt")).ToList());
        int lastStartIndex = 0;
        List<string> currentPanel = new List<string>();
        while (lines.Count > 0)
        {
            string line = lines.Dequeue();
            if (string.IsNullOrEmpty(line))
            {
                panels.Add(new Panel(currentPanel));
                currentPanel.Clear();
            }
            else
            {
                currentPanel.Add(line);
            }
        }

        panels.Add(new Panel(currentPanel));
    }

    public override string SolvePart1()
    {
        return panels.Select(p => p.GetReflectionScore())
            .Sum(s => s)
            .ToString();
    }

    public override string SolvePart2()
    {
        foreach (var item in panels)
        {
            item.FindNewIndexSmudge();
        }
        return panels.Select(p => p.GetReflectionScore())
            .Sum(s => s)
            .ToString();
    }

    class Panel
    {
        private List<string> lines;
        public bool verticalReflection = false;
        public int reflectionIndex = -1;

        public Panel(List<string> lines)
        {
            this.lines = new List<string>(lines);
            int index = GetReflectionIndex();
            if (index < 0)
            {
                Transpose();
                index = GetReflectionIndex();
            }
        }

        private void Transpose()
        {
            verticalReflection = !verticalReflection;
            List<string> newLines = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < lines[0].Length; i++)
            {
                stringBuilder.Clear();
                for (int j = 0; j < lines.Count; j++)
                {
                    stringBuilder.Append(lines[j][i]);
                }
                newLines.Add(stringBuilder.ToString());
            }
            this.lines = newLines;
        }

        // 29846
        public int GetReflectionScore()
        {
            return (reflectionIndex + 1) * (verticalReflection ? 1 : 100);
        }

        private int GetReflectionIndex(int ignoreIndex = -1)
        {
            for (int i = 0; i < lines.Count - 1; i++)
            {
                bool isReflection = true;
                for (int j = 0; j <= i; j++)
                {
                    int checkLine = i - j;
                    int mirrorLine = i + 1 + j;
                    if (checkLine >= 0 && checkLine < lines.Count
                        && mirrorLine >= 0 && mirrorLine < lines.Count)
                    {
                        if (lines[checkLine] != lines[mirrorLine])
                        {
                            isReflection = false;
                            break;
                        }
                    }
                }
                if (isReflection && i!=ignoreIndex)
                {
                    reflectionIndex = i;

                    return i;
                }
            }
            return -1;
        }

        public int FindNewIndexSmudge()
        {
            int originalReflectionIndex = reflectionIndex;
            for (int i = 0; i < lines.Count - 1; i++)
            {
                for (int j = 0; j < lines[0].Length; j++)
                {
                    var charList = lines[i].ToList();
                    charList[j] = Invert(charList[j]);
                    lines[i] = new string(charList.ToArray());

                    int newIndex = GetReflectionIndex(originalReflectionIndex);
                    charList[j] = Invert(charList[j]);
                    lines[i] = new string(charList.ToArray());
                    if (newIndex != -1 && newIndex != originalReflectionIndex)
                    {
                        return newIndex;
                    }
                }
            }

            Transpose();

            for (int i = 0; i < lines.Count - 1; i++)
            {
                for (int j = 0; j < lines[0].Length; j++)
                {
                    var charList = lines[i].ToList();
                    charList[j] = Invert(charList[j]);
                    lines[i] = new string(charList.ToArray());

                    int newIndex = GetReflectionIndex();
                    charList[j] = Invert(charList[j]);
                    lines[i] = new string(charList.ToArray());
                    if (newIndex != -1)
                    {
                        return newIndex;
                    }
                }
            }
            return -1;
        }

        private char Invert(char v)
        {
            return v == '.' ? '#' : '.';
        }
        public override string ToString()
        {
            return string.Join(Environment.NewLine, lines);
        }
    }
}

