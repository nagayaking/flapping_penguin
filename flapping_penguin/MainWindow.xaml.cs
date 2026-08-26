using System;
using System.Windows;

namespace flapping_penguin
{
    public partial class MainWindow : Window
    {
        // 検知用のクラスを変数として用意
        private ScrollDetector m_ScrollDetector;
        private KeyboardDetector m_KeyboardDetector;

        private PenguinAssets m_Assets;
        private PenguinAnimator m_Animator;
        private WindowMover m_WindowMover;

        public MainWindow()
        {
            InitializeComponent();

            m_Assets = new PenguinAssets();
            m_Animator = new PenguinAnimator(CatImage, CatImageScale, m_Assets);
            m_WindowMover = new WindowMover(this);

            // 各種監視のスタート
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

        // キーが押されたときの処理（パタパタさせる）
        private async void KeyboardDetector_OnKeyPressed()
        {
            await m_Animator.PlayWingFlapAsync();
        }

        // スクロールが検知されたときに呼ばれる処理
        private void ScrollDetector_OnScrollDetected(int scrollAmount)
        {
            Dispatcher.Invoke(() =>
            {
                var direction = scrollAmount > 0 ? ScrollDirection.Right : ScrollDirection.Left;

                m_Animator.ShowSliding(direction);
                m_WindowMover.MoveForScroll(direction);
            });
        }

        // 監視の解除と終了処理
        private void Unsubscribe()
        {
            m_Animator?.Stop();

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
            m_WindowMover.PlaceAtInitialPosition();
        }
    }
}
