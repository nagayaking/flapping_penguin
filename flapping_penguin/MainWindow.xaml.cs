using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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
        private BitmapImage imgSliding;  // スクロール中（スライディング）

        // 次は右羽を上げる番かどうかを記憶するフラグ
        private bool _isRightWingNext = true;

        private const int ActionDelayMilliseconds = 100; // アクション時の画像を表示する時間（ミリ秒）
        private const int ScrollStopDelayMilliseconds = 200; // スクロールが止まったと判定するまでの時間（ミリ秒）

        // スクロールが止まったことを検知するためのタイマー
        private DispatcherTimer m_ScrollStopTimer;

        public MainWindow()
        {
            InitializeComponent();

            // 1. 画像をメモリに読み込んでおく
            imgBothDown = new BitmapImage(new Uri("Images/penguin-LR-down.png", UriKind.Relative));
            imgBothUp = new BitmapImage(new Uri("Images/penguin-LR-up.png", UriKind.Relative));
            imgRightUp = new BitmapImage(new Uri("Images/penguin-R-up-L-down.png", UriKind.Relative));
            imgLeftUp = new BitmapImage(new Uri("Images/penguin-L-up-R-down.png", UriKind.Relative));
            imgSliding = new BitmapImage(new Uri("Images/penguin_sliding.png", UriKind.Relative));

            // アプリ起動時の初期画像を設定（待機中は「両羽下げ」としています）
            CatImage.Source = imgBothDown;

            // スクロール停止検知用タイマーの準備（動かすのはスクロール検知時）
            m_ScrollStopTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(ScrollStopDelayMilliseconds)
            };
            m_ScrollStopTimer.Tick += ScrollStopTimer_Tick;

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
            // 反転状態をリセット（スクロール中に反転していた場合に備えて）
            CatImageScale.ScaleX = 1;

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
                // スライディング中の画像に切り替え、一定時間スクロールがなければ元に戻す
                CatImage.Source = imgSliding;
                m_ScrollStopTimer.Stop();
                m_ScrollStopTimer.Start();

                if (scrollAmount > 0)
                {
                    this.Left += 30;
                    CatImageScale.ScaleX = -1; // 右へ移動するので画像を反転
                }
                else
                {
                    this.Left -= 30;
                    CatImageScale.ScaleX = 1; // 左へ移動するので通常向き
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

                // 5. 画面端に到達したときのワープ処理（全モニターの端から端へ）
                // 横方向（X軸）の拡大率を取得します（M11を使用します）
                double dpiScaleX = 1.0;
                if (source != null)
                {
                    dpiScaleX = source.CompositionTarget.TransformToDevice.M11;
                }

                // 接続されている全モニターの中で、一番左の座標と一番右の物理座標を取得し、拡大率で論理座標に変換します
                double minLeft = System.Windows.Forms.Screen.AllScreens.Min(s => s.WorkingArea.Left) / dpiScaleX;
                double maxRight = System.Windows.Forms.Screen.AllScreens.Max(s => s.WorkingArea.Right) / dpiScaleX;

                if (this.Left > maxRight)
                {
                    // 一番右のモニターの右端を完全に越えたら、一番左のモニターの左端（画面外）へワープ
                    this.Left = minLeft - this.Width;
                }
                else if (this.Left < minLeft - this.Width)
                {
                    // 一番左のモニターの左端を完全に越えたら、一番右のモニターの右端へワープ
                    this.Left = maxRight;
                }
            });
        }

        // スクロールが止まってから一定時間経ったときに呼ばれる処理
        private void ScrollStopTimer_Tick(object sender, EventArgs e)
        {
            m_ScrollStopTimer.Stop();
            CatImage.Source = imgBothDown;
            CatImageScale.ScaleX = 1; // 反転状態をリセット
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
