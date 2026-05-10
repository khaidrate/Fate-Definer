using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FateDefiner.Models
{
    /// <summary>
    /// D&amp;D-style character statistics. Implements INotifyPropertyChanged so
    /// the UI automatically recalculates modifiers when base stats are edited.
    /// </summary>
    public class CharacterStats : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // ── Core Stats ──────────────────────────────────────────────────────────
        private int _strength = 10;
        public int Strength
        {
            get => _strength;
            set { _strength = value; OnPropertyChanged(); OnPropertyChanged(nameof(StrengthMod)); }
        }

        private int _dexterity = 10;
        public int Dexterity
        {
            get => _dexterity;
            set { _dexterity = value; OnPropertyChanged(); OnPropertyChanged(nameof(DexterityMod)); }
        }

        private int _constitution = 10;
        public int Constitution
        {
            get => _constitution;
            set { _constitution = value; OnPropertyChanged(); OnPropertyChanged(nameof(ConstitutionMod)); }
        }

        private int _intelligence = 10;
        public int Intelligence
        {
            get => _intelligence;
            set { _intelligence = value; OnPropertyChanged(); OnPropertyChanged(nameof(IntelligenceMod)); }
        }

        private int _wisdom = 10;
        public int Wisdom
        {
            get => _wisdom;
            set { _wisdom = value; OnPropertyChanged(); OnPropertyChanged(nameof(WisdomMod)); }
        }

        private int _charisma = 10;
        public int Charisma
        {
            get => _charisma;
            set { _charisma = value; OnPropertyChanged(); OnPropertyChanged(nameof(CharismaMod)); }
        }

        // ── Combat Stats ────────────────────────────────────────────────────────
        private int _maxHP = 10;
        public int MaxHP { get => _maxHP; set { _maxHP = value; OnPropertyChanged(); } }

        private int _currentHP = 10;
        public int CurrentHP { get => _currentHP; set { _currentHP = value; OnPropertyChanged(); } }

        private int _armorClass = 10;
        public int ArmorClass { get => _armorClass; set { _armorClass = value; OnPropertyChanged(); } }

        private int _speed = 30;
        public int Speed { get => _speed; set { _speed = value; OnPropertyChanged(); } }

        private int _proficiencyBonus = 2;
        public int ProficiencyBonus { get => _proficiencyBonus; set { _proficiencyBonus = value; OnPropertyChanged(); } }

        private int _initiative = 0;
        public int Initiative { get => _initiative; set { _initiative = value; OnPropertyChanged(); } }

        // ── Computed Modifiers ───────────────────────────────────────────────────
        private static int Mod(int stat) => (stat - 10) / 2;

        public string StrengthMod     => FormatMod(Mod(Strength));
        public string DexterityMod    => FormatMod(Mod(Dexterity));
        public string ConstitutionMod => FormatMod(Mod(Constitution));
        public string IntelligenceMod => FormatMod(Mod(Intelligence));
        public string WisdomMod       => FormatMod(Mod(Wisdom));
        public string CharismaMod     => FormatMod(Mod(Charisma));

        private static string FormatMod(int m) => m >= 0 ? $"+{m}" : $"{m}";
    }
}
