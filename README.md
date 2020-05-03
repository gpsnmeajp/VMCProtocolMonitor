# VMCProtocolMonitor
[VMCProtocol](https://sh-akira.github.io/VirtualMotionCaptureProtocol/)の受信内容を表示するソフトウェア。
ごく単純に受信内容をコンソールに流すモードと、VMCProtocolに基づいてブラウザに一覧表示するモードがあります。

**[ダウンロード](https://github.com/gpsnmeajp/VMCProtocolMonitor/releases)**

[VMC Protocol対応](https://sh-akira.github.io/VirtualMotionCaptureProtocol/)  
<img src="https://github.com/gpsnmeajp/VMCProtocolMonitor/blob/README-image/vmpc_logo_128x128.png?raw=true"></img>

## 現在利用可能な機能
- Streamモード(コンソールで閲覧)
- Listモード(ブラウザで閲覧)

## 設定
- setting.jsonにて、待受ポートを設定します。
- ListModeをtrueにするとListモードになります。ブラウザ(既定で http://127.0.0.1:8888/ )に一覧で表示します。
- ListModeをfalseにするとStreamモードです。コンソールに受信したそのまま表示します。(この場合HTTPサーバーは起動しません)
- VMCProtocolMonitor.exeを起動すると待受状態になります。
- VMCProtocol v2.5を前提にしています。
- /index.htm, /script.js, /worker.js, /style.cssの中身は変更可能です。これ以外のファイル名については読み込みを行いません。
- /list.dat は受信データを表すための仮想的なファイルです。javascriptから読み込んで表示する前提です。

# [お問合せ先(Discordサーバー)](https://discord.gg/nGapSR7)
