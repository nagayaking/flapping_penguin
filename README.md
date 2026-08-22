# flapping_penguin

## セットアップ

初回clone後、ビルド前に NuGet パッケージの復元が必要です。

Visual Studio でソリューションを右クリック → **NuGet パッケージの復元** を実行してください。

`packages` フォルダーは `.gitignore` 対象のため、復元しないと `Gma.System.MouseKeyHook` 関連の型が見つからずビルドに失敗します。
