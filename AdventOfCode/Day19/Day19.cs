using AdventOfCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
internal class Day19 : Solution
{
    static Dictionary<string, Procedure> procedureMap = new Dictionary<string, Procedure>();
    static List<string> lines;
    static List<Part> parts = new List<Part>();

    static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
    static string fileName = Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}.txt");
    static string outputFile = Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}Simplified.txt");
    static long part2Total;
    static Day19()
    {
        lines = File.ReadAllLines(fileName).ToList();
        List<string> degenerateKeys = new List<string>();
        bool parseProcedures = true;
        int i = 0;
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                parseProcedures = false;
            }
            else
            {
                if (parseProcedures)
                {
                    var newProcedure = new Procedure(line);
                    procedureMap.Add(newProcedure.name, newProcedure);
                    if (newProcedure.IsDegenerateSink())
                        degenerateKeys.Add(newProcedure.name);
                }
                else
                {
                    parts.Add(new Part(line));
                }
            }
            i++;
        }



        foreach (string key in degenerateKeys)
        {
            foreach (var item in procedureMap)
            {
                item.Value.operations.Where(op => op.destination == key).ForEach(op =>
                {
                    Operation degenerateOperation = procedureMap[key].operations[0];
                    if (degenerateOperation.actionType == ActionType.Accept)
                        op.actionType = ActionType.Accept;
                    else
                        op.actionType = ActionType.Reject;

                    op.destination = degenerateOperation.destination;
                });
            }
            procedureMap.Remove(key);
        }
        degenerateKeys.Clear();

        foreach (var item in procedureMap)
        {
            if (item.Value.IsDegenerateSink())
                degenerateKeys.Add(item.Key);
        }
        foreach (string key in degenerateKeys)
        {
            foreach (var item in procedureMap)
            {
                item.Value.operations.Where(op => op.destination == key).ForEach(op =>
                {
                    Operation degenerateOperation = procedureMap[key].operations[0];
                    if (degenerateOperation.actionType == ActionType.Accept)
                        op.actionType = ActionType.Accept;
                    else
                        op.actionType = ActionType.Reject;

                    op.destination = degenerateOperation.destination;
                });
            }
            procedureMap.Remove(key);
        }
        degenerateKeys.Clear();

        foreach (var item in procedureMap)
        {
            int a = item.Value.CountDegenerateDefaultActions();
            if (a > 0)
            {
                int bb = 0;
            }
        }



        Stack<List<Operation>> fullOperations = new Stack<List<Operation>>();

        List<long> accepted = new List<long>();

        for (i = procedureMap["in"].operations.Count - 1; i >= 0; i--)
        {
            Procedure firstProcedure = procedureMap["in"];
            var operation = firstProcedure.operations[firstProcedure.operations.Count-1- i];
            fullOperations.Push(new List<Operation> { operation });
        }
        while (fullOperations.Count > 0)
        {
            var currentList = fullOperations.Pop();
            Operation lastOperation = currentList.Last();
            if (lastOperation.actionType == ActionType.Accept || lastOperation.actionType == ActionType.Reject)
            {
                if (lastOperation.actionType == ActionType.Accept)
                {
                    var passing = GetPassingCount(currentList);
                    // Console.Write(string.Join(" --> ", currentList));
                    // Console.WriteLine($" ==> {passing}");
                    accepted.Add( passing );
                }
            }
            else
            {
                var nextProcedure = procedureMap[lastOperation.destination];
                for (i = nextProcedure.operations.Count - 1; i >= 0; i--)
                {
                    var operation = nextProcedure.operations[i];
                    List<Operation> newList = new List<Operation>(currentList);
                    for (int j = 0; j < i; j++)
                    {
                        var additionalOperation = nextProcedure.operations[j];
                        Parameter newParameter = additionalOperation.parameter; 
                        ConditionType newCondition;
                        int newCompareValue;

                        if (additionalOperation.conditionType == ConditionType.LessThan)
                        {
                            newCondition = ConditionType.GreaterThan;
                            newCompareValue = additionalOperation.compareValue - 1;
                        }
                        else
                        {
                            newCondition = ConditionType.LessThan;
                            newCompareValue = additionalOperation.compareValue + 1;
                        }
                        var prereq = new Operation()
                        {
                            parameter = newParameter,
                            destination = additionalOperation.destination,
                            conditionType = newCondition,
                            compareValue = newCompareValue,
                        };
                        newList.Add(prereq);
                    }
                    newList.Add(operation);
                    fullOperations.Push(newList);
                }
            }
        }

        part2Total = accepted.Sum();
        //Console.WriteLine(accepted.Sum());
    }
    static long GetPassingCount(List<Operation> operations)
    {
        long minX = 1, maxX = 4000;
        long minM = 1, maxM = 4000;
        long minA = 1, maxA = 4000;
        long minS = 1, maxS = 4000;
        foreach (var operation in operations)
        {
            switch (operation.parameter)
            {
                case Parameter.x:
                    if (operation.conditionType == ConditionType.GreaterThan)
                    {
                        minX = Math.Max(minX, operation.compareValue + 1);
                    }
                    else
                    {
                        maxX = Math.Min(maxX, operation.compareValue - 1);
                    }
                    break;
                case Parameter.m:
                    if (operation.conditionType == ConditionType.GreaterThan)
                    {
                        minM = Math.Max(minM, operation.compareValue + 1);
                    }
                    else
                    {
                        maxM = Math.Min(maxM, operation.compareValue - 1);
                    }
                    break;
                case Parameter.a:
                    if (operation.conditionType == ConditionType.GreaterThan)
                    {
                        minA = Math.Max(minA, operation.compareValue + 1);
                    }
                    else
                    {
                        maxA = Math.Min(maxA, operation.compareValue - 1);
                    }
                    break;
                case Parameter.s:
                    if (operation.conditionType == ConditionType.GreaterThan)
                    {
                        minS = Math.Max(minS, operation.compareValue + 1);
                    }
                    else
                    {
                        maxS = Math.Min(maxS, operation.compareValue - 1);
                    }
                    break;
            }
        }

       // Console.Write($"[x({minX} - {maxX}), m({minM} - {maxM}), a({minA} - {maxA}), s({minS} - {maxS})]");
        return (maxX - minX + 1) * (maxM - minM + 1) * (maxA - minA + 1) * (maxS - minS + 1);
    }

    public override string SolvePart1()
    {
        foreach (Part part in parts)
        {
            procedureMap["in"].Execute(part);
        }
        return parts.Where(part => part.result == Result.Accepted).Select(part => part.GetRating()).Sum().ToString();
    }

    public override string SolvePart2()
    {
        return part2Total.ToString();
    }

    private void PruneGraph()
    {
        var keys = procedureMap.Keys.ToArray();
        foreach (string name in keys)
        {
            var procedure = procedureMap[name];
        }
    }

    class Procedure
    {
        int depth = 0;
        public string name;
        readonly string sourceString;
        public List<Operation> operations = new List<Operation>();
        public Procedure(string s)
        {
            this.sourceString = s;
            var entries = s.Split('{');
            name = entries[0];
            var procedureStrings = entries[1].Substring(0, entries[1].Length - 1).Split(',');
            procedureStrings.ForEach(s =>
            {
                operations.Add(new Operation(s, operations.Count > 0 ? operations[operations.Count - 1] : null));
            });
        }

        public void Execute(Part part)
        {
            foreach (var operation in operations)
            {
                if (operation.TestPart(part))
                {
                    operation.ActOnPart(part);
                    break;
                }
            }
        }

        public override string ToString()
        {
            return sourceString;
        }

        internal bool IsDegenerateSink()
        {
            bool a = operations.All(op => op.actionType == ActionType.Accept);
            bool b = operations.All(op => op.actionType == ActionType.Reject);
            bool c = operations.All(op => op.actionType == ActionType.Send && op.destination == operations[0].destination);
            return a || b || c;
        }

        internal int CountDegenerateDefaultActions()
        {
            int defaultCases = 0;
            string defaultAction = operations.Last().destination;
            for (int i = operations.Count - 2; i >= 0; i--)
            {
                if (operations[i].destination == defaultAction)
                {
                    defaultCases++;
                }
                else
                    break;
            }
            return defaultCases;
        }
    }

    class Operation
    {
        readonly string sourceString;
        public string destination;
        public  Parameter parameter;
        public  ConditionType conditionType;
        public ActionType actionType;
        public  int compareValue;

        public Operation()
        {

        }

        public Operation(string s, Operation lastOperation)
        {
            this.sourceString = s;

            if (s.Contains(':'))
            {
                string split;
                if (s.Contains("<"))
                {
                    split = "<";
                    conditionType = ConditionType.LessThan;
                }
                else
                {
                    split = ">";
                    conditionType = ConditionType.GreaterThan;
                }

                var conditionalStrings = s.Split(":");
                s = conditionalStrings[0];
                string[] compareStrings = s.Split(split);
                parameter = (Parameter)compareStrings[0][0];
                compareValue = int.Parse(compareStrings[1]);


                s = conditionalStrings[1];
                if (s.Contains("A"))
                {
                    destination = "A";
                    actionType = ActionType.Accept;
                }
                else if (s.Contains("R"))
                {
                    destination = "R";
                    actionType = ActionType.Reject;
                }
                else
                {
                    actionType = ActionType.Send;
                    destination = s;
                }
            }
            else  // unconditional
            {
                parameter = lastOperation.parameter;
                if (lastOperation.conditionType == ConditionType.LessThan)
                {
                    conditionType = ConditionType.GreaterThan;
                    compareValue = lastOperation.compareValue - 1;
                }
                else
                {
                    conditionType = ConditionType.LessThan;
                    compareValue = lastOperation.compareValue + 1;
                }


                if (s.Contains("A"))
                {
                    actionType = ActionType.Accept;
                    destination = "A";
                }
                else if (s.Contains("R"))
                {
                    actionType = ActionType.Reject;
                    destination = "R";
                }
                else
                {
                    actionType = ActionType.Send;
                    destination = s;
                }
            }
        }

        public bool TestPart(Part part)
        {
            switch (conditionType)
            {
                case ConditionType.LessThan:
                    return part.GetParameter(parameter) < compareValue;
                case ConditionType.GreaterThan:
                    return part.GetParameter(parameter) > compareValue;
            }
            return true;
        }

        public void ActOnPart(Part part)
        {
            switch (actionType)
            {
                case ActionType.Accept:
                    part.result = Result.Accepted;
                    break;
                case ActionType.Reject:

                    part.result = Result.Rejected;
                    break;
                case ActionType.Send:
                    procedureMap[destination].Execute(part);
                    break;
            }
        }

        public override string ToString()
        {
            if (conditionType == ConditionType.LessThan)
                return $"{parameter}<{compareValue}:{destination}";
            else if (conditionType == ConditionType.GreaterThan)
                return $"{parameter}>{compareValue}:{destination}";
            else
                return $"{destination}";
        }

    }



    class Part
    {
        public Result result = Result.None;

        int x, m, a, s;
        public int GetParameter(Parameter parameter)
        {
            switch (parameter)
            {
                case Parameter.x:
                    return x;
                case Parameter.m:
                    return m;
                case Parameter.a:
                    return a;
                case Parameter.s:
                    return s;
            }
            return -1;
        }

        public Part(string s)
        {
            var entries = s.Substring(1, s.Length - 2).Split(',');
            entries.ForEach(entry =>
            {
                var entries = entry.Split("=");
                switch (entries[0])
                {
                    case "x":
                        x = int.Parse(entries[1]);
                        break;
                    case "m":
                        m = int.Parse(entries[1]);
                        break;
                    case "a":
                        a = int.Parse(entries[1]);
                        break;
                    case "s":
                        this.s = int.Parse(entries[1]);
                        break;
                    default:
                        break;
                }
            });
        }

        public int GetRating()
        {
            return x + m + a + s;
        }

        public override string ToString()
        {
            return $"{{x={x},m={m},a={a},s={s}}}: {result.ToString()}";
        }
    }

    enum Parameter { x = 'x', m = 'm', a = 'a', s = 's' };

    enum ConditionType { LessThan, GreaterThan, None };
    enum ActionType { Accept, Reject, Send };

    enum Result { Accepted, Rejected, None };
}
