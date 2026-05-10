using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FateDefiner.ViewModels
{
    /// <summary>
    /// ViewModel for the Dice Roller panel.
    /// Demonstrates: Loops (rolling N dice), Methods, Exception Handling, Strings.
    /// </summary>
    public class DiceRollerViewModel : BaseViewModel
    {
        private static readonly Random _rng = new Random();

        private int    _count     = 1;
        private int    _modifier  = 0;
        private string _result    = "Roll the dice!";
        private string _formula   = string.Empty;
        private bool   _hasResult = false;

        public int NumberOfDice
        {
            get => _count;
            set => SetProperty(ref _count, Math.Max(1, Math.Min(value, 99)));
        }

        public int Modifier
        {
            get => _modifier;
            set => SetProperty(ref _modifier, Math.Max(-99, Math.Min(value, 99)));
        }

        public string ResultDisplay
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        public string FormulaDisplay
        {
            get => _formula;
            set => SetProperty(ref _formula, value);
        }

        public bool HasResult
        {
            get => _hasResult;
            set => SetProperty(ref _hasResult, value);
        }

        public ObservableCollection<string> History { get; } = new();

        public ICommand RollD4Command      { get; }
        public ICommand RollD6Command      { get; }
        public ICommand RollD8Command      { get; }
        public ICommand RollD10Command     { get; }
        public ICommand RollD12Command     { get; }
        public ICommand RollD20Command     { get; }
        public ICommand RollD100Command    { get; }
        public ICommand ClearHistoryCommand{ get; }

        public DiceRollerViewModel()
        {
            RollD4Command       = new RelayCommand(_ => Roll(4));
            RollD6Command       = new RelayCommand(_ => Roll(6));
            RollD8Command       = new RelayCommand(_ => Roll(8));
            RollD10Command      = new RelayCommand(_ => Roll(10));
            RollD12Command      = new RelayCommand(_ => Roll(12));
            RollD20Command      = new RelayCommand(_ => Roll(20));
            RollD100Command     = new RelayCommand(_ => Roll(100));
            ClearHistoryCommand = new RelayCommand(_ => History.Clear(), _ => History.Count > 0);
        }

        /// <summary>
        /// Rolls NumberOfDice dice with the given number of sides, adds Modifier,
        /// records the result in History.  Uses a Loop to roll multiple dice.
        /// </summary>
        private void Roll(int sides)
        {
            try
            {
                // Loop technique: roll each die individually to capture individual rolls
                int[] rolls = new int[_count];
                int total = 0;
                for (int i = 0; i < _count; i++)
                {
                    rolls[i] = _rng.Next(1, sides + 1);
                    total   += rolls[i];
                }
                total += _modifier;

                // String formatting technique
                string modStr = _modifier == 0 ? string.Empty
                              : _modifier >  0 ? $" + {_modifier}"
                                               : $" − {Math.Abs(_modifier)}";

                string rollsStr = _count == 1
                    ? string.Empty
                    : $"  [{string.Join(", ", rolls)}]";

                FormulaDisplay  = $"{_count}d{sides}{modStr}";
                ResultDisplay   = total.ToString();
                HasResult       = true;

                string entry = $"{FormulaDisplay} = {total}{rollsStr}";
                History.Insert(0, entry);

                // Keep history bounded — Loop + exception safety
                while (History.Count > 50)
                    History.RemoveAt(History.Count - 1);
            }
            catch (Exception ex)
            {
                ResultDisplay = $"Error: {ex.Message}";
            }
        }
    }
}
