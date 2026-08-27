using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace flapping_penguin
{
    public partial class SettingsWindow : Window
    {
        // 外部から受け取った設定データを保持する変数
        private AppSettings _currentSettings;

        // コンストラクタを書き換えて、AppSettingsを受け取れるようにします
        public SettingsWindow(AppSettings settings)
        {
            InitializeComponent();
            _currentSettings = settings;

            // 画面が開いたときに、現在の設定データを画面（UI）に反映させる
            MovementToggle.IsChecked = _currentSettings.IsMovementEnabled;
            SpeedSlider.Value = _currentSettings.MovementSpeed;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // 画面を閉じるときに、画面（UI）の数値を設定データに保存する
            _currentSettings.IsMovementEnabled = MovementToggle.IsChecked ?? false;
            _currentSettings.MovementSpeed = (int)SpeedSlider.Value;

            this.DialogResult = true;
        }
    }
}