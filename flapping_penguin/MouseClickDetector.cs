using System;
using System.Windows;
using System.Windows.Input;

namespace flapping_penguin
{
    // マウスクリックを検知する専用のクラス
    public class MouseClickDetector
    {
        public event Action OnLeftClicked;
        public event Action OnRightClicked;

        // 監視対象の部品（画像など）を保持する変数
        private UIElement m_TargetElement;

        // コンストラクタで監視対象を受け取る
        public MouseClickDetector(UIElement targetElement)
        {
            m_TargetElement = targetElement;
        }

        public void Start()
        {
            // WPF標準のクリック検知（対象部品の上だけで反応する）
            m_TargetElement.MouseLeftButtonUp += TargetElement_MouseLeftButtonUp;
            m_TargetElement.MouseRightButtonUp += TargetElement_MouseRightButtonUp;
        }

        public void Stop()
        {
            if (m_TargetElement != null)
            {
                m_TargetElement.MouseLeftButtonUp -= TargetElement_MouseLeftButtonUp;
                m_TargetElement.MouseRightButtonUp -= TargetElement_MouseRightButtonUp;
            }
        }

        private void TargetElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnLeftClicked?.Invoke();
        }

        private void TargetElement_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnRightClicked?.Invoke();
        }
    }
}