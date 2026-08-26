using System;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace flapping_penguin
{
    // マウスクリック検知専用のクラス
    public class MouseClickDetector
    {
        private IKeyboardMouseEvents m_GlobalHook;

        public event Action OnLeftClicked;
        public event Action OnRightClicked;

        public void Start()
        {
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.MouseClick += GlobalHook_MouseClick;
        }

        private void GlobalHook_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                OnLeftClicked?.Invoke();
            }
            else if (e.Button == MouseButtons.Right)
            {
                OnRightClicked?.Invoke();
            }
        }

        public void Stop()
        {
            if (m_GlobalHook != null)
            {
                m_GlobalHook.MouseClick -= GlobalHook_MouseClick;
                m_GlobalHook.Dispose();
                m_GlobalHook = null;
            }
        }
    }
}
