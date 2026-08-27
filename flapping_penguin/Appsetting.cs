using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flapping_penguin
{
    // 設定情報を管理する専用のクラス
    public class AppSettings
    {
        // プロパティとして設定値を保持（初期値もここで定義します）
        public bool IsMovementEnabled { get; set; } = true;
        public int MovementSpeed { get; set; } = 30;
    }
}
