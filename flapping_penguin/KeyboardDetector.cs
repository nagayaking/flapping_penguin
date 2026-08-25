using System;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace flapping_penguin
{
    // キーボード検知専用のクラス
    public class KeyboardDetector
    {
        private IKeyboardMouseEvents m_GlobalHook;

        // 別ファイル（MainWindow）にお知らせするためのイベント
        public event Action OnKeyPressed;
        public event Action OnKeyReleased;

        public void Start()
        {
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.KeyDown += GlobalHook_KeyDown;
            m_GlobalHook.KeyUp += GlobalHook_KeyUp;
        }

        private void GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {
            // キーが押されたらお知らせを発信
            OnKeyPressed?.Invoke();
        }

        private void GlobalHook_KeyUp(object sender, KeyEventArgs e)
        {
            // キーが離されたらお知らせを発信
            OnKeyReleased?.Invoke();
        }

        public void Stop()
        {
            if (m_GlobalHook != null)
            {
                m_GlobalHook.KeyDown -= GlobalHook_KeyDown;
                m_GlobalHook.KeyUp -= GlobalHook_KeyUp;
                m_GlobalHook.Dispose();
                m_GlobalHook = null;
            }
        }
    }
}