using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using MvvmHelpers.Commands;

namespace KanaLearn
{
    public class MainViewModel : INotifyPropertyChanged
    {
#pragma warning disable 0067
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore 0067

        private readonly static Dictionary<string, string[]> KanaMap = new Dictionary<string, string[]>
        {
            ["あ"] = new string[] { "a" },
            ["ア"] = new string[] { "a" },
            ["い"] = new string[] { "i" },
            ["イ"] = new string[] { "i" },
            ["う"] = new string[] { "u" },
            ["ウ"] = new string[] { "u" },
            ["え"] = new string[] { "e" },
            ["エ"] = new string[] { "e" },
            ["お"] = new string[] { "o" },
            ["オ"] = new string[] { "o" },
            ["か"] = new string[] { "ka" },
            ["カ"] = new string[] { "ka" },
            ["き"] = new string[] { "ki" },
            ["キ"] = new string[] { "ki" },
            ["く"] = new string[] { "ku" },
            ["ク"] = new string[] { "ku" },
            ["け"] = new string[] { "ke" },
            ["ケ"] = new string[] { "ke" },
            ["こ"] = new string[] { "ko" },
            ["コ"] = new string[] { "ko" },
            ["さ"] = new string[] { "sa" },
            ["サ"] = new string[] { "sa" },
            ["し"] = new string[] { "si", "shi" },
            ["シ"] = new string[] { "si", "shi" },
            ["す"] = new string[] { "su" },
            ["ス"] = new string[] { "su" },
            ["せ"] = new string[] { "se" },
            ["セ"] = new string[] { "se" },
            ["そ"] = new string[] { "so" },
            ["ソ"] = new string[] { "so" },
            ["た"] = new string[] { "ta" },
            ["タ"] = new string[] { "ta" },
            ["ち"] = new string[] { "ti", "chi" },
            ["チ"] = new string[] { "ti", "chi" },
            ["つ"] = new string[] { "tu", "tsu" },
            ["ツ"] = new string[] { "tu", "tsu" },
            ["て"] = new string[] { "te" },
            ["テ"] = new string[] { "te" },
            ["と"] = new string[] { "to" },
            ["ト"] = new string[] { "to" },
            ["な"] = new string[] { "na" },
            ["ナ"] = new string[] { "na" },
            ["に"] = new string[] { "ni" },
            ["ニ"] = new string[] { "ni" },
            ["ぬ"] = new string[] { "nu" },
            ["ヌ"] = new string[] { "nu" },
            ["ね"] = new string[] { "ne" },
            ["ネ"] = new string[] { "ne" },
            ["の"] = new string[] { "no" },
            ["ノ"] = new string[] { "no" },
            ["は"] = new string[] { "ha" },
            ["ハ"] = new string[] { "ha" },
            ["ひ"] = new string[] { "hi" },
            ["ヒ"] = new string[] { "hi" },
            ["ふ"] = new string[] { "hu", "fu" },
            ["フ"] = new string[] { "hu", "fu" },
            ["へ"] = new string[] { "he" },
            ["ヘ"] = new string[] { "he" },
            ["ほ"] = new string[] { "ho" },
            ["ホ"] = new string[] { "ho" },
            ["ま"] = new string[] { "ma" },
            ["マ"] = new string[] { "ma" },
            ["み"] = new string[] { "mi" },
            ["ミ"] = new string[] { "mi" },
            ["む"] = new string[] { "mu" },
            ["ム"] = new string[] { "mu" },
            ["め"] = new string[] { "me" },
            ["メ"] = new string[] { "me" },
            ["も"] = new string[] { "mo" },
            ["モ"] = new string[] { "mo" },
            ["や"] = new string[] { "ya" },
            ["ヤ"] = new string[] { "ya" },
            ["ゆ"] = new string[] { "yu" },
            ["ユ"] = new string[] { "yu" },
            ["よ"] = new string[] { "yo" },
            ["ヨ"] = new string[] { "yo" },
            ["ら"] = new string[] { "ra" },
            ["ラ"] = new string[] { "ra" },
            ["り"] = new string[] { "ri" },
            ["リ"] = new string[] { "ri" },
            ["る"] = new string[] { "ru" },
            ["ル"] = new string[] { "ru" },
            ["れ"] = new string[] { "re" },
            ["レ"] = new string[] { "re" },
            ["ろ"] = new string[] { "ro" },
            ["ロ"] = new string[] { "ro" },
            ["わ"] = new string[] { "wa" },
            ["ワ"] = new string[] { "wa" },
            ["を"] = new string[] { "wo", "o" },
            ["ヲ"] = new string[] { "wo", "o" },
            ["ん"] = new string[] { "n", "nn" },
            ["ン"] = new string[] { "n", "nn" },
        };

        private DispatcherTimer mainTimer = new DispatcherTimer();

        public MainViewModel()
        {
            mainTimer.Interval = TimeSpan.FromMilliseconds(int.Parse(ConfigurationManager.AppSettings["interval"] ?? "3000"));
            mainTimer.Tick += (sender, e) => MainTimer_Tick();
            StartCommand = new Command(Start, () => !Running);
            PauseCommand = new Command(Pause, () => Running);
            ConfirmCommand = new Command(Confirm);
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                CurrentKana = "ふ";
                Input = "fu";
                CorrectAnswer = "hu";
            }
        }

        public bool Running { get; set; }

        private void OnRunningChanged()
        {
            StartCommand.RaiseCanExecuteChanged();
            PauseCommand.RaiseCanExecuteChanged();
        }

        public Command StartCommand { get; }

        public void Start()
        {
            if (kanas.Count == 0) InitializeKanas();
            if (CurrentKana == kanas.Last()) CurrentKana = null;
            if (CurrentKana == null) MainTimer_Tick();
            mainTimer.Start();
            Running = true;
        }

        public Command PauseCommand { get; }

        public void Pause()
        {
            mainTimer.Stop();
            Running = false;
        }

        public Command ConfirmCommand { get; }

        public void Confirm()
        {
            if (Running)
            {
                Pause();
                if (MainTimer_Tick()) Start();
            }
            else
            {
                Start();
            }
        }

        public string? CurrentKana { get; set; }

        public string? Input { get; set; }

        public string? CorrectAnswer { get; set; }

        private readonly List<string> kanas = new List<string>();

        private void InitializeKanas()
        {
            kanas.Clear();
            kanas.AddRange(KanaMap.Keys.Random());
        }

        private bool MainTimer_Tick()
        {
            CorrectAnswer = null;
            if (CurrentKana != null) kanas.Remove(CurrentKana);
            bool res = CurrentKana == null || KanaMap[CurrentKana].Contains(Input ?? string.Empty);
            if (res)
            {
                CurrentKana = kanas.FirstOrDefault();
            }
            else
            {
                CorrectAnswer = string.Join(
#if NETCOREAPP
                    '/'
#else
                    "/"
#endif
                    , KanaMap[CurrentKana!]);
                kanas.Add(CurrentKana!);
                Pause();
            }
            Input = null;
            if (CurrentKana == null) Pause();
            return res;
        }
    }

    static class RandomHelper
    {
        public readonly static Random Generator = new Random();

        public static IEnumerable<T> Random<T>(this IEnumerable<T> source)
        {
            List<T> s = source.ToList();
            int count = s.Count;
            for (int i = 0; i < count; i++)
            {
                int index = Generator.Next(s.Count);
                yield return s[index];
                s.RemoveAt(index);
            }
        }
    }
}
