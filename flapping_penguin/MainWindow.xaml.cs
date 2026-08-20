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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gma.System.MouseKeyHook;

namespace flapping_penguin
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        // グローバルキーフックを管理するための変数
        private IKeyboardMouseEvents m_GlobalHook;

        public MainWindow()
        {
            InitializeComponent();

            // ウィンドウが開いたときに監視を開始する
            Subscribe();
        }

        // 監視の開始
        private void Subscribe()
        {
            m_GlobalHook = Hook.GlobalEvents();

            // キーが押された瞬間、離された瞬間を登録
            m_GlobalHook.KeyDown += OnKeyDown;
            m_GlobalHook.KeyUp += OnKeyUp;
        }

        // 何らかのキーが「押された瞬間」に動く処理
        // （※WPFの機能と名前が被るため、System.Windows.Forms.KeyEventArgs と長めに書いています）
        private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // Visual Studioの出力ウィンドウに文字を出します
            System.Diagnostics.Debug.WriteLine("↓ キーが押されました");
        }

        // 何らかのキーが「離された瞬間」に動く処理
        private void OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // Visual Studioの出力ウィンドウに文字を出します
            System.Diagnostics.Debug.WriteLine("↑ キーが離されました");
        }

        // 監視の解除
        private void Unsubscribe()
        {
            if (m_GlobalHook != null)
            {
                m_GlobalHook.KeyDown -= OnKeyDown;
                m_GlobalHook.KeyUp -= OnKeyUp;
                m_GlobalHook.Dispose();
            }
        }

        // ウィンドウ（アプリ）の右上の「×」ボタン等で閉じられるときに呼ばれる処理
        protected override void OnClosed(EventArgs e)
        {
            // アプリ終了時に絶対にフックを解除する
            Unsubscribe();
            base.OnClosed(e);
        }
    }
}