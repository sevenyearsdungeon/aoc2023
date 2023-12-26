using AdventOfCode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

internal class Day20 : Solution
{

    static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
    static string inputFilePath = Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}.txt");
    static string outputFilePath = Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}output.txt");
    static Queue<Signal> signalQueue = new Queue<Signal>();

    static Dictionary<string, Module> moduleLookup = new Dictionary<string, Module>();
    static Day20()
    {
        var lines = File.ReadAllLines(inputFilePath).ToList();
        foreach (var line in lines) // generate modules
        {
            Module newModule = ParseModule(line);
            moduleLookup.Add(newModule.name, newModule);
        }
        foreach (var name in moduleLookup.Values.SelectMany(mod => mod.receiverNames).ToList()) // create sink modules
        {

            if (!moduleLookup.ContainsKey(name))
                moduleLookup.Add(name, new SignalSink(name));
        }
        foreach (var module in moduleLookup.Values) // configure recievers
        {
            module.SetReceivers(module.receiverNames.Select(name =>
            {
                Module receiever = moduleLookup[name];
                return receiever;
            }));
        }
        foreach (var receiverModule in moduleLookup.Values) // configure senders
        {

            IEnumerable<Module> inputModules = moduleLookup.Values.Where(module => module.receivers.Contains(receiverModule));
            receiverModule.senders = inputModules.ToList();
            if (receiverModule is Conjuction)
            {
                receiverModule.SetInputs(inputModules);
            }
        }
    }

    static Module ParseModule(string s)
    {
        string moduleDescription = s.GetStringBefore("->");
        string[] receiverNames = s.GetSequenceAfter("->", ",", StringSplitOptions.TrimEntries);

        if (moduleDescription.StartsWith(Broadcaster.symbol))
        {
            return new Broadcaster() { receiverNames = receiverNames, name = Broadcaster.symbol };
        }
        else if (moduleDescription.StartsWith(FlipFlop.symbol))
        {
            return new FlipFlop() { receiverNames = receiverNames, name = moduleDescription.GetStringAfter(FlipFlop.symbol, StringSplitOptions.TrimEntries) };
        }
        else if (moduleDescription.StartsWith(Conjuction.symbol))
        {
            return new Conjuction() { receiverNames = receiverNames, name = moduleDescription.GetStringAfter(Conjuction.symbol, StringSplitOptions.TrimEntries) };
        }
        return null;
    }

    public override string SolvePart1()
    {
        int signalCountLow = 0;
        int signalCountHigh = 0;
        for (int i = 0; i < 1000; i++)
        {
            moduleLookup[Broadcaster.symbol].Send(false);
            signalCountLow++;
            while (signalQueue.Count > 0)
            {
                Signal signal = signalQueue.Dequeue();
                if (signal.pulse)
                    signalCountHigh++;
                else
                    signalCountLow++;
                ProcessSignal(signal, false);
            }
        }

        return (signalCountLow * signalCountHigh).ToString();
    }
    // &kh -> rx
    // &hz -> kh
    // &hd -> hp,js,hz
    // %jk -> zq,hd
    // %tl -> jk,hd
    // %kj -> tl,hd
    // %pb -> hd,kj
    // %gr -> hd,pb
    // %mv -> hd,gr
    // %js -> mv

    // &hz -> &kh -> rx

    // &hd -> hp,js,hz
    // %jk -> zq,hd
    // %tl -> jk,hd
    // %kj -> tl,hd
    // %pb -> hd,kj
    // %gr -> hd,pb
    // %mv -> hd,gr
    // %js -> mv


    public override string SolvePart2()
    {
        long a = ButtonPressesToGetTo(moduleLookup["hz"]) *
            ButtonPressesToGetTo(moduleLookup["xm"]) *
            ButtonPressesToGetTo(moduleLookup["pv"]) *
            ButtonPressesToGetTo(moduleLookup["qh"]);

        return a.ToString();
    }

    long ButtonPressesToGetTo(Module module)
    {
        Reset();
        int i = 1;
        int sigCount = 0;
        long i1=0, i2;
        bool run = true;
        for (; run; i++)
        {
            moduleLookup[Broadcaster.symbol].Send(false);
            while (signalQueue.Count > 0)
            {
                Signal signal = signalQueue.Dequeue();
                if (signal.sender == module)
                {
                    if (signal.pulse)
                    {
                        sigCount++;
                       // Debug.WriteLine($"{i} {sigCount} {module}");
                        if (sigCount == 1)
                        {
                            i1 = i;
                        }
                        if (sigCount == 2)
                        {
                            i2 = i;
                            run = false;
                        }
                    }
                }
                ProcessSignal(signal, false);
            }
        }
        return i1;
    }
    // 3910816774656000 wrong
    // 3908827549440000 too high
    // 3906849509636437 wrong


    void Reset()
    {
        moduleLookup.Values.ForEach(module => module.Reset());
        signalQueue.Clear();
    }


    private void ProcessSignal(Signal signal, bool debug = false)
    {
        if (debug) { Debug.WriteLine(signal); }
        signal.receiver.Receive(signal.sender, signal.pulse);
    }

    class Signal
    {
        public Module sender;
        public Module receiver;
        public bool pulse;

        public Signal(Module sender, Module receiver, bool signal)
        {
            this.sender = sender;
            this.receiver = receiver;
            this.pulse = signal;
        }

        public override string ToString()
        {
            return $"{sender.name} -{(pulse ? "high" : "low")}-> {receiver.name}";
        }
    }

    abstract class Module
    {
        public string name;
        public string[] receiverNames;
        public List<Module> receivers; // modules that this module sends to 
        public List<Module> senders;  // modules taht send to this module

        public abstract void Receive(Module inputModule, bool signal);
        public virtual void Send(bool signal)
        {
            receivers.ForEach(receiver => signalQueue.Enqueue(new Signal(this, receiver, signal)));
        }

        public void SetReceivers(IEnumerable<Module> receiverModules) => receivers = receiverModules.ToList();

        public virtual void SetInputs(IEnumerable<Module> inputModules) { }

        public virtual void Reset() { }

    }

    class Broadcaster : Module
    {
        internal static string symbol = "broadcaster";

        public override void Receive(Module inputModule, bool signal) { }

        public override string ToString() => $"broadcaster -> " + string.Join(",", receiverNames);
    }

    class FlipFlop : Module
    {
        bool flipFlopState = false;
        public static readonly char symbol = '%';
        public override void Receive(Module inputModule, bool signal)
        {
            if (signal)
                return;

            flipFlopState = !flipFlopState;
            Send(flipFlopState);
        }
        public override string ToString() => symbol + name + $" -> " + string.Join(",", receiverNames);
        public override void Reset()
        {
            flipFlopState = false;
        }
    }

    class Conjuction : Module
    {
        public static readonly char symbol = '&';
        Dictionary<Module, bool> inputBuffer = new Dictionary<Module, bool>();
        public override void Receive(Module inputModule, bool signal)
        {
            inputBuffer[inputModule] = signal;
            Send(!inputBuffer.Values.All(b => b));
        }

        public override void SetInputs(IEnumerable<Module> inputModules)
        {
            inputModules.ForEach(im => inputBuffer.Add(im, false));
        }

        public override string ToString() => symbol + name + $" -> " + string.Join(",", receiverNames);

        public override void Reset()
        {
            inputBuffer.Keys.ForEach(key => inputBuffer[key] = false);
        }
    }

    class SignalSink : Module
    {
        public SignalSink(string name)
        {
            this.name = name;
            this.receiverNames = new string[0];
            this.receivers = new List<Module>();
        }
        public override void Receive(Module inputModule, bool signal)
        {

        }
    }
}
