using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace flapping_penguin
{
    public partial class MainWindow : Window
    {
        // 検知用のクラスを変数として用意
        private ScrollDetector m_ScrollDetector;
        private KeyboardDetector m_KeyboardDetector;

        // ペンギンの画像4種類
        private BitmapImage imgBothDown; // 両羽下げ
        private BitmapImage imgBothUp;   // 両羽上げ
        private BitmapImage imgRightUp;  // 右羽上げ・左羽下げ
        private BitmapImage imgLeftUp;   // 左羽上げ・右羽下げ

        // 次は右羽を上げる番かどうかを記憶するフラグ
        private bool _isRightWingNext = true;

        private const int ActionDelayMilliseconds = 100; // アクション時の画像を表示する時間（ミリ秒）

        public MainWindow()
        {
            InitializeComponent();

            // 1. 画像をメモリに読み込んでおく
            imgBothDown = new BitmapImage(new Uri("Images/penguin-LR-down.png", UriKind.Relative));
            imgBothUp = new BitmapImage(new Uri("Images/penguin-LR-up.png", UriKind.Relative));
            imgRightUp = new BitmapImage(new Uri("Images/penguin-R-up-L-down.png", UriKind.Relative));
            imgLeftUp = new BitmapImage(new Uri("Images/penguin-L-up-R-down.png", UriKind.Relative));

            // アプリ起動時の初期画像を設定（待機中は「両羽下げ」としています）
            CatImage.Source = imgBothDown;

            // 2. 各種監視のスタート
            Subscribe();
        }

        // 監視の開始
        private void Subscribe()
        {
            // スクロール検知の初期化と開始
            m_ScrollDetector = new ScrollDetector();
            m_ScrollDetector.OnScrollDetected += ScrollDetector_OnScrollDetected;
            m_ScrollDetector.Start();

            // キーボード検知の初期化と開始
            m_KeyboardDetector = new KeyboardDetector();
            m_KeyboardDetector.OnKeyPressed += KeyboardDetector_OnKeyPressed;
            m_KeyboardDetector.Start();
        }

        // ==============================================
        // キーが押されたときの処理（パタパタさせる）
        // ==============================================
        private async void KeyboardDetector_OnKeyPressed()
        {
            // ① どちらの羽を上げるか判定して画像を変える
            if (_isRightWingNext)
            {
                CatImage.Source = imgRightUp; // 右を上げる
            }
            else
            {
                CatImage.Source = imgLeftUp;  // 左を上げる
            }

            // ② 次の入力のために、右と左の順番を反転させる（trueならfalse、falseならtrueに）
            _isRightWingNext = !_isRightWingNext;

            // ③ 指定ミリ秒だけ待つ
            await Task.Delay(ActionDelayMilliseconds);

            // ④ 元の待機状態（両羽下げ）に戻す
            CatImage.Source = imgBothDown;
        }

        // ==============================================
        // スクロールが検知されたときに呼ばれる処理
        // ==============================================
        private void ScrollDetector_OnScrollDetected(int scrollAmount)
        {
            Dispatcher.Invoke(() =>
            {
                if (scrollAmount > 0)
                {
                    this.Left += 30;
                }
                else
                {
                    this.Left -= 30;
                }

                int centerX = (int)(this.Left + (this.Width / 2));
                int currentY = (int)this.Top;
                var currentScreen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point(centerX, currentY));

                PresentationSource source = PresentationSource.FromVisual(this);
                double dpiScaleY = 1.0;
                if (source != null)
                {
                    dpiScaleY = source.CompositionTarget.TransformToDevice.M22;
                }

                this.Top = (currentScreen.WorkingArea.Bottom / dpiScaleY) - this.Height;

                int minLeft = System.Windows.Forms.Screen.AllScreens.Min(s => s.WorkingArea.Left);
                int maxRight = System.Windows.Forms.Screen.AllScreens.Max(s => s.WorkingArea.Right);

                if (this.Left > maxRight)
                {
                    this.Left = minLeft - this.Width;
                }
                else if (this.Left < minLeft - this.Width)
                {
                    this.Left = maxRight;
                }
            });
        }

        // ==============================================
        // 監視の解除と終了処理
        // ==============================================
        private void Unsubscribe()
        {
            if (m_ScrollDetector != null)
            {
                m_ScrollDetector.OnScrollDetected -= ScrollDetector_OnScrollDetected;
                m_ScrollDetector.Stop();
                m_ScrollDetector = null;
            }
            if (m_KeyboardDetector != null)
            {
                m_KeyboardDetector.OnKeyPressed -= KeyboardDetector_OnKeyPressed;
                m_KeyboardDetector.Stop();
                m_KeyboardDetector = null;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            Unsubscribe();
            base.OnClosed(e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Right - Width;
            Top = workArea.Bottom - Height;
        }
    }
}