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

        // スクロール検知を管理するための変数
        private ScrollDetector m_ScrollDetector;

        // 画像を毎回読み込むと動作が重くなるため、変数として保持しておきます
        private BitmapImage normalImage;
        private BitmapImage actionImage;

        private const int ActionDelayMilliseconds = 100; // アクション時の画像を表示する時間（ミリ秒）

        public MainWindow()
        {
            InitializeComponent();

            // ウィンドウが開いたときに監視を開始する
            Subscribe();
            // アプリ起動時に画像を一度だけメモリに読み込んでおく
            normalImage = new BitmapImage(new Uri("Images/penguin-LR-down.jpg", UriKind.Relative));
            actionImage = new BitmapImage(new Uri("Images/penguin-LR-up.jpg", UriKind.Relative));

            // アプリ起動時の初期画像を設定
            CatImage.Source = normalImage;
        }

        // 監視の開始
        private void Subscribe()
        {
            m_GlobalHook = Hook.GlobalEvents();

            // キーが押された瞬間、離された瞬間を登録
            m_GlobalHook.KeyDown += OnKeyDown;
            m_GlobalHook.KeyUp += OnKeyUp;

            // スクロール検知の初期化と開始
            m_ScrollDetector = new ScrollDetector();
            m_ScrollDetector.OnScrollDetected += ScrollDetector_OnScrollDetected;
            m_ScrollDetector.Start();
        }

        // スクロールが検知されたときに呼ばれる処理
        private void ScrollDetector_OnScrollDetected(int scrollAmount)
        {
            Dispatcher.Invoke(() =>
            {
                // 1. まずは横(Left)に移動させる
                if (scrollAmount > 0)
                {
                    this.Left += 30;
                }
                else
                {
                    this.Left -= 30;
                }

                // 2. 現在のウィンドウの中心座標を計算する
                int centerX = (int)(this.Left + (this.Width / 2));
                int currentY = (int)this.Top;

                // 3. 中心座標が「現在どのモニター上にあるか」を判定する
                // System.Drawing.Pointを使うため、型の変換を行っています
                var currentScreen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point(centerX, currentY));

                // 4. 判定されたモニターの「作業領域（タスクバーを除いた領域）」の底辺に、ウィンドウの底辺を合わせる
                this.Top = currentScreen.WorkingArea.Bottom - this.Height;

                // 5. 画面端に到達したときのワープ処理（全モニターの端から端へ）
                // 接続されている全モニターの中で、一番左の座標と一番右の座標を取得
                int minLeft = System.Windows.Forms.Screen.AllScreens.Min(s => s.WorkingArea.Left);
                int maxRight = System.Windows.Forms.Screen.AllScreens.Max(s => s.WorkingArea.Right);

                if (this.Left > maxRight)
                {
                    // 一番右のモニターの右端を越えたら、一番左のモニターの左端へワープ
                    this.Left = minLeft - this.Width;
                }
                else if (this.Left < minLeft - this.Width)
                {
                    // 一番左のモニターの左端を越えたら、一番右のモニターの右端へワープ
                    this.Left = maxRight;
                }
            });
        }

        // 何らかのキーが「押された瞬間」に動く処理
        // （※WPFの機能と名前が被るため、System.Windows.Forms.KeyEventArgs と長めに書いています）
        private async void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // Visual Studioの出力ウィンドウに文字を出します
            System.Diagnostics.Debug.WriteLine("↓ キーが押されました");

            // 1. アクション時の画像（キーを叩いた状態）に変更
            CatImage.Source = actionImage;

            // 2. 指定したミリ秒（ここでは50ms）だけ待機。この間も画面は固まりません。
            await Task.Delay(ActionDelayMilliseconds);

            // 3. 元の画像（待機状態）に戻す
            CatImage.Source = normalImage;
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
            if (m_ScrollDetector != null)
            {
                m_ScrollDetector.OnScrollDetected -= ScrollDetector_OnScrollDetected;
                m_ScrollDetector.Stop();
                m_ScrollDetector = null;
            }
        }

        // ウィンドウ（アプリ）の右上の「×」ボタン等で閉じられるときに呼ばれる処理
        protected override void OnClosed(EventArgs e)
        {
            // アプリ終了時に絶対にフックを解除する
            Unsubscribe();
            base.OnClosed(e);
        }

        // イベントハンドラに「async」を追加して非同期処理にします
        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. アクション時の画像（キーを叩いた状態）に変更
            CatImage.Source = actionImage;

            // 2. 指定したミリ秒だけ待機。この間も画面は固まりません。
            await Task.Delay(ActionDelayMilliseconds);

            // 3. 元の画像（待機状態）に戻す
            CatImage.Source = normalImage;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Right - Width;
            Top = workArea.Bottom - Height;
        }
    }
}