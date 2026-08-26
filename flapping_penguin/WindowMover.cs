using System;
using System.Linq;
using System.Windows;

namespace flapping_penguin
{
    // マルチモニタを考慮したウィンドウ位置移動専用のクラス
    public class WindowMover
    {
        private const int ScrollMoveStepPixels = 30; // スクロール時にウィンドウを移動させるピクセル数

        private readonly Window m_Window;

        public WindowMover(Window window)
        {
            m_Window = window;
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
