using System;

namespace flapping_penguin
{
    public class SettingsController
    {
        // 移動のオンオフを取得・設定し、変更されたら保存する
        public bool IsMovementEnabled
        {
            get { return Properties.Settings.Default.IsMovementEnabled; }
            set
            {
                Properties.Settings.Default.IsMovementEnabled = value;
                Properties.Settings.Default.Save();
            }
        }

        // 移動スピードを取得・設定し、変更されたら保存する
        public int MovementSpeed
        {
            get { return Properties.Settings.Default.MovementSpeed; }
            set
            {
                Properties.Settings.Default.MovementSpeed = value;
                Properties.Settings.Default.Save();
            }
        }
        public void OpenSettings(System.Windows.Window ownerWindow)
        {
            // 設定画面のインスタンスを生成
            SettingsWindow settingsWin = new SettingsWindow();

            // 親ウィンドウを指定（メイン画面の中央に表示させるため）
            settingsWin.Owner = ownerWindow;

            // 設定画面をモーダル（手前に固定）で開く
            settingsWin.ShowDialog();
        }
    }
}