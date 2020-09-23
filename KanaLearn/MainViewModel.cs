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

        private readonly static Dictionary<string, string> KanaMap = new Dictionary<string, string>
        {
            ["あ"] = "a",
            ["ア"] = "a",
            ["い"] = "i",
            ["イ"] = "i",
            ["う"] = "u",
            ["ウ"] = "u",
            ["え"] = "e",
            ["エ"] = "e",
            ["お"] = "o",
            ["オ"] = "o",
            ["か"] = "ka",
            ["カ"] = "ka",
            ["き"] = "ki",
            ["キ"] = "ki",
            ["く"] = "ku",
            ["ク"] = "ku",
            ["け"] = "ke",
            ["ケ"] = "ke",
            ["こ"] = "ko",
            ["コ"] = "ko",
            ["さ"] = "sa",
            ["サ"] = "sa",
            ["し"] = "si",
            ["シ"] = "si",
            ["す"] = "su",
            ["ス"] = "su",
            ["せ"] = "se",
            ["セ"] = "se",
            ["そ"] = "so",
            ["ソ"] = "so",
            ["た"] = "ta",
            ["タ"] = "ta",
            ["ち"] = "ti",
            ["チ"] = "ti",
            ["つ"] = "tu",
            ["ツ"] = "tu",
            ["て"] = "te",
            ["テ"] = "te",
            ["と"] = "to",
            ["ト"] = "to",
            ["な"] = "na",
            ["ナ"] = "na",
            ["に"] = "ni",
            ["ニ"] = "ni",
            ["ぬ"] = "nu",
            ["ヌ"] = "nu",
            ["ね"] = "ne",
            ["ネ"] = "ne",
            ["の"] = "no",
            ["ノ"] = "no",
            ["は"] = "ha",
            ["ハ"] = "ha",
            ["ひ"] = "hi",
            ["ヒ"] = "hi",
            ["ふ"] = "hu",
            ["フ"] = "hu",
            ["へ"] = "he",
            ["ヘ"] = "he",
            ["ほ"] = "ho",
            ["ホ"] = "ho",
            ["ま"] = "ma",
            ["マ"] = "ma",
            ["み"] = "mi",
            ["ミ"] = "mi",
            ["む"] = "mu",
            ["ム"] = "mu",
            ["め"] = "me",
            ["メ"] = "me",
            ["も"] = "mo",
            ["モ"] = "mo",
            ["や"] = "ya",
            ["ヤ"] = "ya",
            ["ゆ"] = "yu",
            ["ユ"] = "yu",
            ["よ"] = "yo",
            ["ヨ"] = "yo",
            ["ら"] = "ra",
            ["ラ"] = "ra",
            ["り"] = "ri",
            ["リ"] = "ri",
            ["る"] = "ru",
            ["ル"] = "ru",
            ["れ"] = "re",
            ["レ"] = "re",
            ["ろ"] = "ro",
            ["ロ"] = "ro",
            ["わ"] = "wa",
            ["ワ"] = "wa",
            ["を"] = "wo",
            ["ヲ"] = "wo",
            ["ん"] = "n",
            ["ン"] = "n"
        };

        private DispatcherTimer mainTimer = new DispatcherTimer();

        public MainViewModel()
        {
            mainTimer.Interval = TimeSpan.FromMilliseconds(int.Parse(ConfigurationManager.AppSettings["interval"] ?? "3000"));
            mainTimer.Tick += MainTimer_Tick;
            StartCommand = new Command(Start, () => !Running);
            PauseCommand = new Command(Pause, () => Running);
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
            if (CurrentKana == null) MainTimer_Tick(null, EventArgs.Empty);
            mainTimer.Start();
            Running = true;
        }

        public Command PauseCommand { get; }

        public void Pause()
        {
            mainTimer.Stop();
            Running = false;
        }

        public void Confirm()
        {
            if (Running)
                mainTimer.Stop();
            MainTimer_Tick(null, EventArgs.Empty);
            if (Running)
                mainTimer.Start();
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

        private void MainTimer_Tick(object? sender, EventArgs e)
        {
            CorrectAnswer = null;
            if (CurrentKana != null) kanas.Remove(CurrentKana);
            if (CurrentKana == null || KanaMap[CurrentKana] == Input)
            {
                CurrentKana = kanas.FirstOrDefault();
            }
            else
            {
                CorrectAnswer = KanaMap[CurrentKana];
                kanas.Add(CurrentKana);
                Pause();
            }
            Input = null;
            if (CurrentKana == null) Pause();
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
