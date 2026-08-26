using System;
using System.Windows.Media.Imaging;

namespace flapping_penguin
{
    // ペンギンの画像リソースの読み込みと保持専用のクラス
    public class PenguinAssets
    {
        public BitmapImage BothDown { get; } // 両羽下げ
        public BitmapImage BothUp { get; }   // 両羽上げ
        public BitmapImage RightUp { get; }  // 右羽上げ・左羽下げ
        public BitmapImage LeftUp { get; }   // 左羽上げ・右羽下げ
        public BitmapImage Sliding { get; }  // スクロール中（スライディング）

        public PenguinAssets()
        {
            BothDown = Load("Images/penguin-LR-down.png");
            BothUp = Load("Images/penguin-LR-up.png");
            RightUp = Load("Images/penguin-R-up-L-down.png");
            LeftUp = Load("Images/penguin-L-up-R-down.png");
            Sliding = Load("Images/penguin_sliding.png");
        }

        private static BitmapImage Load(string relativeUri)
        {
            return new BitmapImage(new Uri(relativeUri, UriKind.Relative));
        }
    }
}
