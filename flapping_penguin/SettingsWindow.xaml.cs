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
        private SettingsController _settingsController;

        public SettingsWindow()
        {
            InitializeComponent();
            _settingsController = new SettingsController();

            // 画面が開いたときに、保存されている設定をUIに反映する
            MovementToggle.IsChecked = _settingsController.IsMovementEnabled;
            SpeedSlider.Value = _settingsController.MovementSpeed;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // チェックボックスとスライダーの数値をコントローラー経由で保存する
            _settingsController.IsMovementEnabled = MovementToggle.IsChecked ?? false;
            _settingsController.MovementSpeed = (int)SpeedSlider.Value;

            // 画面を閉じる
            this.DialogResult = true;
        }

        // アプリ終了ボタンが押されたときの処理
        private void ExitButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // アプリを完全に終了させる（Win32 APIのフック等も自動で安全に解除されます）
            System.Windows.Application.Current.Shutdown();
        }
    }
}