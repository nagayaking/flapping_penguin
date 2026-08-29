using System;
using System.Windows;

namespace flapping_penguin
{
    public partial class MainWindow : Window
    {
        // 検知用のクラスを変数として用意
        private ScrollDetector m_ScrollDetector;
        private KeyboardDetector m_KeyboardDetector;
        private MouseClickDetector m_MouseClickDetector;

        private PenguinAssets m_Assets;
        private PenguinAnimator m_Animator;
        private WindowMover m_WindowMover;
        private PenguinSpeech m_PenguinSpeech;
        private SettingsController m_SettingsController;

        public MainWindow()
        {
            InitializeComponent();

            m_Assets = new PenguinAssets();
            m_Animator = new PenguinAnimator(CatImage, CatImageScale, m_Assets);
            m_WindowMover = new WindowMover(this);
            m_PenguinSpeech = new PenguinSpeech();
            m_SettingsController = new SettingsController();

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

            // マウスクリック検知の初期化と開始
            m_MouseClickDetector = new MouseClickDetector();
            m_MouseClickDetector.OnLeftClicked += MouseClickDetector_OnLeftClicked;
            m_MouseClickDetector.OnRightClicked += MouseClickDetector_OnRightClicked;
            m_MouseClickDetector.Start();
        }

        // キーが押されたときの処理（キーの種類に応じて動きを変える）
        private async void KeyboardDetector_OnKeyPressed(System.Windows.Forms.Keys key)
        {
            switch (key)
            {
                case System.Windows.Forms.Keys.Space:
                    await m_Animator.PlayJumpAsync();
                    break;
                case System.Windows.Forms.Keys.Enter:
                    await m_Animator.PlayBanzaiAsync();
                    break;
                default:
                    await m_Animator.PlayWingFlapAsync();
                    break;
            }
        }

        // 左クリックされたときの処理（喋る）
        private void MouseClickDetector_OnLeftClicked()
        {
            m_PenguinSpeech.Speak();
        }

        // 右クリックされたときの処理（設定画面を開く）
        private void MouseClickDetector_OnRightClicked()
        {
            // ペンギンの画像（CatImage）の上にマウスカーソルがある時だけ実行する
            if (CatImage.IsMouseOver)
            {
                // 設定コントローラーに、自分自身(this)を親ウィンドウとして渡して画面を開かせる
                m_SettingsController.OpenSettings(this);
            }
        }

        // スクロールが検知されたときに呼ばれる処理
        private void ScrollDetector_OnScrollDetected(int scrollAmount)
        {
            if (!m_SettingsController.IsMovementEnabled)
            {
                return;
            }
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
            if (m_MouseClickDetector != null)
            {
                m_MouseClickDetector.OnLeftClicked -= MouseClickDetector_OnLeftClicked;
                m_MouseClickDetector.OnRightClicked -= MouseClickDetector_OnRightClicked;
                m_MouseClickDetector.Stop();
                m_MouseClickDetector = null;
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
