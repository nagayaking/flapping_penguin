using System;
using System.Windows.Forms;
using Gma.System.MouseKeyHook; // マウス検知にもこのパッケージを使います

namespace flapping_penguin
{
    // スクロール検知専用のクラス
    public class ScrollDetector
    {
        private IKeyboardMouseEvents m_GlobalHook;

        // 別ファイル（MainWindowなど）に「スクロールされたよ！」と知らせるためのイベント（電話のようなもの）
        // <int> はスクロール量（プラスなら上、マイナスなら下）を渡すためのものです
        public event Action<int> OnScrollDetected;

        // 監視のスタート
        public void Start()
        {
            m_GlobalHook = Hook.GlobalEvents();

            // マウスのホイールが回されたときのイベントを登録
            m_GlobalHook.MouseWheel += GlobalHook_MouseWheel;
        }

        // 実際にホイールが回された瞬間に動く処理
        private void GlobalHook_MouseWheel(object sender, MouseEventArgs e)
        {
            // e.Delta にはスクロール量（通常は上なら +120、下なら -120）が入っています
            int scrollAmount = e.Delta;

            // MainWindowなどに「回されたよ！」とお知らせ（発信）する
            OnScrollDetected?.Invoke(scrollAmount);
        }

        // 監視のストップ（終了時に必ず呼ぶ）
        public void Stop()
        {
            if (m_GlobalHook != null)
            {
                m_GlobalHook.MouseWheel -= GlobalHook_MouseWheel;
                m_GlobalHook.Dispose();
                m_GlobalHook = null;
            }
        }
    }
}