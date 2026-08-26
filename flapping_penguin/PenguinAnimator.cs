using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace flapping_penguin
{
    // ペンギンの見た目のアニメーション（羽ばたき・スライディング表示・待機復帰）を管理するクラス
    public class PenguinAnimator
    {
        private const int ActionDelayMilliseconds = 100; // アクション時の画像を表示する時間（ミリ秒）
        private const int ScrollStopDelayMilliseconds = 200; // スクロールが止まったと判定するまでの時間（ミリ秒）

        private readonly Image m_Image;
        private readonly ScaleTransform m_ImageScale;
        private readonly PenguinAssets m_Assets;
        private readonly DispatcherTimer m_ScrollStopTimer;

        // 次は右羽を上げる番かどうかを記憶するフラグ
        private bool m_IsRightWingNext = true;

        public PenguinAnimator(Image targetImage, ScaleTransform targetScale, PenguinAssets assets)
        {
            m_Image = targetImage;
            m_ImageScale = targetScale;
            m_Assets = assets;

            m_ScrollStopTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(ScrollStopDelayMilliseconds)
            };
            m_ScrollStopTimer.Tick += OnScrollStopTimerTick;

            ResetToIdle();
        }

        // キーが押されたときの処理（パタパタさせる）
        public async Task PlayWingFlapAsync()
        {
            // 反転状態をリセット（スクロール中に反転していた場合に備えて）
            m_ImageScale.ScaleX = 1;

            // ① どちらの羽を上げるか判定して画像を変える
            if (m_IsRightWingNext)
            {
                m_Image.Source = m_Assets.RightUp; // 右を上げる
            }
            else
            {
                m_Image.Source = m_Assets.LeftUp;  // 左を上げる
            }

            // ② 次の入力のために、右と左の順番を反転させる
            m_IsRightWingNext = !m_IsRightWingNext;

            // ③ 指定ミリ秒だけ待つ
            await Task.Delay(ActionDelayMilliseconds);

            // ④ 元の待機状態（両羽下げ）に戻す
            m_Image.Source = m_Assets.BothDown;
        }

        // スペースキーでジャンプさせる（担当：いのま）
        public Task PlayJumpAsync()
        {
            // TODO: ジャンプアニメーションを実装
            return Task.CompletedTask;
        }

        // エンターキーでバンザイさせる（担当：いのま）
        public Task PlayBanzaiAsync()
        {
            // TODO: バンザイアニメーションを実装
            return Task.CompletedTask;
        }

        // スクロール中のスライディング表示に切り替える
        public void ShowSliding(ScrollDirection direction)
        {
            m_Image.Source = m_Assets.Sliding;

            // 右へ移動するときは反転、左へ移動するときは通常向き
            m_ImageScale.ScaleX = direction == ScrollDirection.Right ? -1 : 1;

            m_ScrollStopTimer.Stop();
            m_ScrollStopTimer.Start();
        }

        // Window終了時に呼ぶ後始末
        public void Stop()
        {
            m_ScrollStopTimer.Stop();
            m_ScrollStopTimer.Tick -= OnScrollStopTimerTick;
        }

        // スクロールが止まってから一定時間経ったときに呼ばれる処理
        private void OnScrollStopTimerTick(object sender, EventArgs e)
        {
            m_ScrollStopTimer.Stop();
            ResetToIdle();
        }

        private void ResetToIdle()
        {
            m_Image.Source = m_Assets.BothDown;
            m_ImageScale.ScaleX = 1; // 反転状態をリセット
        }
    }
}
