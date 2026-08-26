using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace flapping_penguin
{
    // マルチモニタを考慮したウィンドウ位置移動専用のクラス
    public class WindowMover
    {
        private const int ScrollMoveStepPixels = 30; // スクロール時にウィンドウを移動させるピクセル数
        private const double JumpInitialVelocity = 900; // ジャンプ初速（ピクセル/秒、上向き）
        private const double JumpGravity = 8000; // ジャンプ中の重力加速度（ピクセル/秒^2）
        private const int JumpFrameIntervalMilliseconds = 16; // ジャンプのアニメーション更新間隔（約60fps）

        private readonly Window m_Window;
        private bool m_IsJumping; // 連続入力で位置がずれるのを防ぐためのフラグ

        public WindowMover(Window window)
        {
            m_Window = window;
        }

        // スペースキーでジャンプさせる（重力加速度を使って放物線を描くように動かす）
        public async Task PlayJumpAsync()
        {
            if (m_IsJumping)
            {
                return;
            }

            m_IsJumping = true;

            double groundTop = m_Window.Top;
            double velocity = -JumpInitialVelocity; // 上向きをマイナスとして扱う
            double offset = 0;
            double deltaTimeSeconds = JumpFrameIntervalMilliseconds / 1000.0;

            // 重力で速度を落としながら移動させ、地面（元の高さ）に戻ってきたら終了
            while (true)
            {
                velocity += JumpGravity * deltaTimeSeconds;
                offset += velocity * deltaTimeSeconds;

                if (offset >= 0)
                {
                    break;
                }

                m_Window.Top = groundTop + offset;
                await Task.Delay(JumpFrameIntervalMilliseconds);
            }

            m_Window.Top = groundTop;

            m_IsJumping = false;
        }

        // 起動時の初期配置（作業領域の右下）
        public void PlaceAtInitialPosition()
        {
            var workArea = SystemParameters.WorkArea;
            m_Window.Left = workArea.Right - m_Window.Width;
            m_Window.Top = workArea.Bottom - m_Window.Height;
        }

        // スクロールに応じてウィンドウを移動させる
        public void MoveForScroll(ScrollDirection direction)
        {
            MoveHorizontally(direction);
            SnapToCurrentScreenBottom();
            WrapAroundScreenEdges();
        }

        private void MoveHorizontally(ScrollDirection direction)
        {
            if (direction == ScrollDirection.Right)
            {
                m_Window.Left += ScrollMoveStepPixels;
            }
            else
            {
                m_Window.Left -= ScrollMoveStepPixels;
            }
        }

        private void SnapToCurrentScreenBottom()
        {
            int centerX = (int)(m_Window.Left + (m_Window.Width / 2));
            int currentY = (int)m_Window.Top;
            var currentScreen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point(centerX, currentY));

            PresentationSource source = PresentationSource.FromVisual(m_Window);
            double dpiScaleY = 1.0;
            if (source != null)
            {
                dpiScaleY = source.CompositionTarget.TransformToDevice.M22;
            }

            m_Window.Top = (currentScreen.WorkingArea.Bottom / dpiScaleY) - m_Window.Height;
        }

        private void WrapAroundScreenEdges()
        {
            int minLeft = System.Windows.Forms.Screen.AllScreens.Min(s => s.WorkingArea.Left);
            int maxRight = System.Windows.Forms.Screen.AllScreens.Max(s => s.WorkingArea.Right);

            if (m_Window.Left > maxRight)
            {
                m_Window.Left = minLeft - m_Window.Width;
            }
            else if (m_Window.Left < minLeft - m_Window.Width)
            {
                m_Window.Left = maxRight;
            }
        }
    }
}
