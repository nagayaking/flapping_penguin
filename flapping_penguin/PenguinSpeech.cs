using System;
using System.Windows;
using System.Windows.Threading;

namespace flapping_penguin
{
    public class PenguinSpeech
    {
        private DispatcherTimer _hideTimer;
        private Random _random;

        // セリフの候補
        private readonly string[] _speeches = new string[]
        {
            "皇帝ペンギンだからって肯定すると思うなよ",
            "変化が怖いなら前には進めないぞ",
            "出会いがあるなら別れがあるのは必然",
            "自分の弱さを認めろ",
            "先々見ないで目の前のものに集中しようや",
            "とりあえず人と話せば何か変わるんちゃう",
            "早起きできる奴は強い",
            "どんな時でも感情的になる奴は幼稚",
            "他人のために行動しろ",
            "言い訳の天才になっても、人生豊かにならないぞ",
            "失敗を笑う奴より、挑戦すらしない自分を恥じろ",
            "自分の機嫌くらい、自分で取れるようになれ",
            "『いつかやる』の『いつか』は、死ぬまで来ない",
            "命とられるわけじゃないし、やってみようや",
            "悩む暇あるなら、とりあえず外歩いてこいや",
            "過去は変えられないが、過去の『意味』なら今日から変えられる",
            "孤独を愛せない奴は、誰と一緒にいても寂しいままやで",
            "愚者は努力しても愚者",
            "世界はお前の母親じゃない",
            "迷ったら楽しい方を選べ",
            "謝れない大人は、ただのデカい子供だ",
            "さっさと動け",
            "準備不足を『想定外』ってごまかすな",
            "こだわりは捨てるな。ただ、こだわりに殺されるな",
            "期待するから裏切られるんやで",
            "悩むのは、お前が暇な証拠だ",
            "逃げ道は自分で塞げ",
            "根拠のない自信を持て",
            "頑張らないのも才能だぞ"
        };

        public PenguinSpeech()
        {
            _random = new Random();
            _hideTimer = new DispatcherTimer();
            _hideTimer.Interval = TimeSpan.FromSeconds(2);
            _hideTimer.Tick += HideTimer_Tick;
        }

        public void Speak()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow == null) return;

            // 現在のマウスの画面上の絶対座標を取得
            var mousePos = System.Windows.Forms.Cursor.Position;

            // ペンギン（ウィンドウ）の画面上の位置とサイズを取得
            double left = mainWindow.Left;
            double top = mainWindow.Top;
            double width = mainWindow.Width;
            double height = mainWindow.Height;

            // マウスがペンギンの外側だったら、ここで処理をストップして何もしない
            if (mousePos.X < left || mousePos.X > left + width ||
                mousePos.Y < top || mousePos.Y > top + height)
            {
                return;
            }


            // ここから下は、ペンギンの上でクリックされた時だけ実行されます
            int index = _random.Next(_speeches.Length);

            mainWindow.SpeechText.Text = _speeches[index];
            mainWindow.SpeechPopup.IsOpen = true;

            _hideTimer.Stop();
            _hideTimer.Start();
        }

        private void HideTimer_Tick(object sender, EventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.SpeechPopup.IsOpen = false;
            }
            _hideTimer.Stop();
        }
    }
}